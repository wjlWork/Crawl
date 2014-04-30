$(document).ready(function () {
    $('#error1').hide();
    $('#error2').hide();

    $('#submit').click(function (event) {
        var data = $('#DisPlay_Name').val();
        data = $.trim(data);
        if (data.length < 1) {
            $('#error1').show();
            event.preventDefault();
        }
        else {
            $('#error1').hide();
        }
        if ($("input[type='checkbox']").is(':checked')) {
            $('#error2').hide();
        }
        else {
            $('#error2').show();
            event.preventDefault();
        }
    });

    $("#addConfig").on('click', function () {
        popWin.showWin("800", "600", "增加配置项", "http://localhost:6466/Home/CreateConfig");
    });

    $("#Watermark").on('click', function () {
        popWin.showWin("800", "600", "水印位置配置", "http://localhost:6466/Watermark/Index");
    });
});

//打开水印位置配置页面
function watermarkconfig(actiontype, url) {
    popWin.showWin("800", "600", "编辑水印位置", "http://localhost:6466/Watermark/WatermarkConfig?actiontype=" + actiontype + "&url=" + url);
}

//打开网站配置修改页面
function updatedata(id, url) {
    popWin.showWin("800", "600", "修改配置项", "http://localhost:6466/Home/CreateConfig?id=" + id + "&url=" + url);
}