using Chartwell.Feature.CorporateContact.Models;
using Chartwell.Foundation.utility;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

namespace Chartwell.Feature.CorporateContact.Controllers
{
  public class CorporateContactController : Controller
  {
    string constring = ConfigurationManager.ConnectionStrings["SitecoreOLP"].ToString();
    // GET: CorporateContact
    public ActionResult Index()
    {
      var t = Request.Url.Host;
      ChartwellUtiles util = new ChartwellUtiles();

      var ParentPropertyItemID = Sitecore.Context.Item.ID;
      var ParentPropertyItem = util.PropertyDetails(ParentPropertyItemID).Where(x => x.Language == Sitecore.Context.Language.Name).ToList();
      var PropertyItem = ParentPropertyItem[0].GetItem();

      //var lang = PropertyItem.Language.Name; // Sitecore.Context.Language;

      SqlConnection conn = new SqlConnection(constring);
      SqlCommand cmd = new SqlCommand();
      SqlDataReader reader;

      cmd.CommandText = @"Select * from CorporateEnquirySubject where CorporateEnquirySubjectID != 1";

      cmd.CommandType = CommandType.Text;

      cmd.Connection = conn;

      conn.Open();

      reader = cmd.ExecuteReader();
      CorporateContactModel corporateModel = new CorporateContactModel();
      corporateModel.ItemLanguage = PropertyItem.Language.Name;

      PrepareLabelsForEmail(corporateModel);

      corporateModel.SubjectList = GetCorpSubjectDetails(reader, corporateModel, corporateModel.ItemLanguage).ToList();

      corporateModel.PropertyList = GetCorpPropertyDetails(corporateModel, corporateModel.ItemLanguage).OrderBy(o => o.ResidenceOfInterest).ToList();
      return PartialView(corporateModel);
    }

    private static List<CorporateContactModel> GetCorpPropertyDetails(CorporateContactModel corporateModel, string lang)
    {
      var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext();
      List<CorporateContactModel> propertyList = new List<CorporateContactModel>();

      var results = context.GetQueryable<SearchResultItem>()
           .Where(x => x.TemplateName == "Property Page")
           .Where(x => x.Name != "__Standard Values")
           .Where(x => x.Language == corporateModel.ItemLanguage)
           .OrderBy(o => o.Name).ToList();

      foreach (var property in results)
      {
        //var database = Sitecore.Context.Database;
        //var PropertyItem = database.GetItem(property.ItemId);
        //ChartwellUtiles util = new ChartwellUtiles();

        //var ParentPropertyItemID = Sitecore.Context.Item.ID;
        //var PropertyItem = util.PropertyDetails(property.ItemId)[0].GetItem();

        CorporateContactModel contactProp = new CorporateContactModel();

        contactProp.PropertyID = !string.IsNullOrEmpty(property.GetField("property id").Value) ?
        property.GetField("property id").Value : string.Empty;
        contactProp.ResidenceOfInterest = !string.IsNullOrEmpty(property.GetField("property name").Value) ?
        property.GetField("property name").Value : string.Empty;

        propertyList.Add(contactProp);
      }
      return propertyList.ToList();
    }

    private static List<CorporateEnquirySubject> GetCorpSubjectDetails(SqlDataReader reader, CorporateContactModel corporateModel, string lang)
    {
      List<CorporateEnquirySubject> subjectList = new List<CorporateEnquirySubject>();

      while (reader.Read())
      {
        CorporateEnquirySubject subject = new CorporateEnquirySubject
        {
          CorporateEnquirySubjectID = Convert.ToInt32(reader["CorporateEnquirySubjectID"]),
          CorporateEnquirySubjectName = corporateModel.ItemLanguage == "en" ? reader["CorporateEnquirySubjectName"].ToString() : reader["CorporateEnquirySubjectNameFr"].ToString(),
          CorporateEnquirySubjectEmailDistribution = reader["CorporateEnquirySubjectEmailDistribution"].ToString(),
          //CorporateEnquirySubjectNameFr =  reader["CorporateEnquirySubjectNameFr"].ToString()
        };
        subjectList.Add(subject);
      }
      return subjectList.ToList();
    }

    public ActionResult CorporateOfficeInfo()
    {
      return View();
    }

    public ActionResult SubmitMsg()
    {
      return PartialView();
    }
    public CorporateEnquirySubject GetCorpSubDetails(string SubjectID)
    {
      SqlConnection conn = new SqlConnection(constring);
      SqlCommand cmd = new SqlCommand();
      SqlDataReader reader;

      cmd.Parameters.AddWithValue("SubjectID", SubjectID);
      cmd.CommandText = @"Select * from CorporateEnquirySubject where CorporateEnquirySubjectID = @SubjectID";
      cmd.CommandType = CommandType.Text;
      cmd.Connection = conn;
      conn.Open();
      reader = cmd.ExecuteReader();

      var lang = Sitecore.Context.Language;
      CorporateEnquirySubject subject = new CorporateEnquirySubject();

      while (reader.Read())
      {
        {
          subject.CorporateEnquirySubjectName = lang.Name.ToString() == "en" ? reader["CorporateEnquirySubjectName"].ToString() : reader["CorporateEnquirySubjectNameFr"].ToString();
          subject.CorporateEnquirySubjectEmailDistribution = reader["CorporateEnquirySubjectEmailDistribution"].ToString();
          //CorporateEnquirySubjectNameFr =  reader["CorporateEnquirySubjectNameFr"].ToString()
        };
      }

      return subject;
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult Index(CorporateContactModel contact)
    {
      var req = Request.Form.Count;

      CorporateEnquirySubject details = GetCorpSubDetails(contact.Subject);
      contact.CorporateEnquirySubjectEmail = details.CorporateEnquirySubjectEmailDistribution;

      var propname = GetCorpPropertyDetails(contact, contact.ItemLanguage).Where(x => x.PropertyID == contact.ResidenceOfInterest).FirstOrDefault();
      contact.PropertyName = propname.ResidenceOfInterest;

      contact.CorporateEnquirySubjectLine = contact.ItemLanguage == "en" ? ConfigurationManager.AppSettings["CorporatEmailSubject"].ToString() + " " + contact.PropertyName.ToTitleCase() : ConfigurationManager.AppSettings["CorporatEmailSubjectFr"].ToString() + " " + contact.PropertyName.ToTitleCase();

      SqlDataReader rdr = null;
      SqlConnection conn = new SqlConnection(constring);
      conn.Open();
      SqlCommand cmd = new SqlCommand("sp_InsertCorporateEnquiry", conn)
      {
        CommandType = CommandType.StoredProcedure
      };
      cmd.Parameters.AddWithValue("@FirstName", contact.FirstName.Trim());
      cmd.Parameters.AddWithValue("@LastName", !string.IsNullOrEmpty(contact.LastName) ? contact.LastName.Trim() : string.Empty);
      cmd.Parameters.AddWithValue("@PhoneNo", !string.IsNullOrEmpty(contact.PhoneNo) ? contact.PhoneNo.Trim() : string.Empty);
      cmd.Parameters.AddWithValue("@EMailAddress", contact.EMailAddress.Trim());
      cmd.Parameters.AddWithValue("@Question", !string.IsNullOrEmpty(contact.Questions) ? contact.Questions.Trim() : string.Empty);
      cmd.Parameters.AddWithValue("@YardiID", Int32.Parse(contact.ResidenceOfInterest));
      cmd.Parameters.AddWithValue("@PropertyName", contact.PropertyName);
      //cmd.Parameters.AddWithValue("@Subject", details.CorporateEnquirySubjectName);
      cmd.Parameters.AddWithValue("@Consent", contact.ConsentToConnect);
      //cmd.Parameters.AddWithValue("@CorporateEnquirySubjectEmail", details.CorporateEnquirySubjectEmailDistribution.Trim());
      //cmd.Parameters.AddWithValue("@CorporateEnquirySubjectID", Convert.ToInt32(contact.Subject));
      //cmd.Parameters.AddWithValue("@FormTypeName", contact.ItemLanguage == "en" ? ConfigurationManager.AppSettings["CorporateContactUs"].ToString() : ConfigurationManager.AppSettings["CorporateContactUsFr"].ToString());
      cmd.Parameters.AddWithValue("@ContactLanguage", contact.ItemLanguage == "en" ? "English" : "French");
      cmd.Parameters.AddWithValue("@EmailSubjectLine", contact.CorporateEnquirySubjectLine);
      cmd.Parameters.AddWithValue("@CorporateEnquirySubject", details.CorporateEnquirySubjectName);
      rdr = cmd.ExecuteReader();

      try
      {
        SendMail(contact);
      }
      catch (SmtpException ex)
      {
        string msg = Translate.Text("EmailServerErrorMsg1") + "<br>";
        msg += Translate.Text(ex.Message.ToTitleCase().Replace(" ", "").Replace(".", "")) + "<br>"; //ex.Message 
        msg += "<br>";
        msg += Translate.Text("EmailServerErrorRefresh1");
        msg += " " + "<a href=" + Request.UrlReferrer + ">" + Sitecore.Globalization.Translate.Text("EmailServerErrorRefresh2") + "</a>";
        msg += " " + Translate.Text("EmailServerErrorRefresh3");
        Response.Write(msg);
        contact.SendEmailError = true;
      }
      return PartialView("SubmitMsg", contact);
    }

    private void SendMail(CorporateContactModel contact)
    {
      StringBuilder emailBody = new StringBuilder();

      var fromAddress = contact.ItemLanguage == "en" ? ConfigurationManager.AppSettings["CorporateEmailFrom"].ToString() : ConfigurationManager.AppSettings["CorporateEmailFromFr"].ToString();
      var toAddress = string.Empty;
      if (contact.FirstName.Trim().ToLower() == "test")
      {
        toAddress = "patelshirin@gmail.com";
      }
      else
      {
        toAddress = contact.CorporateEnquirySubjectEmail;
      }

      string fromPassword = contact.ItemLanguage == "en" ? ConfigurationManager.AppSettings["CorporateEmailPassword"].ToString() : ConfigurationManager.AppSettings["CorporateEmailPasswordFr"].ToString();
      string subject = contact.ItemLanguage == "en" ? ConfigurationManager.AppSettings["CorporatEmailSubject"].ToString().ToTitleCase() + " " + contact.PropertyName.ToTitleCase() : ConfigurationManager.AppSettings["CorporatEmailSubjectFr"].ToString().ToTitleCase() + " " + contact.PropertyName.ToTitleCase();

      PrepareEmail(contact, emailBody);

      var smtp = new System.Net.Mail.SmtpClient();
      {
        smtp.Host = ConfigurationManager.AppSettings["SMTPHOSTNAME"].ToString();
        smtp.Port = 587;
        smtp.EnableSsl = true;
        smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
        smtp.Credentials = new System.Net.NetworkCredential(fromAddress, fromPassword);

      }
      smtp.Send(fromAddress, toAddress, subject, emailBody.ToString());
    }

    private static void PrepareLabelsForEmail(CorporateContactModel contact)
    {
      contact.DisplayEmailFirstName = Translate.Text("FirstName");
      contact.DisplayEmailLastName = Translate.Text("LastName");
      contact.DisplayEmailPhoneNo = Translate.Text("PhoneNo");
      contact.DisplayEmailId = Translate.Text("Email");
      contact.DisplayEmailQuestion = Translate.Text("Questions");
      contact.DisplayEmailConsent = Translate.Text("ConsentToConnect");
      contact.ContactUsConfirmMsg1 = Translate.Text("ContactUsConfirmMsg1");
      contact.ContactUsConfirmMsg2 = Translate.Text("ContactUsConfirmMsg2");
      contact.ContactUsConfirmMsg3 = Translate.Text("ContactUsConfirmMsg3");
    }

    private static void PrepareEmail(CorporateContactModel contact, StringBuilder body1)
    {
      body1.Append(contact.DisplayEmailFirstName + ": " + contact.FirstName + Environment.NewLine);
      body1.Append(contact.DisplayEmailLastName + ": " + contact.LastName + Environment.NewLine);
      body1.Append(contact.DisplayEmailPhoneNo + ": " + contact.PhoneNo + Environment.NewLine);
      body1.Append(contact.DisplayEmailId + ": " + contact.EMailAddress + Environment.NewLine);
      body1.Append(contact.DisplayEmailQuestion + ": " + Environment.NewLine + contact.Questions + Environment.NewLine);
      String Consenttxt = Convert.ToBoolean(contact.ConsentToConnect) ? (contact.ItemLanguage == "en" ? "Yes" : "Oui") : (contact.ItemLanguage == "fr" ? "Non" : "No");
      body1.Append(contact.DisplayEmailConsent + ": " + Consenttxt + Environment.NewLine);
    }



  }
}