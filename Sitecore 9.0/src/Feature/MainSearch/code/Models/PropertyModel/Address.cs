using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chartwell.Feature.MainSearch.Models.PropertyModel
{
  public class Address
  {
    public string StreetName { get; set; }
    public string City { get; set; }
    public string Province { get; set; }
    public string PostalCode { get; set; }
  }
}