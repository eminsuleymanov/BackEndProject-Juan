$(document).ready(function () {
    $(".productModal").click(function (e) {
        e.preventDefault();
        let url = $(this).attr('href');
        fetch(url)
            .then(res => {
                return res.text();

            })
            .then(data => {
                $('.modal-content').html(data);
                // prodct details slider active
                $('.product-large-slider').slick({
                    fade: true,
                    arrows: false,
                    asNavFor: '.pro-nav'
                });
                // product details slider nav active
                $('.pro-nav').slick({
                    slidesToShow: 4,
                    asNavFor: '.product-large-slider',
                    arrows: false,
                    focusOnSelect: true
                });
            })
    })
    //$(".categoryFilter").click(function () {
    //    let categoryId = $('.categoryFilter').attr('data-id');


    //})

    $('.rangeFilter').click(function (e) {
        e.preventDefault();

        let val = $('.rangeInput').val();

        fetch('shop/list?range=' + val)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('.shopList').html(data)
            })
    })


})
