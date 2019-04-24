using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chartwell.Feature.Photo.Model
{
    public class PhotoGalleryModel
    {
        public List<Item> SelectedPhotoItem { get; set; }
        public List<string> ImageUrls { get; set; }
        public HtmlString PropertyFormattedAddress { get; set; }
        public Item InnerItem { get; set; }
    }
}