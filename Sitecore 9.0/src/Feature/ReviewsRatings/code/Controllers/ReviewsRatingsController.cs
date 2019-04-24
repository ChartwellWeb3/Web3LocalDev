using Chartwell.Feature.ReviewsRatings.Models;
using Chartwell.Feature.ReviewsRatings.ViewModel;
using Chartwell.Foundation.utility;
using Chartwell.OP;
using Chartwell.OP.DA;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.ReviewsRatings.Controllers
{
  public class ReviewsRatingsController : Controller
  {
    string constring = ConfigurationManager.ConnectionStrings["ReviewsRatingsDB"].ToString();

    // GET: ReviewsRatings
    public PartialViewResult Index()
    {
      var database = Sitecore.Context.Database;

      var propertyPage = Sitecore.Context.Item.Parent.Paths;
      var PropertyItem = database.GetItem(propertyPage.FullPath);
      PropertyModel prop = GetPropertyDetails(PropertyItem, database);
      string strIsDelete = "False";

      //ReviewsRatingsModel model = new ReviewsRatingsModel();
      //PrepareLabelsForEmail(model);

      #region Get ratings from external DB
      List<ReviewsRatingsModel> ratingsList = new List<ReviewsRatingsModel>();

      SqlConnection conn = new SqlConnection(constring);
      SqlCommand cmd = new SqlCommand();
      SqlDataReader reader;

      cmd.CommandText = @"Select (SELECT count(*) FROM VW_SC_RRModule where propertyIdfromyardi = @strPropertyID and Isdelete = @strIsDelete group by propertyIdfromyardi) as TotalReviews, 
	                       (Select ((Sum(convert(int, ratings)))/(count(*))) from VW_SC_RRModule where propertyIdfromyardi = @strPropertyID and Isdelete = @strIsDelete) as OverAllRatings, 
	                        * from VW_SC_RRModule where propertyIdfromyardi = @strPropertyID and Isdelete = @strIsDelete order by CommentDate desc; Select * from RoleIdentification; 
                        Select Top  1 * from Captcha c, CaptchaImages ci where c.CaptchaImageID = ci.CaptchaImageID
						            order by newid()";

      //cmd.CommandText = @"Select (SELECT count(*) FROM VW_SC_RRModule where propertyId = @strPropertyID group by PropertyID) as TotalReviews, 
      //                  (Select ((Sum(convert(int, ratings)))/(count(*))) from VW_SC_RRModule where propertyId = @strPropertyID) as OverAllRatings, 
      //                   * from VW_SC_RRModule where propertyId = @strPropertyID order by CommentDate desc; Select * from RoleIdentification; 
      //                  Select Top  1 * from Captcha c, CaptchaImages ci where c.CaptchaImageID = ci.CaptchaImageID
      //            order by newid()";

      cmd.CommandType = CommandType.Text;
      cmd.Parameters.AddWithValue("strPropertyID", prop.PropertyID.ToString());
      cmd.Parameters.AddWithValue("strIsDelete", strIsDelete);

      cmd.Connection = conn;

      conn.Open();

      reader = cmd.ExecuteReader();

      ReviewsRatingViewModel ratingsVM = new ReviewsRatingViewModel
      {
        Property = prop,
        Reviews = GetReviews(reader),
        RoleList = GetRoles(reader),
        Captcha = GetCaptcha(reader),
        Review =  PrepareLabelsForEmail()
      };
      //ratingsVM.Review = PrepareLabelsForEmail();
      reader.Close();
      conn.Close();

      #region
      ratingsVM.CaptchaImagesList = new List<string>
      {
        ratingsVM.Captcha.CaptchaAnswerImageName.ToString().ToLower(),
        ratingsVM.Captcha.FakeImageName.ToString().ToLower(),
        ratingsVM.Captcha.FakeImageNameSecond.ToString().ToLower()
      };

      Random captchaImages = new Random();
      var numbers = Enumerable.Range(1, 3).OrderBy(i => captchaImages.Next()).ToList();

      List<string> captchaImagesRandomList = new List<string>();
      foreach (int number in numbers)
      {
        captchaImagesRandomList.Add(ratingsVM.CaptchaImagesList[number - 1]);
      }

      ratingsVM.CaptchaImagesList = captchaImagesRandomList;
      #endregion

      #endregion  Get ratings from external DB
      TempData["RatingsVM"] = ratingsVM;
      return PartialView(ratingsVM);
    }

    private CaptchaModel GetCaptcha(SqlDataReader reader)
    {
      var lang = Sitecore.Context.Language;

      CaptchaModel captchaModel = new CaptchaModel();
      reader.NextResult();
      while (reader.Read())
      {
        CaptchaModel captcha = new CaptchaModel
        {
          CaptchaQuestion = lang.Name == "en" ? reader["CaptchaQuestion"].ToString() : reader["CaptchaQuestionfr"].ToString(),
          CaptchaAnswerImageName = reader["CaptchaAnswerImageName"].ToString(),
          FakeImageName = reader["FakeImageUrl"].ToString().Split('/').Last(),
          FakeImageNameSecond = reader["FakeImageUrlSecond"].ToString().Split('/').Last()
        };
        captchaModel = captcha;
      }


      var fakeImageName = System.IO.Path.GetFileNameWithoutExtension(captchaModel.FakeImageName.ToString());
      captchaModel.FakeImageName = fakeImageName;

      var fakeImageNameSecond = System.IO.Path.GetFileNameWithoutExtension(captchaModel.FakeImageNameSecond.ToString());
      captchaModel.FakeImageNameSecond = fakeImageNameSecond;

      return captchaModel;

    }

    private List<RoleModel> GetRoles(SqlDataReader reader)
    {
      List<RoleModel> roles = new List<RoleModel>();
      reader.NextResult();
      while (reader.Read())
      {
        RoleModel role = new RoleModel
        {
          RoleID = Convert.ToInt32(reader["RoleIdentificationID"]),
          RoleIDName = reader["RoleIdentificationName"].ToString()
        };
        roles.Add(role);
      }
      return roles;
    }

    [HttpPost]
    [AllowAnonymous]
    public ActionResult Index(ReviewsRatingsModel UReview)
    {
      var RRUrl = Request.UrlReferrer;


      var captchaAns = Request.Form["Captcha.CaptchaAnswerImageName"].ToLower();
      var selectedCaptcha = Request.Form["CaptchaImagesList"].ToLower();
      ReviewsRatingViewModel vm = TempData["RatingsVM"] as ReviewsRatingViewModel;
      var model = TempData["RatingsVM"] as ReviewsRatingViewModel;
      if (ModelState.IsValid)
      {
        if (captchaAns == selectedCaptcha)
        {
          RRMasterDT userReview = new RRMasterDT
          {
            propertyid = Convert.ToInt32(Request.Form["Property.PropertyID"]), //new HtmlString(model.Property.PropertyName.ToString()).ToString(),
            propertyname = Request.Form["Property.PropertyName"].ToString(),
            ratings = Convert.ToInt32(Request.Form["Ratings"]),
            firstname = Request.Form["FirstName"].ToString(),
            lastname = Request.Form["LastName"].ToString(),
            Email = Request.Form["Email"].ToString(),
            roleidentification = Convert.ToInt32(Request.Form["RoleIdentification"]),
            comments = Request.Form["Comments"].ToString(),
            IsEn = Sitecore.Context.Language.Name == "en",
            IsFr = Sitecore.Context.Language.Name == "fr"
          };

          RRMasterDA reviewDA = new RRMasterDA(constring);
          reviewDA.InsertSC(userReview);
          if(userReview.ratings <= 3)
          {
            SendMail(userReview, model);
          }
        }
        else
        {
          ModelState.AddModelError("CaptchaImagesList", "Wrong Captcha");
          return View(model);
        }
      }

      return Redirect(RRUrl.ToString());
    }

    public List<ReviewsRatingsModel> GetReviews(SqlDataReader reader)
    {
      List<ReviewsRatingsModel> rr = new List<ReviewsRatingsModel>();
      while (reader.Read())
      {
        ReviewsRatingsModel ratings = new ReviewsRatingsModel
        {
          CommentDate = Convert.ToDateTime(reader["CommentDate"]).ToString("dd/MM/yyyy"),
          FirstName = reader["FirstName"].ToString(),
          LastName = reader["LastName"].ToString(),
          Ratings = reader["ratings"].ToString(),
          Comments = reader["CommentsEnFr"].ToString(),
          Email = reader["Email"].ToString(),
          TotalReviewsCnt = reader["TotalReviews"].ToString(),
          OverallRatings = reader["OverallRatings"].ToString()
        };
        rr.Add(ratings);
      }
      return rr;
    }

    public PropertyModel GetPropertyDetails(Item propertyItem, Database db)
    {
      ChartwellUtiles util = new ChartwellUtiles();
      string strProvinceID = propertyItem.Fields["Province"].ToString();
      ID ProvinceID = new ID(strProvinceID);
      var Provinceitem = db.GetItem(ProvinceID);
      string strProvinceName = Provinceitem.Name.ToString();
      string strPropertyFormattedAddress = util.FormattedAddress(propertyItem, strProvinceName);
      PropertyModel property = new PropertyModel
      {
        PropertyID = propertyItem.Fields["Property ID"].ToString(),
        PropertyName = propertyItem.Fields["Property Name"].ToString(),
        PropertyAddress = propertyItem.Fields["Street name and number"].ToString(),
        CityName = propertyItem.Fields["Selected City"].ToString(),
        ProvinceName = strProvinceName,
        PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),
        PostalCode = propertyItem.Fields["Postal Code"].ToString()
      };
      return property;
    }

    private void SendMail(RRMasterDT review, ReviewsRatingViewModel model)
    {
      StringBuilder emailBody = new StringBuilder();

      var fromAddress = review.IsEn ? ConfigurationManager.AppSettings["ContactUsEmailFrom"].ToString() : ConfigurationManager.AppSettings["ContactUsEmailFromFr"].ToString();
      var toAddress = string.Empty;
      if (review.firstname.Trim().ToLower() == "test")
      {
        toAddress = "patelshirin@gmail.com";
      }
      else
      {
        toAddress = "communications@chartwell.com";
      }
      string fromPassword = ConfigurationManager.AppSettings["ContactUsEmailPass"].ToString();

      string subject = "Review && Ratings for " + review.propertyname;

      PrepareEmail(review, model, emailBody);

      var smtp = new SmtpClient();
      {
        smtp.Host = ConfigurationManager.AppSettings["SMTPHOSTNAME"].ToString();
        smtp.Port = 587;
        smtp.EnableSsl = true;
        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtp.Credentials = new System.Net.NetworkCredential(fromAddress, fromPassword);

      }
      smtp.Send(fromAddress, toAddress, subject, emailBody.ToString());
    }

    private static void PrepareEmail(RRMasterDT review, ReviewsRatingViewModel model, StringBuilder body1)
    {
      body1.Append(model.Review.DisplayEmailFirstName + ": " + review.firstname + Environment.NewLine);
      body1.Append(model.Review.DisplayEmailLastName + ": " + review.lastname + Environment.NewLine);
      body1.Append(model.Review.DisplayEmailId + ": " + review.Email + Environment.NewLine);
      body1.Append(model.Review.DisplayEmailCommentDate + ": "  + DateTime.Now.ToShortDateString() + Environment.NewLine);
      body1.Append(model.Review.DisplayEmailComment + ": " + review.comments + Environment.NewLine);
      body1.Append(model.Review.DisplayEmailRatings + ": " + review.ratings + Environment.NewLine);
    }

    private ReviewsRatingsModel PrepareLabelsForEmail()
    {
      ReviewsRatingsModel model = new ReviewsRatingsModel
      {
        DisplayEmailFirstName = Translate.Text("FirstName"),
        DisplayEmailLastName = Translate.Text("LastName"),
        DisplayEmailId = Translate.Text("Email"),
        DisplayEmailCommentDate = Translate.Text("CommentDate"),
        DisplayEmailComment = Translate.Text("Comments"),
        DisplayEmailRatings = Translate.Text("Ratings")
      };

      return model;
    }

  }
}