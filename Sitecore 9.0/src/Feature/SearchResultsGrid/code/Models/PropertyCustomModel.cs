using Chartwell.Feature.MainSearch.Models.PropertyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chartwell.Feature.SearchResultsGrid.Models
{
    public class PropertyCustomModel
    {
        public string SplitterPageTitle { get; set; }
        public string SplitterPageDescription { get; set; }
        public List<PropertySearchModel> lstProperty { get; set; }

        public HashSet<string> lstPropertyProvince { get; set; }
    }
}