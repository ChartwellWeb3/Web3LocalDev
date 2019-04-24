using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.StaticPages.Controllers
{
    public class RedirectController : Controller
    {
        // GET: Redirect
        public ActionResult Index()
        {

            var database = Sitecore.Context.Database;
            string itemUrl = string.Empty;
            var CurrentItem = Sitecore.Context.Item;
            itemUrl = CurrentItem.Fields["RedirectURL"].ToString();
            string reqPath= CurrentItem.Fields["RequestedURL"].ToString();
            if (reqPath != CurrentItem.Paths.Path)
                return Redirect(itemUrl.ToLower());
            else
                return null;
        }
    }
}