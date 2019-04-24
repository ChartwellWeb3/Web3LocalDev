using Chartwell.Feature.NearbyResidences.Models;
using Chartwell.Foundation.utility;
using log4net;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.NearbyResidences.Controllers
{
  public class NearbyResidencesController : Controller
  {
    //string constring = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString();
    // GET: NearbyResidences

    public ActionResult Index()
    {
      ILog Logger = LogManager.GetLogger("ChartwellLog");
      List<NearbyResidencesModel> propertyList = new List<NearbyResidencesModel>();


      var database = Sitecore.Context.Database;


      var NearbyItemPath = PageContext.Current.Item.Paths.Path;
      var PropertyItemPath = PageContext.Current.Item.Paths.ParentPath;

      var PropertyItem = database.GetItem(PropertyItemPath);
      //  var PropertyItemUrl = Sitecore.Links.LinkManager.GetItemUrl(PropertyItem) + "/Overview";
      string SelectedLanguage = PropertyItem.Language.ToString();
      string selectedProperty = PropertyItem.Fields["Property Name"].ToString();
      string strLatitude = PropertyItem.Fields["Latitude"].ToString();
      string strLongitude = PropertyItem.Fields["Longitude"].ToString();
      string strCoordinate = strLatitude + "," + strLongitude;
      var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext();
      var Queryresults = context.GetQueryable<SearchResultItem>()
                         .Where(item => item.Language == SelectedLanguage)

                        .WithinRadius(s => s.Location, strCoordinate, 60)
                        .OrderByDistance(s => s.Location, strCoordinate);

      if (Queryresults.ToList().Count < 5)
      {
        Queryresults.ToList().Clear();
        Queryresults = context.GetQueryable<SearchResultItem>()
                         .Where(item => item.Language == SelectedLanguage)

                        .WithinRadius(s => s.Location, strCoordinate, 1000)
                        .OrderByDistance(s => s.Location, strCoordinate);
      }
      var listResult = Queryresults.ToList();
      string header = "Nearby Residences";
      if (SelectedLanguage != "en")
        header = "Résidences à proximité";
      NearbyResidencesModel results;


      int i = 0;
      foreach (SearchResultItem row in listResult)
      {
        var NearbyPropertyItem = database.GetItem(row.ItemId);
        if (i <= 4)
        {
          try
          {
            ChartwellUtiles util = new ChartwellUtiles();

            var t = Sitecore.Links.LinkManager.GetItemUrl(NearbyPropertyItem, new UrlOptions
            {
              UseDisplayName = true,
              LowercaseUrls = true,
              LanguageEmbedding = LanguageEmbedding.Always,
              LanguageLocation = LanguageLocation.FilePath,
              Language = Sitecore.Context.Language

            });
            string strProvinceID = NearbyPropertyItem.Fields["Province"].ToString();
            ID ProvinceID = new Sitecore.Data.ID(strProvinceID);
            var Provinceitem = database.GetItem(ProvinceID);
            string strProvinceName = Provinceitem.Fields["Province Name"].ToString();
            string strNearbyPropName = NearbyPropertyItem.Fields["Property Name"].ToString();

            string FormattedAddress = util.FormattedAddress(NearbyPropertyItem, strProvinceName);
            string strPhone = util.GetPhoneNumber(NearbyPropertyItem);
            string strPropertyTypeID = NearbyPropertyItem.Fields["property type"].ToString();
            string strPropertyType = string.Empty;


            if (!string.IsNullOrEmpty(strPropertyTypeID))
            {
              if (strPropertyTypeID.Contains("RET"))
                strPropertyType = "RET";
              else
              {
                ID PropertyTypeID = new ID(strPropertyTypeID);
                var PropertyTypeItem = database.GetItem(PropertyTypeID);
                strPropertyType = PropertyTypeItem.Fields["property type"].ToString();
              }
            }
            Double dbDistance = distance(double.Parse(strLatitude, CultureInfo.InvariantCulture), double.Parse(strLongitude, CultureInfo.InvariantCulture), row.Location.Latitude, row.Location.Longitude, 'K');
            results = new NearbyResidencesModel
            {

              PropertyName = new HtmlString(NearbyPropertyItem.Fields["Property Name"].ToString()),
              PageHeader = new HtmlString(header),
              PropertyFormattedAddress = new HtmlString(FormattedAddress),
              PhoneNo = new HtmlString(strPhone),
              PropertyItemUrl = !t.Contains("sumach") ? t + "/" + util.GetDictionaryItem("overview", Sitecore.Context.Language.Name) : Request.Url.Scheme + "://" + Request.Url.Host + "/en/the-sumach",
              Distance = dbDistance,
              InnerItem = NearbyPropertyItem
            };
            if (strPropertyType != "LTC" && strNearbyPropName != selectedProperty)
            {
              propertyList.Add(results);
              i++;
            }
          }
          catch (Exception e)
          {

            Logger.Error("Exception occured for Property" + NearbyPropertyItem.Name);
            Logger.Error(e);
          }
        }

      }

      // MapDA mapDA = new MapDA(constring);
      //    mapDS = mapDA.GetPropertyDistance(float.Parse(strLatitude), float.Parse(strLongitude), 4);

      // DataTable dt = mapDS.Tables[0];
      //var results = from row in dt.AsEnumerable()
      //              select new NearbyResidencesModel
      //              {
      //                PropertyName = new HtmlString(row.Field<string>("PropertyName")),
      //                PropertyAddress = new HtmlString(row.Field<string>("StreetAddress")),
      //                CityName = new HtmlString(row.Field<string>("City")),
      //                ProvinceName = new HtmlString(row.Field<string>("Province")),
      //                PostalCode = new HtmlString(row.Field<string>("PostalCode")),
      //                PhoneNo = new HtmlString(row.Field<string>("PhoneNo")),
      //                Distance = (row.Field<Double>("dis")),
      //                PropertyItemUrl = PropertyItemUrl
      //              };





      return View(propertyList);
    }


    public double distance(double lat1, double lon1, double lat2, double lon2, char unit)
    {
      double theta = lon1 - lon2;
      double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
      dist = Math.Acos(dist);
      dist = rad2deg(dist);
      dist = dist * 60 * 1.1515;
      if (unit == 'K')
      {
        dist = dist * 1.609344;
      }
      else if (unit == 'N')
      {
        dist = dist * 0.8684;
      }
      return (dist);
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  This function converts decimal degrees to radians             :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    private double deg2rad(double deg)
    {
      return (deg * Math.PI / 180.0);
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  This function converts radians to decimal degrees             :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    private double rad2deg(double rad)
    {
      return (rad / Math.PI * 180.0);
    }

  }
}
