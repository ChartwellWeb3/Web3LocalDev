using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chartwell.Feature.ReviewsRatings.Models
{
  public class PropertyModel
  {
    public string PropertyID { get; set; }
    public string PropertyName { get; set; }
    public string PropertyAddress { get; set; }
    public string CityName { get; set; }
    public string ProvinceName { get; set; }
    public string PostalCode { get; set; }
   public HtmlString PropertyFormattedAddress { get; set; }
    }
}