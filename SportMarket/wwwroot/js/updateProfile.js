$(document).ready(function () {
    $('.update__item__image input').on('change', function (event) {
        let img = $(this).parent().children('img');
        for (let item of event.target.files) {
            var fr = new FileReader();
            fr.readAsDataURL(item)
            fr.onload = function (e) {
                img.attr("src", e.currentTarget.result)
            };
        }
    });
});