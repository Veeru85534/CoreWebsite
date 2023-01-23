// Initialize tooltips
var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl)
})

function addWhishList(PID) {
    $.ajax({
        type: "POST",
        url: "/Home/AddWishList",
        data: {
            "ProductId": PID
        },
        success: function (response) {
            if (response.code === 0) {
                var windowurl = $(location).attr('pathname');
                //window.location = "http://localhost:5216/User/Login";
                $("#hideToast").html(`<div class="toast show align-items-center bg-danger" role="alert" aria-live="assertive" aria-atomic="true">
                                        <div class="d-flex">
                                            <div class="toast-body text-white">
                                                            `+ response.message + `<b><a href='/User/Login?ReturnUrl=${windowurl}'> Sign In</a></b>
                                            </div>
                                            <button type="button" class="btn-close me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                                        </div>
                                    </div>`);
                $('.submitClick').prop('disabled', false);
            }
            if (response.code === 2) {
                $("#hideToast").html(`<div class="toast show align-items-center bg-danger" role="alert" aria-live="assertive" aria-atomic="true">
                                        <div class="d-flex">
                                            <div class="toast-body text-white">
                                                    `+ response.message + `
                                            </div>
                                            <button type="button" class="btn-close me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                                        </div>
                                    </div>`);
                $('.submitClick').prop('disabled', false);
            }
            if (response.code === 1) {
                $("#hideToast").html(`<div class="toast show align-items-center bg-success" role="alert" aria-live="assertive" aria-atomic="true">
                                        <div class="d-flex">
                                            <div class="toast-body text-white">
                                                    `+ response.message + `
                                            </div>
                                            <button type="button" class="btn-close me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                                        </div>
                                    </div>`);
                $('.submitClick').prop('disabled', false);
            }

        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            console.log(response.responseText);
        }
    });
};

/*SideNavar with scroll down start*/
document.addEventListener("DOMContentLoaded", function () {

    document.querySelectorAll('.sidebar .nav-link').forEach(function (element) {

        element.addEventListener('click', function (e) {

            let nextEl = element.nextElementSibling;
            let parentEl = element.parentElement;

            if (nextEl) {
                e.preventDefault();
                let mycollapse = new bootstrap.Collapse(nextEl);

                if (nextEl.classList.contains('show')) {
                    mycollapse.hide();
                } else {
                    mycollapse.show();
                    // find other submenus with class=show
                    var opened_submenu = parentEl.parentElement.querySelector('.submenu.show');
                    // if it exists, then close all of them
                    if (opened_submenu) {
                        new bootstrap.Collapse(opened_submenu);
                    }

                }
            }

        });
    })

});
/*SideNavar with scroll down end*/



$(document).ready(function () {

    ////Drop Down For state and City Start
    var StateId = $("#getStateValue").val();
    $.ajax({
        type: "GET",
        url: "/User/StateDropDownList",
        data: "{}",
        success: function (data) {
            var s = '<option  value="">Select State</option>';
            for (var i = 0; i < data.length; i++) {
                //console.log(data[i])
                if (StateId == data[i].id){
                    s += '<option selected value="' + data[i].id + '">' + data[i].name + '</option>';                    
                }     
                if (StateId != data[i].id) {
                s += '<option value="' + data[i].id + '">' + data[i].name + '</option>';
                }
            }
            $("#StateMasterPic").html(s);
            $("#StateMasterPic1").html(s);
        }
    });
    var CityId = $("#getCityValue").val();
    $.ajax({
        type: "GET",
        url: `/User/GetCityDropDownList?stateid=0`,
        data: "{}",
        success: function (data) {
            var s = '<option  value="">Select City</option>';
            for (var i = 0; i < data.length; i++) {
                //console.log(data[i])
                if (CityId == data[i].id) {
                    s += '<option selected value="' + data[i].id + '">' + data[i].name + '</option>';
                }
                if (CityId != data[i].id) {
                    s += '<option value="' + data[i].id + '">' + data[i].name + '</option>';
                }
                
            }
            $("#CityMasterPic").html(s);            
            $("#CityMasterPic1").html(s);            
        }
    });
    $("#StateMasterPic").change(function () {
        var statevalue = $(this).val();
        $('#CityMasterPic').removeAttr('disabled');
        $.ajax({
            type: "GET",
            url: `/User/GetCityDropDownList?stateid=${statevalue}`,
            data: "{}",
            success: function (data) {
                var s = '<option  value="">Select City</option>';
                for (var i = 0; i < data.length; i++) {
                    //console.log(data[i])
                    s += '<option value="' + data[i].id + '">' + data[i].name + '</option>';
                }
                $("#CityMasterPic1").html(s);
            }
        });
    });
    ////Drop Down For state and City Start
    $(".submitClick").one('click', function (event) {
        event.preventDefault();
        //do something
        $(this).prop('disabled', true);
    });
    //$('#page_effect').hide("slow");
    //$('#page_effect').hide("fade", {}, 1000);
    //$('#page_effect').fadeOut(2000);
    //$('.hideToast').delay(6000).fadeOut();

    //$(".avoidDoubleClick").Onclick(function () {
    //    $(".avoidDoubleClick").attr("disabled","disabled");
    //}); 
    //$(document).bind("contextmenu", function (e) {
    //    e.preventDefault();
    //});

    //$(document).keydown(function (e) {
    //    if (e.which === 123) {
    //        return false;
    //    }
    //});
    /*Ajax Call For Category DropDown in Layout Page Start */
    $.ajax({
        type: "GET",
        url: "/Home/CategoryListDropDown",
        data: "{}",
        success: function (data) {
            var s = '<option  value="null">Select Category</option>';
            for (var i = 0; i < data.length; i++) {
                //console.log(data[i])
                s += '<option value="' + data[i].id + '">' + data[i].name + '</option>';
            }
            $("#PickUpLocation").html(s);           
        }
    });
    $.ajax({
        type: "GET",
        url: `/Home/ProductListDropDown?id=0`,
        data: "{}", success: function (data) {
            var s = '';
            for (var i = 0; i < data.length; i++) { s += '<option value="' + data[i].name + '">' + data[i].name + '</option>'; } $("#PickUpLocationProduct").html(s);
        }
    });
    $(".navbar-nav li").hover(function () {
        var isHovered = $(this).is(":hover");
        if (isHovered) {
            $(this).children("ul").stop().slideDown(300);
        } else {
            $(this).children("ul").stop().slideUp(300);
        }
    });  
  
    /*Ajax Call For Category DropDown in Layout Page End */

    /*Hover on item using jquery start*/
    $(".item1").hover(
        function () {
            $(this).addClass(" shadow-lg mb-2  hoveronitem bg-white");
        }, function () {
            $(this).removeClass(" shadow-lg mb-2 hoveronitem bg-white");
        }
    );
    /*Hover on item using jquery end*/

});