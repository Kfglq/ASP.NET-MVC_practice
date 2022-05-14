
$(function () {
    $(document).delegate('#CreateArticleModal #createBtn', 'click', function () {
        $('#CreateArticleModal form').submit();
    });
});