
$(function () {
    $(document).delegate('#UploadModal #uploadBtn', 'click', function () {
        $('#UploadModal form').submit();
    });
    $(document).delegate('#AlbumListBlock a.showImgLink', 'click', function (e) {
        var $this = $(this);
        $.ajax({
            url: $this.attr('href'), success: function (data) {
                if (data.length > 0) {
                    $('#showImgBlock').removeClass('hidden');
                    $('#showImg').attr('src', data);
                }
                else {
                    $('#showImgBlock').addClass('hidden');
                    $('#showImg').attr('src', '');
                    alert('找無此圖片');
                }
            }, error: function (jqXHR) {
                $('#showImgBlock').addClass('hidden');
                $('#showImg').attr('src', '');
                alert('找無此圖片');
            }
        });
        e.preventDefault();
        return false;
    });
});