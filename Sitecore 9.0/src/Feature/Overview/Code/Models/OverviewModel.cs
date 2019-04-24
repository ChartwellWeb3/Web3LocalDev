using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
namespace Chartwell.Feature.Overview.Models
{
    public class OverviewModel
    {
        public HtmlString PropertyName { get; set; }
        public HtmlString PropertySelectedLanguage { get; set; }
        public HtmlString PropertyTagLine { get; set; }
        public HtmlString PropertyAddress { get; set; }
        public HtmlString CityName { get; set; }
        public HtmlString ProvinceName { get; set; }
        public HtmlString PostalCode { get; set; }
        public HtmlString PropertyDescription { get; set; }
        public HtmlString GoogleReviewKeyword { get; set; }
        public string ReviewURL { get; set; }
        public HtmlString VideoLink { get; set; }
        public HtmlString BrochureURL { get; set; }
        public HtmlString PropertyPhotoItem { get; set; }
        public HtmlString PropertyFormattedAddress { get; set; }
        public Item InnerItem { get; set; }
        public string PropertyType { get; set; }
        public bool isLandingpage { get; set; }
    }
}