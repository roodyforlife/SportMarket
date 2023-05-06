let cityArray = [];
$(document).ready(function () {
    $('.content__search__category').hover(function (event) {
        if (event.type == "mouseenter") {
            $('.search__category__menu').addClass('_active__search__menu');
        } else if (event.type == "mouseleave") {
            $('.search__category__menu').removeClass('_active__search__menu');
        }
    });

    $('.content__search__sort').hover(function (event) {
        if (event.type == "mouseenter") {
            $('.search__sort__menu').addClass('_active__search__menu');
        } else if (event.type == "mouseleave") {
            $('.search__sort__menu').removeClass('_active__search__menu');
        }
    });

    $('.sort__menu__item').on('click', function () {
        let sortType = $(this).attr("sort");
        let sortText = $(this).attr("sortValue");
        $('.SortValue').text(sortText);
        $('.SortInput').val(sortType);
        $('.search__sort__menu').removeClass('_active__search__menu');
    });

    $('.CostFrom').on('input', function () {
        if (+$(this).val() < 0) {
            $(this).val(0)
        }
    });
});