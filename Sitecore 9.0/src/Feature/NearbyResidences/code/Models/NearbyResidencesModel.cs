using Sitecore.Data.Items;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Chartwell.Feature.NearbyResidences.Models
{
  public class NearbyResidencesModel
  {
    public HtmlString PropertyName { get; set; }
        public HtmlString PageHeader { get; set; }
        public HtmlString PropertyAddress { get; set; }
    public HtmlString CityName { get; set; }
    public HtmlString ProvinceName { get; set; }
    public HtmlString PostalCode { get; set; }
    public HtmlString PhoneNo { get; set; }
    public Double  Distance { get; set; }
   public HtmlString PropertyFormattedAddress { get; set; }
   public string PropertyItemUrl { get; set; }
  public Item InnerItem { get; set; }

    }
}