using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.StaticPages.Controllers
{
  public class StaticPagesController : Controller
  {
    // GET: StaticPages
    public ActionResult Index(string PartialItemUrlText, string StaticLanguage)
    {
      string itemUrl = string.Empty;
      //if (!string.IsNullOrEmpty(PartialItemUrlText) && PartialItemUrlText == Sitecore.Globalization.Translate.Text("Home"))
      //  itemUrl = "http://" + Request.Url.Host + "/" + Sitecore.Globalization.Translate.Text("Home"); // "Home-page";
      //else
      //{
      PartialItemUrlText = PartialItemUrlText.Replace(" ", "-");
      itemUrl = Request.Url.Scheme + "://" + Request.Url.Host + "/" + StaticLanguage + "/" + PartialItemUrlText;
      //}
      return Redirect(itemUrl.ToLower());
    }
  }
}