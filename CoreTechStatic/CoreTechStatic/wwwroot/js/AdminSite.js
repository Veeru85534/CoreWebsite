
$(document).ready(function () {


    $("#myInput").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#myTable tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    /////Hsn Master DropDown Start    
    $.ajax({
        type: "GET",
        url: "/Admin/HsnMasterDropDown",
        data: "{}",
        success: function (data) {
            var s = '<option  value="">Select HsnCode</option>';
            for (var i = 0; i < data.length; i++) {
                //console.log(data[i])
                s += '<option value="' + data[i].id + '">' + data[i].name + '</option>';
            }
            $("#hsnMasterDropDown").html(s);
        }
    });
    ////End
    /////Hsn Master DropDown Start
    $.ajax({
        type: "GET",
        url: "/Admin/CategoryDropDown",
        data: "{}",
        success: function (data) {
            var s = '<option  value="">Select Category</option>';
            for (var i = 0; i < data.length; i++) {
                //console.log(data[i])
                s += '<option value="' + data[i].id + '">' + data[i].name + '</option>';
            }
            $("#categoryDropDown").html(s);
        }
    });
    ////End
});