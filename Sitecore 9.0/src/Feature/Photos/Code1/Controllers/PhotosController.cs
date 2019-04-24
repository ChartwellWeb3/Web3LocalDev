using Chartwell.Feature.Photo.Model;
using Chartwell.Foundation.utility;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using Sitecore.Resources.Media;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.Photo.Controllers
{
    public class PhotosController : Controller
    {
        // GET: Photos
        public PartialViewResult Index()
        {
            return PartialView("~/Views/Photos/Index.cshtml", CreateModel());
        }

        private PhotoGalleryModel CreateModel()
        {


            var database = Sitecore.Context.Database;

            ChartwellUtiles util = new ChartwellUtiles();

            var PhotoItemPath= PageContext.Current.Item.Paths.Path;
            var PropertyItemPath= PageContext.Current.Item.Paths.ParentPath;
            Sitecore.Data.Fields.MultilistField PhotoList;
            List <Item> lstPropertyPhotos = new List<Item>();
            //Sitecore.Data.Fields.MultilistField PhotoList = PhotoItem.Fields["PropertyImage"];
            List<string> imageUrls = new List<string>();
            var PhotoItem = database.GetItem(PropertyItemPath);
            string strProvinceID = PhotoItem.Fields["Province"].ToString();
            ID ProvinceID = new ID(strProvinceID);
            var Provinceitem = database.GetItem(ProvinceID);
            string strProvinceName = Provinceitem.Fields["Province Name"].ToString();

            string strPropertyFormattedAddress = util.FormattedAddress(PhotoItem, strProvinceName);
            PhotoList = PhotoItem.Fields["Photos"];
            if(PhotoList.Count == 0)
            {
               var  DefaultPhotoItem = database.GetItem(PhotoItemPath);
                PhotoList = DefaultPhotoItem.Fields["PropertyImage"];

            }
            if (PhotoList != null && PhotoList.TargetIDs != null)
            {

                foreach (var item in PhotoList.GetItems())

                {
                    string hashedUrl = HashingUtils.ProtectAssetUrl(Sitecore.StringUtil.EnsurePrefix('/', Sitecore.Resources.Media.MediaManager.GetMediaUrl(item)));

                    imageUrls.Add(hashedUrl);



                }
            }
            var viewModel = new PhotoGalleryModel
            {
                ImageUrls = imageUrls,
                PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),
                InnerItem = PhotoItem,
                SelectedPhotoItem = lstPropertyPhotos
            };
            return viewModel;
        }

        public PartialViewResult ChartwellCarousel()
        {
            //var item = PageContext.Current.Item;
            //var database = Sitecore.Context.Database;
            //var PropertyItemPath = PageContext.Current.Item.Paths.ParentPath;
            //var PropertyItem = database.GetItem(PropertyItemPath);
            //var slideIds = Sitecore.Data.ID.ParseArray(PropertyItem.Fields["PropertyCarousel"].ToString());
            //var viewModel = new ChartwellCarouselViewModel
            //{
            //    CarouselSlides =
            //        slideIds.Select(i =>
            //            new ChartwellCarouselSlideViewModel
            //            {
            //                Item = item.Database.GetItem(i)
            //            }).ToList()
            //};
            //return View("~/Views/ChartwellCarousel/Index.cshtml", viewModel);

            return PartialView("~/Views/ChartwellCarousel/Index.cshtml", CarouselModel());
        }
             private PhotoGalleryModel CarouselModel()
        {


            var database = Sitecore.Context.Database;

           
         
            var PhotoItemPath = PageContext.Current.Item.Paths.Path;
            var PropertyItemPath = PageContext.Current.Item.Paths.ParentPath;
            Sitecore.Data.Fields.MultilistField PhotoList;
            List<Item> lstPropertyPhotos = new List<Item>();
            //Sitecore.Data.Fields.MultilistField PhotoList = PhotoItem.Fields["PropertyImage"];
            List<string> imageUrls = new List<string>();
            var PhotoItem = database.GetItem(PropertyItemPath);
           
            PhotoList = PhotoItem.Fields["PropertyCarousel"];
           
            if (PhotoList != null && PhotoList.TargetIDs != null)
            {

                foreach (var item in PhotoList.GetItems())

                {
                    string hashedUrl = HashingUtils.ProtectAssetUrl(Sitecore.StringUtil.EnsurePrefix('/', Sitecore.Resources.Media.MediaManager.GetMediaUrl(item)));

                    imageUrls.Add(hashedUrl);



                }
            }
            var viewModel = new PhotoGalleryModel
            {
                ImageUrls = imageUrls,

                SelectedPhotoItem = lstPropertyPhotos
            };
            return viewModel;
        }
    }
    }
