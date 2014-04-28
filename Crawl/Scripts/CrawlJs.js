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
});

//$('#test1').on('click', function () {
//    layer.msg('Hello layer', 2, -1); //2秒后自动关闭，-1代表不显示图标
//});