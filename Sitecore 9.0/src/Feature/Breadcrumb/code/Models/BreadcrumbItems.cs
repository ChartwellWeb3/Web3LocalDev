using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;

namespace Chartwell.Project.Website.Models
{
    public class BreadcrumbItems
    {
        public string HostSite { get; set; }
        public string PropertyURL { get; set; }
        public List<Item> BreadcrumbItem { get; set; }
    }
}