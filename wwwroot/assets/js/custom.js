$(document).ready(function () {

    $("#searchInput").keyup(function () {
        let search = $(this).val();
        //console.log(search);
        if (search.length > 2) {
            fetch("product/search?search=" + search)
                .then(res => {
                    return res.text()
                })
                .then(data => {
                    $("#searchBody").html(data);

                })
        }
        else {
            $('#searchBody').html('')
        }
         

    })

    
    $(".notification").html($(".cartcount").text());

    $(".addTobasket").click(function (e) {
        e.preventDefault();
        let productId = $(this).data('id');
        fetch("basket/AddToBasket?id=" + productId)
            .then(res => {
                return res.text();

            })
            .then(data => {
                $(".minicart-content-box").html(data);
                //let cartCount  = $(".cartcount").text()
                $(".notification").html($(".cartcount").text())  
                //let cartCount = $(".notification").html();
                //cartCount = cartCount === "" ? 0 : parseInt(cartCount);
                //$(".notification").html(cartCount + 1);
                //let count = parseInt($(".notification").text()) || 0; 
                //$(".notification").text(count + 1);
            })


    })

    $(document).on('click', ".basketdelete", function (e) {
        e.preventDefault();

        let productId = $(this).data('id');

        fetch("basket/DeleteFromBasket?id=" + productId, {
            method: 'POST'
        })
            .then(res => {
                return res.text();
            })
            .then(data => {

                $(".minicart-content-box").html(data);
                let cartCount = $(".cartcount").text()
                $(".notification").html($(".cartcount").text())
                //$(".productTable").html(data);
                
            })
    });
    $(document).on('keyup', '.productCount', function (e) {
        e.preventDefault();
        let count = $(this).val();
        let productId = $(this).attr('data-productId');

        fetch('/Product/ChangeBasketProductCount/' + productId + '?count=' + count).then(res => {
            return res.text();
        }).then(data => {
            $(".productTable").html(data);
             
        })

    })




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
    $(".categoryFilter").click(function (e) {
        e.preventDefault();
        let categoryId = $('.categoryFilter').data('id');

        let val = $('.rangeInput').val();

        //let categoryId = $('.categoryFilter').attr('data-id');
        fetch('shop/filter?categoryId=' + categoryId + '&range=' + val)
            .then(res => {
                return res.text();
            })
            .then(data => {
                console.log(categoryId);
                $('.shopList').html(data)
            })

    })
    //$(document).on('click', '.page-link', function (event) {
    //    event.preventDefault();
    //    var url = $(this).attr('href');
    //    var page = $(this).data('page');
    //    loadProducts(url, page);
    //    function loadProducts(url, page) {
    //        $.get(url, { pageIndex: page })
    //            .done(function (data) {
    //                $('.shopList').html(data);
    //            })
    //            .fail(function (jqXHR, textStatus, errorThrown) {
    //                alert('Error loading products: ' + textStatus + ' ' + errorThrown);
    //            });
    //    }
    //});
    
    $('.rangeFilter').click(function (e) {
        e.preventDefault();
        var urlParams = new URLSearchParams(window.location.search);
        var pageIndex = urlParams.get('pageIndex');
        let val = $('.rangeInput').val();

        fetch('shop/filter?pageIndex=' + pageIndex + '&range=' + val)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('.shopList').html(data)
            })
    })


})
