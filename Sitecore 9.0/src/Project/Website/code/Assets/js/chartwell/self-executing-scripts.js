//TO DO: Go through every one of these ones that aren't commented and
//    get rid of them if possible. 
//    or document/comment so we know what they are.
$('.dropdown-toggle').dropdown();

$('#carouselNewDevelopment').carousel();

$('.responsive-tabs').responsiveTabs({
  accordionOn: ['xs', 'sm']
});

$(window).on('load resize', function () {
  if ($(window).width() < 992) {
    $(".btn-group-vertical").addClass("btn-group btn-group-lg");
    $(".btn-group-vertical").removeClass("btn-group-vertical");
  } else {
    $(".btn-group").addClass("btn-group-vertical");
    $(".btn-group-vertical").removeClass("btn-group btn-group-lg");
   }
});
$(document).ready(function () {
  // SLIDER
  var sitename = location.host;
  var sitepath = location.pathname.replace("photos", "Contactus");
  var siteurl = "https://" + sitename + sitepath;

  var href = sitepath
  var i = 0;

  $('[data-fancybox="images"]').fancybox(

    {
      src: href,
      type: 'iframe',
      openEffect: 'elastic',
      closeEffect: 'elastic',
      nextSpeed: 0, //important
      prevSpeed: 0, //important
      margin: [44, 0, 22, 0],
      thumbs: {
        autoStart: true,
        axis: 'x'
      },
      beforeShow: function () {
        if (i == 0) {
          $("#Chartwelliframe").attr("src", siteurl);
          i++;
        }

      },
      afterShow: function (opts, obj) {
        i = 0;
        if ($(window).width() < 992) {
          $(".fancybox-community__form-container").removeClass("expanded")
          $(".fancybox-button--arrow_right").addClass("arrow-right-wider")
          $("[data-fancybox-contactform]").text("Open Form")
          console.log('windows size is small');
        } else {
          $("fancybox-button--arrow_right").removeClass("arrow-right-wider ")
          console.log('windows size is big right');
          var iframeBody = $('#Chartwelliframe').contents().find('body');;
          iframeBody.attr("style", "background:#e1f1ff");
          var iframeFooter = $('#Chartwelliframe').contents().find('footer');;
          iframeFooter.attr("style", "border:none");

        }
      }
    });

  $(document).on('click', '[data-fancybox-contactform]', function () {
    //$("[data-fancybox-contactform]").text(function (i, v) {
    //return v == "Open Form" ? "Hide Form" : "Open Form";
    //});
    $(".fancybox-community__form-container").toggleClass("expanded", 500, "easeOutSine");
    $(".fancybox-button.fancybox-button--arrow_right").toggleClass("arrow-right-wider", 500, "easeOutSine");
    console.log('toggleclass');
  });

  // gallery contact form toggle icon
  $(document).on('click', '[data-fancybox-contactform]', function () {
    //$(".fancybox-button--contactForm").click(function(){
    $('img', this).toggle();
    //});
  });

  $('.grid').isotope({
    // set itemSelector so .grid-sizer is not used in layout
    itemSelector: '.grid-item',
    percentPosition: true,
    masonry: {
      // use element for option
      columnWidth: '.grid-sizer'
    }
  })

});