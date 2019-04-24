using Chartwell.Feature.ContactUS.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Globalization;
using Sitecore.Links;
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

namespace Chartwell.Feature.ContactUs.Controllers
{

  public class ContactUSController : Controller
  {
    string constring = ConfigurationManager.ConnectionStrings["SitecoreOLP"].ToString();

    // GET: ContactUS
    public ActionResult Index()
    {
      ChartwellUtiles util = new ChartwellUtiles();
      ContactUSModel contact = new ContactUSModel();
      TimeOfDayOfVisitDropDown(contact);
      PrepareLabelsForEmail(contact);
      ID ParentPropertyItemID = Context.Item.ID;
      Item PropertyItem = (from x in util.PropertyDetails(ParentPropertyItemID)
                           where x.Language == Context.Language.Name
                           select x).FirstOrDefault().GetItem();
      Template templateType = TemplateManager.GetTemplate(PropertyItem);
      if (templateType.Name != "Standard template")
      {
        if (templateType.Name == "SplitterPage")
        {
          contact.PropertyPhoneNo = PropertyItem.Fields["SplitterPhone"].Value;
          string text3 = contact.PropertyName = (contact.NonContactUsFormName = PropertyItem.Name.ToTitleCase());
          List<SearchResultItem> propertyDetailsResults = util.GetYardiForCommunity(PropertyItem.DisplayName);
          List<ContactUSModel> propertyDetails = (from c in propertyDetailsResults
                                                  select new ContactUSModel
                                                  {
                                                    YardiID = c.GetItem().Fields["Property ID"].Value,
                                                    PropertyName = c.GetItem().Fields["Property Name"].Value
                                                  }).ToList();
          contact.YardiID = propertyDetails[0].YardiID;
          contact.SplitterPageContactUsHeader = true;
          contact.PropertyType = "PropertyContactUsFormSubmit";
          contact.ItemLanguage = PropertyItem.Language.Name.ToString();
        }
        else
        {
          Item contactUsPropertyItemPath = (from x in util.PropertyDetails(ParentPropertyItemID)
                                            where x.Language == PropertyItem.Language.Name
                                            select x).FirstOrDefault().GetItem().Parent;
          contact.PropertyPhoneNo = util.GetPhoneNumber(contactUsPropertyItemPath);
          int propertyID = System.Convert.ToInt32(contactUsPropertyItemPath.Fields["Property ID"].Value);
          contact.YardiID = contactUsPropertyItemPath.Fields["Property ID"].Value;
          string text3 = contact.PropertyName = (contact.NonContactUsFormName = PropertyItem.Parent.Name.ToTitleCase());
          string strPropertyType = (from x in util.PropertyDetails(new ID(contactUsPropertyItemPath.Fields["property type"].ToString()))
                                    where x.Language == PropertyItem.Language.Name
                                    select x).FirstOrDefault().GetItem().Name;
          if (strPropertyType.Equals("LTC"))
          {
            contact.PropertyType = "LTC_ContactFormSubmit";
          }
          else
          {
            contact.PropertyType = "PropertyContactUsFormSubmit";
          }
          contact.SplitterPageContactUsHeader = false;
          contact.ItemLanguage = contactUsPropertyItemPath.Language.Name.ToString();
          contact.ChartwellEmail = util.GetEmail(contactUsPropertyItemPath);
        }
      }
      return View(contact);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Index(ContactUSModel contact)
    {
      string emailSubject2 = string.Empty;
      if (string.IsNullOrEmpty(contact.PropertyName))
      {
        contact.PropertyName = contact.NonContactUsFormName;
      }
      emailSubject2 = ((!contact.VisitDate.HasValue) ? ((contact.ItemLanguage == "en") ? (ConfigurationManager.AppSettings["ContactUsEmailSubject"].ToString() + " " + contact.PropertyName.ToTitleCase()) : (ConfigurationManager.AppSettings["ContactUsEmailSubjectFr"].ToString() + " " + contact.PropertyName.ToTitleCase())) : ((contact.ItemLanguage == "en") ? (ConfigurationManager.AppSettings["ContactUsPVEmailSubject"].ToString() + " " + contact.PropertyName.ToTitleCase()) : (ConfigurationManager.AppSettings["ContactUsPVEmailSubjectFr"].ToString() + " " + contact.PropertyName.ToTitleCase())));
      if (base.ModelState.IsValid)
      {
        PersonDT person = new PersonDT();
        try
        {
          person.FirstName = contact.FirstName.Trim();
          person.LastName = ((!string.IsNullOrEmpty(contact.LastName)) ? contact.LastName.Trim() : string.Empty);
          string text3 = person.PhoneNumber = (contact.PhoneNo = ((!string.IsNullOrEmpty(contact.ContactPhoneNo)) ? contact.ContactPhoneNo.Trim() : string.Empty));
          person.EmailAddress = contact.EmailAddress.Trim();
          person.ContactMeForSubscription = System.Convert.ToBoolean(contact.ConsentToConnect);
          person.Questions = ((!string.IsNullOrEmpty(contact.Question)) ? contact.Question.Trim() : string.Empty);
          person.YardiID = contact.YardiID;
          person.PropertyName = contact.PropertyName;
          person.PVDate = contact.VisitDate;
          person.PVTime = (contact.VisitDate.HasValue ? contact.TimeOfDayForVisit : string.Empty);
          person.EmailSubjectLine = emailSubject2;
          person.ContactLanguage = ((contact.ItemLanguage == "en") ? "English" : "French");
          StringBuilder LogEntry = new StringBuilder();
          LogEntry.Append("[INFO] : Contact Form Submitted successfully");
          LogEntry.AppendLine("Language : (" + contact.ItemLanguage + ")  Form Name : " + contact.PropertyName);
          ContactFormLog(LogEntry.ToString());
        }
        catch (Exception ex2)
        {
          StringBuilder msg5 = new StringBuilder();
          msg5.Append("Exception Type: ");
          msg5.AppendLine(ex2.GetType().ToString());
          msg5.AppendLine("Exception: " + ex2.Message);
          msg5.AppendLine("Stack Trace: ");
          if (ex2.StackTrace != null)
          {
            msg5.AppendLine(ex2.StackTrace);
          }
          msg5.AppendLine("Language : (" + contact.ItemLanguage + ")  Form Name : " + contact.PropertyName);
          ContactFormLog(msg5.ToString());
          base.Response.Write("Internal Error");
          contact.SendEmailError = true;
          return PartialView("ContactMsg", contact);
        }
        PersonDA Person = new PersonDA(constring);
        Person.InsertContactUs(person);
        try
        {
          SendMail(contact);
        }
        catch (SmtpException ex)
        {
          string msg4 = Translate.Text("EmailServerErrorMsg1") + "<br>";
          msg4 += Translate.Text("EmailServerErrorRefresh1");
          msg4 = msg4 + " <a href=" + base.Request.UrlReferrer + ">" + Translate.Text("EmailServerErrorRefresh2") + "</a>";
          msg4 = msg4 + " " + Translate.Text("EmailServerErrorRefresh3");
          base.Response.Write(msg4);
          contact.SendEmailError = true;
          StringBuilder EmailServerErrorLogMsg = new StringBuilder();
          EmailServerErrorLogMsg.AppendLine("Email Server error");
          EmailServerErrorLogMsg.AppendLine(ex.Message);
          ContactFormLog(EmailServerErrorLogMsg.ToString());
        }
        contact.ContextLanguage = Context.Language;
        return PartialView("ContactMsg", contact);
      }
      base.ModelState.Clear();
      return View(contact);
    }

    private void ContactFormLog(string ContactFormMessage)
    {
      string hostName = new DirectoryInfo(base.Request.PhysicalApplicationPath).Name;
      string dirName = "C:\\inetpub\\wwwroot\\" + hostName + "\\App_Data\\ContactFormsLog\\" + DateTime.Now.ToString("MMM") + DateTime.Now.Year.ToString();
      string filename = dirName + "\\ContactFormsLogEntries - " + DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt";
      if (!Directory.Exists(dirName))
      {
        Directory.CreateDirectory(dirName);
      }
      string logFile2 = "~/App_Data/ContactFormsLog/" + DateTime.Now.ToString("MMM") + DateTime.Now.Year.ToString() + "/ContactFormsLogEntries - " + DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt";
      logFile2 = base.HttpContext.Server.MapPath(logFile2);
      StreamWriter sw = new StreamWriter(logFile2, append: true);
      sw.WriteLine("********** {0} **********", DateTime.Now);
      sw.Write(ContactFormMessage);
      sw.Close();
    }

    private static void PrepareLabelsForEmail(ContactUSModel contact)
    {
      contact.DisplayEmailFirstName = Translate.Text("FirstName");
      contact.DisplayEmailLastName = Translate.Text("LastName");
      contact.DisplayEmailPhoneNo = Translate.Text("PhoneNo");
      contact.DisplayEmailId = Translate.Text("Email");
      contact.DisplayEmailQuestion = Translate.Text("Question");
      contact.DisplayEmailVisitDate = Translate.Text("PreferredDateForPV");
      contact.DisplayEmailVisitTime = Translate.Text("Tour");
      contact.DisplayEmailConsent = Translate.Text("ConsentToConnect");
      contact.ContactUsConfirmMsg1 = Translate.Text("ContactUsConfirmMsg1");
      contact.ContactUsConfirmMsg2 = Translate.Text("ContactUsConfirmMsg2");
      contact.PVConfirmationMsg = Translate.Text("PVConfirmationMsg");
    }

    private static void TimeOfDayOfVisitDropDown(ContactUSModel contact)
    {
      contact.TimeOfDayOfVisitList = new SelectList(new SelectListItem[4]
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
    }

    private static void SendMail(ContactUSModel contact)
    {
      string body = string.Empty;
      StringBuilder emailBody = new StringBuilder();
      string fromAddress = (contact.ItemLanguage == "en") ? ConfigurationManager.AppSettings["ContactUsEmailFrom"].ToString() : ConfigurationManager.AppSettings["ContactUsEmailFromFr"].ToString();
      string toAddress = (contact.FirstName.Trim().ToLower() == "test") ? "patelshirin@gmail.com" : ((!string.IsNullOrEmpty(contact.ChartwellEmail)) ? contact.ChartwellEmail : ConfigurationManager.AppSettings["EMAILCOMMONCONTACTUS"].ToString());
      string fromPassword = (contact.ItemLanguage == "en") ? ConfigurationManager.AppSettings["ContactUsEmailPass"].ToString() : ConfigurationManager.AppSettings["ContactUsEmailPassFr"].ToString();
      string subject = (!contact.VisitDate.HasValue) ? ((contact.ItemLanguage == "en") ? (ConfigurationManager.AppSettings["ContactUsEmailSubject"].ToString() + " " + contact.PropertyName.ToTitleCase()) : (ConfigurationManager.AppSettings["ContactUsEmailSubjectFr"].ToString() + " " + contact.PropertyName.ToTitleCase())) : ((contact.ItemLanguage == "en") ? (ConfigurationManager.AppSettings["ContactUsPVEmailSubject"].ToString() + " " + contact.PropertyName.ToTitleCase()) : (ConfigurationManager.AppSettings["ContactUsPVEmailSubjectFr"].ToString() + " " + contact.PropertyName.ToTitleCase()));
      PrepareEmail(contact, emailBody);
      SmtpClient smtp = new SmtpClient();
      smtp.Host = ConfigurationManager.AppSettings["SMTPHOSTNAME"].ToString();
      smtp.Port = 587;
      smtp.EnableSsl = true;
      smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
      smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);
      smtp.Send(fromAddress, toAddress, subject, emailBody.ToString());
    }

    private static void PrepareEmail(ContactUSModel contact, StringBuilder body1)
    {
      body1.Append(contact.DisplayEmailFirstName + ": " + contact.FirstName + Environment.NewLine);
      body1.Append(contact.DisplayEmailLastName + ": " + contact.LastName + Environment.NewLine);
      body1.Append(contact.DisplayEmailPhoneNo + ": " + contact.PhoneNo + Environment.NewLine);
      body1.Append(contact.DisplayEmailId + ": " + contact.EmailAddress + Environment.NewLine);
      body1.Append(contact.DisplayEmailQuestion + ": " + Environment.NewLine + contact.Question + Environment.NewLine);
      string Consenttxt = (!System.Convert.ToBoolean(contact.ConsentToConnect)) ? ((contact.ItemLanguage == "fr") ? "Non" : "No") : ((contact.ItemLanguage == "en") ? "Yes" : "Oui");
      body1.Append(contact.DisplayEmailConsent + ": " + Consenttxt + Environment.NewLine);
      if (contact.VisitDate.HasValue)
      {
        body1.Append(contact.DisplayEmailVisitDate + ": " + contact.VisitDate.Value.ToShortDateString() + Environment.NewLine);
        body1.Append(contact.DisplayEmailVisitTime + ": " + contact.TimeOfDayForVisit);
      }
    }

    public ActionResult ContactMsg()
    {
      return PartialView();
    }
  }
}