function onPositionUpdate(e){var a=getApiLocation();console.log(a);var t=e.coords.latitude,o=e.coords.longitude,r=$("#Language").val();$.ajax({url:a+"LocationBasedSearch/LatLngSearch",type:"GET",dataType:"json",async:!0,data:{Latitude:t,Longitude:o,LocContextLang:r},success:function(e){if(null!==e.City&&""!==e.City){console.log("postal"+$("#PostalCode").val()),console.log("property"+$("#PropertyName").val()),""==$("#PostalCode").val()&&""==$("#PropertyName").val()&&$("#City").val(e.City);var a=$("<a>");a.attr("href",e.PropertyItemUrl),a.text(e.PropertyName),a.addClass("locDiv link"),$("#locDiv .closest").html(a),$("#nearResidenceId").removeClass("hide")}}})}!function(e){"function"==typeof define&&define.amd?define(["../widgets/datepicker"],e):e(jQuery.datepicker)}(function(e){return e.regional.fr={closeText:"Fermer",prevText:"Précédent",nextText:"Suivant",currentText:"Aujourd'hui",monthNames:["janvier","février","mars","avril","mai","juin","juillet","août","septembre","octobre","novembre","décembre"],monthNamesShort:["janv.","févr.","mars","avr.","mai","juin","juil.","août","sept.","oct.","nov.","déc."],dayNames:["dimanche","lundi","mardi","mercredi","jeudi","vendredi","samedi"],dayNamesShort:["dim.","lun.","mar.","mer.","jeu.","ven.","sam."],dayNamesMin:["D","L","M","M","J","V","S"],weekHeader:"Sem.",dateFormat:"yy-mm-dd",firstDay:1,isRTL:!1,showMonthAfterYear:!1,buttonImage:"/assets/images/layout/icon-calandar.png",showOn:"both",minDate:1,changeYear:!0,changeMonth:!0,beforeShowDay:$.datepicker.noWeekends,yearSuffix:""},e.setDefaults(e.regional.fr),e.regional.fr}),function(e){"function"==typeof define&&define.amd?define(["../widgets/datepicker"],e):e(jQuery.datepicker)}(function(e){return e.regional.en={closeText:"Done",prevText:"Prev",nextText:"Next",currentText:"Today",monthNames:["January","February","March","April","May","June","July","August","September","October","November","December"],monthNamesShort:["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"],dayNames:["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"],dayNamesShort:["Sun","Mon","Tue","Wed","Thu","Fri","Sat"],dayNamesMin:["Su","Mo","Tu","We","Th","Fr","Sa"],weekHeader:"Wk",dateFormat:"mm/dd/yy",firstDay:1,showOn:"both",buttonImage:"/assets/images/layout/icon-calandar.png",minDate:1,changeYear:!0,changeMonth:!0,beforeShowDay:$.datepicker.noWeekends,isRTL:!1,showMonthAfterYear:!1,yearSuffix:""},e.setDefaults(e.regional.en),e.regional.en}),$("#City, #PostalCode, #PropertyName").blur(function(){$("#City, #PostalCode, #PropertyName").prop("readonly",!1).prop("disabled",!1)}),$("#City").click(function(){$("#PostalCode, #PropertyName").prop("readonly",!0).prop("disabled",!0).addClass("inactiveSearchField").removeClass("activeSearchField").val(""),$(this).prop("disabled",!1).removeClass("inactiveSearchField").addClass("activeSearchField")}),$("#PostalCode").click(function(){$("#City, #PropertyName").prop("readonly",!0).prop("disabled",!0).addClass("inactiveSearchField").removeClass("activeSearchField").val(""),$(this).prop("disabled",!1).removeClass("inactiveSearchField").addClass("activeSearchField")}),$("#PropertyName").click(function(){$("#City, #PostalCode").prop("readonly",!0).prop("disabled",!0).addClass("inactiveSearchField").removeClass("activeSearchField").val(""),$(this).prop("disabled",!1).removeClass("inactiveSearchField").addClass("activeSearchField")}),$(window).on("load resize",function(){$(window).width()<992?($(".btn-group-vertical").addClass("btn-group btn-group-lg"),$(".btn-group-vertical").removeClass("btn-group-vertical")):($(".btn-group").addClass("btn-group-vertical"),$(".btn-group-vertical").removeClass("btn-group btn-group-lg"))}),$(".gridCity .panel-primary > a > img, .gridCity .panel-heading, .gridCity .viewResBtn > a").hover(function(){$(this).closest(".panel").addClass("panelSearchHover")},function(){$(this).closest(".panel").removeClass("panelSearchHover")}),$(".splitPageWrap .panel-assisted > a > img, .splitPageWrap .panel-assisted .panel-heading, .splitPageWrap .panel-assisted .viewResBtn > a").hover(function(){$(this).closest(".panel").addClass("panelSearchHoverAssisted")},function(){$(this).closest(".panel").removeClass("panelSearchHoverAssisted")}),$(document).ready(function(){$("#visitdate").attr("readonly","readonly").datepicker(),document.getElementById("bgPropertyImage")&&($(window).width(),-1<$("#bgPropertyImage").attr("style").indexOf("none")&&($("#bgPropertyImage").css("height","50px"),$(".resContainerUp").css("margin-top","-45px")))}),$(".gridCity > .col-md-4:nth-child(3n+1)").css("clear","left"),$('*[data-ajax-mode="replace"]').click(function(){setTimeout(function(){$(".gridCity > .col-md-4:nth-child(3n+1)").css("clear","left")},4e3)}),navigator.geolocation?navigator.geolocation.getCurrentPosition(onPositionUpdate):alert("navigator.geolocation is not available"),$(".dropdown-toggle").dropdown(),$("#carouselNewDevelopment").carousel(),$(".responsive-tabs").responsiveTabs({accordionOn:["xs","sm"]}),$(window).on("load resize",function(){$(window).width()<992?($(".btn-group-vertical").addClass("btn-group btn-group-lg"),$(".btn-group-vertical").removeClass("btn-group-vertical")):($(".btn-group").addClass("btn-group-vertical"),$(".btn-group-vertical").removeClass("btn-group btn-group-lg"))}),$(document).ready(function(){var e=location.host,a=location.pathname.replace("photos","Contactus"),t="https://"+e+a,o=a,r=0;$('[data-fancybox="images"]').fancybox({src:o,type:"iframe",openEffect:"elastic",closeEffect:"elastic",nextSpeed:0,prevSpeed:0,margin:[44,0,22,0],thumbs:{autoStart:!0,axis:"x"},beforeShow:function(){0==r&&($("#Chartwelliframe").attr("src",t),r++)},afterShow:function(e,a){(r=0,$(window).width()<992)?($(".fancybox-community__form-container").removeClass("expanded"),$(".fancybox-button--arrow_right").addClass("arrow-right-wider"),$("[data-fancybox-contactform]").text("Open Form"),console.log("windows size is small")):($("fancybox-button--arrow_right").removeClass("arrow-right-wider "),console.log("windows size is big right"),$("#Chartwelliframe").contents().find("body").attr("style","background:#e1f1ff"),$("#Chartwelliframe").contents().find("footer").attr("style","border:none"))}}),$(document).on("click","[data-fancybox-contactform]",function(){$(".fancybox-community__form-container").toggleClass("expanded",500,"easeOutSine"),$(".fancybox-button.fancybox-button--arrow_right").toggleClass("arrow-right-wider",500,"easeOutSine"),console.log("toggleclass")}),$(document).on("click","[data-fancybox-contactform]",function(){$("img",this).toggle()}),$(".grid").isotope({itemSelector:".grid-item",percentPosition:!0,masonry:{columnWidth:".grid-sizer"}})});