using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chartwell.Feature.DiningService.Models
{
    public class DiningModel
    {
        public HtmlString PropertyGuid { get; set; }
        public HtmlString PropertyName { get; set; }
        public HtmlString PropertyTagLine { get; set; }
        public HtmlString PropertyAddress { get; set; }
        public HtmlString CityName { get; set; }
        public HtmlString ProvinceName { get; set; }
        public HtmlString PostalCode { get; set; }
        public HtmlString DiningDescription { get; set; }
        public HtmlString DiningTitle { get; set; }
        public List<Item> DiningServiceItem { get; set; }
        public Item TemplateItem { get; set; }
        public Item InnerItem { get; set; }
        public HtmlString PropertyFormattedAddress { get; set; }
        public HtmlString BrochureURL { get; set; }
    }
}