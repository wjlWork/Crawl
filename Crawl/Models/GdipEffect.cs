using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Reflection;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace Crawl.Models
{
    public class GdipEffect
    {
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = true)]
        private static extern void CopyMemory(IntPtr Dest, IntPtr src, int Length);

        private static Bitmap Bmp;
        private static IntPtr ImageCopyPointer, ImagePointer;
        private static int DataLength;

        /// <summary>
        /// 加高斯模糊
        /// </summary>
        /// <param name="imgPath">源图片路径</param>
        /// <param name="WaterMarkPicPath">水印图片的路径</param>
        /// <param name="_watermarkPosition">加高斯模糊的位置</param>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        public static void GdipPic(string imgPath,string WaterMarkPicPath,string _watermarkPosition, int _width, int _height)
        {
            try
            {
                //string path1 = @"E:\WorkSpace\MyTest\WindowsFormsApplication1\WindowsFormsApplication1\image\Hydrangeas.jpg";
                string NewPath = Helpers.FillPath(imgPath, "~/image/uploadfile");
                System.IO.File.Copy(imgPath, NewPath);

                Bmp = (Bitmap)Bitmap.FromFile(imgPath);
                BitmapData BmpData = new BitmapData();
                Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadWrite, Bmp.PixelFormat, BmpData);    //  用原始格式LockBits,得到图像在内存中真正地址，这个地址在图像的大小，色深等未发生变化时，每次Lock返回的Scan0值都是相同的。
                ImagePointer = BmpData.Scan0;                                                                                   //  记录图像在内存中的真正地址
                DataLength = BmpData.Stride * BmpData.Height;                                                                   //  记录整幅图像占用的内存大小
                ImageCopyPointer = Marshal.AllocHGlobal(DataLength);                                                            //  直接用内存数据来做备份，AllocHGlobal在内部调用的是LocalAlloc函数
                CopyMemory(ImageCopyPointer, ImagePointer, DataLength);                                                         //  这里当然也可以用Bitmap的Clone方式来处理，但是我总认为直接处理内存数据比用对象的方式速度快。
                Bmp.UnlockBits(BmpData);
                //Pic.Image = Bmp;
                UpdateImage(NewPath, WaterMarkPicPath, _watermarkPosition, _width, _height);
            }
            catch (Exception d)
            {
                Console.WriteLine(d.Message);
            }
            finally
            {
                Bmp.Dispose();
            }

        }

        private static void UpdateImage(string SavePath, string WaterMarkPicPath, string _watermarkPosition, int _width, int _height)
        {
            if (Bmp != null)
            {

                Image watermark = new Bitmap(WaterMarkPicPath);

                int xpos = 0;
                int ypos = 0;
                int WatermarkWidth = 0;
                int WatermarkHeight = 0;
                double bl = 1d;

                //计算水印图片的比率
                //取背景的1/4宽度来比较
                if ((_width > watermark.Width * 4) && (_height > watermark.Height * 4))
                {
                    bl = 1;
                }
                else if ((_width > watermark.Width * 4) && (_height < watermark.Height * 4))
                {
                    bl = Convert.ToDouble(_height / 4) / Convert.ToDouble(watermark.Height);

                }
                else if ((_width < watermark.Width * 4) && (_height > watermark.Height * 4))
                {
                    bl = Convert.ToDouble(_width / 4) / Convert.ToDouble(watermark.Width);
                }
                else
                {
                    if ((_width * watermark.Height) > (_height * watermark.Width))
                    {
                        bl = Convert.ToDouble(_height / 4) / Convert.ToDouble(watermark.Height);

                    }
                    else
                    {
                        bl = Convert.ToDouble(_width / 4) / Convert.ToDouble(watermark.Width);

                    }

                }

                WatermarkWidth = Convert.ToInt32(watermark.Width * bl);
                WatermarkHeight = Convert.ToInt32(watermark.Height * bl);

                switch (_watermarkPosition)
                {
                    case "WM_TOP_LEFT":
                        xpos = 10;
                        ypos = 10;
                        break;
                    case "WM_TOP_RIGHT":
                        xpos = _width - WatermarkWidth - 10;
                        ypos = 10;
                        break;
                    case "WM_BOTTOM_RIGHT":
                        xpos = _width - WatermarkWidth - 10;
                        ypos = _height - WatermarkHeight - 10;
                        break;
                    case "WM_BOTTOM_LEFT":
                        xpos = 10;
                        ypos = _height - WatermarkHeight - 10;
                        break;
                }


                //string path2 = @"E:\WorkSpace\MyTest\WindowsFormsApplication1\WindowsFormsApplication1\image\Hydrangeas111.jpg";

                CopyMemory(ImagePointer, ImageCopyPointer, DataLength);             // 需要恢复原始的图像数据，不然模糊就会叠加了。
                Rectangle Rect = new Rectangle(xpos, ypos, WatermarkWidth + xpos, WatermarkHeight + ypos);
                Stopwatch Sw = new Stopwatch();
                Sw.Start();
                Bmp.GaussianBlur(ref Rect, 20, false);              //设置模糊值从0-100
                Sw.Stop();

                //lblInfo.Text = "用时" + Sw.ElapsedMilliseconds.ToString() + "ms";
                //Pic.Image = Bmp;
                string NewSavePath = Helpers.FillPath(SavePath, "~/image/uploadfile/GfipImg");
                Bmp.Save(NewSavePath);


                //删除文件
                if (System.IO.File.Exists(SavePath))
                {
                    try
                    {
                        System.IO.File.Delete(SavePath);
                    }
                    catch (System.IO.IOException e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                }


            }
        }


        public static void AddGdipPic(string imgPath, string WaterMarkPicPath, string _watermarkPosition)
        {
            string NewPath = Helpers.FillPath(imgPath, "~/image/uploadfile/GfipImg");
            Image image = Image.FromFile(imgPath);
            try
            {
                GdipPic(imgPath, WaterMarkPicPath, _watermarkPosition, image.Width, image.Height);
            }
            catch (Exception d)
            {
                Console.WriteLine(d.Message);
            }
            finally
            {
                image.Dispose();
            }


            //删除文件
            if (System.IO.File.Exists(imgPath))
            {
                try
                {
                    System.IO.File.Delete(imgPath);
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
       
            System.IO.File.Move(NewPath, imgPath);
        }



    }
}