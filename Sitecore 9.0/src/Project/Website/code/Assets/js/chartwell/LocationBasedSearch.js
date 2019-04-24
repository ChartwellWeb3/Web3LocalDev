if (navigator.geolocation) {
  navigator.geolocation.getCurrentPosition(onPositionUpdate);
}
else
  alert("navigator.geolocation is not available");

function onPositionUpdate(position) {

  var ApiLocation = getApiLocation(); //function is in autocomplete.js
  console.log(ApiLocation);

  var Lat = position.coords.latitude;
  var Lng = position.coords.longitude;
  var contextLang = $('#Language').val();
  
  $.ajax({
    url: ApiLocation + "LocationBasedSearch/LatLngSearch",
   
    type: "GET",
    dataType: "json",
    async: true,
    data: { Latitude: Lat, Longitude: Lng, LocContextLang: contextLang },
    success: function (data) {
      if (data.City !== null && data.City !== "") {

        console.log("postal" + $("#PostalCode").val());
        console.log("property" + $("#PropertyName").val());

        if ( $("#PostalCode").val()=="" && $("#PropertyName").val() == "") {
          $("#City").val(data.City);
        }


        var link = $("<a>");
        link.attr("href", data.PropertyItemUrl);
        link.text(data.PropertyName);
        link.addClass("locDiv link");
        $("#locDiv .closest").html(link);
        $('#nearResidenceId').removeClass('hide');
      }
    }
  });

}

