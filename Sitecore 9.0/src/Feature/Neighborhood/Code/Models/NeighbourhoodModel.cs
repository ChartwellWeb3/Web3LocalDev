using Sitecore.Data.Items;
using System.Collections.Generic;
using System.Web;

namespace Chartwell.Feature.Neighborhood.Models
{
  public class NeighbourhoodModel
    {
        public HtmlString PropertyName { get; set; }
        public HtmlString PropertyTagLine { get; set; }
        public HtmlString PropertyAddress { get; set; }
        public HtmlString CityName { get; set; }
        public HtmlString ProvinceName { get; set; }
        public HtmlString PostalCode { get; set; }
        public HtmlString NeighbourhoodName { get; set; }
        public HtmlString PropertyGuid { get; set; }
        public List<Item> NeighbourhoodItem { get; set; }
        public Item InnerItem { get; set; }

        public Item TemplateItem { get; set; }
        
        public HtmlString PropertyFormattedAddress { get; set; }
    }
}