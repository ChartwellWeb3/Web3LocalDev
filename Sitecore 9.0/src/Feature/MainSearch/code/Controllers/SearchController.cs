using Chartwell.Feature.MainSearch.Models.PropertyModel;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Chartwell.Feature.MainSearch.Controllers
{
  public class SearchController : Controller
  {
    private ChartwellUtiles util = new ChartwellUtiles();

    // GET: Search
    public ActionResult Index(PropertySearchModel property)
    {
      property.Language = Context.Language.Name;

      return View(property);
    }

    [HttpPost]
    public JsonResult SearchProperty(string Prefix)
    {
      var searchLanguage = Context.Language;
      List<SearchResultItem> results = null;

      var db = Context.Database.Name;

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        results = context.GetQueryable<SearchResultItem>()
         .Where(x => x.Path.StartsWith("/sitecore/content/Chartwell/PropertySearch"))
         .Where(x => x.TemplateName.Equals("PropertySearch"))
         //.Where(x => x.Name.Contains(Prefix.Replace("'", "").Replace("-", " ").RemoveDiacritics().ToLower()))
         .Where(x => x.Language == searchLanguage.Name)
         .ToList();
      }


      IEnumerable<PropertySearchModel> propertyNameList = null;

      propertyNameList = (from c in results
                          select new PropertySearchModel
                          {
                            PropertyName = c.GetItem().Fields["Property Name"].Value
                          })
                      .Where(x => x.PropertyName.Replace("'", "").Replace("-", " ")
                                                .RemoveDiacritics().ToLower()
                                                .Contains(Prefix.Replace("'", "").Replace("-", " ")
                                                .RemoveDiacritics().ToLower()))
                      .OrderBy(o => o.PropertyName).Take(25)
                      .ToList();

      return Json(propertyNameList, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public JsonResult CitySearch(string term)
    {

      string contextLanguage;
      contextLanguage = Request.Form["lang"];

      //var searchBucketFolder = term.Length == 1 ? "/" + term.Substring(0, 1)  : "/" + term.Substring(0, 2).Insert(1, "/");

      List<SearchResultItem> matches;

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {

        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.Path.StartsWith("/sitecore/content/Chartwell/Content Shared Folder/City")); //+ searchBucketFolder));
        //predicate = predicate.And(p => p.Name != "City");
        //predicate = predicate.And(p => p["City Name"].Contains(term.ToTitleCase()));
        predicate = predicate.And(p => p.Name.StartsWith(term.ToTitleCase().Replace(" ", "")));
        predicate = predicate.Or(p => p["City Name"].Contains(term.ToTitleCase()));

        predicate = predicate.And(p => p.Language == contextLanguage);
        matches = context.GetQueryable<SearchResultItem>().Where(predicate).Take(50).ToList();
      }

      IEnumerable<PropertySearchModel> CityNameList = null;

      CityNameList = (from p in matches
                      group p by p.GetItem().Fields["City Name"].Value into g
                      select new PropertySearchModel
                      {
                        City = g.Key.ToString()
                      })
                      //.Where(x=>x.City.ToLower().Contains(term.ToLower()))
                      .OrderBy(o => o.City).ToList();

      return Json(CityNameList, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public ActionResult Index()
    {

      var lang = Request.Form["Language"];

      if (Request.Form.Count > 0)
      {
        var qsList = Request.Form.AllKeys
              .Select(key => new { Name = key.ToString(), Value = Request.Form[key.ToString()] })
              .Where(k => !string.IsNullOrWhiteSpace(k.Value) && !string.IsNullOrWhiteSpace(k.Name) &&
                  (k.Name == "Language" || k.Name == "City" || k.Name == "PropertyName" || k.Name == "PostalCode"))
              .ToList();

        var newUrl = string.Empty;
        if (qsList.Count > 1)
        {
          var queryStrKey = string.Empty;

          if (qsList[1].Name == "City" || qsList[1].Name == "Nom de la ville")
          {
            queryStrKey = util.GetDictionaryItem("CitySearch", qsList[0].Value);
          }
          else if (qsList[1].Name == "PropertyName")
          {
            queryStrKey = "PropertyName";
          }
          else if (qsList[1].Name == "PostalCode")
          {
            queryStrKey = "PostalCode";
          }
          newUrl = Request.Url.Scheme + "://" + Request.Url.Host + "/" + util.GetDictionaryItem("SearchResults", qsList[0].Value) + "/?" + queryStrKey + "=" + qsList[1].Value.Replace(" ", "-");
        }
        else
        {
          newUrl = Request.Url.Scheme + "://" + Request.Url.Host;
        }
        return Redirect(newUrl);

      }

      else
      {
        string value1 = Request["PropertyName"];
        var newUrl = Request.Url.Scheme + "://" + Request.Url.Host + "?" + value1.Replace(" ", "-");
        return Redirect(newUrl);
      }
    }
  }
}
