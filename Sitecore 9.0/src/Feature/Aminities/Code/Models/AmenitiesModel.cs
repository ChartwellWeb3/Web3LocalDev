using System.Collections.Generic;
using System.Web;
using Sitecore.Data.Items;

namespace Chartwell.Feature.Amenities.Models
{
    public class AmenitiesModel
    {
        public List<Item> AmenitiesItem { get; set; }
        public HtmlString CityName { get; set; }
        public HtmlString PostalCode { get; set; }
        public HtmlString PropertyAddress { get; set; }
        public HtmlString PropertyName { get; set; }
        public HtmlString PropertyTagLine { get; set; }
        public HtmlString ProvinceName { get; set; }
        public HtmlString PropertyGuid { get; set; }
        public HtmlString AmenitiesTitle { get; set; }
        public HtmlString AmenitiesDescription { get; set; }
        public Item TemplateItem { get; set; }
        public Item InnerItem { get; set; }
        public HtmlString PropertyFormattedAddress { get; set; }
    }
}