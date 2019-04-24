$(document).ready(function () {
  $("#PropertyName").autocomplete({
    source: function (request, response) {
      $.ajax({
        //url: "/sitecore/shell/api/sitecore/Search/SearchProperty",
        url: "/api/sitecore/Search/SearchProperty",
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
});

$(document).ready(function () {
  $("#City").autocomplete({
    //minlength: 4,
    source: function (request, response) {
      $.ajax({
        //url: "/sitecore/shell/api/sitecore/Search/CitySearch",
        url: "/api/sitecore/Search/CitySearch",
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



