let cityArray = [];
$(document).ready(function () {
      // textarea count symbols
    $('.create__item__textarea textarea').on('change keyup paste', function () {
        $('.create__item___count span').html(this.value.length);
    });
});

// validation for uploaded photo
function UploadPhotos(event) {
    let img = $(event.target).parent().children('img');
    for (let item of event.target.files) {
        var fr = new FileReader();
        fr.readAsDataURL(item)
        fr.onload = function (e) {
            img.attr("src", e.currentTarget.result)
        };
    }
}