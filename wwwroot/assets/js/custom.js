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

    console.log(window.location.pathname)

    $(document).on('click', ".basketdelete", function (e) {
        e.preventDefault();

        let url = $(this).attr('href');

        fetch(url)
            .then(res => {
                return res.text();
            })
            .then(data => {

                $(".minicart-content-box").html(data);
                let cartCount = $(".cartcount").text()
                $(".notification").html($(".cartcount").text())
                //$(".productTable").html(data);
                //if (window.location.pathname == "/basket") {
                    fetch("basket/RefreshIndex")
                      .then(res1 => {
                          return res1.text();
                      })
                      .then(data1 => {
                          $(".productTable").html(data1);

                      })
                        
                
                
            })
        
                 
    });

    $(document).on('click', ".cartdelete", function (e) {
        e.preventDefault();

        let url = $(this).attr('href');
        fetch(url).then(res => res.text()).then(data =>
        {
            $(".productTable").html(data);
            fetch('/basket/RefreshBasket').then(res1 => res1.text())
                .then(data1 => {
                    $(".minicart-content-box").html(data1);
                    let cartCount = $(".cartcount").text()
                    $(".notification").html($(".cartcount").text())
                })
        })

    })


    $(document).on('keyup', '.productCount', function (e) {
        e.preventDefault();
        let count = $(this).val();
        let productId = $(this).attr('data-productId');

        fetch('/Product/ChangeBasketProductCount/' + productId + '?count=' + count).then(res => {
            return res.text();
        }).then(data => {
            $(".productTable").html(data);
            fetch('/Product/RefreshCartProductCount').then(res1 => res1.text())
                .then(refreshdata => {
                    $(".minicart-content-box").html(refreshdata);
                    let cartCount = $(".cartcount").text()
                    $(".notification").html($(".cartcount").text())
                })
             
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
    //$(".categoryFilter").click(function (e) {
    //    e.preventDefault();
    //    let categoryId = $('.categoryFilter').data('id');

    //    let val = $('.rangeInput').val();

    //    //let categoryId = $('.categoryFilter').attr('data-id');
    //    fetch('shop/filter?categoryId=' + categoryId + '&range=' + val)
    //        .then(res => {
    //            return res.text();
    //        })
    //        .then(data => {
    //            console.log(categoryId);
    //            $('.shopList').html(data)
    //        })

    //})
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
       
        let val = $('.rangeInput').val();

        let pageIndex = $('.pagination-box').find('.active').find('a').data('page');


        fetch('shop/filter?range=' + val + '&pageIndex=' + pageIndex)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('.shopList').html(data)
            })
    })


})
