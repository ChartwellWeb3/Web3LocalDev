using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Collections;
using Sitecore;
using Sitecore.Data.Items;
using System;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;
using Sitecore.Data;

namespace Chartwell.Feature.ChartwellTopHeader.Controllers
{
    public class ChartwellTopHeaderController : Controller
    {
        // GET: ChartwellLanguage
        public ActionResult Index()
        {

            var database = Sitecore.Context.Database;
            var ItemPath = PageContext.Current.Item.Paths.Path;
            var PropertyItemPath = PageContext.Current.Item.Paths.ParentPath;
            var PropertyItem = database.GetItem(PropertyItemPath);
            string selectedlang = Sitecore.Context.Language.ToString();
            if (PropertyItem.Template.Name == "Property Page")
            {

                string strPropertyTypeID = PropertyItem.Fields["property type"].ToString();
                string strPropertyType = string.Empty;
                if (!string.IsNullOrEmpty(strPropertyTypeID))
                {
                    ID PropertyTypeID = new ID(strPropertyTypeID);
                    var PropertyTypeItem = database.GetItem(PropertyTypeID);
                    strPropertyType = PropertyTypeItem.Fields["property type"].ToString();
                }
                string strProvinceID = PropertyItem.Fields["Province"].ToString();
                ID ProvinceID = new ID(strProvinceID);
                var Provinceitem = database.GetItem(ProvinceID);
                string strProvinceName = Provinceitem.Fields["Province Name"].ToString().ToLower();
                // if (strPropertyType == "RET" && strProvinceName == "ontario")
                //if (strPropertyType == "RET")
                if(strProvinceName == "quebec" || strProvinceName == "québec")
                {
                    if(selectedlang =="en")
                    return View("~/Views/ChartwellTopHeader/Index.cshtml");
                    else { return View("~/Views/ChartwellTopHeader/Indexfr.cshtml"); }
                }
                else
                    return new EmptyResult();
            }
            else
            {

                // return new EmptyResult();

                if (selectedlang == "en")
                    return View("~/Views/ChartwellTopHeader/Index.cshtml");
                else { return View("~/Views/ChartwellTopHeader/Indexfr.cshtml"); }
            }
        }

      
    }
}