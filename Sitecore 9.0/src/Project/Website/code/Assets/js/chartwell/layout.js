// layout needs

//re-enable search fields
$("#City, #PostalCode, #PropertyName").blur(function () {
  $("#City, #PostalCode, #PropertyName").prop('readonly', false).prop('disabled', false);
});

// Search bar - toggle to disable/enable fields --------------------------------------------
$("#City").click(function () {
  $("#PostalCode, #PropertyName").prop('readonly', true).prop('disabled', true).addClass("inactiveSearchField").removeClass("activeSearchField").val("");
  $(this).prop('disabled', false).removeClass("inactiveSearchField").addClass("activeSearchField");
});
$("#PostalCode").click(function () {
  $("#City, #PropertyName").prop('readonly', true).prop('disabled', true).addClass("inactiveSearchField").removeClass("activeSearchField").val("");
  $(this).prop('disabled', false).removeClass("inactiveSearchField").addClass("activeSearchField");
});
$("#PropertyName").click(function () {
  $("#City, #PostalCode").prop('readonly', true).prop('disabled', true).addClass("inactiveSearchField").removeClass("activeSearchField").val("");
  $(this).prop('disabled', false).removeClass("inactiveSearchField").addClass("activeSearchField");
});

// left navigation transformation for mobile version ----------------------------------------
$(window).on('load resize', function () {
  if ($(window).width() < 992) {
    $(".btn-group-vertical").addClass("btn-group btn-group-lg");
    $(".btn-group-vertical").removeClass("btn-group-vertical");
  }
  else {
    $(".btn-group").addClass("btn-group-vertical");
    $(".btn-group-vertical").removeClass("btn-group btn-group-lg");
  }
});

// Search Page hover on panel -----------------------------------------------------------------
$(".gridCity .panel-primary > a > img, .gridCity .panel-heading, .gridCity .viewResBtn > a").hover(function(){
	$(this).closest( ".panel" ).addClass("panelSearchHover");
}, function() {
	$(this).closest(".panel").removeClass("panelSearchHover");
});

$(".splitPageWrap .panel-assisted > a > img, .splitPageWrap .panel-assisted .panel-heading, .splitPageWrap .panel-assisted .viewResBtn > a").hover(function(){
	$(this).closest( ".panel" ).addClass("panelSearchHoverAssisted");
}, function() {
	$(this).closest(".panel").removeClass("panelSearchHoverAssisted");
});


// Contact Form Validation
$(document).ready(function () {

  $('#visitdate').attr('readonly', 'readonly').datepicker();
  /* permanent Banner move after .resQuote */

  // Residences without image
  //var bgAddressCleanup = /\"|\'|\)/g;
  if (document.getElementById('bgPropertyImage')) { //existence check first
      if ($(window).width() > 992) {
        if ($('#bgPropertyImage').attr('style').indexOf('none') > -1) {
          $('#bgPropertyImage').css('height', '50px');
          $('.resContainerUp').css('margin-top', '-45px');
        }
      }
      else {
        if ($('#bgPropertyImage').attr('style').indexOf('none') > -1) {
          $('#bgPropertyImage').css('height', '50px');
          $('.resContainerUp').css('margin-top', '-45px');
        }
      }
  }	
});// document ready



// .gridCity > .col-md-4:nth-child(3n+1) START
	$(".gridCity > .col-md-4:nth-child(3n+1)").css("clear", "left");

	$('*[data-ajax-mode="replace"]').click(
    function() {
        setTimeout(
            function() {
                $(".gridCity > .col-md-4:nth-child(3n+1)").css("clear", "left");
            },
            4000);
    });
// END gridCity

