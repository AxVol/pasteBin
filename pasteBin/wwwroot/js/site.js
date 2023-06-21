$(function () {
    function copyToClipboard(element) {
        var $temp = $("<input>");
        $("body").append($temp);
        $temp.val($(element).text()).select();
        document.execCommand("copy");
        $temp.remove();
    }

    $(".create-btn").on("click", function () {
        copyToClipboard("#url-fields");
        $(".link-alert").fadeIn("slow");
    });
});