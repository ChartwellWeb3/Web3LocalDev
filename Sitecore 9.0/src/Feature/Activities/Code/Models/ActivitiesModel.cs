using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chartwell.Feature.Activities.Models
{
    public class ActivitiesModel
    {
        public HtmlString PropertyGuid { get; set; }
        public HtmlString PropertyName { get; set; }
        public HtmlString PropertyTagLine { get; set; }
        public HtmlString PropertyAddress { get; set; }
        public HtmlString CityName { get; set; }
        public HtmlString ProvinceName { get; set; }
        public HtmlString PostalCode { get; set; }
        public HtmlString ActivitiesName { get; set; }
        public HtmlString ActivitiesDescription { get; set; }
        public List<Item> ActivitiesItem { get; set; }
        public Item TemplateItem { get; set; }
        public Item InnerItem { get; set; }
        public HtmlString PropertyFormattedAddress { get; set; }

        public HtmlString BrochureURL { get; set; }
    }
}