//$(".month-select").change(function () {
//    disableSelection();
//});

//$(".year-select").change(function () {
//    disableSelection();
//});

//var disableSelection = function () {
//    $(".report-select").attr('disabled', 'disabled');
//    $(".report-button").attr('disabled', 'disabled');
//};

$(".disable .year-select, .disable .month-select").on("change", function (e) {
    //var td = sel.closest("td");
    //td.html($("<div/>", { "class": "nodata" }).css({ "height": "26px", "line-height": "26px", "border": "none", "padding": "0" }).append("Getting clients for the select period..."));

    $(".report-select").css("visibility", "hidden");
    $(".report-button").prop("disabled", true);

    //this is needed because we are hiding the user select when the date changes
    //which means it the selected value won't be visible on the server
    var sel = $(".report-select");
    var selectedUser = sel.val();

    $(".selected-user").val(selectedUser);
});

