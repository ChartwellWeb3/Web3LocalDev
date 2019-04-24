function getApiLocation() {
  var autoCompleteAPILocationPrepend = '/sitecore/shell/api/sitecore/';
  if (document.location.hostname == 'chartwellcm' || document.location.hostname.indexOf('lab.') != -1) {
    autoCompleteAPILocationPrepend = '/api/sitecore/';
  }
  return autoCompleteAPILocationPrepend;
}

$(document).ready(function () {
  var ApiLocation = getApiLocation();
  console.log(ApiLocation);

  $("#PropertyName").autocomplete({
    source: function (request, response) {
      $.ajax({
        url: ApiLocation + "Search/SearchProperty",
        type: "POST",
        dataType: "json",
        data: { Prefix: request.term },
        success: function (data) {
          response($.map(data, function (item) {
            var propertyName = item.PropertyName;
            return { label: propertyName, value: propertyName };
          }));

        }
      });
    }
  });

  $("#City").autocomplete({
    //minlength: 4,
    source: function (request, response) {
      $.ajax({
        url: ApiLocation + "Search/CitySearch",
        type: "POST",
        dataType: "json",
        data: { term: request.term, lang: $('#Language').val() },
        success: function (data) {
          response($.map(data, function (item) {
            var cityName = item.City;
            return { label: cityName, value: cityName };
          }));
        }
      });
    }
  });
});



