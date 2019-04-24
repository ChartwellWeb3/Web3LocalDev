//$(function () {
//  $(".visitdate-en").datepicker({
//    beforeShowDay: $.datepicker.noWeekends,
//    showOn: "button",
//    buttonImage: "/assets/images/layout/icon-calandar.png",
//    buttonImageOnly: true,
//    buttonText: "Select date for Visit",
//    minDate: 0,
//    changeYear: true,
//    changeMonth: true
//  });
//});

//$(function () {
//  $(".visitdate-fr").datepicker({
//    beforeShowDay: $.datepicker.noWeekends,
//    showOn: "button",
//    buttonImage: "/assets/images/layout/icon-calandar.png",
//    buttonImageOnly: true,
//    buttonText: "Select date for Visit",
//    minDate: 0,
//    changeYear: true,
//    changeMonth: true
//  });
//});

$(function () {
  $(".visitdate-en").datepicker($.datepicker.regional["en"]);
  $("#locale").on("change", function () {
    $("#datepicker").datepicker("option",
      $.datepicker.regional[$(this).val()]);
  });
});

$(function () {
  $(".visitdate-fr").datepicker($.datepicker.regional["fr"]);
  $("#locale").on("change", function () {
    $("#datepicker").datepicker("option",
      $.datepicker.regional[$(this).val()]);
  });
});