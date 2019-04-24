using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Resources.Media;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Chartwell.Foundation.utility
{
  public class ChartwellUtiles
  {

    public ChartwellUtiles()
    { }
    /// <summary>
    /// For retrieving isUnity phone number from Primary contact of the properties
    /// </summary>
    /// <returns></returns>
    public String GetPhoneNumber(Item item)
    {
      string itemPath = item.Paths.Path.ToString().ToLower();
      //  itemPath = itemPath.Replace(Sitecore.Context.Data.Site.RootPath.ToLower(), "");
      //   itemPath = itemPath.Replace(Sitecore.Context.Data.Site.StartItem.ToLower(), "");
      string phonenumber = "";
      foreach (Item root in item.Children)
      {
        if (root.Name.Contains("primarycontact"))
        {
          phonenumber = root.Fields["Phone"].ToString();
        }
        if (phonenumber == "" && root.Name.Contains("landlinecontact")) //only get landlineinformation when there is no unity number in the property
        {
          phonenumber = root.Fields["Phone"].ToString();
        }
      }

      return SetPhoneNumberWithDashes(phonenumber.Trim());
    }

    public String GetEmail(Item item)
    {
      string itemPath = item.Paths.Path.ToString().ToLower();
      //  itemPath = itemPath.Replace(Sitecore.Context.Data.Site.RootPath.ToLower(), "");
      //   itemPath = itemPath.Replace(Sitecore.Context.Data.Site.StartItem.ToLower(), "");
      string email = "";
      foreach (Item root in item.Children)
      {
        if (root.Name.Contains("primarycontact"))
        {
          email = root.Fields["Email"].ToString();
        }
        if (email == "" && root.Name.Contains("landlinecontact")) //only get landlineinformation when there is no unity number in the property
        {
          email = root.Fields["Email"].ToString();
        }
      }

      return email.Trim();
    }

    public string GetDictionaryItem(string Label, string Language)
    {
      List<SearchResultItem> matches;

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();

        predicate = predicate.And(p => p.Path.StartsWith("/sitecore/system/Dictionary"));
        predicate = predicate.And(p => p.Name == Label);

        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }

      string dictionaryPhrase = matches.Where(x => x.Language == Language).Select(x => x.Fields["phrase"]).FirstOrDefault().ToString();
      //return matches;
      return dictionaryPhrase;
    }

    #region methods from Dlls on Prod
    public List<string> ValidSearchKeywords(string tUrl)
    {
      return (from s in (from y in (from x in HttpUtility.UrlDecode(tUrl).Split('&')[0].Split('=')
                                    where x != "item"
                                    select x into st
                                    where !string.IsNullOrEmpty(st)
                                    select st).FirstOrDefault().Split('/')
                         where !string.IsNullOrEmpty(y)
                         select y).ToList()
              where !s.Equals("retirement-homes") && !s.Equals("retirement-residences") && !s.Equals("résidences-pour-retraités") &&
                    !s.Equals("soins-de-longue-durée") && !s.Equals("long-term-care-homes") &&
                    !s.Equals("continuum-of-care") && !s.Equals("complexes-évolutifs")
              select s).ToList();
    }

    public bool ValidRedirectRequests(string iUrl)
    {
      return iUrl.ToLower().Contains("retirement-homes") || iUrl.ToLower().Contains("retirement-living") || iUrl.ToLower().Contains("la-vie-en-résidence") || iUrl.ToLower().Contains("résidences-de-prestige") || iUrl.ToLower().Contains("luxurious") || iUrl.ToLower().Contains("retirement-residences") || iUrl.ToLower().Contains("résidences-pour-retraités") || iUrl.ToLower().Contains("soins-de-longue-durée") || iUrl.ToLower().Contains("long-term-care-homes") || iUrl.ToLower().Contains("continuum-of-care") || iUrl.ToLower().Contains("complexes-évolutifs") || iUrl.ToLower().Contains("overview") || iUrl.ToLower().Contains("résultat-de-la-recherche") || iUrl.ToLower().Contains("accueil");
    }

    public List<SearchResultItem> SearchItemFromAddressBar(string term)
    {
      List<SearchResultItem> source;
      using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        Expression<Func<SearchResultItem, bool>> first = PredicateBuilder.True<SearchResultItem>();
        first = first.And((SearchResultItem p) => p.TemplateName.Equals("Property Page"));
        first = first.And((SearchResultItem p) => p.Name.Contains(term));
        source = providerSearchContext.GetQueryable<SearchResultItem>().Where(first).ToList();
      }
      return source.ToList();
    }

    public List<SearchResultItem> GetOverviewHomePageItem(string phrase)
    {
      phrase = HttpUtility.UrlDecode(phrase.Replace("/en/", "").Replace("/fr/", "").Replace("/", "")
          .Replace("-", " ")).ToLower();
      List<SearchResultItem> source;
      using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        Expression<Func<SearchResultItem, bool>> first = PredicateBuilder.True<SearchResultItem>();
        if (phrase.Contains("overview"))
        {
          first = first.And((SearchResultItem p) => p.Path.Contains(phrase));
        }
        else
        {
          first = first.Or((SearchResultItem p) => p.TemplateName.Equals("Static Pages"));
          first = first.And((SearchResultItem p) => !p.Name.Equals("__Standard Values"));
        }
        source = providerSearchContext.GetQueryable<SearchResultItem>().Where(first).ToList();
      }
      return (from x in source
              where x.GetItem().DisplayName.ToLower().Equals(phrase)
              select x).ToList();
    }

    public SearchResultItem GetLeftNavItemName(List<string> urlItem)
    {
      string partialItemPath = string.Empty;
      string itemTemplate = string.Empty;
      if (urlItem != null && urlItem.Count() == 1)
      {
        partialItemPath = urlItem[0].Replace("-", " ");
        itemTemplate = "overview";
      }
      else
      {
        partialItemPath = urlItem[1].Replace("-", " ");
        itemTemplate = urlItem[0];
      }
      List<SearchResultItem> source;
      using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        Expression<Func<SearchResultItem, bool>> first = PredicateBuilder.True<SearchResultItem>();
        first = first.And((SearchResultItem p) => p.Path.StartsWith("/sitecore/content/Chartwell/retirement-residences/" + partialItemPath));
        source = providerSearchContext.GetQueryable<SearchResultItem>().Where(first).ToList();
      }
      return (from x in source
              where x.GetItem().DisplayName.ToLower().Equals(itemTemplate)
              select x).FirstOrDefault();
    }

    public SearchResultItem GetFieldName(string url)
    {
      SearchResultItem result = null;
      string tUrl = (from x in url.Split('/')
                     where !string.IsNullOrEmpty(x)
                     select x).Last();
      using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        Expression<Func<SearchResultItem, bool>> first = PredicateBuilder.True<SearchResultItem>();
        first = first.And((SearchResultItem p) => p.TemplateName.Equals("Property Page"));
        List<SearchResultItem> source = providerSearchContext.GetQueryable<SearchResultItem>().Where(first).ToList();
        result = (from x in source
                  where x.GetItem().DisplayName.Contains(tUrl.ToLower()) || x.GetField("Selected City").Value.ToLower().Contains(tUrl.ToLower()) || x.GetField("Postal code").Value.ToLower().Contains(tUrl.ToLower())
                  select x).FirstOrDefault();
      }
      return result;
    }

    public string CheckAndSetLangForItem(List<string> sList, string iUrl, string LanguageFrom404Item)
    {
      return (iUrl.Contains("continuum-of-care") || iUrl.Contains("retirement-living") || iUrl.Contains("luxurious")) ? "en" : 
              ((LanguageFrom404Item.Contains("résidences-pour-retraités") || LanguageFrom404Item.Contains("soins-de-longue-durée") || 
              LanguageFrom404Item.Contains("complexes-évolutifs") || LanguageFrom404Item.Contains("résultat-de-la-recherche") || 
              LanguageFrom404Item.Contains("accueil") || LanguageFrom404Item.Contains("la-vie-en-résidence") || 
              LanguageFrom404Item.Contains("aperçu") || LanguageFrom404Item.Contains("résidences-de-prestige")) ? "fr" : 
              (((Context.Item != null && !Context.Item.Name.Contains("404")) || 
              LanguageFrom404Item.Contains("overview")) ? GetLeftNavItemName(sList).GetItem().Language.Name : Context.Language.Name));
    }

    #endregion

    public List<SearchResultItem> PropertyDetails(ID itemID)
    {
      List<SearchResultItem> CitySearchDistanceResults = null;
      using (var propcontext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        CitySearchDistanceResults = propcontext.GetQueryable<SearchResultItem>()
         .Where(x => x.ItemId == itemID)
         //.Where(x => x.Language == Sitecore.Context.Language.Name)
         .ToList();
      }

      return CitySearchDistanceResults;
    }

    #region
    //public List<SearchResultItem> GetYardiForCommunity(string CommunityName)
    //{
    //  List<SearchResultItem> CommunityResidencesResults = null;
    //  using (var propcontext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
    //  {
    //    CommunityResidencesResults = propcontext.GetQueryable<SearchResultItem>()
    //     .Where(x => x.TemplateName == "Property Page")
    //     .Where(x => x.Name != "__Standard Values")
    //     .Where(x => x["Property ID"] != "99999")
    //     //.Where(x => x["SplitterURL"].Contains(CommunityName))
    //     //.Where(x => x.Language == Sitecore.Context.Language.Name)
    //     .ToList();
    //  }

    //  List<SearchResultItem> propName = CommunityResidencesResults.Where(x => x.GetField("SplitterURL").Value.RemoveDiacritics().Replace("-", " ").Replace("'", "").Contains(CommunityName.ToLower())).ToList();
    //  CommunityResidencesResults = propName;

    //  //var YardiIDForCommunity = CommunityResidencesResults.Where(x => x["name"].Contains("retirement residence") || x["name"].Contains("residence pour retraites")).Select(i => i.GetField("property id").Value).FirstOrDefault();
    //  List<SearchResultItem> YardiIDForCommunity = CommunityResidencesResults.Where(x => x["name"].Contains("retirement residence") || x["name"].Contains("residence pour retraites")).ToList();

    //  return YardiIDForCommunity;
    //}
    #endregion

    public List<SearchResultItem> GetYardiForCommunity(string CommunityName)
    {
      List<SearchResultItem> source = null;
      using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        source = (from x in providerSearchContext.GetQueryable<SearchResultItem>()
                  where x.TemplateName == "Property Page"
                  where x.Name != "__Standard Values"
                  where x["Property ID"] != "99999"
                  where x["SplitterURL"].Equals(CommunityName)
                  select x).ToList();
      }
      return (from x in source
              where x["name"].Contains("retirement residence") || x["name"].Contains("residence pour retraites")
              select x).ToList();
    }

    public IEnumerable<SearchResultItem> SearchSelectedCity(string language, string searchCriteria)
    {
      IEnumerable<SearchResultItem> results = null;
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        results = context.GetQueryable<SearchResultItem>()
         .Where(x => x.TemplateName == "City")
         .Where(x => x.Language == language)
         //.Where(x => x["City Name"] == searchCriteria)
         .Where(x => x.Name == searchCriteria.RemoveDiacritics().Replace(" ", ""))
         .OrderBy(o => o.Name).ToList();
      }
      return results.ToList();
    }

    public IEnumerable<SearchResultItem> SearchSelectedCityWithSpaces(string language, string searchCriteria)
    {
      IEnumerable<SearchResultItem> results = null;

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        results = context.GetQueryable<SearchResultItem>()
         .Where(x => x.TemplateName == "City")
         .Where(x => x.Language == language)
         .Where(x => x.Name == searchCriteria.RemoveDiacritics().TrimPunctuation().ToLower())
         .OrderBy(o => o.Name).ToList();
      }
      return results.ToList();
    }

    public IEnumerable<SearchResultItem> SwitchSelectedCity(string language, string searchCriteria)
    {
      IEnumerable<SearchResultItem> results = null;
      List<SearchResultItem> matches;

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();

        predicate = predicate.And(p => p.Path.StartsWith("/sitecore/content/Chartwell/Content Shared Folder/City"));
        predicate = predicate.And(p => p["City Name"] == searchCriteria.Replace("-", " "));

        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }

      string selectedItem = "";
      ID selectedItemID = new ID();
      foreach (SearchResultItem item in matches)
      {
        if (item.GetItem().Fields["City Name"].Value.ToLower() == searchCriteria.Replace("-", " ").ToLower())
        {
          selectedItem = item.Name;
          selectedItemID = item.ItemId;
          break;
        }
      }
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        results = context.GetQueryable<SearchResultItem>()
         .Where(x => x.TemplateName == "City")
         .Where(x => x.ItemId == selectedItemID)
         .OrderBy(o => o.Name).ToList();
      }

      return results.ToList();
    }

    public string FormattedAddress(Item PropertyItem, string ProvinceName)
    {
      string PropertyFormattedAddress = "";
      if (PropertyItem.Language.ToString() == "en")
      {
        PropertyFormattedAddress = PropertyItem.Fields["Street name and number"].ToString() + ", " + PropertyItem.Fields["Selected City"].ToString() + ", " + ProvinceName + " " + PropertyItem.Fields["Postal code"].ToString();



      }

      else
      {
        string str = PropertyItem.Fields["Postal code"].ToString();
        string postalcode = Indent(3) + str;
        PropertyFormattedAddress = PropertyItem.Fields["Street name and number"].ToString() + ", " + PropertyItem.Fields["Selected City"].ToString() + " (" + ProvinceName + ")" + postalcode; //to add extra space



      }
      return PropertyFormattedAddress;
    }

    public static string Indent(int count)
    {
      return "&nbsp;&nbsp;";
    }

    public string SetPhoneNumberWithDashes(string phoneNumberValue)
    {

      string phone = RemoveSpecialCharacters(phoneNumberValue);
      //Check if it is null or contains any non-digits


      //Check if it is in the format : ###-###-####
      if (!Regex.IsMatch(phone, @"\d{3}\-\d{3}\-\d{4}"))
      {
        if (phone.Length == 10)
          return string.Format("{0:###-###-####}", double.Parse(phone));
        else
          return phoneNumberValue;
      }




      //Otherwise return the empty string
      return string.Empty;
    }

    public string RemoveSpecialCharacters(string str)
    {
      StringBuilder sb = new StringBuilder();
      foreach (char c in str)
      {
        if ((c >= '0' && c <= '9'))
        {
          sb.Append(c);
        }
      }
      return sb.ToString();
    }

    public string GetLegacyPropertyName(int propertyID, string constring)
    {
      SqlConnection conn = new SqlConnection(constring);
      SqlCommand cmd = new SqlCommand();
      SqlDataReader reader;

      cmd.CommandText = @"Select * from property where propertyIdfromYardi = " + propertyID + " and IsDeleted = 0";

      cmd.CommandType = CommandType.Text;

      cmd.Connection = conn;

      conn.Open();

      reader = cmd.ExecuteReader();
      string contactPropertyName = string.Empty;
      while (reader.Read())
      {
        contactPropertyName = reader["PropertyNameEn"].ToString();
      };

      return contactPropertyName;
    }

    public void InsertPropertyIntoSitecoreOLP(int propertyID, string propertyName, string constring)
    {
      SqlConnection conn = new SqlConnection(constring);
      SqlCommand cmd = new SqlCommand();
      SqlDataReader reader;

      cmd.CommandText = @"Insert into Property (PropertyNameEn, PropertyIDFromYardi) values ('" + propertyName + "', " + propertyID + ")";

      cmd.CommandType = CommandType.Text;

      cmd.Connection = conn;

      conn.Open();

      reader = cmd.ExecuteReader();
    }

    public Language SetLanguageFromUrl(string sanitizedUrl)
    {
      if (sanitizedUrl.Contains("résultat-de-la-recherche"))
        sanitizedUrl = "/fr" + sanitizedUrl;

      var LangFromUrl = sanitizedUrl.Split('/').Where(x => !string.IsNullOrEmpty(x)).Where(x => x.Equals("en") || x.Equals("fr")).FirstOrDefault();
      Language.TryParse(string.IsNullOrEmpty(LangFromUrl) ? "en" : LangFromUrl, out Language LanguageFromUrl);
      Context.SetLanguage(LanguageFromUrl, true);
      return Context.Language;
    }

    public List<SearchResultItem> GetItemForUrl(string url)
    {
      List<SearchResultItem> matches;
      var tUrl = url.Split('/').Where(x => !string.IsNullOrEmpty(x)).Last();
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.TemplateName.Equals("SplitterPage"));
        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }

      var SearchResultItem = matches.Where(x => x.GetItem().DisplayName.ToLower().Equals(tUrl)).ToList();
      return SearchResultItem;
    }

    public SearchResultItem GetItemForUrl(string url, string urlLang)
    {
      List<SearchResultItem> matches;
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.TemplateName.Equals("Property Page") &&
        (p.Name.Equals(url.Replace("-", " ").Replace("'", "").RemoveDiacritics()) || p.Name.Contains(url.Replace("-", " ").Replace("'", "").RemoveDiacritics())));
        predicate = predicate.Or(p => p.TemplateName.Equals("SplitterPage"));
        predicate = predicate.Or(p => p.TemplateName.Equals("URL-Mapping") && p["DNN URL"].Contains(url));
        predicate = predicate.Or(p => p.TemplateName.Equals("Custom Property"));
        predicate = predicate.And(p => !p.Name.Equals("__Standard Values"));
        predicate = predicate.And(p => p.Language == urlLang);
        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }

      var SearchResultItem = matches.FirstOrDefault(f => f.GetItem()["DNN URL"].Contains(url)
      || f.Name.Equals(url.Replace("-", " ").Replace("'", "").RemoveDiacritics())
      || f.Name.Contains(url.Replace("-", " ").Replace("'", "").RemoveDiacritics())
      || f.GetItem().DisplayName.ToLower().Equals(url)
      || f.GetItem().DisplayName.Equals(url)
      );
      return SearchResultItem;
    }

    public List<SearchResultItem> GetStaticPageItem(string phrase)
    {
      List<SearchResultItem> matches;
      List<SearchResultItem> searchResultItems = null;
      var splitStaticPageItem = phrase.Split('/').Where(x => !string.IsNullOrEmpty(x)).Last();

      splitStaticPageItem = splitStaticPageItem.Replace("/en/", "").Replace("/fr/", "").Replace("/", "").Replace("-", " ").ToLower();
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.Or(p => p.TemplateName.Equals("Static Pages"));
        predicate = predicate.And(p => !p.Name.Equals("__Standard Values"));
        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();

        searchResultItems = matches.Where(x => x.GetItem().DisplayName.Replace("-", " ").ToLower().Equals(splitStaticPageItem) || x.GetItem().DisplayName.Replace("-", " ").ToLower().Contains(splitStaticPageItem)).ToList();
        if (searchResultItems != null && searchResultItems.Count() == 0)
          searchResultItems = matches.Where(x => x.GetItem().DisplayName.Replace("-", " ").Replace(" ", "").ToLower().Equals(splitStaticPageItem)).ToList();

        if (searchResultItems != null && searchResultItems.Count() == 0)
        {
          predicate = PredicateBuilder.True<SearchResultItem>();

          predicate = predicate.And(p => p.Path.StartsWith("/sitecore/content/URLRedirect/Static"));
          predicate = predicate.And(p => p.TemplateName.Equals("URL-Mapping"));
          predicate = predicate.And(p => p["DNN URL"].Equals(phrase));
          matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
          searchResultItems = matches.ToList();
        }
      }

      return searchResultItems;
    }

    public List<Item> GetStaticPageChildItem(Item item)
    {
      List<Item> child = new List<Item>();
      if (item.Name == "getting-started")
      {
        child = item.Children.InnerChildren.Where(x => x.Name == "understanding-the-benefits").ToList();
      }
      else if (item.Name == "help-me-choose")
      {
        child = item.Children.InnerChildren.Where(x => x.Name == "exploring-your-options").ToList();
      }
      else if (item.Name == "our-services")
      {
        child = item.Children.InnerChildren.Where(x => x.Name == "dining-experience").ToList();
      }
      else if (item.Name == "learn")
      {
        child = item.Children.InnerChildren.Where(x => x.Name == "step-by-step-resources").ToList();
      }
      else
      {
        child = item.Children.InnerChildren.Where(x => x.Name == "welcome").ToList();
      }

      return child;
    }

    public string GetitemUrl(Item item, Sitecore.Globalization.Language language)
    {
      string url = LinkManager.GetItemUrl(Sitecore.Context.Database.GetItem(item.ID, LanguageManager.GetLanguage(language.ToString())), new UrlOptions
      {
        UseDisplayName = true,
        LowercaseUrls = true,
        LanguageEmbedding = LanguageEmbedding.Always,
        LanguageLocation = LanguageLocation.FilePath,
        Language = language

      });
      url = url.Replace(" ", "-");
      return url;

    }

  }



  public static class CustomStringExtensions
  {
    public static String RemoveDiacritics(this String s)
    {
      String normalizedString = s.Normalize(NormalizationForm.FormD);
      StringBuilder stringBuilder = new StringBuilder();

      for (int i = 0; i < normalizedString.Length; i++)
      {
        Char c = normalizedString[i];
        if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
        {
          stringBuilder.Append(c);
        }
      }

      return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string TrimPunctuation(this string value)
    {
      // Count start punctuation.
      for (int i = 0; i < value.Length; i++)
      {
        if (char.IsPunctuation(value[i]))
        {
          string p1 = value.Substring(0, i);
          string p2 = value.Substring(i + 1);
          value = p1 + p2;
        }
      }
      return value;
    }

    public static string AddSpacesToSentence(this string text)
    {
      if (string.IsNullOrWhiteSpace(text))
        return "";
      StringBuilder newText = new StringBuilder(text.Length * 2);
      newText.Append(text[0]);
      for (int i = 1; i < text.Length; i++)
      {
        if (char.IsUpper(text[i]) && text[i - 1] != ' ')
          newText.Append(' ');
        newText.Append(text[i]);
      }
      return newText.ToString();
    }

    public static string ToTitleCase(this string s) =>
    CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLower());

    public static string RemoveExtraSpaces(this string s) =>
    String.Join(" ", s.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));

    public static string InsertBraces(this string value)
    {
      string[] arr1 = new string[] { "Alberta", "British Columbia", "Ontario", "Quebec" };

      var firstString = string.Empty;
      var secondString = string.Empty;
      int cnt = 0;
      bool insertBraces = false;

      string[] splitStr = value.Split(' ');
      if (splitStr.Count() == 1)
        return value;

      //if (splitStr.Count() == 3)
      //  splitStr[1] = splitStr[1] + " " + splitStr[2];

      //List<string> wordList = new List<string>();
      //wordList.Add(splitStr[0]);
      //wordList.Add(splitStr[1] + " " + splitStr[2]);

      foreach (string s in splitStr)
      {
        if (!arr1.Contains(s))
        {
          cnt++;
        }
      }

      if (cnt == splitStr.Count())
        return value;
      if (arr1.Contains(splitStr[0]))
        insertBraces = false;
      else
      {
        insertBraces = true;

      }
      if (insertBraces)
      {
        foreach (string s in arr1)
        {
          if (value.Contains(s))
          {
            var lengthStr = s.Length;
            firstString = value.Substring(0, value.Length - lengthStr);
            secondString = value.Substring(value.Trim().Length - lengthStr);
            break;
          }
        }

        if (!string.IsNullOrEmpty(splitStr[0]) && insertBraces)
          value = firstString + "(" + secondString + ")";
        else
          value = firstString + secondString;
      }
      return value;
    }

    public static string GetUrl(this MediaItem item, MediaUrlOptions options = null)
    {
      if (item == null)
        throw new ArgumentNullException("item");

      if (options == null)
        options = new MediaUrlOptions();

      var url = MediaManager.GetMediaUrl(item, options);

#if SITECORE8
    url = HashingUtils.ProtectAssetUrl(url);
#endif

      url = url.Replace(" ", "%20");

      return url;
    }
  }

}



