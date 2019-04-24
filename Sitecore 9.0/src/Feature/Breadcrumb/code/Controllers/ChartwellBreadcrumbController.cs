using Chartwell.Foundation.utility;
using Chartwell.Project.Website.Models;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using Sitecore.Sites;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Chartwell.Feature.Breadcrumb.Controllers
{
  public class ChartwellBreadcrumbController : Controller
  {
    ChartwellUtiles util = new ChartwellUtiles();
    // GET: Breadcrumb
    public PartialViewResult Index()
    {


      return PartialView("~/Views/Breadcrumb/Breadcrumb.cshtml", CreateModel(Context.Item, Context.Site));
    }

    public BreadcrumbItems CreateModel(Item current, SiteContext site)
    {





      List<Item> breadcrumbs = new List<Item>();
      var PropertyItemPath = PageContext.Current.Item.Paths.ParentPath;

      Item homeItem = Sitecore.Context.Database.GetItem(PropertyItemPath);
      // string strPropertyURL = "/sitecore/content/Chartwell/property Page/" + selPropertyName + "/overview";
      string strPropertyURL = PropertyItemPath.Replace("/sitecore/content/Chartwell", "").Replace(" ", "-") + "/" + util.GetDictionaryItem("overview", Context.Language.Name);
      while (current.DisplayName.ToLower() != "chartwell")
      {

        if (current == homeItem || current.DisplayName.Contains("Property Page"))
          break;

        breadcrumbs.Add(current);



        current = current.Parent;
      }

      breadcrumbs.Reverse();


      var viewModel = new BreadcrumbItems
      {
        HostSite = Request.Url.Host,
        PropertyURL = strPropertyURL,
        BreadcrumbItem = breadcrumbs


      };
      return viewModel;
    }


  }
}