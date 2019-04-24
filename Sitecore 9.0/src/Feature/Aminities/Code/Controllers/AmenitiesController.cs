using Chartwell.Feature.Amenities.Models;
using Chartwell.Foundation.utility;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.Amenities.Controllers
{
  public class AmenitiesController : Controller
    {
        // GET: Amenities
        private AmenitiesModel CreateModel()
        {
            var database = Sitecore.Context.Database;
            //string selPropertyName = "";
            ChartwellUtiles util = new ChartwellUtiles();
            //if (WebUtil.GetUrlName(0).ToLower() == "amenities" || WebUtil.GetUrlName(0) == "aménagements")
            //    selPropertyName = WebUtil.GetUrlName(1);
            //else if (WebUtil.GetUrlName(1).ToLower() == "amenities" || WebUtil.GetUrlName(1) == "aménagements")
            //    selPropertyName = WebUtil.GetUrlName(0);

            //var PropertyItem = database.GetItem("/sitecore/content/Chartwell/property Page/" + selPropertyName);
            var AmenitiesItemPath = PageContext.Current.Item.Paths.Path;
            var PropertyItemPath = PageContext.Current.Item.Paths.ParentPath;
            var PropertyItem = database.GetItem(PropertyItemPath);
            var AmentitiesTemplateItem = database.GetItem(AmenitiesItemPath);
            string sitecoreid = PropertyItem.ID.ToString();
            string strPropertyName = PropertyItem.Fields["Property Name"].ToString();
            string strPropertyTag = PropertyItem.Fields["Property Tag Line"].ToString();
            string strPropertyAddress = PropertyItem.Fields["Street name and number"].ToString();
            string strPostalCode = PropertyItem.Fields["Postal code"].ToString();

            //string strCityID = PropertyItem.Fields["City"].ToString();
            //ID CityID = new ID(strCityID);
            //var Cityitem = database.GetItem(CityID);
            //string strCityName = Cityitem.Fields["City Name"].ToString();
            string strCityName = PropertyItem.Fields["Selected City"].ToString();
            string strProvinceID = PropertyItem.Fields["Province"].ToString();
            ID ProvinceID = new ID(strProvinceID);
            var Provinceitem = database.GetItem(ProvinceID);
            string strProvinceName = Provinceitem.Fields["Province Name"].ToString();
            string strPropertyFormattedAddress = util.FormattedAddress(PropertyItem, strProvinceName);

            string strAmenitiesTitle = PropertyItem.Fields["Amenities Section Title"].ToString();
            string strAmenitiesDesc = PropertyItem.Fields["Amenities Section Description"].ToString();
            List<Item> lstAmenities = new List<Item>();
            Sitecore.Data.Fields.MultilistField AmenitiesList = PropertyItem.Fields["Property Amenities"];

            if (AmenitiesList != null && AmenitiesList.TargetIDs != null)
            {

                foreach (ID id in AmenitiesList.TargetIDs)

                {

                    Item targetItem = Sitecore.Context.Database.Items[id];
                    lstAmenities.Add(targetItem);


                }
            }
            var viewModel = new AmenitiesModel
            {
                PropertyGuid = new HtmlString(sitecoreid),
                PropertyName = new HtmlString(strPropertyName),
                PropertyTagLine = new HtmlString(strPropertyTag),
                PropertyAddress = new HtmlString(strPropertyAddress),
                CityName = new HtmlString(strCityName),
                ProvinceName = new HtmlString(strProvinceName),
                PostalCode = new HtmlString(strPostalCode),
                AmenitiesTitle = new HtmlString(strAmenitiesTitle),
                AmenitiesDescription = new HtmlString(strAmenitiesDesc),
                AmenitiesItem = lstAmenities.OrderBy(o => o.Name).ToList(),
                InnerItem = PropertyItem,
                PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),
               
                TemplateItem = AmentitiesTemplateItem
            };
            return viewModel;
        }

        public PartialViewResult Index()
        {

            return PartialView("~/Views/Amenities/Amenities.cshtml", CreateModel());
        }
    }
}