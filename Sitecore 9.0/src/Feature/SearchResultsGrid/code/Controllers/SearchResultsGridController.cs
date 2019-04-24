// Chartwell.Feature.SearchResultsGrid.Controllers.SearchResultsGridController
using Chartwell.Feature.MainSearch.Models.PropertyModel;
using Chartwell.Feature.SearchResultsGrid.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;
using Sitecore.Resources.Media;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

public class SearchResultsGridController : Controller
{
  private readonly string constring = ConfigurationManager.ConnectionStrings["SitecoreOLP"].ToString();

  private readonly ChartwellUtiles GetItem = new ChartwellUtiles();

  private IEnumerable<SearchResultItem> results = null;

  public string language = Context.Language.ToString();

  private bool foundSearchResults = false;

  public List<PropertySearchModel> PropertyList
  {
    get;
    set;
  }

  public ActionResult SearchResults(PropertySearchModel property)
  {
    PropertyList = new List<PropertySearchModel>();
    property.RegionList = RegionsDDL(language);
    string text = string.Empty;
    string text2 = string.Empty;
    string text3 = HttpUtility.UrlDecode(base.Request.Url.PathAndQuery).TrimEnd('/');
    if (text3.Equals("/en/search-results") || text3.Equals("/search-results") || text3.Equals("/fr/résultat-de-la-recherche") || text3.Equals("/résultat-de-la-recherche"))
    {
      GetItem.SetLanguageFromUrl(text3);
      if (text3.Replace("/en", "").Replace("/fr", "").Replace("/", "")
          .Equals(Translate.Text("SearchResults")))
      {
        text = GetItem.GetDictionaryItem("AllProperties", language);
        text2 = GetItem.GetDictionaryItem("Region", language);
      }
    }
    else
    {
      text = base.Request.Params[0];
      text2 = base.Request.Params.Keys[0];
    }
    if (text2.ToLower() == GetItem.GetDictionaryItem("CitySearch", language).ToLower())
    {
      property.City = text.Replace("-", " ");
    }
    else if (text2 == "PropertyName")
    {
      property.PropertyName = text.Replace("-", " ").Replace("'", "").RemoveDiacritics()
          .ToLower();
    }
    else if (text2 == "PostalCode")
    {
      property.PostalCode = text.Replace("-", " ");
    }
    if (!string.IsNullOrEmpty(text) && text2.Equals(GetItem.GetDictionaryItem("Region", language)))
    {
      if (text.Replace("-", " ").Equals(GetItem.GetDictionaryItem("AllProperties", language)))
      {
        property.City = "canada";
      }
      else
      {
        property.City = text.Replace("-", " ");
      }
      property.CityLandingPageButton = "RET";
      property.Display_RegionsDD = true;
      property.IsRegion = true;
    }
    else
    {
      property.Display_RegionsDD = false;
      property.IsRegion = false;
    }
    if (!string.IsNullOrWhiteSpace(property.PropertyName) || !string.IsNullOrEmpty(property.City) || !string.IsNullOrWhiteSpace(property.PostalCode))
    {
      string empty = string.Empty;
      string searchType = text2;
      if (!string.IsNullOrEmpty(property.City))
      {
        empty = property.City;
        return CitySearchResults(empty, property.IsRegion, searchType);
      }
      if (!string.IsNullOrEmpty(property.PropertyName))
      {
        empty = property.PropertyName;
        property.SearchType = "PropertyName";
        bool CheckSplitPage = false;
        results = CheckForSplitterPage(language, empty, ref CheckSplitPage).ToList();
        if ((results != null && results.Count() == 1) & CheckSplitPage)
        {
          List<SearchResultItem> list = results.ToList();
          string url = list[0].GetField("SplitterURL").ToString();
          return Redirect(url);
        }
        results = SearchResidence(language, empty).ToList();
        if (results != null && results.Count() == 0)
        {
          foundSearchResults = false;
          results = SearchAll(language);
          PropertyList = (from o in ProcessResults(results, language, 9)
                          orderby o.Province
                          select o).ToList();
          PropertyList[0].PageSize = 9;
          string text6 = property.PropertyType = (PropertyList[0].CityLandingPageButton = "RET");
          PropertyList[0].SearchType = property.SearchType;
          PropertyList[0].SearchText = empty.ToTitleCase();
          PropertyList[0].Display_RegionsDD = true;
          PropertyList[0].IsRegion = true;
          PropertyList[0].RegionList = RegionsDDL(language);
          PropertyList[0].FoundCitySearch = foundSearchResults;
          PropertyList[0].CityLandingPageText = GetItem.GetDictionaryItem("SearchResultsNotFound", language);
        }
        else
        {
          if (results.Count() == 1)
          {
            List<SearchResultItem> list2 = results.ToList();
            int.TryParse(list2[0].GetField("IsSplitter").Value, out int result);
            if ((result != 0) ? true : false)
            {
              string value = list2[0].GetField("SplitterURL").Value;
              return Redirect(value);
            }
            List<SearchResultItem> list3 = (from x in GetItem.PropertyDetails(list2[0].ItemId)
                                            where x.Language == language
                                            select x).ToList();
            Item item = list3[0].GetItem();
            if (item.Name.ToLower().Contains(empty.ToLower()) || item.Fields["Property Name"].Value.ToLower().Contains(empty.ToLower()))
            {
              string itemUrl = LinkManager.GetItemUrl(item, new UrlOptions
              {
                UseDisplayName = true,
                LowercaseUrls = true,
                LanguageEmbedding = LanguageEmbedding.Always,
                LanguageLocation = LanguageLocation.FilePath,
                Language = item.Language
              });
              if (itemUrl.Contains("sumach"))
              {
                string url2 = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/en/the-sumach";
                return Redirect(url2);
              }
              itemUrl = itemUrl + "/" + GetItem.GetDictionaryItem("overview", language);
              return Redirect(itemUrl);
            }
            string itemUrl2 = LinkManager.GetItemUrl(item, new UrlOptions
            {
              UseDisplayName = true,
              LowercaseUrls = true,
              LanguageEmbedding = LanguageEmbedding.Always,
              LanguageLocation = LanguageLocation.FilePath,
              Language = item.Language
            });
            if (itemUrl2.Contains("sumach"))
            {
              string url3 = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/en/the-sumach";
              return Redirect(url3);
            }
            foundSearchResults = false;
            PropertyList = (from o in ProcessResults(results, language, 9)
                            orderby o.Province
                            select o).ToList();
            PropertyList[0].PageSize = 9;
            PropertyList[0].CityLandingPageText = GetItem.GetDictionaryItem("FollowingResults", language);
            PropertyList[0].SearchText = empty.ToTitleCase();
            PropertyList[0].SearchType = "PropertyName";
            PropertyList[0].CityLandingPageButton = "RET";
            PropertyList[0].Display_RegionsDD = false;
            PropertyList[0].IsRegion = false;
            return PartialView(PropertyList.ToList());
          }
          property.SearchResults = (foundSearchResults = false);
          PropertyList = new List<PropertySearchModel>();
          PropertyList = (from o in ProcessResults(results, language, 9)
                          orderby o.Province
                          select o).ToList();
          PropertyList[0].PageSize = 9;
          string text6 = property.PropertyType = (PropertyList[0].CityLandingPageButton = "RET");
          PropertyList[0].CityLandingPageText = GetItem.GetDictionaryItem("FollowingResults", language);
          PropertyList[0].SearchText = empty.ToTitleCase();
          PropertyList[0].SearchType = "PropertyName";
          PropertyList[0].SearchResults = false;
        }
        return PartialView(PropertyList.ToList());
      }
      if (!string.IsNullOrEmpty(property.PostalCode))
      {
        empty = property.PostalCode;
        return CitySearchResults(empty, property.IsRegion, searchType);
      }
      return PartialView();
    }
    return PartialView();
  }

  private ActionResult CitySearchResults(string searchCriteria, bool? isRegion, string searchType)
  {
    int pageSize = 9;
    if (searchType != "PostalCode")
    {
      results = from x in GetItem.SwitchSelectedCity(language, searchCriteria)
                where x.Language == language
                select x;
    }
    else
    {
      LatLngForPostalCode(searchCriteria.Replace(" ", "").Replace("-", ""), out string Lat, out string _);
      if (!string.IsNullOrEmpty(Lat))
      {
        foundSearchResults = true;
      }
      else
      {
        foundSearchResults = false;
      }
    }
    if ((results != null && results.Count() > 0) || foundSearchResults)
    {
      foundSearchResults = ((!System.Convert.ToBoolean(isRegion)) ? true : false);
      PropertyList = CitySearchDistance(results, language, pageSize, "RET", searchCriteria, searchType);
      PropertyList[0].SearchText = ((searchType == "PostalCode") ? searchCriteria.ToUpper() : searchCriteria.ToTitleCase());
      PropertyList[0].Language = language;
      PropertyList[0].FoundCitySearch = true;
      PropertyList[0].CityLandingPageButton = "RET";
      PropertyList[0].CityLandingPage = ((!System.Convert.ToBoolean(isRegion)) ? true : false);
      PropertyList[0].CityLandingPageText = GetItem.GetDictionaryItem("Retirement Homes in and around", language);
      PropertyList[0].PageSize = 9;
      PropertyList[0].SearchResults = foundSearchResults;
      PropertyList[0].SearchType = searchType;
      PropertyList[0].Display_RegionsDD = System.Convert.ToBoolean(isRegion);
      PropertyList[0].IsRegion = isRegion;
      PropertyList[0].RegionList = (System.Convert.ToBoolean(isRegion) ? RegionsDDL(language) : null);
      if (searchCriteria.ToLower() == "canada")
      {
        if (PropertyList[0].RegionList != null && PropertyList[0].RegionList.Count() > 0)
        {
          SelectListItem selectListItem = (from x in PropertyList[0].RegionList
                                           where x.Text == GetItem.GetDictionaryItem("AllProperties", language)
                                           select x).FirstOrDefault();
          selectListItem.Selected = true;
        }
      }
      else if (PropertyList[0].RegionList != null && PropertyList[0].RegionList.Count() > 0)
      {
        SelectListItem selectListItem2 = (from x in PropertyList[0].RegionList
                                          where x.Text == searchCriteria
                                          select x).FirstOrDefault();
        selectListItem2.Selected = true;
      }
      if (searchType == "PostalCode")
      {
        PropertyList[0].CityLandingPageText = ((PropertyList[0].PropertyType == "RET") ? GetItem.GetDictionaryItem("RetirementHomesNear", language) : GetItem.GetDictionaryItem("LongTermCareHomesNear", language));
      }
      else
      {
        PropertyList[0].CityLandingPageText = ((PropertyList[0].PropertyType == "RET") ? GetItem.GetDictionaryItem("Retirement Homes in and around", language) : GetItem.GetDictionaryItem("LongTermCareResidencesInAndAround", language));
      }
      return PartialView(PropertyList);
    }
    results = SearchAll(language);
    PropertyList = ProcessResults(results, language, pageSize);
    string text3 = PropertyList[0].PropertyType = (PropertyList[0].CityLandingPageButton = "RET");
    PropertyList[0].Display_RegionsDD = true;
    PropertyList[0].IsRegion = true;
    PropertyList[0].FoundCitySearch = false;
    PropertyList[0].SearchText = ((searchType == "PostalCode") ? searchCriteria.ToUpper() : searchCriteria.ToTitleCase());
    PropertyList[0].CityLandingPageText = GetItem.GetDictionaryItem("SearchResultsNotFound", language);
    PropertyList[0].SearchResults = false;
    PropertyList[0].SearchType = searchType;
    PropertyList[0].PageSize = 9;
    PropertyList[0].RegionList = RegionsDDL(language);
    SelectListItem selectListItem3 = (from x in PropertyList[0].RegionList
                                      where x.Text == searchCriteria
                                      select x).FirstOrDefault();
    if (selectListItem3 != null)
    {
      selectListItem3.Selected = true;
    }
    else
    {
      selectListItem3 = (from x in PropertyList[0].RegionList
                         where x.Text == GetItem.GetDictionaryItem("AllProperties", language)
                         select x).FirstOrDefault();
      selectListItem3.Selected = true;
    }
    return PartialView(PropertyList.ToList());
  }

  public List<SelectListItem> RegionsDDL(string Language)
  {
    List<SelectListItem> list = new List<SelectListItem>();
    string empty = string.Empty;
    empty = base.Request.Url.ToString();
    list.Add(new SelectListItem
    {
      Text = GetItem.GetDictionaryItem("AllProperties", Language),
      Value = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + Language + "/" + GetItem.GetDictionaryItem("SearchResults", Language) + "/?" + GetItem.GetDictionaryItem("Region", Language) + "=" + GetItem.GetDictionaryItem("AllProperties", Language).Replace(" ", "-")
    });
    list.Add(new SelectListItem
    {
      Text = GetItem.GetDictionaryItem("Alberta", Language),
      Value = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + Language + "/" + GetItem.GetDictionaryItem("SearchResults", Language) + "/?" + GetItem.GetDictionaryItem("Region", Language) + "=" + GetItem.GetDictionaryItem("Alberta", Language)
    });
    list.Add(new SelectListItem
    {
      Text = GetItem.GetDictionaryItem("BritishColumbia", Language),
      Value = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + Language + "/" + GetItem.GetDictionaryItem("SearchResults", Language) + "/?" + GetItem.GetDictionaryItem("Region", Language) + "=" + GetItem.GetDictionaryItem("BritishColumbia", Language).Replace(" ", "-")
    });
    list.Add(new SelectListItem
    {
      Text = GetItem.GetDictionaryItem("Ontario", Language),
      Value = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + Language + "/" + GetItem.GetDictionaryItem("SearchResults", Language) + "/?" + GetItem.GetDictionaryItem("Region", Language) + "=" + GetItem.GetDictionaryItem("Ontario", Language)
    });
    list.Add(new SelectListItem
    {
      Text = GetItem.GetDictionaryItem("Quebec", Language),
      Value = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + Language + "/" + GetItem.GetDictionaryItem("SearchResults", Language) + "/?" + GetItem.GetDictionaryItem("Region", Language) + "=" + GetItem.GetDictionaryItem("Quebec", Language)
    });
    return list.ToList();
  }

  private void LatLngForPostalCode(string searchCriteria, out string Lat, out string Lng)
  {
    SqlDataReader sqlDataReader = null;
    SqlConnection sqlConnection = new SqlConnection(constring);
    sqlConnection.Open();
    SqlCommand sqlCommand = new SqlCommand("sp_SCGetPostalCode", sqlConnection)
    {
      CommandType = CommandType.StoredProcedure
    };
    sqlCommand.Parameters.AddWithValue("@PostCode", searchCriteria);
    sqlDataReader = sqlCommand.ExecuteReader();
    Lat = string.Empty;
    Lng = string.Empty;
    do
    {
      if (!sqlDataReader.Read())
      {
        return;
      }
    }
    while (string.IsNullOrWhiteSpace(sqlDataReader["City"].ToString()));
    Lat = System.Convert.ToDouble(sqlDataReader["Latitude"]).ToString().Replace(",", ".");
    Lng = System.Convert.ToDouble(sqlDataReader["Longitude"]).ToString().Replace(",", ".");
  }

  private IEnumerable<SearchResultItem> SearchAll(string language)
  {
    using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
    {
      results = (from x in providerSearchContext.GetQueryable<SearchResultItem>()
                 where x.TemplateName == "Property Page"
                 where x.Language == language
                 where x.Name != "__Standard Values"
                 where x["Property ID"] != "99999"
                 select x into o
                 orderby o.Name
                 select o).ToList();
    }
    return results.ToList();
  }

  private IEnumerable<SearchResultItem> SearchRegion(string language, string searchCriteria)
  {
    using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
    {
      results = (from x in providerSearchContext.GetQueryable<SearchResultItem>()
                 where x.TemplateName == "Property Page"
                 where x.Language == language
                 where x.Name != "__Standard Values"
                 select x into o
                 orderby o.Name
                 select o).ToList();
    }
    return results.ToList();
  }

  private IEnumerable<SearchResultItem> SearchPostalCode(string language, string searchCriteria)
  {
    using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
    {
      results = (from x in providerSearchContext.GetQueryable<SearchResultItem>()
                 where x.Path.StartsWith("/sitecore/content/Chartwell/retirement-residences")
                 where x.TemplateName.Equals("Property Page")
                 where x.Language == language
                 where x["Property ID"] != "99999"
                 select x into o
                 orderby o.Name
                 select o).Take(1);
    }
    return results.ToList();
  }

  private IEnumerable<SearchResultItem> SearchSelectedCity(string language, string searchCriteria)
  {
    using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
    {
      results = (from x in providerSearchContext.GetQueryable<SearchResultItem>()
                 where x.TemplateName == "City"
                 where x.Language == language
                 where x.Name == searchCriteria.RemoveDiacritics().Replace(" ", "").ToLower()
                 select x into o
                 orderby o.Name
                 select o).ToList();
    }
    return results.ToList();
  }

  private IEnumerable<SearchResultItem> SearchSelectedCityWithSpaces(string language, string searchCriteria)
  {
    using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
    {
      results = (from x in providerSearchContext.GetQueryable<SearchResultItem>()
                 where x.TemplateName == "City"
                 where x.Language == language
                 where x.Name == searchCriteria.RemoveDiacritics().TrimPunctuation().ToLower()
                 select x into o
                 orderby o.Name
                 select o).ToList();
    }
    return results.ToList();
  }

  private IEnumerable<SearchResultItem> SearchResidence(string language, string searchCriteria)
  {
    searchCriteria = searchCriteria.RemoveDiacritics().Replace("'", "").Replace("-", " ")
        .TrimPunctuation()
        .ToLower();
    using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
    {
      results = (from x in providerSearchContext.GetQueryable<SearchResultItem>()
                 where x.TemplateName == "Property Page"
                 where x.Language == language
                 where x.Name.Contains(searchCriteria)
                 where x.Name != "__Standard Values"
                 where x["property ID"] != "99999"
                 select x into o
                 orderby o.Name
                 select o).ToList();
    }
    if (results.Count() == 0)
    {
      using (IProviderSearchContext providerSearchContext2 = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        results = (from x in providerSearchContext2.GetQueryable<SearchResultItem>()
                   where x.TemplateName == "Property Page"
                   where x.Language == language
                   where x.Name != "__Standard Values"
                   where x["property ID"] != "99999"
                   select x into o
                   orderby o.Name
                   select o).ToList();
      }
      List<SearchResultItem> list = (List<SearchResultItem>)(results = (from x in results
                                                                        where x.GetField("Property Name").Value == searchCriteria
                                                                        select x).ToList());
    }
    return results.ToList();
  }

  private IEnumerable<SearchResultItem> CheckForSplitterPage(string language, string searchCriteria, ref bool CheckSplitPage)
  {
    searchCriteria = searchCriteria.RemoveDiacritics().Replace("'", "").Replace("-", " ")
        .TrimPunctuation()
        .ToLower();
    using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
    {
      results = (from x in providerSearchContext.GetQueryable<SearchResultItem>()
                 where x.TemplateName == "Property Page"
                 where x.Name.Contains(searchCriteria)
                 where x["property ID"] != "99999"
                 where x.Language == language
                 select x into o
                 orderby o.Name
                 select o).ToList();
    }
    bool stringToBool = false;
    bool flag = false;
    flag = (from i in results
            where bool.TryParse(i["IsSplitter"], out stringToBool)
            select stringToBool into b
            where b.Equals(obj: true)
            select b).Count().Equals(results.Count());
    CheckSplitPage = false;
    if (flag)
    {
      results = (from x in results
                 where x.Language == Context.Language.Name
                 select x).Take(1);
      CheckSplitPage = true;
    }
    return (from x in results
            where x.Language == Context.Language.Name
            select x).ToList();
  }

  private List<PropertySearchModel> ProcessResults(IEnumerable<SearchResultItem> results, string Language, int pageSize)
  {
    PropertyList = new List<PropertySearchModel>();
    ChartwellUtiles util = new ChartwellUtiles();
    IEnumerable<PropertySearchModel> source = (from item in results
                                               select new PropertySearchModel
                                               {
                                                 ItemID = item.ItemId.ToString(),
                                                 PropertyID = item.GetField("property id").Value,
                                                 PropertyName = item.GetField("property name").Value,
                                                 PropertyType = PropertyType(Language, item.GetItem()),
                                                 PropertyDesc = item.GetField("property description").Value,
                                                 PropertyTagLine = item.GetField("property tag line").Value,
                                                 USP = item.GetField("USP").Value,
                                                 PhoneNo = util.GetPhoneNumber(item.GetItem()),
                                                 StreetName = item.GetField("Street name and number").Value,
                                                 City = item.GetField("Selected City").Value,
                                                 PostalCode = item.GetField("Postal Code").Value,
                                                 Province = ProvinceName(Language, item.GetItem()),
                                                 Country = item.GetField("Country").Value,
                                                 PropertyItemUrl = GetPropertyUrl(Language, item.GetItem()),
                                                 PropertyFormattedAddress = new HtmlString(util.FormattedAddress(item.GetItem(), ProvinceName(Language, item.GetItem()))),
                                                 InnerItem = item.GetItem(),
                                                 PropertyImage = GetImageUrl(item.GetItem()),
                                                 CityLandingPageText = ((PropertyType(Language, item.GetItem()) == "RET") ? "Retirement Homes" : "Long Term Care Residences"),
                                                 PageTitle = item.GetField("PageTitle").Value,
                                                 PageDescription = item.GetField("pagedescription").Value,
                                                 PageKeyword = item.GetField("pagekeyword").Value,
                                                 SearchResults = (foundSearchResults ? true : false),
                                                 PageSize = 9,
                                                 Language = Language,
                                                 PageStartIndex = 0
                                               } into o
                                               orderby o.Province
                                               select o).Skip(0).Take(pageSize);
    PropertyList = source.ToList();
    return (from o in PropertyList
            orderby o.Province
            select o).ToList();
  }

  public ActionResult CityLandingPage(string SearchText, string CityLandingPageButton, string SearchType, int PageSize, bool? IsRegion, string Language, bool? FoundCitySearch)
  {
    PropertySearchModel propertySearchModel = new PropertySearchModel();
    string text = base.Request.Params[0].ToLower();
    string text2 = base.Request.Params.Keys[0];
    if (System.Convert.ToBoolean(IsRegion))
    {
      if (text.Equals(Translate.Text("AllProperties")))
      {
        propertySearchModel.City = "canada";
      }
      else
      {
        propertySearchModel.City = text;
      }
      propertySearchModel.IsRegion = true;
      FoundCitySearch = ((SearchType == GetItem.GetDictionaryItem("Region", Language)) | FoundCitySearch);
    }
    else
    {
      propertySearchModel.Display_RegionsDD = false;
      propertySearchModel.IsRegion = false;
    }
    if (!string.IsNullOrEmpty(SearchType) && SearchType.Contains("PropertyName"))
    {
      results = SearchResidence(Language, SearchText);
      if (results != null && results.Count() == 0)
      {
        results = SearchAll(Language);
      }
      PageSize += 9;
      PropertyList = (from o in ProcessResults(results, Language, PageSize)
                      orderby o.Province
                      select o).ToList();
      PropertyList[0].CityLandingPageText = ((!string.IsNullOrEmpty(CityLandingPageButton) && System.Convert.ToBoolean(!IsRegion)) ? Translate.Text("FollowingResults") : Translate.Text("SearchResultsNotFound"));
      PropertyList[0].SearchText = ((SearchType == "PostalCode") ? SearchText.ToUpper() : SearchText.ToTitleCase());
      PropertyList[0].SearchType = "PropertyName";
      PropertyList[0].SearchResults = false;
      PropertyList[0].Display_RegionsDD = (System.Convert.ToBoolean(IsRegion) ? true : false);
      PropertyList[0].IsRegion = System.Convert.ToBoolean(IsRegion);
      PropertyList[0].PageSize = PageSize;
      PropertyList[0].CityLandingPageButton = CityLandingPageButton;
      if (System.Convert.ToBoolean(IsRegion))
      {
        PropertyList[0].RegionList = RegionsDDL(Language);
      }
    }
    else if (!string.IsNullOrEmpty(SearchType) && (SearchType.ToLower() == GetItem.GetDictionaryItem("CitySearch", Language).ToLower() || SearchType.Contains("PostalCode") || SearchType.Contains(GetItem.GetDictionaryItem("Region", Language))))
    {
      PageSize += 9;
      if (System.Convert.ToBoolean(FoundCitySearch))
      {
        if (SearchType.ToLower() == GetItem.GetDictionaryItem("CitySearch", Language).ToLower() || SearchType == GetItem.GetDictionaryItem("Region", Language))
        {
          results = (from x in GetItem.SwitchSelectedCity(Language, SearchText)
                     where x.Language == Language
                     select x).ToList();
        }
        else
        {
          results = SearchPostalCode(language, SearchText);
        }
        PropertyList = CitySearchDistance(results, Language, PageSize, CityLandingPageButton, SearchText, SearchType);
        PropertyList[0].SearchType = SearchType;
        if (SearchType == "PostalCode")
        {
          PropertyList[0].CityLandingPageText = ((PropertyList[0].PropertyType == "RET") ? GetItem.GetDictionaryItem("RetirementHomesNear", Language) : GetItem.GetDictionaryItem("LongTermCareHomesNear", Language));
        }
        else
        {
          PropertyList[0].CityLandingPageText = ((PropertyList[0].PropertyType == "RET") ? GetItem.GetDictionaryItem("Retirement Homes in and around", Language) : GetItem.GetDictionaryItem("LongTermCareResidencesInAndAround", Language));
        }
        PropertyList[0].SearchText = ((SearchType == "PostalCode") ? SearchText.ToUpper() : SearchText.ToTitleCase());
        PropertyList[0].Language = Language;
        PropertyList[0].CityLandingPage = true;
        PropertyList[0].CityLandingPageButton = CityLandingPageButton;
        PropertyList[0].SearchResults = ((!System.Convert.ToBoolean(IsRegion)) ? true : false);
        PropertyList[0].PageSize = PageSize;
        PropertyList[0].FoundCitySearch = ((!System.Convert.ToBoolean(IsRegion)) ? true : false);
        PropertyList[0].Display_RegionsDD = (System.Convert.ToBoolean(IsRegion) ? true : false);
        PropertyList[0].IsRegion = System.Convert.ToBoolean(IsRegion);
        PropertyList[0].RegionList = (System.Convert.ToBoolean(IsRegion) ? RegionsDDL(Language) : null);
        if (PropertyList[0].RegionList != null && PropertyList[0].RegionList.Count() > 0)
        {
          SelectListItem selectListItem = (from x in PropertyList[0].RegionList
                                           where x.Text == SearchText
                                           select x).FirstOrDefault();
          if (selectListItem != null)
          {
            selectListItem.Selected = true;
          }
          else
          {
            selectListItem = (from x in PropertyList[0].RegionList
                              where x.Text == GetItem.GetDictionaryItem("AllProperties", language)
                              select x).FirstOrDefault();
            selectListItem.Selected = true;
          }
        }
      }
      else
      {
        results = SearchAll(Language);
        PropertyList = ProcessResults(results, language, PageSize);
        PropertyList[0].SearchType = SearchType;
        PropertyList[0].SearchText = ((SearchType == "PostalCode") ? SearchText.ToUpper() : SearchText.ToTitleCase());
        PropertyList[0].CityLandingPageButton = CityLandingPageButton;
        PropertyList[0].Language = Language;
        PropertyList[0].CityLandingPageText = GetItem.GetDictionaryItem("SearchResultsNotFound", Language);
        PropertyList[0].SearchResults = false;
        PropertyList[0].Display_RegionsDD = (System.Convert.ToBoolean(IsRegion) ? true : false);
        PropertyList[0].IsRegion = System.Convert.ToBoolean(IsRegion);
        PropertyList[0].PageSize = PageSize;
        PropertyList[0].FoundCitySearch = false;
        PropertyList[0].RegionList = (System.Convert.ToBoolean(IsRegion) ? RegionsDDL(Language) : null);
        if (PropertyList[0].RegionList != null && PropertyList[0].RegionList.Count() > 0)
        {
          SelectListItem selectListItem2 = (from x in PropertyList[0].RegionList
                                            where x.Text == SearchText
                                            select x).FirstOrDefault();
          if (selectListItem2 != null)
          {
            selectListItem2.Selected = true;
          }
          else
          {
            selectListItem2 = (from x in PropertyList[0].RegionList
                               where x.Text == GetItem.GetDictionaryItem("AllProperties", language)
                               select x).FirstOrDefault();
            selectListItem2.Selected = true;
          }
        }
      }
    }
    return PartialView(PropertyList);
  }

  public PartialViewResult Index()
  {
    return PartialView("~/Views/SearchResultsGrid/RandomIndex.cshtml", RandomModel());
  }

  private PropertyCustomModel RandomModel()
  {
    Database database = Context.Database;
    Item item = PageContext.Current.Item;
    List<Item> list = new List<Item>();
    MultilistField multilistField = item.Fields["Property"];
    string splitterPageTitle = item.Fields["Title"].ToString();
    string splitterPageDescription = item.Fields["Description"].ToString();
    if (multilistField != null && multilistField.TargetIDs != null)
    {
      ID[] targetIDs = multilistField.TargetIDs;
      foreach (ID itemID in targetIDs)
      {
        Item item2 = Context.Database.Items[itemID];
        list.Add(item2);
      }
    }
    List<PropertySearchModel> list2 = new List<PropertySearchModel>();
    foreach (Item item6 in list)
    {
      Database database2 = Context.Database;
      Item item3 = database2.GetItem(item6.ID);
      string itemUrl = LinkManager.GetItemUrl(item3, new UrlOptions
      {
        UseDisplayName = true,
        LowercaseUrls = true,
        LanguageEmbedding = LanguageEmbedding.Always,
        LanguageLocation = LanguageLocation.FilePath,
        Language = Context.Language
      });
      string id = item3.Fields["Province"].ToString();
      ID itemId = new ID(id);
      Item item4 = database.GetItem(itemId);
      string text = item4.Fields["Province Name"].ToString();
      ChartwellUtiles chartwellUtiles = new ChartwellUtiles();
      string value = chartwellUtiles.FormattedAddress(item3, text);
      PropertySearchModel item5 = new PropertySearchModel
      {
        ItemID = item6.ID.ToString(),
        PropertyID = item6.Fields["property id"].ToString(),
        PropertyName = item6.Fields["Property Name"].ToString(),
        PropertyDesc = item6.Fields["Property Description"].ToString(),
        PropertyTagLine = item6.Fields["Property Tag Line"].ToString(),
        USP = item6.Fields["USP"].ToString(),
        PhoneNo = chartwellUtiles.GetPhoneNumber(item6),
        StreetName = item6.Fields["Street name and number"].ToString(),
        City = item6.Fields["Selected City"].ToString(),
        PostalCode = item6.Fields["Postal code"].ToString(),
        Province = text,
        PropertyItemUrl = itemUrl + "/" + Translate.Text("overview"),
        PropertyFormattedAddress = new HtmlString(value),
        InnerItem = item3,
        Longitude = item6.Fields["Longitude"].ToString(),
        Latitude = item6.Fields["Latitude"].ToString()
      };
      list2.Add(item5);
    }
    return new PropertyCustomModel
    {
      lstProperty = list2,
      SplitterPageTitle = splitterPageTitle,
      SplitterPageDescription = splitterPageDescription
    };
  }

  public PartialViewResult SplitterPageIndex()
  {
    return PartialView("~/Views/SearchResultsGrid/SplitterPage.cshtml", CreateSplitterModel());
  }

  private PropertyCustomModel CreateSplitterModel()
  {
    Database database = Context.Database;
    Item item = Context.Item;
    List<Item> list = new List<Item>();
    MultilistField multilistField = item.Fields["SelectedProperty"];
    string splitterPageTitle = item.Fields["Title"].ToString();
    string splitterPageDescription = item.Fields["Description"].ToString();
    if (multilistField != null && multilistField.TargetIDs != null)
    {
      ID[] targetIDs = multilistField.TargetIDs;
      foreach (ID itemID in targetIDs)
      {
        Item item2 = Context.Database.Items[itemID];
        list.Add(item2);
      }
    }
    List<PropertySearchModel> list2 = new List<PropertySearchModel>();
    int num = 0;
    foreach (Item item6 in list)
    {
      num++;
      Database database2 = Context.Database;
      Item item3 = database2.GetItem(item6.ID);
      string itemUrl = LinkManager.GetItemUrl(item3, new UrlOptions
      {
        UseDisplayName = true,
        LowercaseUrls = true,
        LanguageEmbedding = LanguageEmbedding.Always,
        LanguageLocation = LanguageLocation.FilePath,
        Language = Context.Language
      });
      ChartwellUtiles chartwellUtiles = new ChartwellUtiles();
      string id = item3.Fields["Province"].ToString();
      ID itemId = new ID(id);
      Item item4 = database.GetItem(itemId);
      string text = item4.Fields["Province Name"].ToString();
      string fieldName = "PropertyDescription" + num;
      string value = chartwellUtiles.FormattedAddress(item3, text);
      PropertySearchModel item5 = new PropertySearchModel
      {
        ItemID = item6.ID.ToString(),
        PropertyID = item6.Fields["property id"].ToString(),
        PropertyName = item6.Fields["Property Name"].ToString(),
        PropertyDesc = item6.Fields["Property Description"].ToString(),
        PropertyTagLine = item6.Fields["Property Tag Line"].ToString(),
        PhoneNo = chartwellUtiles.GetPhoneNumber(item6),
        USP = item.Fields[fieldName].ToString(),
        StreetName = item6.Fields["Street name and number"].ToString(),
        City = item6.Fields["Selected City"].ToString(),
        PostalCode = item6.Fields["Postal code"].ToString(),
        Province = text,
        PropertyItemUrl = itemUrl + "/" + Translate.Text("overview"),
        PropertyFormattedAddress = new HtmlString(value),
        InnerItem = item3,
        Longitude = item6.Fields["Longitude"].ToString(),
        Latitude = item6.Fields["Latitude"].ToString(),
        SplitterPageTitle = splitterPageTitle,
        SplitterPageDescription = splitterPageDescription
      };
      list2.Add(item5);
    }
    return new PropertyCustomModel
    {
      lstProperty = list2,
      SplitterPageTitle = splitterPageTitle,
      SplitterPageDescription = splitterPageDescription
    };
  }

  public PartialViewResult NewPropertyIndex()
  {
    return PartialView("~/Views/SearchResultsGrid/NewResidencePage.cshtml", CreateNewPropertyModel());
  }

  private PropertyCustomModel CreateNewPropertyModel()
  {
    Database database = Context.Database;
    string query = "//*[@#Is New Property# ='1']";
    Item[] array = Context.Database.SelectItems(query);
    List<Item> list = new List<Item>();
    HashSet<string> hashSet = new HashSet<string>();
    List<PropertySearchModel> list2 = new List<PropertySearchModel>();
    Item[] array2 = array;
    foreach (Item item in array2)
    {
      Database database2 = Context.Database;
      Item item2 = database2.GetItem(item.ID);
      string itemUrl = LinkManager.GetItemUrl(item2, new UrlOptions
      {
        UseDisplayName = true,
        LowercaseUrls = true,
        LanguageEmbedding = LanguageEmbedding.Always,
        LanguageLocation = LanguageLocation.FilePath,
        Language = Context.Language
      });
      string id = item2.Fields["Province"].ToString();
      ID itemId = new ID(id);
      Item item3 = database.GetItem(itemId);
      string text = item3.Fields["Province Name"].ToString();
      ChartwellUtiles chartwellUtiles = new ChartwellUtiles();
      string value = chartwellUtiles.FormattedAddress(item2, text);
      hashSet.Add(text);
      PropertySearchModel item4 = new PropertySearchModel
      {
        ItemID = item.ID.ToString(),
        PropertyID = item.Fields["property id"].ToString(),
        PropertyName = item.Fields["Property Name"].ToString(),
        PropertyDesc = item.Fields["Property Description"].ToString(),
        PropertyTagLine = item.Fields["Property Tag Line"].ToString(),
        USP = item.Fields["USP"].ToString(),
        PhoneNo = chartwellUtiles.GetPhoneNumber(item),
        StreetName = item.Fields["Street name and number"].ToString(),
        City = item.Fields["Selected City"].ToString(),
        PostalCode = item.Fields["Postal code"].ToString(),
        Province = text,
        PropertyItemUrl = itemUrl + "/" + Translate.Text("overview"),
        PropertyFormattedAddress = new HtmlString(value),
        InnerItem = item2,
        Longitude = item.Fields["Longitude"].ToString(),
        Latitude = item.Fields["Latitude"].ToString()
      };
      list2.Add(item4);
    }
    return new PropertyCustomModel
    {
      lstProperty = list2,
      lstPropertyProvince = hashSet
    };
  }

  public List<PropertySearchModel> CitySearchDistance(IEnumerable<SearchResultItem> queryResult, string Language, int PageSize, string paramPropertyType, string searchCriteria, string searchType)
  {
    IEnumerable<PropertySearchModel> source = null;
    string strLatitude = string.Empty;
    string strLongitude = string.Empty;
    if (searchType == "PostalCode")
    {
      LatLngForPostalCode(searchCriteria.Replace(" ", ""), out string Lat, out string Lng);
      strLatitude = Lat;
      strLongitude = Lng;
    }
    else
    {
      SearchResultItem searchResultItem = (from x in GetItem.PropertyDetails(queryResult.ToList()[0].ItemId)
                                           where x.Language == Language
                                           select x).ToList().FirstOrDefault();
      strLatitude = searchResultItem.Fields["lat"].ToString();
      strLongitude = searchResultItem.Fields["lng"].ToString();
    }
    string o = strLatitude + "," + strLongitude;
    ChartwellUtiles util = new ChartwellUtiles();
    List<PropertySearchModel> list = new List<PropertySearchModel>();
    using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
    {
      Expression<Func<SearchResultItem, bool>> first = PredicateBuilder.True<SearchResultItem>();
      first = first.And((SearchResultItem d) => d.Language == Language);
      first = first.And((SearchResultItem d) => d.TemplateName == "Property Page");
      first = first.And((SearchResultItem d) => d["Property ID"] != "99999");
      first = first.And((SearchResultItem d) => d.Name != "__Standard Values");
      List<SearchResultItem> source2 = providerSearchContext.GetQueryable<SearchResultItem>().Where(first).WithinRadius((SearchResultItem s) => s.Location, o, 500.0)
          .OrderByDistance((SearchResultItem d) => d.Location, o)
          .ToList();
      source = (from item in source2
                select new PropertySearchModel
                {
                  ItemID = item.ItemId.ToString(),
                  PropertyID = item.GetField("property id").Value,
                  PropertyName = item.GetField("property name").Value,
                  PropertyType = PropertyType(Language, item.GetItem()),
                  PropertyDesc = item.GetField("property description").Value,
                  PropertyTagLine = item.GetField("property tag line").Value,
                  USP = item.GetField("USP").Value,
                  PhoneNo = util.GetPhoneNumber(item.GetItem()),
                  StreetName = item.GetField("Street name and number").Value,
                  City = item.GetField("Selected City").Value,
                  PostalCode = item.GetField("Postal Code").Value,
                  Province = ProvinceName(Language, item.GetItem()),
                  Country = item.GetField("Country").Value,
                  PropertyItemUrl = GetPropertyUrl(Language, item.GetItem()),
                  PropertyFormattedAddress = new HtmlString(util.FormattedAddress(item.GetItem(), ProvinceName(Language, item.GetItem()))),
                  InnerItem = item.GetItem(),
                  Distance = Distance(double.Parse(strLatitude, CultureInfo.InvariantCulture), double.Parse(strLongitude, CultureInfo.InvariantCulture), item.Location.Latitude, item.Location.Longitude, 'K'),
                  PropertyImage = GetImageUrl(item.GetItem()),
                  CityLandingPageText = ((PropertyType(Language, item.GetItem()) == "RET") ? "Retirement Homes" : "Long Term Care Residences"),
                  PageTitle = item.GetField("PageTitle").Value,
                  PageDescription = item.GetField("pagedescription").Value,
                  PageKeyword = item.GetField("pagekeyword").Value,
                  SearchResults = (foundSearchResults ? true : false),
                  PageSize = 9,
                  Language = language,
                  PageStartIndex = 0
                } into x
                where x.PropertyType == paramPropertyType
                select x).Skip(0).Take(PageSize).ToList();
    }
    return source.ToList();
  }

  private string GetPropertyUrl(string Language, Item PropertyItem)
  {
    string itemUrl = LinkManager.GetItemUrl(PropertyItem, new UrlOptions
    {
      UseDisplayName = true,
      LowercaseUrls = true,
      AlwaysIncludeServerUrl = false,
      LanguageEmbedding = LanguageEmbedding.Always,
      LanguageLocation = LanguageLocation.FilePath,
      Language = PropertyItem.Language
    });
    string empty = string.Empty;
    empty = ((!itemUrl.Contains("sumach")) ? (itemUrl + "/" + Translate.TextByLanguage("overview", PropertyItem.Language)) : (base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/en/the-sumach"));
    return empty.Replace("/sitecore/shell/chartwell", "");
  }

  private string GetImageUrl(Item SearchPropertyItem)
  {
    ImageField imageField = SearchPropertyItem.Fields["Thumbnail Photo"];
    string text = (imageField != null) ? MediaManager.GetMediaUrl(imageField.MediaItem) : string.Empty;
    return text.Replace("/sitecore/shell", "");
  }

  private string PropertyType(string Language, Item SearchPropertyItem)
  {
    string name = GetItem.PropertyDetails(new ID(SearchPropertyItem.Fields["property type"].ToString())).FirstOrDefault().GetItem()
        .Name;
    string text = SearchPropertyItem.Fields["property type"].ToString();
    string result = string.Empty;
    if (!string.IsNullOrEmpty(text))
    {
      ID itemID = new ID(text);
      List<SearchResultItem> list = (from x in GetItem.PropertyDetails(itemID)
                                     where x.Language == Language
                                     select x).ToList();
      Item item = list[0].GetItem();
      result = item.Fields["property type"].ToString();
    }
    return result;
  }

  private string ProvinceName(string Language, Item SearchPropertyItem)
  {
    string text = SearchPropertyItem.Fields["Province"].ToString();
    string result = string.Empty;
    if (!string.IsNullOrEmpty(text))
    {
      ID itemID = new ID(text);
      List<SearchResultItem> list = (from x in GetItem.PropertyDetails(itemID)
                                     where x.Language == Language
                                     select x).ToList();
      Item item = list[0].GetItem();
      result = item.Fields["Province Name"].ToString();
    }
    return result;
  }

  public List<PropertySearchModel> PostalCodeDistance(IEnumerable<SearchResultItem> queryResult, string lat, string lng, string Language)
  {
    List<PropertySearchModel> list = new List<PropertySearchModel>();
    foreach (SearchResultItem item6 in queryResult)
    {
      string o = lat + "," + lng;
      IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext();
      IOrderedQueryable<SearchResultItem> source = (from item in providerSearchContext.GetQueryable<SearchResultItem>()
                                                    where item.Language == Language
                                                    select item).WithinRadius((SearchResultItem s) => s.Location, o, 500.0).OrderByDistance((SearchResultItem s) => s.Location, o);
      List<SearchResultItem> list2 = source.ToList();
      foreach (SearchResultItem item7 in list2)
      {
        List<SearchResultItem> list3 = (from x in GetItem.PropertyDetails(item7.ItemId)
                                        where x.Language == Language
                                        select x).ToList();
        Item item2 = list3[0].GetItem();
        ChartwellUtiles chartwellUtiles = new ChartwellUtiles();
        string itemUrl = LinkManager.GetItemUrl(item2, new UrlOptions
        {
          UseDisplayName = true,
          LowercaseUrls = true,
          LanguageEmbedding = LanguageEmbedding.Always,
          LanguageLocation = LanguageLocation.FilePath,
          Language = item2.Language
        });
        string text = itemUrl + "/" + GetItem.GetDictionaryItem("overview", Language);
        string text2 = item2.Fields["Province"].ToString();
        string text3 = string.Empty;
        if (!string.IsNullOrEmpty(text2))
        {
          ID itemID = new ID(text2);
          List<SearchResultItem> list4 = (from x in GetItem.PropertyDetails(itemID)
                                          where x.Language == Language
                                          select x).ToList();
          Item item3 = list4[0].GetItem();
          text3 = item3.Fields["Province Name"].ToString();
        }
        string text4 = item2.Fields["property type"].ToString();
        string text5 = string.Empty;
        if (!string.IsNullOrEmpty(text4))
        {
          ID itemID2 = new ID(text4);
          List<SearchResultItem> list5 = (from x in GetItem.PropertyDetails(itemID2)
                                          where x.Language == Language
                                          select x).ToList();
          Item item4 = list5[0].GetItem();
          text5 = item4.Fields["property type"].ToString();
        }
        string value = item7.GetField("property name").Value;
        ImageField imageField = item2.Fields["Thumbnail Photo"];
        string mediaUrl = MediaManager.GetMediaUrl(imageField.MediaItem);
        double distance = Distance(double.Parse(lat, CultureInfo.InvariantCulture), double.Parse(lng, CultureInfo.InvariantCulture), item7.Location.Latitude, item7.Location.Longitude, 'K');
        string value2 = chartwellUtiles.FormattedAddress(item2, text3);
        PropertySearchModel item5 = new PropertySearchModel
        {
          ItemID = item7.ItemId.ToString(),
          PropertyID = item7.GetField("property id").Value,
          PropertyName = value,
          PropertyType = text5,
          PropertyDesc = item7.GetField("property description").Value,
          PropertyTagLine = item7.GetField("property tag line").Value,
          USP = item7.GetField("USP").Value,
          PhoneNo = chartwellUtiles.GetPhoneNumber(item2),
          StreetName = item7.GetField("Street name and number").Value,
          City = item7.GetField("Selected City").Value,
          PostalCode = item7.GetField("Postal Code").Value,
          Province = text3,
          Country = item7.GetField("Country").Value,
          PropertyItemUrl = text.Replace("/sitecore/shell/chartwell", ""),
          PropertyFormattedAddress = new HtmlString(value2),
          Longitude = item7.GetField("Longitude").Value,
          Latitude = item7.GetField("Latitude").Value,
          InnerItem = item2,
          Distance = distance,
          PropertyImage = mediaUrl.Replace("/sitecore/shell", ""),
          Language = Language,
          CityLandingPageText = ((text5 == "RET") ? "Retirement Homes" : "Long Term Care Residences"),
          SearchResults = (foundSearchResults ? true : false)
        };
        list.Add(item5);
        base.TempData["ListWithDistance"] = list;
      }
    }
    return list;
  }

  public double Distance(double lat1, double lon1, double lat2, double lon2, char unit)
  {
    double deg = lon1 - lon2;
    double d = Math.Sin(Deg2rad(lat1)) * Math.Sin(Deg2rad(lat2)) + Math.Cos(Deg2rad(lat1)) * Math.Cos(Deg2rad(lat2)) * Math.Cos(Deg2rad(deg));
    d = Math.Acos(d);
    d = Rad2deg(d);
    d = d * 60.0 * 1.1515;
    switch (unit)
    {
      case 'K':
        d *= 1.609344;
        break;
      case 'N':
        d *= 0.8684;
        break;
    }
    return d;
  }

  private double Deg2rad(double deg)
  {
    return deg * Math.PI / 180.0;
  }

  private double Rad2deg(double rad)
  {
    return rad / Math.PI * 180.0;
  }
}
