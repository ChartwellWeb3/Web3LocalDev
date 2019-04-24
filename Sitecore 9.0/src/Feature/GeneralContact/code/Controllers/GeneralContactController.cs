using Chartwell.Feature.GeneralContact.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Globalization;
using SitecoreOLP.OP;
using SitecoreOLP.OP.DA;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

public class GeneralContactController : Controller
{
  private string constring = ConfigurationManager.ConnectionStrings["SitecoreOLP"].ToString();

  public ActionResult Index()
  {
    GeneralContactModel generalContactModel = new GeneralContactModel();
    generalContactModel.TimeOfDayOfVisitList = new SelectList(new SelectListItem[4]
    {
            new SelectListItem
            {
                Text = Translate.Text("AnyTime"),
                Value = Translate.Text("AnyTime"),
                Selected = true
            },
            new SelectListItem
            {
                Text = Translate.Text("Morning"),
                Value = Translate.Text("Morning")
            },
            new SelectListItem
            {
                Text = Translate.Text("Afternoon"),
                Value = Translate.Text("Afternoon")
            },
            new SelectListItem
            {
                Text = Translate.Text("Evening"),
                Value = Translate.Text("Evening")
            }
    }, "Text", "Value");
    ChartwellUtiles chartwellUtiles = new ChartwellUtiles();
    ID iD = Context.Item.ID;
    List<SearchResultItem> list = (from x in chartwellUtiles.PropertyDetails(iD)
                                   where x.Language == Context.Language.Name
                                   select x).ToList();
    Item item = list[0].GetItem();
    generalContactModel.ItemLanguage = item.Language.Name;
    PrepareLabelsForEmail(generalContactModel);
    Template template = TemplateManager.GetTemplate(item);
    if (template.Name != "Standard template")
    {
      if (template.Name == "Static Pages")
      {
        string text2 = generalContactModel.PropertyName = (generalContactModel.NonContactUsFormName = item.DisplayName);
      }
      else if (!(template.Name == "Blog Post"))
      {
        string text2 = generalContactModel.PropertyName = (generalContactModel.NonContactUsFormName = item.DisplayName);
      }
      else
      {
        string text2 = generalContactModel.PropertyName = (generalContactModel.NonContactUsFormName = template.Name + " " + item.DisplayName);
      }
    }
    else
    {
      string text6 = base.Request.QueryString.ToString();
      if (text6.Contains("PropertyName"))
      {
        string text2 = generalContactModel.PropertyName = generalContactModel.NonContactUsFormName = "Search Results for " + Request.QueryString[0].ToTitleCase();
      }
      else if (text6.Contains("PostalCode"))
      {
        //string text2 = generalContactModel.PropertyName = generalContactModel.NonContactUsFormName = Translate.Text("Retirement Homes in and around") + " " + Request.QueryString[0].ToTitleCase();
        string text2 = generalContactModel.PropertyName = generalContactModel.NonContactUsFormName = Translate.Text("RetirementHomesNear") + " " + Request.QueryString[0].ToUpper();
      }
      else
      {
        //string text2 = generalContactModel.PropertyName = generalContactModel.NonContactUsFormName = Translate.Text("RetirementHomesNear") + " " + Request.QueryString[0].ToUpper();
        string text2 = generalContactModel.PropertyName = generalContactModel.NonContactUsFormName = Translate.Text("Retirement Homes in and around") + " " +
                      (Request.QueryString.Count != 0 ? Request.QueryString[0].ToTitleCase() : "Canada");

      }
    }
    return View(generalContactModel);
  }

  [HttpPost]
  [AllowAnonymous]
  [ValidateAntiForgeryToken]
  public ActionResult Index(GeneralContactModel contact)
  {
    if (base.ModelState.IsValid)
    {
      string empty = string.Empty;
      contact.YardiID = "22222";
      if (string.IsNullOrEmpty(contact.PropertyName))
      {
        contact.PropertyName = contact.NonContactUsFormName;
      }
      empty = (contact.VisitDate.HasValue ? ((!contact.PropertyName.Contains("Blog Post")) ? ((contact.ItemLanguage == "en") ? (ConfigurationManager.AppSettings["ContactUsPVEmailSubject"].ToString() + " " + contact.PropertyName.ToTitleCase()) : (ConfigurationManager.AppSettings["ContactUsPVEmailSubjectFr"].ToString() + " " + contact.PropertyName.ToTitleCase())) : ((contact.ItemLanguage == "en") ? (ConfigurationManager.AppSettings["ContactUsPVEmailSubject"].ToString() + " Blog: " + contact.PropertyName.ToTitleCase()) : (ConfigurationManager.AppSettings["ContactUsPVEmailSubjectFr"].ToString() + " Blogue: " + contact.PropertyName.ToTitleCase()))) : ((!contact.PropertyName.Contains("Blog Post")) ? ((contact.ItemLanguage == "en") ? (ConfigurationManager.AppSettings["ContactUsEmailSubject"].ToString() + " " + contact.PropertyName.ToTitleCase()) : (ConfigurationManager.AppSettings["ContactUsEmailSubjectFr"].ToString() + " " + contact.PropertyName.ToTitleCase())) : ((contact.ItemLanguage == "en") ? (ConfigurationManager.AppSettings["ContactUsEmailSubject"].ToString() + " Blog: " + contact.PropertyName.ToTitleCase()) : (ConfigurationManager.AppSettings["ContactUsEmailSubjectFr"].ToString() + " Blogue: " + contact.PropertyName.ToTitleCase()))));
      PersonDT personDT = new PersonDT();
      try
      {
        personDT.FirstName = contact.FirstName.Trim();
        personDT.LastName = ((!string.IsNullOrEmpty(contact.LastName)) ? contact.LastName.Trim() : string.Empty);
        string text3 = personDT.PhoneFaxNumber = (contact.PhoneNo = ((!string.IsNullOrEmpty(contact.ContactPhoneNo)) ? contact.ContactPhoneNo.Trim() : string.Empty));
        personDT.EmailAddress = contact.EmailAddress.Trim();
        personDT.ContactMeForSubscription = System.Convert.ToBoolean(contact.ConsentToConnect);
        personDT.Questions = ((!string.IsNullOrEmpty(contact.Question)) ? contact.Question.Trim() : string.Empty);
        personDT.YardiID = contact.YardiID;
        personDT.PropertyName = ((!string.IsNullOrEmpty(contact.PropertyName)) ? contact.PropertyName.Trim() : string.Empty);
        personDT.CityName = ((!string.IsNullOrEmpty(contact.ContactCity)) ? contact.ContactCity : string.Empty);
        personDT.PVDate = contact.VisitDate;
        personDT.PVTime = (contact.VisitDate.HasValue ? contact.TimeOfDayForVisit : string.Empty);
        personDT.EmailSubjectLine = empty;
        personDT.ContactLanguage = ((contact.ItemLanguage == "en") ? "English" : "French");
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("[INFO] : Contact Form Submitted successfully");
        stringBuilder.AppendLine("Language : (" + contact.ItemLanguage + ")  Form Name : " + contact.NonContactUsFormName);
        stringBuilder.AppendLine();
        ContactFormLog(stringBuilder.ToString());
      }
      catch (Exception ex)
      {
        StringBuilder stringBuilder2 = new StringBuilder();
        stringBuilder2.Append("Exception Type: ");
        stringBuilder2.AppendLine(ex.GetType().ToString());
        stringBuilder2.AppendLine("Exception: " + ex.Message);
        stringBuilder2.AppendLine("Stack Trace: ");
        if (ex.StackTrace != null)
        {
          stringBuilder2.AppendLine(ex.StackTrace);
          stringBuilder2.AppendLine();
        }
        stringBuilder2.AppendLine("Language : (" + contact.ItemLanguage + ")  Form Name : " + contact.NonContactUsFormName);
        ContactFormLog(stringBuilder2.ToString());
        base.Response.Write("Internal Error");
        contact.SendEmailError = true;
        return PartialView("ContactMsg", contact);
      }
      PersonDA personDA = new PersonDA(constring);
      personDA.GeneralInsertContactUs(personDT);
      try
      {
        SendMail(contact);
      }
      catch (SmtpException ex2)
      {
        string str = Translate.Text("EmailServerErrorMsg1") + "<br>";
        str = str + Translate.Text(ex2.Message.ToTitleCase().Replace(" ", "").Replace(".", "")) + "<br>";
        str += "<br>";
        str += Translate.Text("EmailServerErrorRefresh1");
        str = str + " <a href=" + base.Request.UrlReferrer + ">" + Translate.Text("EmailServerErrorRefresh2") + "</a>";
        str = str + " " + Translate.Text("EmailServerErrorRefresh3");
        base.Response.Write(str);
        contact.SendEmailError = true;
      }
      return PartialView("GeneralContactMsg", contact);
    }
    base.ModelState.Clear();
    return View(contact);
  }

  private void ContactFormLog(string ContactFormMessage)
  {
    string name = new DirectoryInfo(base.Request.PhysicalApplicationPath).Name;
    string text = "C:\\inetpub\\wwwroot\\" + name + "\\App_Data\\ContactFormsLog\\" + DateTime.Now.ToString("MMM") + DateTime.Now.Year.ToString();
    string text2 = text + "\\ContactFormsLogEntries - " + DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt";
    if (!Directory.Exists(text))
    {
      Directory.CreateDirectory(text);
    }
    string path = "~/App_Data/ContactFormsLog/" + DateTime.Now.ToString("MMM") + DateTime.Now.Year.ToString() + "/ContactFormsLogEntries - " + DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt";
    path = base.HttpContext.Server.MapPath(path);
    StreamWriter streamWriter = new StreamWriter(path, append: true);
    streamWriter.WriteLine("********** {0} **********", DateTime.Now);
    streamWriter.Write(ContactFormMessage);
    streamWriter.Close();
  }

  private void SendMail(GeneralContactModel contact)
  {
    StringBuilder stringBuilder = new StringBuilder();
    string text = (contact.ItemLanguage == "en") ? ConfigurationManager.AppSettings["ContactUsEmailFrom"].ToString() : ConfigurationManager.AppSettings["ContactUsEmailFromFr"].ToString();
    string empty = string.Empty;
    empty = ((!(contact.FirstName.Trim().ToLower() == "test")) ? ConfigurationManager.AppSettings["EMAILCOMMONCONTACTUS"].ToString() : "patelshirin@gmail.com");
    string password = ConfigurationManager.AppSettings["ContactUsEmailPass"].ToString();
    string empty2 = string.Empty;
    empty2 = ((!contact.VisitDate.HasValue) ? ((contact.ItemLanguage == "en") ? (ConfigurationManager.AppSettings["ContactUsEmailSubject"].ToString() + " " + contact.PropertyName.ToTitleCase()) : (ConfigurationManager.AppSettings["ContactUsEmailSubjectFr"].ToString() + " " + contact.PropertyName.ToTitleCase())) : ((contact.ItemLanguage == "en") ? (ConfigurationManager.AppSettings["ContactUsPVEmailSubject"].ToString() + " " + contact.PropertyName.ToTitleCase()) : (ConfigurationManager.AppSettings["ContactUsPVEmailSubjectFr"].ToString() + " " + contact.PropertyName.ToTitleCase())));
    PrepareEmail(contact, stringBuilder);
    SmtpClient smtpClient = new SmtpClient();
    smtpClient.Host = ConfigurationManager.AppSettings["SMTPHOSTNAME"].ToString();
    smtpClient.Port = 587;
    smtpClient.EnableSsl = true;
    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
    smtpClient.Credentials = new NetworkCredential(text, password);
    smtpClient.Send(text, empty, empty2, stringBuilder.ToString());
  }

  private static void PrepareLabelsForEmail(GeneralContactModel contact)
  {
    contact.DisplayEmailFirstName = Translate.Text("FirstName");
    contact.DisplayEmailLastName = Translate.Text("LastName");
    contact.DisplayEmailPhoneNo = Translate.Text("PhoneNo");
    contact.DisplayEmailId = Translate.Text("Email");
    contact.DisplayEmailCityName = Translate.Text("City");
    contact.DisplayEmailQuestion = Translate.Text("Question");
    contact.DisplayEmailVisitDate = Translate.Text("PreferredDateForPV");
    contact.DisplayEmailVisitTime = Translate.Text("Tour");
    contact.DisplayEmailConsent = Translate.Text("ConsentToConnect");
    contact.DisplayRetirementHomesNear = Translate.Text("RetirementHomesNear");
    contact.DisplayRetirementHomesInAndAround = Translate.Text("Retirement Homes In And Around");
    contact.ContactUsConfirmMsg1 = Translate.Text("ContactUsConfirmMsg1");
    contact.ContactUsConfirmMsg2 = Translate.Text("ContactUsConfirmMsg2");
    contact.PVConfirmationMsg = Translate.Text("PVConfirmationMsg");
  }

  private static void PrepareEmail(GeneralContactModel contact, StringBuilder body1)
  {
    body1.Append(contact.DisplayEmailFirstName + ": " + contact.FirstName + Environment.NewLine);
    body1.Append(contact.DisplayEmailLastName + ": " + contact.LastName + Environment.NewLine);
    body1.Append(contact.DisplayEmailPhoneNo + ": " + contact.PhoneNo + Environment.NewLine);
    body1.Append(contact.DisplayEmailId + ": " + contact.EmailAddress + Environment.NewLine);
    body1.Append(contact.DisplayEmailCityName + ": " + contact.ContactCity + Environment.NewLine);
    body1.Append(contact.DisplayEmailQuestion + ": " + Environment.NewLine + contact.Question + Environment.NewLine);
    string str = (!System.Convert.ToBoolean(contact.ConsentToConnect)) ? ((contact.ItemLanguage == "fr") ? "Non" : "No") : ((contact.ItemLanguage == "en") ? "Yes" : "Oui");
    body1.Append(contact.DisplayEmailConsent + ": " + str + Environment.NewLine);
    if (contact.VisitDate.HasValue)
    {
      body1.Append(contact.DisplayEmailVisitDate + ": " + contact.VisitDate.Value.ToShortDateString() + Environment.NewLine);
      body1.Append(contact.DisplayEmailVisitTime + ": " + contact.TimeOfDayForVisit);
    }
  }

  public ActionResult GeneralContactMsg()
  {
    return PartialView();
  }
}
