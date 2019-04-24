using Chartwell.Feature.MainSearch.Models.PropertyModel;
using Chartwell.Foundation.utility;
using Newtonsoft.Json.Linq;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Sites;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Chartwell.Feature.LocationBasedSearch.Controllers
{
  public class LocationBasedSearchController : Controller
  {
    string constring = ConfigurationManager.ConnectionStrings["SitecoreOLP"].ToString();

    private ChartwellUtiles util = new ChartwellUtiles();

    // GET: LocationBasedSearch
    public ActionResult Index(PropertySearchModel property)
    {
      property.Language = Context.Language.Name;

      return View(property);
    }

    //[HttpPost]
    public JsonResult LatLngSearch(string Latitude, string Longitude)
    {

      //IGeocoder geocoder = new GoogleGeocoder() { ApiKey = "AIzaSyALHmbmlBrfguDPP4ilvnNPs4QftexaS2A" };
      //IEnumerable<Address> addresses = geocoder.ReverseGeocode(double.Parse(Latitude), double.Parse(Longitude));
      //var address = addresses.First().FormattedAddress;

      //var addressDetails = address.Split(',');

      List<string> LocationDetails = new List<string>();
      string GeoCity = string.Empty;
      string Province = string.Empty;
      string PostalCode = string.Empty;
      string CountryCode = string.Empty;

      var propertyItemUrl = string.Empty;
      var propertyName = string.Empty;

      LocationDetails = GeoNameLocation(Latitude, Longitude);
      GeoCity = LocationDetails[0].ToTitleCase();
      Province = LocationDetails[1];
      PostalCode = LocationDetails[2];
      CountryCode = LocationDetails[3];

      //if (Province == "QC")
      //{
      //  Language.TryParse("fr", out Language lang);
      //  Context.SetLanguage(lang, true);
      //  //using (new LanguageSwitcher("fr")) { }
      //}
      //else
      //{
      //  Language.TryParse("en", out Language lang);
      //  Context.SetLanguage(lang, true);
      //}

      if (CountryCode == "CA")
      {
        SqlDataReader rdr = null;
        SqlConnection conn = new SqlConnection(constring);
        SqlCommand cmd = null;
        conn.Open();

        if (string.IsNullOrEmpty(GeoCity))
        {
          cmd = new SqlCommand("sp_SCGetCity", conn)
          {
            CommandType = CommandType.StoredProcedure
          };
          cmd.Parameters.AddWithValue("@PostCode", PostalCode.Trim());
        }
        else
        {
          cmd = new SqlCommand("sp_SCLocSearch", conn)
          {
            CommandType = CommandType.StoredProcedure
          };
          cmd.Parameters.AddWithValue("@PostCode", PostalCode.Trim());
          if (Latitude.Length > 6)
          {
            cmd.Parameters.AddWithValue("@Latitude", Latitude.Substring(0, 6).Trim());
          }
          else
          {
            cmd.Parameters.AddWithValue("@Latitude", Latitude.Trim());
          }
        }
        rdr = cmd.ExecuteReader();

        string currLat = string.Empty;
        string currLng = string.Empty;
        string city = string.Empty;
        if (rdr.HasRows)
        {
          while (rdr.Read())
          {
            city = rdr["City"].ToString().ToTitleCase();
          };
          if (string.IsNullOrEmpty(GeoCity) || (!string.IsNullOrEmpty(GeoCity) && GeoCity != city))
            GeoCity = city;
        }
        else
        {
          if (string.IsNullOrEmpty(GeoCity) && string.IsNullOrEmpty(city))
            GeoCity = string.Empty;
        }

        if (!string.IsNullOrEmpty(GeoCity))
        {

          var sitecoreCity = util.SearchSelectedCity(Context.Language.Name, GeoCity.RemoveDiacritics().Replace(" ", "").Replace("-", "").ToLower())
            .Where(x => x.Name.ToLower() == GeoCity.RemoveDiacritics().Replace(" ", "").Replace("-", "").ToLower()).FirstOrDefault();

          if (sitecoreCity == null)
          {
            sitecoreCity = util.SearchSelectedCity(Context.Language.Name, city.RemoveDiacritics().Replace(" ", "").Replace("-", "").ToLower())
              .Where(x => x.Name.ToLower() == city.RemoveDiacritics().Replace(" ", "").Replace("-", "").ToLower()).FirstOrDefault();
          }

          if (sitecoreCity == null)
          {
            sitecoreCity = util.SearchSelectedCityWithSpaces(Context.Language.Name, city)
              .Where(x => x.Name.ToLower() == city.RemoveDiacritics().TrimPunctuation().ToLower()).FirstOrDefault();
          }

          if (sitecoreCity != null)
          {
            city = sitecoreCity.GetItem().Fields["city name"].Value;
            GeoCity = city;
            currLat = sitecoreCity.GetItem().Fields["Lat"].Value;
            currLng = sitecoreCity.GetItem().Fields["Lng"].Value;

            var closestResidence = ClosestResidenceDetails(GeoCity, currLat, currLng, Context.Language.Name);
            propertyItemUrl = closestResidence.PropertyItemUrl.Replace("/sitecore/shell/chartwell", ""); 
            if (propertyItemUrl.ToLower().Contains("sumach"))
            {
              propertyItemUrl = Request.Url.Scheme + "://" + Request.Url.Host + "/en/the-sumach";
            }
            propertyName = closestResidence.PropertyName;
          }
          else
          {
            GeoCity = string.Empty;
            Latitude = string.Empty;
            Longitude = string.Empty;
          }
        }
      }
      else
      {
        GeoCity = string.Empty;
        Latitude = string.Empty;
        Longitude = string.Empty;
      }
      var property = new PropertySearchModel { City = GeoCity, PropertyItemUrl = propertyItemUrl, PropertyName = propertyName, Latitude = Latitude, Longitude = Longitude };

      return Json(property, JsonRequestBehavior.AllowGet);
    }

    public PropertySearchModel ClosestResidenceDetails(string City, string Lat, string Lng, string Language)
    {

      string strCoordinate = Lat + "," + Lng;
      PropertySearchModel FilteredList = null;

      using (IProviderSearchContext context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        System.Linq.Expressions.Expression<Func<SearchResultItem, bool>> predicate = PredicateBuilder.True<SearchResultItem>();

        predicate = predicate.And(d => d.Language == Language);
        predicate = predicate.And(d => d.TemplateName == "Property Page");
        predicate = predicate.And(d => d["Property ID"] != "99999");
        predicate = predicate.And(d => d.Name != "__Standard Values");
        //predicate = predicate.And(d => d["Selected City"] == City);


        IEnumerable<SearchResultItem> Queryresults = context.GetQueryable<SearchResultItem>()
          .Where(predicate)
          .WithinRadius(s => s.Location, strCoordinate, 500)
          .OrderByDistance(d => d.Location, strCoordinate)
          .ToList();


        FilteredList = (from item in Queryresults
                        select new PropertySearchModel
                        {
                          ItemID = item.ItemId.ToString(),
                          PropertyID = item.GetField("property id").Value,
                          PropertyName = item.GetField("property name").Value,
                          PropertyItemUrl = GetPropertyUrl(Language, item.GetItem()),
                          PropertyType = PropertyType(Language, item.GetItem())

                        }).Where(x => x.PropertyType == "RET").Take(1).FirstOrDefault();

      }
      //return FilteredList.FirstOrDefault().PropertyItemUrl;
      return FilteredList;
    }

    private static List<string> GeoNameLocation(string currLat, string currLng)
    {
      string City = string.Empty;
      string Province = string.Empty;
      string PostalCode = string.Empty;
      string CountryCode = string.Empty;

      List<string> locDetails = new List<string>();
      var url = "https://secure.geonames.org/findNearbyPostalCodesJSON?lat=" + currLat + "&lng=" + currLng + "&maxRows=1&username=shirin";
      WebRequest webrequest = WebRequest.Create(url);

      //WebResponse response = webrequest.GetResponse();
      //using (var stream = new StreamReader(response.GetResponseStream()))
      //{
      //  XDocument xmlDoc = new XDocument();
      //  xmlDoc = XDocument.Parse(stream.ReadToEnd());
      //  City = xmlDoc.Descendants("adminName2").FirstOrDefault().Value;
      //  //if(string.IsNullOrEmpty(City))
      //  //{
      //  //  City = xmlDoc.Descendants("adminName1").FirstOrDefault().Value;
      //  //}
      //}

      using (WebResponse wrs = webrequest.GetResponse())
      using (Stream stream = wrs.GetResponseStream())
      using (StreamReader reader = new StreamReader(stream))
      {
        var json = reader.ReadToEnd();
        JToken token = JObject.Parse(json);

        City = (string)token.SelectToken("postalCodes[0].adminName2");
        if (string.IsNullOrEmpty(City))
        {
          City = (string)token.SelectToken("postalCodes[0].placeName");
          if (string.IsNullOrEmpty(City))
            City = (string)token.SelectToken("postalCodes[0].adminName1");

        }
        locDetails.Add(City);
        Province = (string)token.SelectToken("postalCodes[0].adminCode1");
        locDetails.Add(Province);
        PostalCode = (string)token.SelectToken("postalCodes[0].postalCode");
        locDetails.Add(PostalCode);
        CountryCode = (string)token.SelectToken("postalCodes[0].countryCode");
        locDetails.Add(CountryCode);

        return locDetails;
      }

      //return City;
    }

    private string PropertyType(string Language, Item SearchPropertyItem)
    {

      string combStr = util.PropertyDetails(new ID(SearchPropertyItem.Fields["property type"].ToString())).FirstOrDefault().GetItem().Name;
      string strPropertyTypeID = SearchPropertyItem.Fields["property type"].ToString();
      string strPropertyType = string.Empty;
      if (!string.IsNullOrEmpty(strPropertyTypeID))
      {
        ID PropertyTypeID = new ID(strPropertyTypeID);
        List<SearchResultItem> PropertyTypeSearchItem = util.PropertyDetails(PropertyTypeID).Where(x => x.Language == Language).ToList();
        Item PropertyTypeItem = PropertyTypeSearchItem[0].GetItem();
        strPropertyType = PropertyTypeItem.Fields["property type"].ToString();
      }
      return strPropertyType;
    }

    private string GetPropertyUrl(string Language, Item PropertyItem)
    {
      string itemURL = LinkManager.GetItemUrl(PropertyItem, new UrlOptions
      {
        UseDisplayName = true,
        LowercaseUrls = true,
        AlwaysIncludeServerUrl = false,
        LanguageEmbedding = LanguageEmbedding.Always,
        LanguageLocation = LanguageLocation.FilePath,
        Language = Context.Language //PropertyItem.Language 

    });

      return itemURL + "/" + Translate.Text("overview"); ;
    }
  }
}