
using Chartwell.Feature.PageSEO.Models;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Chartwell.Feature.PageSEO.Controllers
{
  public class PageSEOController : Controller
  {
    // GET: PageSEO

    public ActionResult Index(PageMetaDataModel Model)
    {
      string strQueryStringValidation = "";
      NameValueCollection page = Request.QueryString;
      if (page.HasKeys())
        strQueryStringValidation = page.GetKey(0);
      string strPageDescription = string.Empty;
      string strPageKeyword = string.Empty;
      string strPropertyType = string.Empty;
      string strPageTitle3;
      if (page.Count == 0 && (base.Request.Url.PathAndQuery == "/" || base.Request.Url.PathAndQuery == "/fr" || base.Request.Url.PathAndQuery == "/en"))
      {
        strPageTitle3 = "Chartwell Retirement Residences-Canada's Largest Senior Housing Choice";
        strPageDescription = "Chartwell offers a range of senior living options across Canada including independent and assisted living retirement homes, memory care, long term care and extended care residences. We offer our residents a safe and rewarding lifestyle in a seniors housing community that they are proud to call home.";
        if (Context.Language.Name != "en")
        {
          strPageTitle3 = "Résidences pour retraités Chartwell - Le plus grand choix de logements pour personnes âgées au Canada";
          strPageDescription = "Chartwell offre une vaste gamme d’hébergements pour retraités à l'échelle du Canada, des résidences pour personnes autonomes et semi-autonomes sans oublier des centres de soins de longue durée. Nous offrons à nos résidents un milieu de vie sécuritaire et valorisant qu'ils sont heureux d'appeler leur chez-soi.";
        }
      }
      else if (strQueryStringValidation == "City" || strQueryStringValidation == "PropertyName" || strQueryStringValidation == "PostalCode" || strQueryStringValidation == "Nom-de-la-ville")
      {
        if (Sitecore.Context.Language.Name == "en")
        {
          strPageTitle3 = "Chartwell Retirement Residences - SearchResults";
        }
        else
        {
          strPageTitle3 = "Chartwell résidences pour retraités - Résultat de la recherche";
        }

      }
      else
      {
        Item propertyPage = Context.Item;
        Template itemTemplate = TemplateManager.GetTemplate(propertyPage);
        if (itemTemplate.FullName.Contains("LeftNav"))
        {
          Item parentPropertyPage = Context.Item.Parent;
          string strCity2 = parentPropertyPage.Fields["Selected City"].ToString();
          string strPropertyTypeID = parentPropertyPage.Fields["Province"].ToString();
          if (!string.IsNullOrEmpty(strPropertyTypeID))
          {
            ID PropertyTypeID = new ID(strPropertyTypeID);
            Item PropertyTypeItem = Context.Database.GetItem(PropertyTypeID);
            strPropertyType = PropertyTypeItem.Fields["Province Name"].ToString();
          }
          strPageTitle3 = propertyPage.Fields["PageTitle"].ToString() + " " + parentPropertyPage.Fields["Property Name"].ToString();
          if (propertyPage.Name == "overview")
          {
            strPageDescription = parentPropertyPage.Fields["Property Description"].ToString();
            string strPageProperty = parentPropertyPage.Fields["Property Name"].ToString();
            strPageTitle3 = strPageProperty + " - " + strCity2 + ", " + strPropertyType;
          }
          else if (propertyPage.Name == "careservice")
          {
            string leftNavItem2 = "care section description";
            strPageDescription = parentPropertyPage.Fields[leftNavItem2].ToString();
          }
          else
          {
            string leftNavItem = propertyPage.Name + " section description";
            strPageDescription = ((parentPropertyPage.Fields[leftNavItem] == null || !parentPropertyPage.Fields[leftNavItem].HasValue) ? parentPropertyPage.Fields["Property Description"].ToString() : parentPropertyPage.Fields[leftNavItem].ToString());
          }
          if (strPageDescription == string.Empty)
          {
            strPageDescription = parentPropertyPage.Fields["Property Description"].ToString();
          }
          strPageKeyword = propertyPage.Fields["PageKeyword"].ToString();
          Model.SEOCity = strCity2;
          Model.SEOProvince = strPropertyType;
        }
        else
        {
          strPageTitle3 = propertyPage.Fields["PageTitle"].ToString();
          strPageDescription = propertyPage.Fields["PageDescription"].ToString();
          strPageKeyword = propertyPage.Fields["PageKeyword"].ToString();
        }
      }
      Model.PageTitle = strPageTitle3;
      Model.PageDescription = strPageDescription;
      string pageDesc = Regex.Replace(Model.PageDescription, "<(.|\n)*?>", string.Empty);
      IEnumerable<string> strFirstTwoSentences = pageDesc.Split('.').Take(2);
      Model.PageDescription = string.Empty;
      foreach (string s in strFirstTwoSentences)
      {
        Model.PageDescription += s;
      }
      Model.PageKeyword = strPageKeyword;
      return View(Model);
    }
  }
}