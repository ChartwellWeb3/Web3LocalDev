using Chartwell.Feature.LeftNavigation.Models;
using Sitecore.Data.Items;
using Sitecore.Mvc.Common;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Web;
using Chartwell.Foundation.utility;
using Sitecore.Data.Fields;

namespace Chartwell.Feature.LeftNavigation.Controllers
{
    public class LeftNavigationController : Controller
    {
        public string selPropertyName = "";
        // GET: Navigation
        public PartialViewResult Index()
        {

            Item CurrentItem = RenderingContext.Current.ContextItem;

            Item Section = CurrentItem.Parent;

            return PartialView("~/Views/LeftNavigation/LeftNavigation.cshtml", CreateNavigationItem(Section));
        }


        public bool iSFieldDescriptionEmpty(string strFieldValue)
        {
            bool isEmpty = false;

            if (string.IsNullOrEmpty(strFieldValue) || strFieldValue == "NULL")
            { isEmpty = true; }

            return isEmpty;


        }

        private LeftNavigationMenu CreateNavigationItem(Item rootsite)
        {
            var database = Sitecore.Context.Database;

            var PropertyItem = database.GetItem(PageContext.Current.Item.Paths.ParentPath);
            ChartwellUtiles util = new ChartwellUtiles();
            bool isEventDisplay = false;
            string mapaddress = "";
            List<Item> items = new List<Item>();
            items.Clear();
            int i = 0;
             isEventDisplay = chkPropertyEvents(PropertyItem);

            foreach (Item root in rootsite.Children)
            {
                switch (root.Name.ToLower())
                {

                    case "overview":
                        if (PropertyItem.Fields["Property Description"].HasValue)
                        {

                            items.Insert(0, root);

                        }
                        break;
                    case "careservice":
                        if ((PropertyItem.Fields["Care Section Description"].HasValue) & (PropertyItem.Fields["Property Care services"].HasValue))

                        {
                            // items.Insert(i, root);
                            if (i <= 3)
                            {
                                Item tmp = items[0];
                                items.RemoveAt(0);
                                items.Insert(0, root);
                                items.Insert(i, tmp);
                            }
                            else
                            { items.Insert(i, root); }

                            i++;

                        }
                        break;
                    case "dining":
                        if ((!iSFieldDescriptionEmpty(PropertyItem.Fields["Dining Section Description"].ToString())) || (PropertyItem.Fields["Dining Service"].HasValue))

                        {

                            if (i <= 4)
                            {
                                Item tmp = items[1];
                                items.RemoveAt(1);
                                items.Insert(1, root);
                                items.Insert(i, tmp);
                            }
                            else
                            {
                                items.Insert(i, root);
                            }
                            i++;

                        }
                        break;

                    case "direction":
                        {
                            var PropertyLatitude = PropertyItem.Fields["Latitude"].ToString();
                            var PropertyLongitude = PropertyItem.Fields["Longitude"].ToString();
                            mapaddress = "https://maps.google.com?q=" + PropertyLatitude + "," + PropertyLongitude;
                            if (i >= 2)
                            {
                                Item tmp = items[i - 2];
                                items.RemoveAt(i - 2);
                                items.Insert(i - 2, root);
                                items.Insert(i, tmp);
                            }
                            else
                            {
                                items.Insert(i, root);
                              
                            }
                            i++;


                        }
                        break;
                    case "prop-contactus":
                        {

                            i++;
                            items.Insert(i, root);
                            if (i >= 1)
                                items.Last();
                          
                        }
                        break;
                    case "neighborhood":
                        if ((!iSFieldDescriptionEmpty(PropertyItem.Fields["Neighborhood Section Description"].ToString())) || (PropertyItem.Fields["Neighborhood Amenity"].HasValue))

                        {
                            items.Insert(i, root);
                            i++;

                        }
                        break;

                    case "activities":
                        if ((!iSFieldDescriptionEmpty(PropertyItem.Fields["Activities Section Description"].ToString())) || (PropertyItem.Fields["Property Activities"].HasValue))

                        {
                            items.Insert(i, root);
                            i++;
                        }
                        break;
                    case "amenities":
                        if ((!iSFieldDescriptionEmpty(PropertyItem.Fields["Amenities Section Description"].ToString())) || (PropertyItem.Fields["Property Amenities"].HasValue))

                        {
                            Item tmp = items[i - 1];
                            items.RemoveAt(i - 1);
                            items.Insert(i - 1, root);
                            items.Insert(i, tmp);
                            i++;

                        }
                        break;
                    case "wellness":
                        if ((!iSFieldDescriptionEmpty(PropertyItem.Fields["Wellness Section Description"].ToString())) || (PropertyItem.Fields["Property Wellness"].HasValue))

                        {
                            if (i >= 6)
                            {
                                Item tmp = items[i - 4];
                                if (tmp.Name.ToLower() != "careservice" || tmp.Name.ToLower() != "direction")
                                {
                                    items.RemoveAt(i - 4);
                                    items.Insert(i - 4, root);
                                    items.Insert(i, tmp);
                                }
                                else { items.Insert(i, root); }
                            }
                            else
                            {
                                Item tmp = items[i - 1];
                                items.RemoveAt(i - 1);
                                items.Insert(i - 1, root);
                                items.Insert(i, tmp);
                                i++;
                            }
                            i++;
                        }
                        break;

                    case "reviews":
                        {

                            Item tmp = items[i - 1];
                            items.RemoveAt(i - 1);
                            items.Insert(i - 1, root);
                            items.Insert(i, tmp);
                            i++;

                        }

                        break;
                    case "photos":
                        // if (PropertyItem.Fields["Photos"].HasValue)
                        items.Insert(1, root);
                        i++;
                        break;
                    case "virtualtour":
                        if (PropertyItem.Fields["YouTubeLink"].Value.Length > 1)
                        { 
                          items.Insert(2, root);
                        }
                        break;
                    case "suite":
                        // if (PropertyItem.Fields["Photos"].HasValue)
                        //items.Insert(3, root);

                        Item tmp1 = items[2];
                        items.RemoveAt(2);
                        items.Insert(2, root);
                        items.Insert(3, tmp1);
                        i++;
                        break;
                    case "events":
                        // if (PropertyItem.Fields["Photos"].HasValue)
                        items.Insert(i, root);
                        i++;
                        break;
                    default:
                        break;
                }
            }
            LeftNavigationMenu menu = new LeftNavigationMenu() { PhoneNo = util.GetPhoneNumber(PropertyItem), InnerItem = PropertyItem, Children = items, isEventDisplay= isEventDisplay, PropertyLocationUrl = mapaddress };

            return menu;
        }

        private bool chkPropertyEvents(Item PropertyItem)
        {
            bool isDisplayEvent = false;
            if (PropertyItem.Fields["Event Start Date"].HasValue && PropertyItem.Fields["Event End Date"].HasValue)

            {
                DateField startEventDate = (DateField)PropertyItem.Fields["Event Start Date"];
            
                DateField endEventDate = (DateField)PropertyItem.Fields["Event End Date"];
               
                int result1 = DateTime.Compare(startEventDate.DateTime.Date, DateTime.Today);
                int result2 = DateTime.Compare(DateTime.Today,endEventDate.DateTime.Date);
                if (result1 <= 0 && result2 <=0)
                { isDisplayEvent = true; }
            }
                return isDisplayEvent;

        }
    }
}