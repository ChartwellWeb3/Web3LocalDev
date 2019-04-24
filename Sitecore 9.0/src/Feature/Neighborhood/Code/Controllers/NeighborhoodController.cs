using Chartwell.Feature.Neighborhood.Models;
using Chartwell.Foundation.utility;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.Neighborhood.Controllers
{
  public class NeighborhoodController : Controller
    {
        // GET: Neighborhood
        private NeighbourhoodModel CreateModel()
        {


            var database = Sitecore.Context.Database;
            //string _NeighbourDatabase = "/sitecore/content/Chartwell/Content Shared Folder/Neighborhood";
            //string selPropertyName = "";
            ChartwellUtiles util = new ChartwellUtiles();
            //if (WebUtil.GetUrlName(0).ToLower() == "neighbourhood" || WebUtil.GetUrlName(0).Contains("quartier"))
            //    selPropertyName = WebUtil.GetUrlName(1);
            //else if (WebUtil.GetUrlName(1).ToLower() == "neighbourhood" || WebUtil.GetUrlName(1).Contains("quartier"))
            //    selPropertyName = WebUtil.GetUrlName(0);

            //var NeighbourhoodItem = database.GetItem("/sitecore/content/Chartwell/property Page/" + selPropertyName);

            var WellnessItemPath = PageContext.Current.Item.Paths.Path;
            var PropertyItemPath = PageContext.Current.Item.Paths.ParentPath;
            var NeighbourhoodItem = database.GetItem(PropertyItemPath);
            var NeighbourTemplateItem = database.GetItem(WellnessItemPath);
           

            string sitecoreid = NeighbourhoodItem.ID.ToString();
           


            string strPropertyName = NeighbourhoodItem.Fields["Property Name"].ToString();
            string strPropertyTag = NeighbourhoodItem.Fields["Property Tag Line"].ToString();
            string strPropertyAddress = NeighbourhoodItem.Fields["Street name and number"].ToString();
            string strPostalCode = NeighbourhoodItem.Fields["Postal code"].ToString();
           


            string strCityName = NeighbourhoodItem.Fields["Selected City"].ToString();
            string strProvinceID = NeighbourhoodItem.Fields["Province"].ToString();
            ID ProvinceID = new ID(strProvinceID);
            var Provinceitem = database.GetItem(ProvinceID);
            string strProvinceName = Provinceitem.Fields["Province Name"].ToString();

            string strPropertyFormattedAddress = util.FormattedAddress(NeighbourhoodItem, strProvinceName);
            string strNeighbourName = NeighbourhoodItem.Fields["Neighborhood Section Description"].ToString();


            List<Item> lstNeighbourName = new List<Item>();
            Sitecore.Data.Fields.MultilistField NeighbourList = NeighbourhoodItem.Fields["Neighborhood Amenity"];

            if (NeighbourList != null && NeighbourList.TargetIDs != null)
            {

                foreach (ID id in NeighbourList.TargetIDs)

                {

                    Item targetItem = Sitecore.Context.Database.Items[id];
                    lstNeighbourName.Add(targetItem);




                }
            }
            var viewModel = new NeighbourhoodModel
            {
                PropertyGuid = new HtmlString(sitecoreid),
                NeighbourhoodName = new HtmlString(strNeighbourName),
                PropertyName = new HtmlString(strPropertyName),
                PropertyTagLine = new HtmlString(strPropertyTag),
                PropertyAddress = new HtmlString(strPropertyAddress),
                CityName = new HtmlString(strCityName),
                ProvinceName = new HtmlString(strProvinceName),
                PostalCode = new HtmlString(strPostalCode),
                InnerItem = NeighbourhoodItem,
                PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),

                TemplateItem = NeighbourTemplateItem,
                NeighbourhoodItem = lstNeighbourName.OrderBy(o => o.Name).ToList()
            };
            return viewModel;
        }

        public PartialViewResult Index()
        {


            return PartialView("~/Views/Neighborhood/Neighbourhood.cshtml", CreateModel());
            //  return View(viewModel);
            //return PartialView(CreateModel());
        }
    }
}