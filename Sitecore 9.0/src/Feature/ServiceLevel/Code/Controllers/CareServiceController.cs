using Chartwell.Feature.ServiceLevel.Models;
using Chartwell.Foundation.utility;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.ServiceLevel.Controllers
{
  public class CareServiceController : Controller
    {
        // GET: CareService
        private CareServiceModel CreateModel()
        {


            var database = Sitecore.Context.Database;
            //string selPropertyName = "";
            ChartwellUtiles util = new ChartwellUtiles();
            //if (WebUtil.GetUrlName(0) == "servicelevels" || WebUtil.GetUrlName(0).Contains("types de résidences")|| WebUtil.GetUrlName(0) == "CareService")
            //    selPropertyName = WebUtil.GetUrlName(1);
            //else if (WebUtil.GetUrlName(1) == "servicelevels" || WebUtil.GetUrlName(0).Contains("types de résidences") || WebUtil.GetUrlName(1) == "CareService")
            //    selPropertyName = WebUtil.GetUrlName(0);

            //var CareServiceItem = database.GetItem("/sitecore/content/Chartwell/property Page/" + selPropertyName);
            var CareServiceItemPath = PageContext.Current.Item.Paths.Path;
            var PropertyItemPath = PageContext.Current.Item.Paths.ParentPath;
            var CareServiceItem = database.GetItem(PropertyItemPath);
            var CareServiceTemplateItem = database.GetItem(CareServiceItemPath);
            string sitecoreid = CareServiceItem.ID.ToString();
            string _CareServiceDatabase = string.Empty;
            string strCareTitle = CareServiceItem.Fields["Care Section Title"].ToString();
            string strCareDesc = CareServiceItem.Fields["Care Section Description"].ToString();
            string strPropertyName = CareServiceItem.Fields["Property Name"].ToString();
             string strPropertyTag = CareServiceItem.Fields["Property Tag Line"].ToString();
            string strPropertyAddress = CareServiceItem.Fields["Street name and number"].ToString();
            string strPostalCode = CareServiceItem.Fields["Postal code"].ToString();
           
            string strCityName = CareServiceItem.Fields["Selected City"].ToString();
            string strProvinceID = CareServiceItem.Fields["Province"].ToString();
            ID ProvinceID = new ID(strProvinceID);
            var Provinceitem = database.GetItem(ProvinceID);
            string strProvinceName = Provinceitem.Fields["Province Name"].ToString();
            string strPropertyFormattedAddress = util.FormattedAddress(CareServiceItem, strProvinceName);
            Dictionary<Item, HtmlString> lstCareService = new Dictionary<Item, HtmlString>();
            Sitecore.Data.Fields.MultilistField CareServiceList = CareServiceItem.Fields["Property Care services"];

            if (CareServiceList != null && CareServiceList.TargetIDs != null)
            {

                foreach (ID id in CareServiceList.TargetIDs)

                {
                    _CareServiceDatabase = "/sitecore/content/Chartwell/Content Shared Folder/Care Service";
                    Item targetItem = Sitecore.Context.Database.Items[id];
                    string strNeighbourID = targetItem.DisplayName;
                    _CareServiceDatabase += "/" + strNeighbourID;
                    Item CareServiceitem = database.GetItem(_CareServiceDatabase);

                    HtmlString strServiceDescrb = new HtmlString(CareServiceitem.Fields["Care Service Description"].ToString());


                    lstCareService.Add(targetItem, strServiceDescrb);
                    _CareServiceDatabase = string.Empty;

                }
            }
            var viewModel = new CareServiceModel
            {
                CareServiceTitle = new HtmlString(strCareTitle),
                CareServiceName = new HtmlString(strCareDesc),
                PropertyName = new HtmlString(strPropertyName),
                PropertyTagLine = new HtmlString(strPropertyTag),
                PropertyAddress = new HtmlString(strPropertyAddress),
                CityName = new HtmlString(strCityName),
                ProvinceName = new HtmlString(strProvinceName),
                PostalCode = new HtmlString(strPostalCode),
                PropertyGuid = new HtmlString(sitecoreid),
                InnerItem = CareServiceItem,
                PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),
                CareServiceItem = lstCareService,
                TemplateItem= CareServiceTemplateItem
            };
            return viewModel;
        }

        public PartialViewResult Index()
        {


            return PartialView("~/Views/CareService/CareService.cshtml", CreateModel());
            //  return View(viewModel);
            //return PartialView(CreateModel());
        }
    }
}