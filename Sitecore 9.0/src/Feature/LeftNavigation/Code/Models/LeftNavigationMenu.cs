using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
namespace Chartwell.Feature.LeftNavigation.Models
{
    public class LeftNavigationMenu
    {
        public List<Item> Children { get; set; }
        public string PhoneNo { get; set; }
        public string PropertyLocationUrl { get; set; }
        public Item InnerItem { get; set; }
        public bool isEventDisplay { get; set; }
    }
}