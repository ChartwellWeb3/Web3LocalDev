using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.Collections;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Links;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.Language.Controllers
{
  public class ChartwellLanguageController : Controller
  {
    private readonly ChartwellUtiles util = new ChartwellUtiles();

    // GET: ChartwellLanguage
    public PartialViewResult Index()
    {
      SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
      LanguageCollection languages = LanguageManager.GetLanguages(Context.Database);
      foreach (Sitecore.Globalization.Language lang in languages)
      {
        if (lang.ToString() == "fr")
        {
          IEnumerable<SearchResultItem> enumerable = null;
          string text = GetitemUrl(Context.Item, lang);
          if (text.Contains(Translate.TextByLanguage("SearchResults", lang)) && !HttpUtility.UrlDecode(Request.Url.PathAndQuery.Replace("/en", "").Replace("/fr", "").Replace("/", "")).Equals(Translate.Text("SearchResults")))
          {
            string text2 = base.Request.QueryString.GetValues(0).FirstOrDefault();
            string text3 = base.Request.QueryString.Keys[0];
            if (text3.Contains("City") || text3.Contains("Nom-de-la-ville"))
            {
              enumerable = util.SwitchSelectedCity(lang.Name, text2);
            }
            if (enumerable != null && enumerable.Count() > 0)
            {
              string text4 = util.GetDictionaryItem("CitySearch", lang.Name) + "=" + (from x in enumerable
                                                                                      where x.Language == lang.Name
                                                                                      select x.GetItem().Fields["City Name"].Value.Replace(" ", "-")).FirstOrDefault();
              text = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + lang + "/" + Translate.TextByLanguage("SearchResults", lang) + "/?" + text4;
            }
            else if (text3.Contains("Region") || text3.Contains("Région"))
            {
              string text5 = base.Request.QueryString.GetValues(0).FirstOrDefault();
              if (!text5.Equals("All-Properties") && !text5.Equals("Toutes-les-résidences"))
              {
                enumerable = util.SwitchSelectedCity(lang.Name, text5);
                string text6 = util.GetDictionaryItem("Region", lang.Name) + "=" + (from x in enumerable
                                                                                    where x.Language == lang.Name
                                                                                    select x.GetItem().Fields["City Name"].Value.Replace(" ", "-")).FirstOrDefault();
                text = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + lang + "/" + Translate.TextByLanguage("SearchResults", lang) + "/?" + text6;
              }
              else
              {
                string text7 = util.GetDictionaryItem("Region", lang.Name) + "=" + util.GetDictionaryItem("AllProperties", lang.Name);
                text = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + lang + "/" + Translate.TextByLanguage("SearchResults", lang) + "/?" + text7.Replace(" ", "-");
              }
            }
            else
            {
              string empty = string.Empty;
              empty = ((text3.Contains("PropertyName") || text3.Contains("PostalCode")) ? (text3 + "=" + text2) : (util.GetDictionaryItem("CitySearch", lang.Name) + "=" + text2));
              text = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + lang + "/" + Translate.TextByLanguage("SearchResults", lang) + "/?" + empty;
            }
          }
          if (!text.Contains("sumach"))
          {
            sortedDictionary.Add(lang.CultureInfo.DisplayName, text);
          }
        }
        else
        {
          string text8 = GetitemUrl(Context.Item, lang);
          if (text8.Contains(Translate.TextByLanguage("SearchResults", lang)) && !HttpUtility.UrlDecode(base.Request.Url.PathAndQuery.Replace("/en", "").Replace("/fr", "").Replace("/", "")).Equals(Translate.Text("SearchResults")))
          {
            IEnumerable<SearchResultItem> enumerable2 = null;
            if (text8.Contains(Translate.TextByLanguage("SearchResults", lang)))
            {
              string text9 = base.Request.QueryString.GetValues(0).FirstOrDefault();
              if (text9.ToLower() == "ville de québec")
              {
                text9 = "Quebec City";
              }
              string text10 = base.Request.QueryString.Keys[0];
              if (text10.Contains("City") || text10.Contains("Nom-de-la-ville"))
              {
                enumerable2 = util.SwitchSelectedCity(lang.Name, text9);
              }
              if (enumerable2 != null && enumerable2.Count() > 0)
              {
                string text11 = util.GetDictionaryItem("CitySearch", lang.Name) + "=" + (from x in enumerable2
                                                                                         where x.Language == lang.Name
                                                                                         select x.GetItem().Fields["City Name"].Value.Replace(" ", "-")).FirstOrDefault();
                text8 = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + lang + "/" + Translate.TextByLanguage("SearchResults", lang) + "/?" + text11;
              }
              else if (text10.Contains("Region") || text10.Contains("Région"))
              {
                string text12 = base.Request.QueryString.GetValues(0).FirstOrDefault();
                if (!text12.Equals("All-Properties") && !text12.Equals("Toutes-les-résidences"))
                {
                  enumerable2 = util.SwitchSelectedCity(lang.Name, text12);
                  string text13 = util.GetDictionaryItem("Region", lang.Name) + "=" + (from x in enumerable2
                                                                                       where x.Language == lang.Name
                                                                                       select x.GetItem().Fields["City Name"].Value.Replace(" ", "-")).FirstOrDefault();
                  text8 = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + lang + "/" + Translate.TextByLanguage("SearchResults", lang) + "/?" + text13;
                }
                else
                {
                  string text14 = util.GetDictionaryItem("Region", lang.Name) + "=" + util.GetDictionaryItem("AllProperties", lang.Name);
                  text8 = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + lang + "/" + Translate.TextByLanguage("SearchResults", lang) + "/?" + text14.Replace(" ", "-");
                }
              }
              else
              {
                string empty2 = string.Empty;
                empty2 = ((text10.Contains("PropertyName") || text10.Contains("PostalCode")) ? (text10 + "=" + text9) : (util.GetDictionaryItem("CitySearch", lang.Name) + "=" + text9));
                text8 = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + lang + "/" + Translate.TextByLanguage("SearchResults", lang) + "/?" + empty2;
              }
            }
          }
          sortedDictionary.Add(lang.CultureInfo.DisplayName, text8);
        }
      }
      return PartialView("~/Views/ChartwellLanguage/Index.cshtml", sortedDictionary);
    }
    private static string GetitemUrl(Item item, Sitecore.Globalization.Language language)
    {
      string itemUrl = LinkManager.GetItemUrl(Context.Database.GetItem(item.ID, LanguageManager.GetLanguage(language.ToString())), new UrlOptions
      {
        UseDisplayName = true,
        LowercaseUrls = true,
        LanguageEmbedding = LanguageEmbedding.Always,
        LanguageLocation = LanguageLocation.FilePath,
        Language = language
      });
      return itemUrl.Replace(" ", "-");
    }
  }
}