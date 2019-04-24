using Chartwell.Feature.DiningService.Models;
using Chartwell.Foundation.utility;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using Sitecore.Resources.Media;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Linq;
namespace Chartwell.Feature.DiningService.Controllers
{
  public class DiningServiceController : Controller
    {
        // GET: DiningService
        private DiningModel CreateModel()
        {


            var database = Sitecore.Context.Database;
            ChartwellUtiles util = new ChartwellUtiles();
            //string selPropertyName = "";
            //if (WebUtil.GetUrlName(0).ToLower().Contains("dining") || WebUtil.GetUrlName(0).Contains("manger"))
            //    selPropertyName = WebUtil.GetUrlName(1);
            //else if (WebUtil.GetUrlName(1).ToLower().Contains("dining") || WebUtil.GetUrlName(1).Contains("manger"))
            //    selPropertyName = WebUtil.GetUrlName(0);

            //var DiningItem = database.GetItem("/sitecore/content/Chartwell/property Page/" + selPropertyName);
            var DiningItemPath = PageContext.Current.Item.Paths.Path;
            var PropertyItemPath = PageContext.Current.Item.Paths.ParentPath;
            var DiningItem = database.GetItem(PropertyItemPath);
            var DiningTemplateItem = database.GetItem(DiningItemPath);
            string sitecoreid = DiningItem.ID.ToString();
           
            string strDiningTitle = DiningItem.Fields["Dining Section Title"].ToString();
            string strDiningDesc = DiningItem.Fields["Dining Section Description"].ToString();
            string strPropertyName = DiningItem.Fields["Property Name"].ToString();
            string strPropertyTag = DiningItem.Fields["Property Tag Line"].ToString();
            string strPropertyAddress = DiningItem.Fields["Street name and number"].ToString();
            string strPostalCode = DiningItem.Fields["Postal code"].ToString();

            string strCityName = DiningItem.Fields["Selected City"].ToString();
            //ID CityID = new ID(strCityID);
            //var Cityitem = database.GetItem(CityID);
            //string strCityName = Cityitem.Fields["City Name"].ToString();

            string strProvinceID = DiningItem.Fields["Province"].ToString();
            ID ProvinceID = new ID(strProvinceID);
            var Provinceitem = database.GetItem(ProvinceID);
            string strProvinceName = Provinceitem.Fields["Province Name"].ToString();
            string strPropertyFormattedAddress = util.FormattedAddress(DiningItem, strProvinceName);
            List<Item> lstDiningService = new List<Item>();
            Sitecore.Data.Fields.MultilistField DiningList = DiningItem.Fields["Dining Service"];


            string strBrochureLink = "";
            try
            {
                if (DiningItem.Fields["Menus"].ToString() != string.Empty)
                {
                    string strBrochure = DiningItem.Fields["Menus"].ToString();
                    strBrochureLink = HashingUtils.ProtectAssetUrl(Sitecore.StringUtil.EnsurePrefix('/', Sitecore.Resources.Media.MediaManager.GetMediaUrl(database.GetItem(strBrochure))));
                }
            }
            catch { strBrochureLink = ""; }
            if (DiningList != null && DiningList.TargetIDs != null)
            {

                foreach (ID id in DiningList.TargetIDs)

                {

                    Item targetItem = Sitecore.Context.Database.Items[id];
                    lstDiningService.Add(targetItem);

                }
            }
            var viewModel = new DiningModel
            {
                PropertyGuid = new HtmlString(sitecoreid),
                DiningTitle = new HtmlString(strDiningTitle),
                DiningDescription = new HtmlString(strDiningDesc),
                PropertyName = new HtmlString(strPropertyName),
                PropertyTagLine = new HtmlString(strPropertyTag),
                PropertyAddress = new HtmlString(strPropertyAddress),
                CityName = new HtmlString(strCityName),
                ProvinceName = new HtmlString(strProvinceName),
                PostalCode = new HtmlString(strPostalCode),
                InnerItem = DiningItem,
                BrochureURL = new HtmlString(strBrochureLink),
                DiningServiceItem = lstDiningService.OrderBy(o => o.Name).ToList(),
                PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),

                TemplateItem = DiningTemplateItem
            };
            return viewModel;
        }

        public PartialViewResult Index()
        {


            return PartialView("~/Views/DiningService/Dining.cshtml", CreateModel());
            //  return View(viewModel);
            //return PartialView(CreateModel());
        }
    }
}