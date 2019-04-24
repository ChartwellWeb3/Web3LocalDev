using Chartwell.Feature.Overview.Models;
using Chartwell.Foundation.utility;
using log4net;
using Sitecore.Data;
using Sitecore.Mvc.Presentation;
using Sitecore.Resources.Media;
using System;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.Overview.Controllers
{
  public class OverviewController : Controller
  {
    ChartwellUtiles util = new ChartwellUtiles();

    // GET: Overview
    private OverviewModel CreateModel()
    {
      ILog Logger = LogManager.GetLogger("ChartwellLog");
      var viewModel = new OverviewModel();


      var database = Sitecore.Context.Database;

      var OverviewItemPath = PageContext.Current.Item.Paths.Path;
      var PropertyItemPath = PageContext.Current.Item.Paths.ParentPath;

      var PropertyItem = database.GetItem(PropertyItemPath);

      try
      {
        string strPropertyDesc = PropertyItem.Fields["Property Description"].ToString();

        string strPropertyName = PropertyItem.Fields["Property Name"].ToString();
        var landingPage = 0;
        int.TryParse(PropertyItem.Fields["IsPropertyLandingPage"].Value, out landingPage);
        bool IsLandingPage = landingPage != 0 ? true : false;

        var itemURL = Sitecore.Links.LinkManager.GetItemUrl(PropertyItem);
        string strReviewsPath = itemURL + "/reviews";
        string strPropertyTag = PropertyItem.Fields["Property Tag Line"].ToString();
        string strPropertyAddress = PropertyItem.Fields["Street name and number"].ToString();
        string strPostalCode = PropertyItem.Fields["Postal code"].ToString();

        string strCityName = PropertyItem.Fields["Selected City"].ToString();

        string strProvinceID = PropertyItem.Fields["Province"].ToString();
        ID ProvinceID = new ID(strProvinceID);
        var Provinceitem = database.GetItem(ProvinceID);
        string strProvinceName = Provinceitem.Fields["Province Name"].ToString();


        string strPropertyFormattedAddress = util.FormattedAddress(PropertyItem, strProvinceName);
        string strGoogleReviewKeyword = PropertyItem.Fields["Property Google Reviews Keyword"].ToString();
        string strVideoLink = "";
        string strPhotoLink = "";
        string strBrochureLink = "";
        if (PropertyItem.Fields["YouTubeLink"].HasValue)
        {
          strVideoLink = PropertyItem.Fields["YouTubeLink"].ToString();
        }
        try
        {
          if (PropertyItem.Fields["Property Brochure"].ToString() != string.Empty)
          {
            string strBrochure = PropertyItem.Fields["Property Brochure"].ToString();
            strBrochureLink = HashingUtils.ProtectAssetUrl(Sitecore.StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(database.GetItem(strBrochure))));
          }
        }
        catch { strBrochureLink = ""; }
        string strPropertyTypeID = PropertyItem.Fields["property type"].ToString();
        string strPropertyType = string.Empty;
        if (!string.IsNullOrEmpty(strPropertyTypeID))
        {

          ID PropertyTypeID = new ID(strPropertyTypeID);
          var PropertyTypeItem = database.GetItem(PropertyTypeID);
          strPropertyType = PropertyTypeItem.Fields["property type"].ToString();
        }
        viewModel = new OverviewModel
        {
          PropertyDescription = new HtmlString(strPropertyDesc),
          PropertySelectedLanguage = new HtmlString(PropertyItem.Language.ToString()),
          PropertyName = new HtmlString(strPropertyName),
          PropertyType = strPropertyType,
          PropertyTagLine = new HtmlString(strPropertyTag),
          PropertyAddress = new HtmlString(strPropertyAddress),
          CityName = new HtmlString(strCityName),
          ProvinceName = new HtmlString(strProvinceName),
          PostalCode = new HtmlString(strPostalCode),
          GoogleReviewKeyword = new HtmlString(strGoogleReviewKeyword),
          ReviewURL = strReviewsPath,
          VideoLink = new HtmlString(strVideoLink),
          PropertyPhotoItem = new HtmlString(strPhotoLink),
          BrochureURL = new HtmlString(strBrochureLink),
          PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),
          isLandingpage = IsLandingPage,
          InnerItem = PropertyItem,

        };

      }
      catch (Exception e)
      {

        Logger.Error("Exception occured in Overview Proeprty: " + PropertyItem.Name);
        Logger.Error(e);
      }

      return viewModel;
    }

    public PartialViewResult Index()
    {
      //var database = Sitecore.Context.Database;
      ////var ParentPropertyItemID = Sitecore.Context.Item.Parent.ID;
      //var OverviewItemPath = PageContext.Current.Item.Paths.Path;
      //var PropertyItemPath = PageContext.Current.Item.Paths.ParentPath;

      ////var PropertyItem = util.PropertyDetails(ParentPropertyItemID)[0].GetItem(); 
      //var PropertyItem = database.GetItem(PropertyItemPath);

      //if (PropertyItem.Name.Contains("sumach"))
      //{
      //  string sumachUrl = Request.Url.Scheme + "://" + Request.Url.Host + "/en/" + "the-sumach";
      //  return Redirect(sumachUrl);
      //}
      return PartialView("~/Views/Overview/Index.cshtml", CreateModel());

    }

  }
}