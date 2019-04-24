using Chartwell.Feature.RecentlyViewedPages.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.Configuration;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

public class RecentlyViewedPagesController : Controller
{
  private ChartwellUtiles util = new ChartwellUtiles();

  public ActionResult Index()
  {
    List<RecentlyViewedPagesModel> list = new List<RecentlyViewedPagesModel>();
    list = GetRecentInteractions();
    return PartialView(list);
  }

  public List<RecentlyViewedPagesModel> GetRecentInteractions()
  {
    string identifier = Tracker.Current.Contact.ContactId.ToString("N");
    List<RecentlyViewedPagesModel> result = new List<RecentlyViewedPagesModel>();
    using (XConnectClient context = SitecoreXConnectClientConfiguration.GetClient())
    {
      IdentifiedContactReference reference = new IdentifiedContactReference("xDB.Tracker", identifier);
      Contact contact = context.Get(reference, null);
      if (contact != null)
      {
        Guid id = Guid.Parse(contact.Id.ToString());
        ContactExpandOptions expandOptions = new ContactExpandOptions
        {
          Interactions = new RelatedInteractionsExpandOptions
          {
            Limit = int.MaxValue
          }
        };
        Contact contact2 = context.Get(new ContactReference(id), expandOptions);
        List<PageViewEvent> list = new List<PageViewEvent>();
        foreach (Interaction interaction in contact2.Interactions)
        {
          list.AddRange(from x in interaction.Events.OfType<PageViewEvent>()
                        where x.Url.Contains(util.GetDictionaryItem("overview", Context.Language.Name)) || (x.Url.Contains("sumach") && Context.Language.Name == "en" && !x.Url.Contains("404") && !x.Url.Contains(util.GetDictionaryItem("SearchResults", Context.Language.Name)))
                        select x);
        }
        list.Reverse();
        result = (from interaction in list
                  select new RecentlyViewedPagesModel
                  {
                    RecentlyViewedPageUrl = interaction.Url,
                    InteractionItemID = interaction.ItemId,
                    InteractionPropertyName = ((!interaction.Url.Contains("sumach")) ? util.PropertyDetails(new ID(interaction.ItemId.ToString())).FirstOrDefault().GetItem()
                          .Parent.DisplayName.Replace("-", " ").ToTitleCase() : GetItemForSumach().DisplayName.Replace("-", " ").ToTitleCase()),
                    InteractionImage = ((!interaction.Url.Contains("sumach")) ? GetImageUrl(util.PropertyDetails(new ID(interaction.ItemId.ToString())).FirstOrDefault().GetItem()
                              .Parent) : GetImageUrl(GetItemForSumach()))
                  }).GroupBy((RecentlyViewedPagesModel x) => x.InteractionItemID, (Guid key, IEnumerable<RecentlyViewedPagesModel> group) => group.First()).Take(4).ToList();
      }
    }
    return result;
  }

  private string GetImageUrl(Item SearchPropertyItem)
  {
    ImageField imageField = SearchPropertyItem.Fields["Thumbnail Photo"];
    string text = (imageField != null) ? MediaManager.GetMediaUrl(imageField.MediaItem) : string.Empty;
    return text.Replace("/sitecore/shell", "");
  }

  private Item GetItemForSumach()
  {
    List<SearchResultItem> list = null;
    using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
    {
      list = (from x in providerSearchContext.GetQueryable<SearchResultItem>()
              where x.Path.StartsWith("/sitecore/content/Chartwell/retirement-residences")
              where x.Name.Equals("the sumach by chartwell")
              where x.Language == "en"
              select x).ToList();
    }
    return list[0].GetItem();
  }
}