using Sitecore.Data.Items;
using System;

namespace Chartwell.Feature.RecentlyViewedPages.Models
{
  public class RecentlyViewedPagesModel
  {
    public string RecentlyViewedPageUrl { get; set; }

    public Guid InteractionItemID { get; set; }

    public string InteractionImage { get; set; }

    public string InteractionPropertyName { get; set; }

  }
}