using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;

namespace Chartwell.Feature.ServiceLevel.Models
{
    public class CareServiceModel
    {
        public HtmlString PropertyName { get; set; }
        public HtmlString PropertyTagLine { get; set; }
        public HtmlString PropertyAddress { get; set; }
        public HtmlString CityName { get; set; }
        public HtmlString ProvinceName { get; set; }
        public HtmlString PostalCode { get; set; }
        public HtmlString CareServiceTitle { get; set; }
        public HtmlString CareServiceName { get; set; }
        public HtmlString PropertyGuid { get; set; }
        public Dictionary<Item, HtmlString> CareServiceItem { get; set; }
        public HtmlString BrochureURL { get; set; }
        public HtmlString VidUrl { get; set; }

        public HtmlString PropertyFormattedAddress { get; set; }
        public Item InnerItem { get; set; }
        public Item TemplateItem { get; set; }
    }
}