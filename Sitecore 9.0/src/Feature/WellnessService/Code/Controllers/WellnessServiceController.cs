using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Web;
using Sitecore.Data;
using Sitecore.Mvc.Presentation;
using Sitecore.Data.Items;
using Chartwell.Feature.WellnessService.Models;
using Chartwell.Foundation.utility;

namespace Chartwell.Feature.WellnessService.Controllers
{
    public class WellnessServiceController : Controller
    {
        // GET: WellnessService
        private WellnessServiceModel CreateModel()
        {


            var database = Sitecore.Context.Database;
            ChartwellUtiles util = new ChartwellUtiles();
            //string selPropertyName = "";
            //if (WebUtil.GetUrlName(0).ToLower() == "wellness" || WebUtil.GetUrlName(0).ToLower() == "wellness services" || WebUtil.GetUrlName(0).Contains("services"))
            //    selPropertyName = WebUtil.GetUrlName(1);
            //else if (WebUtil.GetUrlName(1) == "wellness" || WebUtil.GetUrlName(1).ToLower() == "wellness services" || WebUtil.GetUrlName(1) == "services de mieux-être")
            //   selPropertyName = WebUtil.GetUrlName(0);

            //var WellnessItem = database.GetItem("/sitecore/content/Chartwell/property Page/" + selPropertyName);

            var WellnessItemPath = PageContext.Current.Item.Paths.Path;
            var PropertyItemPath = PageContext.Current.Item.Paths.ParentPath;
            var WellnessItem = database.GetItem(PropertyItemPath);
            var WellnessTemplateItem = database.GetItem(WellnessItemPath);
            string sitecoreid = WellnessItem.ID.ToString();

            string strWellnessTitle = WellnessItem.Fields["Wellness Section Title"].ToString();
            string strWellnessName = WellnessItem.Fields["Wellness Section Description"].ToString();
            string strPropertyName = WellnessItem.Fields["Property Name"].ToString();
            string strPropertyTag = WellnessItem.Fields["Property Tag Line"].ToString();
            string strPropertyAddress = WellnessItem.Fields["Street name and number"].ToString();
            string strPostalCode = WellnessItem.Fields["Postal code"].ToString();

            //string strCityID = WellnessItem.Fields["City"].ToString();
            //ID CityID = new ID(strCityID);
            //var Cityitem = database.GetItem(CityID);
            //string strCityName = Cityitem.Fields["City Name"].ToString();
            string strCityName = WellnessItem.Fields["Selected City"].ToString();
            string strProvinceID = WellnessItem.Fields["Province"].ToString();
            ID ProvinceID = new ID(strProvinceID);
            var Provinceitem = database.GetItem(ProvinceID);
            string strProvinceName = Provinceitem.Fields["Province Name"].ToString();
            string strPropertyFormattedAddress = util.FormattedAddress(WellnessItem, strProvinceName);
            List<Item> lstWellnessName = new List<Item>();
            Sitecore.Data.Fields.MultilistField WellnessList = WellnessItem.Fields["Property Wellness"];

            if (WellnessList != null && WellnessList.TargetIDs != null)
            {

                foreach (ID id in WellnessList.TargetIDs)

                {

                    Item targetItem = Sitecore.Context.Database.Items[id];
                    lstWellnessName.Add(targetItem);


                }
            }
            var viewModel = new WellnessServiceModel
            {
                WellnessTitle = new HtmlString(strWellnessTitle),
                WellnessName = new HtmlString(strWellnessName),
                PropertyName = new HtmlString(strPropertyName),
                PropertyTagLine = new HtmlString(strPropertyTag),
                PropertyAddress = new HtmlString(strPropertyAddress),
                CityName = new HtmlString(strCityName),
                ProvinceName = new HtmlString(strProvinceName),
                PostalCode = new HtmlString(strPostalCode),
                InnerItem = WellnessItem,
                PropertyGuid = new HtmlString(sitecoreid),
                WellnessItem = lstWellnessName.OrderBy(o => o.Name).ToList(),
                PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),

                TemplateItem = WellnessTemplateItem
            };
            return viewModel;
        }

        public PartialViewResult Index()
        {


            return PartialView("~/Views/WellnessService/Wellness.cshtml", CreateModel());
            //  return View(viewModel);
            //return PartialView(CreateModel());
        }
    }
}