using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.TopHeader.Controllers
{
    public class RedirectController : Controller
    {
        // GET: Redirect
        public ActionResult Request404Page()
        {
            System.Web.HttpContext.Current.Response.StatusCode = 404;
            System.Web.HttpContext.Current.Response.TrySkipIisCustomErrors = true;
            System.Web.HttpContext.Current.Response.StatusDescription = "404 File Not Found";

            return new EmptyResult();
        }
    }
}