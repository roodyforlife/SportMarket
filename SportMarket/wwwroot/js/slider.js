var slideIndex = 1;
showSlides(slideIndex);
function plusSlides(n) {
    showSlides(slideIndex += n);
}

function currentSlide(n) {
    showSlides(slideIndex = n);
}

function showSlides(n) {
    var i;
    var slides = $('.slider__conteiner__slide');
    if (n > slides.length) {
        slideIndex = 1;
    }
    if (n < 1) {
        slideIndex = slides.length;
    }
    slides.css("opacity", "0");
    slides[slideIndex - 1].style.opacity = "1";
}

$(document).ready(function () {

    $('.slider__conteiner__slide').on("swipeleft", function (e) {
        plusSlides(1);
    });
    $('.slider__conteiner__slide').on("swiperight", function (e) {
        plusSlides(-1);
    });

});