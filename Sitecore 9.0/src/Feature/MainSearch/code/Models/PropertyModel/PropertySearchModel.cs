using Chartwell.Feature.PageSEO.Models;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.MainSearch.Models.PropertyModel
{

  public class PropertySearchModel : PageMetaDataModel
  {
    public bool IsSplitter;

    public PropertySearchModel() { }
    //public string CityName { get; set; }

    //[DisplayName("Postal Code")]
    //public string PostalCode { get; set; }
    public string PropertyNameSearch { get; set; }
    [DisplayName("Property Name")]
    public string PropertyName { get; set; }
    public string PropertyType { get; set; }
    public string PropertyDesc { get; set; }
    public string PropertyTagLine { get; set; }
    public string PropertyID { get; set; }
    [DisplayName("Sitecore Item ID")]
    public string ItemID { get; set; }
    public string PhoneNo { get; set; }
    public string USP { get; set; }

    public string PropertyImage { get; set; }
    // Overview
    public string StreetName { get; set; }
    public string City { get; set; }
    public string Province { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public string PropertyItemUrl { get; set; }

    public string Longitude { get; set; }
    public string Latitude { get; set; }

    public bool CityLandingPage { get; set; }
    public string CityLandingPageButton { get; set; }
    public string CityLandingPageText { get; set; }
    public List<string> ProvinceCountryList { get; set; }
    public string SearchText { get; set; }
    public string SearchType { get; set; }
    public HtmlString PropertyFormattedAddress { get; set; }
    public Item InnerItem { get; set; }

    public string SplitterUrl { get; set; }

    public string SplitterPageTitle { get; set; }
    public string SplitterPageDescription { get; set; }

    public bool SearchResults { get; set; }

    public double Distance { get; set; }

    public int PageStartIndex { get; set; }

    public int PageSize { get; set; }

    public bool Display_RegionsDD { get; set; }
    public List<SelectListItem> RegionList { get; set; }

    public bool? IsRegion { get; set; }

    public int CityID { get; set; }

    public string Language { get; set; }

    public int RowCount { get; set; }

    public bool? FoundCitySearch { get; set; }

    public string Lat { get; set; }

    public string Lng { get; set; }

  }
}