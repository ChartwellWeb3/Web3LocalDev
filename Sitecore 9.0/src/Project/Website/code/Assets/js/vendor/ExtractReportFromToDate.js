$(function () {
  $("#fromDate").datepicker({
    beforeShowDay: $.datepicker.noWeekends,
    showOn: "button",
    buttonImage: "/assets/images/layout/icon-calandar.png",
    buttonImageOnly: true,
    buttonText: "Select date for Visit",
    minDate: 0,
    changeYear: true,
    changeMonth: true
  });
});

$(function () {
  $("#toDate").datepicker({
    beforeShowDay: $.datepicker.noWeekends,
    showOn: "button",
    buttonImage: "/assets/images/layout/icon-calandar.png",
    buttonImageOnly: true,
    buttonText: "Select date for Visit",
    minDate: 0,
    changeYear: true,
    changeMonth: true
  });
});