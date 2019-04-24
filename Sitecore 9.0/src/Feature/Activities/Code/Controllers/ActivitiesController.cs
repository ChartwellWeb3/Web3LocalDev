using Chartwell.Feature.Activities.Models;
using Chartwell.Foundation.utility;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using Sitecore.Resources.Media;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Linq;
namespace Chartwell.Feature.Activities.Controllers
{
  public class ActivitiesController : Controller
    {
        // GET: Activities
        private ActivitiesModel CreateModel()
        {


            var database = Sitecore.Context.Database;
            //string selPropertyName = "";
            ChartwellUtiles util = new ChartwellUtiles();
            //if (WebUtil.GetUrlName(0).ToLower() == "activities" || WebUtil.GetUrlName(0).Contains("vie active"))
            //    selPropertyName = WebUtil.GetUrlName(1);
            //else if (WebUtil.GetUrlName(1).ToLower() == "activities" || WebUtil.GetUrlName(1).Contains("vie active"))
            //    selPropertyName = WebUtil.GetUrlName(0);

            //var ActivitiesItem = database.GetItem("/sitecore/content/Chartwell/property Page/" + selPropertyName);
            var ActivitiesItemPath = PageContext.Current.Item.Paths.Path;
            var PropertyItemPath = PageContext.Current.Item.Paths.ParentPath;
            var ActivitiesItem = database.GetItem(PropertyItemPath);
            var ActivitiesTemplateItem = database.GetItem(ActivitiesItemPath);
            string sitecoreid = ActivitiesItem.ID.ToString();
            string strActivitiesDescription = ActivitiesItem.Fields["Activities Section Title"].ToString();
            string strPropertyName = ActivitiesItem.Fields["Property Name"].ToString();
            string strPropertyTag = ActivitiesItem.Fields["Property Tag Line"].ToString();
            string strPropertyAddress = ActivitiesItem.Fields["Street name and number"].ToString();
            string strPostalCode = ActivitiesItem.Fields["Postal code"].ToString();
          
            string strActivitiesDetailDescription = ActivitiesItem.Fields["Activities Section Description"].ToString();
            if (string.IsNullOrEmpty(strActivitiesDetailDescription))
                strActivitiesDetailDescription = "";
             string strCityName = ActivitiesItem.Fields["Selected City"].ToString();
            //ID CityID = new ID(strCityID);
            //var Cityitem = database.GetItem(CityID);
            //string strCityName = Cityitem.Fields["City Name"].ToString();

            string strProvinceID = ActivitiesItem.Fields["Province"].ToString();
            ID ProvinceID = new ID(strProvinceID);
            var Provinceitem = database.GetItem(ProvinceID);
            string strProvinceName = Provinceitem.Fields["Province Name"].ToString();
            string strPropertyFormattedAddress = util.FormattedAddress(ActivitiesItem, strProvinceName);
            List<Item> lstActivitiesName = new List<Item>();
            Sitecore.Data.Fields.MultilistField ActivityList = ActivitiesItem.Fields["Property Activities"];
            string strBrochureLink = "";

            try
            {
                if (ActivitiesItem.Fields["Property Activity Calendar"].ToString() != string.Empty)
                {
                    string strBrochure = ActivitiesItem.Fields["Property Activity Calendar"].ToString();
                    strBrochureLink = HashingUtils.ProtectAssetUrl(Sitecore.StringUtil.EnsurePrefix('/', Sitecore.Resources.Media.MediaManager.GetMediaUrl(database.GetItem(strBrochure))));
                }
            }
            catch
            {
                strBrochureLink = "";

            }

            if (ActivityList != null && ActivityList.TargetIDs != null)
            {

                foreach (ID id in ActivityList.TargetIDs)

                {

                    Item targetItem = Sitecore.Context.Database.Items[id];
                    lstActivitiesName.Add(targetItem);




                }
            }
            var viewModel = new ActivitiesModel
            {
                PropertyGuid = new HtmlString(sitecoreid),
                ActivitiesName = new HtmlString(strActivitiesDescription),
                PropertyName = new HtmlString(strPropertyName),
                PropertyTagLine = new HtmlString(strPropertyTag),
                PropertyAddress = new HtmlString(strPropertyAddress),
                CityName = new HtmlString(strCityName),
                ProvinceName = new HtmlString(strProvinceName),
                PostalCode = new HtmlString(strPostalCode),
                InnerItem = ActivitiesItem,
                ActivitiesItem = lstActivitiesName.OrderBy(o=>o.Name).ToList(),
                BrochureURL = new HtmlString(strBrochureLink),
                PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),
                ActivitiesDescription= new HtmlString(strActivitiesDetailDescription),
                TemplateItem = ActivitiesTemplateItem
            };
            return viewModel;
        }

        public PartialViewResult Index()
        {


            return PartialView("~/Views/Activities/Activities.cshtml", CreateModel());

        }

        public PartialViewResult SuitePlan()
        {


            return PartialView("~/Views/Suite/Suite.cshtml", CreateHeader());

        }

        public PartialViewResult Events()
        {


            return PartialView("~/Views/Events/Events.cshtml", CreateHeader());

        }

        private ActivitiesModel CreateHeader()
        {


            var database = Sitecore.Context.Database;
           
            ChartwellUtiles util = new ChartwellUtiles();
           
            var TemplateItemPath = PageContext.Current.Item.Paths.Path;
            var PropertyItemPath = PageContext.Current.Item.Paths.ParentPath;
            var PropertyItem = database.GetItem(PropertyItemPath);
           
          
         
            string strPropertyName = PropertyItem.Fields["Property Name"].ToString();
            string strPropertyTag = PropertyItem.Fields["Property Tag Line"].ToString();
          

            string strProvinceID = PropertyItem.Fields["Province"].ToString();
            ID ProvinceID = new ID(strProvinceID);
            var Provinceitem = database.GetItem(ProvinceID);
            string strProvinceName = Provinceitem.Fields["Province Name"].ToString();
            string strPropertyFormattedAddress = util.FormattedAddress(PropertyItem, strProvinceName);
          
            var viewModel = new ActivitiesModel
            {
             
             
                PropertyName = new HtmlString(strPropertyName),
                PropertyTagLine = new HtmlString(strPropertyTag),
                InnerItem = PropertyItem,
                PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress)

                
            };
            return viewModel;
        }
    }
}