using Chartwell.Feature.VirtualTour.Models;
using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Web;
using Sitecore.Mvc.Presentation;
using Chartwell.Foundation.utility;

namespace Chartwell.Feature.VirtualTour.Controllers
{
    public class VirtualTourController : Controller
    {
        // GET: VirtualTour
        private VirtualTourModel CreateModel()
        {


            var database = Sitecore.Context.Database;
            ChartwellUtiles util = new ChartwellUtiles();
            //string selPropertyName = "";
            //if (WebUtil.GetUrlName(0).ToLower().Contains("tour"))
            //    selPropertyName = WebUtil.GetUrlName(1);
            //else if (WebUtil.GetUrlName(1).ToLower().Contains("tour"))
            //    selPropertyName = WebUtil.GetUrlName(0);

            //var PropertyBasicItem = database.GetItem("/sitecore/content/Chartwell/property Page/" + selPropertyName);
            var PhotoItemPath = PageContext.Current.Item.Paths.Path;
            var PropertyItemPath = PageContext.Current.Item.Paths.ParentPath;
            var PropertyBasicItem = database.GetItem(PropertyItemPath);
            string strPropertyName = PropertyBasicItem.Fields["Property Name"].ToString();
            string strPropertyTag = PropertyBasicItem.Fields["Property Tag Line"].ToString();
            string strPropertyAddress = PropertyBasicItem.Fields["Street name and number"].ToString();
            string strPostalCode = PropertyBasicItem.Fields["Postal code"].ToString();

          
            string strCityName = PropertyBasicItem.Fields["Selected City"].ToString();
            string strProvinceID = PropertyBasicItem.Fields["Province"].ToString();
            ID ProvinceID = new ID(strProvinceID);
            var Provinceitem = database.GetItem(ProvinceID);
            string strProvinceName = Provinceitem.Fields["Province Name"].ToString();
            string strPropertyFormattedAddress = util.FormattedAddress(PropertyBasicItem, strProvinceName);
            string strVideoLink = PropertyBasicItem.Fields["YouTubeLink"].ToString();
            var viewModel = new VirtualTourModel
            {

                PropertyTagLine = new HtmlString(strPropertyTag),
                PropertyAddress = new HtmlString(strPropertyAddress),
                PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),
                CityName = new HtmlString(strCityName),
                ProvinceName = new HtmlString(strProvinceName),
                PostalCode = new HtmlString(strPostalCode),
                VideoLink = new HtmlString(strVideoLink),
                InnerItem = PropertyBasicItem,

            };
            return viewModel;
        }

        public PartialViewResult Index()
        {


            return PartialView("~/Views/VirtualTour/VirtualTour.cshtml", CreateModel());

        }
    }
}