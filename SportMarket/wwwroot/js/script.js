$(document).ready(function () {
    $('.view__eye').click(function () {
        let input = $(this).parent().children('input');
        input.attr("type", input.attr("type") == "password" ? "text" : "password");
    });

    $('#profile, .close__form').click(function () {
        $('.header__profile__list').toggleClass('active__profile');
    })

    $('.header__profile__list').click(function (e) {
        if (e.target == $('.header__profile__list')[0]) {
            $('.header__profile__list').removeClass('active__profile');
        }
    });

    $('#basket').click(function () {
        $('.basket').toggleClass('basket__active');
        GetBasket();
    })

    $('.basket').click(function (e) {
        if (e.target == $('.basket')[0]) {
            $('.basket').removeClass('basket__active');
        }
    });

    $('.addBasket').click(function (e) {
        if (e.target == $('.addBasket')[0]) {
            $('.addBasket').removeClass('addBasket__active');
        }
    });

    $("#basketCount").on("input", function () {
        if (+$(this).val() < 1)
            $(this).val(1)
    });
});

function AddToLiked(id, event) {
    var formData = new FormData();
    formData.append("productId", id);
    $.ajax({
        url: "/Home/AddLike",
        type: 'POST',
        cache: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (response) {
            $("#CountLike").text(response.count);
            if (response.success) {
                var like = $(event.target).parents('#product__like');
                if (response.likeAdded) {
                    like.find("path").css("fill", "var(--red-color)")
                    $('.hover__like').hover(function () {
                        $(this).css("fill", "var(--dark-gray-color)");
                    }).mouseout(function () {
                        $(this).css("fill", "var(--red-color)")
                    });
                    newAlert("../img/success.png", "Ви додали товар в обрані")
                } else {
                    like.find("path").css("fill", "var(--dark-gray-color)")
                    $('.hover__like').hover(function () {
                        $(this).css("fill", "var(--red-color)");
                    }).mouseout(function () {
                        $(this).css("fill", "var(--dark-gray-color)")
                    });
                    newAlert("../img/fail.png", "Товар видалений з обраних")
                }
            } else {
                newAlert("../img/alert.png", "Ви не увійшли в акаунт")
            }
        }
    });
}

function DeleteLikeAsync(id, event) {
    if (confirm(`Ви дійсно хочете видалити товар з обраних?`)) {
        var formData = new FormData();
        formData.append("likeId", id);
        $.ajax({
            url: "/Home/DeleteLike",
            type: 'POST',
            cache: false,
            contentType: false,
            processData: false,
            data: formData,
            success: function (response) {
                $("#CountLike").text(response);
                $(event.target).parents('.liked__content__item').remove()
                if (response == 0) {
                    $('.liked__content__delete').remove();
                    $('.liked__content__items').remove();
                    $('.liked__content').append(`<div class="likes__content__empty"><h1>Список пустий</h1></div>`)
                }
                newAlert("../img/fail.png", "Товар видалений з обраних")
            }
        });
    }
}


function DeleteLikedList() {
    if (confirm(`Ви дійсно хочете почистити обрані?`)) {
        $.ajax({
            url: "/Home/DeleteLikes",
            type: 'POST',
            cache: false,
            contentType: false,
            processData: false,
            success: function (response) {
                $("#CountLike").text(0);
                $('.liked__content__delete').remove();
                $('.liked__content__items').remove();
                $('.liked__content').append(`<div class="likes__content__empty"><h1>Список пустий</h1></div>`)
                newAlert("../img/fail.png", "Обрані видалені")
            }
        });
    }
}

function newAlert(icon, title) {
    $('.alert').append(` <div class="alert__item">
    <div class="alert__item__block">
        <div class="alert__item__icon"><img src="${icon}" alt=""></div>
        <div class="alert__item__title">
            <h5>${title}</h5>
        </div>
    </div>
</div>`)
    animateAlert()
}

function animateAlert() {
    $(".alert__item").last().animate({ opacity: 1 }, 100)
    Array.from($(".alert__item")).forEach(function (item, index) {
        setTimeout(() => {
            $(".alert__item").first().animate({ opacity: 0 }, 600, "linear", function () {
                $(this).remove()
            })
        }, 1000 * (index + 1));
    })
}


function BasketCount(count, event) {
    var input = $(event.target).parents('.addBasket__content__counter').children('#basketCount');
    if (+input.val() == 1 && count < 0) {
        $(input).val(1)
    } else {
        $(input).val(+input.val() + count)
    }
}

function OpenBasketAdding(productId) {
    var formData = new FormData();
    formData.append("productId", productId);
    $.ajax({
        url: "/Home/GetFullProduct",
        type: 'POST',
        cache: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (response) {
            $('.addBasket').toggleClass('addBasket__active');
            $(".addBasket").html(response);
        }
    });
}

function AddToBasket(productId) {
    var formData = new FormData();
    formData.append("productId", productId);
    formData.append("count", $("#basketCount").val());
    $.ajax({
        url: "/Home/AddToBasket",
        type: 'POST',
        cache: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (response) {
            $("#CountBasket").text(response.count);
            if (response.success) {
                if (response.basketAdded) {
                    newAlert("../img/success.png", "Ви додали товар до кошику")
                    $('.addBasket').removeClass('addBasket__active');
                } else {
                    newAlert("../img/fail.png", "Цей товар вже є у кошику")
                }
            } else {
                newAlert("../img/alert.png", "Ви не увійшли в акаунт")
            }
        }
    });
}

function GetBasket() {
    $.ajax({
        url: "/Home/GetBasket",
        type: 'POST',
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            $(".basket").html(response);
        }
    });
}

function DeleteFromBasket(cardId) {
    var formData = new FormData();
    formData.append("cardId", cardId);
    $.ajax({
        url: "/Home/DeleteFromBasket",
        type: 'POST',
        cache: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (response) {
            GetBasket();
            newAlert("../img/fail.png", "Товар видалений з кошику")
        }
    });
}

function Comment(productId) {
    var commentValue = $(".comments__input__textarea").val();
    var score = $("input[name='Score']:checked").val()
    var success = true;
    if (commentValue.length == 0) {
        newAlert("../img/fail.png", "Ви не неписали текст");
        success = false;
    }

    if (score === undefined) {
        newAlert("../img/fail.png", "Ви не оцінили товар");
        success = false;
    }

    if (success) {
        var formData = new FormData();
        formData.append("Text", commentValue);
        formData.append("Score", score);
        formData.append("productId", productId);
        $.ajax({
            url: "/Home/AddComment",
            type: 'POST',
            cache: false,
            contentType: false,
            processData: false,
            data: formData,
            success: function (response) {
                LoadComments(productId);
                $(".comments__input__textarea").val("");
                newAlert("../img/success.png", "Коментарій написаний");
            }
        });
    }
}

function LoadComments(productId) {
    var formData = new FormData();
    formData.append("productId", productId);
    $.ajax({
        url: "/Home/LoadComments",
        type: 'POST',
        cache: false,
        contentType: false,
        processData: false,
        data: formData,
        success: function (response) {
            $('.comments__items').html(response);
        }
    });
}

function DeleteProduct(productId, event) {
    if (confirm(`Ви дійсно хочете видалити товар?`)) {
        var formData = new FormData();
        formData.append("productId", productId);
        $.ajax({
            url: "/AdminPanel/DeleteProduct",
            type: 'POST',
            cache: false,
            contentType: false,
            processData: false,
            data: formData,
            success: function (response) {
                $(event.target).parents(".admin__table__row").remove();
            }
        });
    }
}

function DeleteCategory(categoryId, event) {
    if (confirm(`Ви дійсно хочете видалити категорію?`)) {
        var formData = new FormData();
        formData.append("categoryId", categoryId);
        $.ajax({
            url: "/AdminPanel/DeleteCategory",
            type: 'POST',
            cache: false,
            contentType: false,
            processData: false,
            data: formData,
            success: function (response) {
                $(event.target).parents(".admin__table__row").remove();
            }
        });
    }
}