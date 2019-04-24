using Chartwell.Foundation.utility;
using log4net;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Pipelines.HttpRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
//using ChartwellApi;

namespace Chartwell.Foundation.Redirect
{
  /// <summary>
  /// Pipeline modification to turn Aliases into 301 redirects
  /// </summary>
  public class RedirectProcessor : HttpRequestProcessor
  {
    public ILog Logger = LogManager.GetLogger("ChartwellLog");
    ChartwellUtiles util = new ChartwellUtiles();

    public override void Process(HttpRequestArgs args)
    {
      bool AllowAccess = false;
      if (Context.Item != null)
      {
        string path = Context.Item.Paths.Path;
        string[] urlPath = path.Split('/');
        if (path.Contains("404"))
        {
          AllowAccess = false;
        }
        if (path.Contains("retirement-residences") || urlPath.Length == 6)
        {
          AllowAccess = true;
        }
        if (path.Contains("continuum-of-care"))
        {
          AllowAccess = true;
          if (Context.Item.Language.Name == "fr")
          {
            Logger.Info("French Continum of Care");
          }
        }
      }
      if (!((Context.Item == null) || AllowAccess))
      {
        return;
      }
      string requestedUrl = HttpContext.Current.Request.Url.ToString();
      string requestedPath = HttpContext.Current.Request.Url.AbsolutePath;
      string rawUrl = Context.RawUrl.Split('?')[0];
      if (rawUrl.Equals("/résultat-de-la-recherche/") || rawUrl.Equals("/résultat-de-la-recherche"))
      {
        rawUrl = "/fr/québec/résidence-pour-retraités-à-québec";
      }
      else
      {
        string EnFrUrl = (from x in rawUrl.Split('/')
                          where !string.IsNullOrEmpty(x)
                          select x).Where(delegate (string x)
                          {
                            if (!x.Equals("en"))
                            {
                              return x.Equals("fr");
                            }
                            return true;
                          }).FirstOrDefault();
        if (!AllowAccess && !string.IsNullOrEmpty(EnFrUrl) && EnFrUrl.Contains("en"))
        {
          rawUrl = rawUrl.Replace("en/", "");
        }
      }
      if (IsSpecialPageCheck(rawUrl))
      {
        rawUrl += "/";
      }
      string[] url = rawUrl.Split('/');
      string lang = "";
      string arrlang = "";
      Logger.Info("incomingurl: " + Context.RawUrl);
      if (url.Length >= 1)
      {
        lang = url[1];
      }
      else
      {
        arrlang = ((url.Length > 2) ? url[2] : "");
      }
      if (lang.ToLower() == "careers" || lang.ToLower() == "carrières" || arrlang == "carrières")
      {
        RedirectCareerSite(requestedUrl, requestedPath, args);
      }
      Database database = Context.Database;
      if (url.Length >= 1)
      {
        try
        {
          if ((database != null && !rawUrl.Contains("sitecore") && !rawUrl.Contains("shell") && !rawUrl.Contains("api") && !rawUrl.Contains("system") && !rawUrl.Contains("LatLngSearch")) || rawUrl.Contains("404"))
          {
            RedirectUrl(rawUrl, args);
          }
        }
        catch (Exception e)
        {
          Logger.Error("  Error Message" + e);
        }
      }
    }
    private bool IsSpecialPageCheck(string rawurl)
    {
      bool isValid = false;
      if (rawurl.Equals("/index") || rawurl.Equals("/careers") || rawurl.Equals("/fr/index") || rawurl.Equals("/fr/carrières") || rawurl.Equals("/unsubscribe") || rawurl.Equals("/family") || rawurl.Equals("/fr/family") || rawurl.Equals("/askedna") || rawurl.Equals("/therese") ||
          rawurl.Equals("/retirement-homes-in-canada") || rawurl.Equals("/fr/résidence-pour-retraités-à-canada") || rawurl.Equals("/ask-edna") || rawurl.Equals("/thérèse") || rawurl.Equals("/welcome-video-for-new-employees") ||
          rawurl.Equals("/our-mission-vision-and-values") || rawurl.Equals("/retirement-living-in-quebec") || rawurl.Equals("/retirement-living-in-calgary"))
      { isValid = true; }
      return isValid;

    }
    private void RedirectCareerSite(string requestedUrl, string requestedPath, HttpRequestArgs args)
    {

      try
      {
        Logger.Info("Career Redirection called for Url: " + requestedUrl);
        requestedUrl = requestedUrl.Replace("careers", "careers/");
        requestedUrl = requestedUrl.Replace("fr/", "");
        requestedUrl = requestedUrl.Replace("carrières", "fr/carrières/");
        var builder = new UriBuilder(requestedUrl);
        builder.Scheme = Uri.UriSchemeHttp;
        builder.Port = -1;
        builder.Host = "careersatchartwell.com";


        string newUrl = builder.Uri.ToString();
        args.HttpContext.Response.StatusCode = 301;
        args.HttpContext.Response.AddHeader("Location", newUrl);
        args.HttpContext.Response.AddHeader("cache-control", "no-cache");
        //  args.Context.Response.RedirectPermanent(newUrl);
        //args.Context.Response.RedirectLocation = newUrl;

        args.HttpContext.Response.Redirect(newUrl, true);
        HttpContext.Current.ApplicationInstance.CompleteRequest();
        args.HttpContext.Response.Flush();

        Logger.Info("Career New Url: " + newUrl);
        return;
      }
      catch (Exception e)
      {

        Logger.Error("Career Redirection Exception Url: " + requestedUrl + " Error Message" + e);
      }

    }

    private void RedirectUrl(string rawUrl, HttpRequestArgs args)
    {
      if ((!rawUrl.Contains("sitecore") && !rawUrl.Contains("shell") && !rawUrl.Contains("api") && !rawUrl.Contains("system") && !rawUrl.Contains("LatLngSearch")) || rawUrl.Contains("404"))
      {
        new List<Item>();
        Language language = Context.Language;
        List<SearchResultItem> matches;
        using (IProviderSearchContext context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
        {
          Expression<Func<SearchResultItem, bool>> predicate2 = PredicateBuilder.True<SearchResultItem>();
          predicate2 = predicate2.And((SearchResultItem p) => p.TemplateName == "URL-Mapping");
          predicate2 = predicate2.And((SearchResultItem p) => p["dnn url"].Equals(rawUrl, StringComparison.InvariantCultureIgnoreCase));
          predicate2 = predicate2.Or((SearchResultItem p) => p["dnn url"].Contains(rawUrl));
          matches = context.GetQueryable<SearchResultItem>().Where(predicate2).ToList();
        }
        SearchResultItem searchResultItem = (from x in matches
                                             where x.GetItem().Fields["dnn url"].Value.Replace("fr/", "").Replace("en/", "").ToLower()
                                                 .Equals(rawUrl.Replace("fr/", "").Replace("en/", "").ToLower())
                                             select x).FirstOrDefault();
        if (searchResultItem != null)
        {
          try
          {
            Logger.Info("redirect url: " + HttpUtility.UrlDecode(rawUrl.ToLower()));
            string newUrl = searchResultItem.GetItem().Fields["new url"].Value.ToLower();
            Logger.Info("Url DNN Redirect called" + newUrl);
            args.HttpContext.Response.StatusCode = int.Parse(searchResultItem.GetItem().Fields["Status"].Value.ToString());
            args.HttpContext.Response.AddHeader("Location", newUrl);
            args.HttpContext.Response.AddHeader("cache-control", "no-cache");
            args.HttpContext.Response.RedirectPermanent(newUrl, endResponse: false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            args.HttpContext.Response.Flush();
          }
          catch (Exception e)
          {
            Logger.Error("newUrl " + searchResultItem.GetItem().Fields["New URL"].Value.ToLower() + e);
          }
        }
      }
    }
  }
}
