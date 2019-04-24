using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Chartwell.Feature.WellnessService.Models
{
    public class WellnessServiceModel
    {
        public HtmlString PropertyName { get; set; }
        public HtmlString PropertyTagLine { get; set; }
        public HtmlString PropertyAddress { get; set; }
        public HtmlString CityName { get; set; }
        public HtmlString ProvinceName { get; set; }
        public HtmlString PostalCode { get; set; }
        public HtmlString WellnessTitle { get; set; }
        public HtmlString WellnessName { get; set; }
        public HtmlString PropertyGuid { get; set; }
        public List<Item> WellnessItem { get; set; }
        public Item TemplateItem { get; set; }
        public Item InnerItem { get; set; }
        public HtmlString PropertyFormattedAddress { get; set; }
    }
}