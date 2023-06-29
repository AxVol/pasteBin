var textareas = document.getElementsByTagName('textarea');
var count = textareas.length;
for (var i = 0; i < count; i++) {
    textareas[i].onkeydown = function (e) {
        if (e.keyCode == 9 || e.which == 9) {
            e.preventDefault();
            var s = this.selectionStart;
            this.value = this.value.substring(0, this.selectionStart) + "\t" + this.value.substring(this.selectionEnd);
            this.selectionEnd = s + 1;
        }
    }
}

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

const show_button = document.querySelector('#show');
const hide_button = document.querySelector('#hide');
const form = document.querySelector('#blablabla');
const popup = document.querySelector('.popup');

show_button.addEventListener('click', () => {
    form.classList.add('open');
    popup.classList.add('popup_open');
});

hide_button.addEventListener('click', () => {
    form.classList.remove('open');
    popup.classList.remove('popup_open');
});