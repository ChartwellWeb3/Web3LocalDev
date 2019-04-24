using Chartwell.Feature.RegionalContact.Models;
using Chartwell.Foundation.utility;
using Sitecore.Globalization;
using SitecoreOLP.OP;
using SitecoreOLP.OP.DA;
using System;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using System.Linq;
using System.IO;
using System.Web;

namespace Chartwell.Feature.RegionalContact.Controllers
{

  public class RegionalContactController : Controller
  {
    string constring = ConfigurationManager.ConnectionStrings["SitecoreOLP"].ToString();

    // GET: RegionalContact
    public ActionResult Index()
    {
      RegionalContactModel contact = new RegionalContactModel();

      TimeOfDayForVisit(contact);

      var database = Sitecore.Context.Database;

      ChartwellUtiles util = new ChartwellUtiles();

      var ParentPropertyItemID = Sitecore.Context.Item.ID;
      var ParentPropertyItem = util.PropertyDetails(ParentPropertyItemID).Where(x => x.Language == Sitecore.Context.Language.Name).ToList();
      var PropertyItem = ParentPropertyItem[0].GetItem();

      contact.PropertyPhoneNo = PropertyItem.Fields["RegionalPhoneNumber"].Value;
      contact.ItemLanguage = PropertyItem.Language.Name;

      contact.PropertyName = contact.NonContactUsFormName = PropertyItem.Fields["Title"].Value.Replace("-", " ");

      PrepareLabelsForEmail(contact);

      return View(contact);
    }

    private static void TimeOfDayForVisit(RegionalContactModel contact)
    {
      contact.TimeOfDayOfVisitList = new SelectList(new[]
{
            new SelectListItem { Text = Translate.Text("AnyTime"), Value = Translate.Text("AnyTime"), Selected =true },
            new SelectListItem { Text = Translate.Text("Morning"), Value = Translate.Text("Morning") },
            new SelectListItem { Text = Translate.Text("Afternoon"), Value = Translate.Text("Afternoon") },
            new SelectListItem { Text = Translate.Text("Evening"), Value = Translate.Text("Evening") }
        }, "Text", "Value");
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]

    public ActionResult Index(RegionalContactModel contact)
    {
      var lang = Sitecore.Context.Language;

      string emailSubject = string.Empty;


      if (string.IsNullOrEmpty(contact.PropertyName))
        contact.PropertyName = contact.NonContactUsFormName;

      if (contact.VisitDate.HasValue)
      {
        emailSubject = contact.ItemLanguage == "en" ? ConfigurationManager.AppSettings["ContactUsPVEmailSubject"].ToString() + " " + contact.PropertyName.ToTitleCase() : ConfigurationManager.AppSettings["ContactUsPVEmailSubjectFr"].ToString() + " " + contact.PropertyName.ToTitleCase();
      }
      else
      {
        emailSubject = contact.ItemLanguage == "en" ? ConfigurationManager.AppSettings["ContactUsEmailSubject"].ToString() + " " + contact.PropertyName.ToTitleCase() : ConfigurationManager.AppSettings["ContactUsEmailSubjectFr"].ToString() + " " + contact.PropertyName.ToTitleCase();
      }

      if (ModelState.IsValid)
      {

        PersonDT person = new PersonDT();
        try
        {
          person.FirstName = contact.FirstName.Trim();
          person.LastName = !string.IsNullOrEmpty(contact.LastName) ? contact.LastName.Trim() : string.Empty;
          person.PhoneFaxNumber = contact.PhoneNo = !string.IsNullOrEmpty(contact.ContactPhoneNo) ? contact.ContactPhoneNo.Trim() : string.Empty;
          person.EmailAddress = contact.EmailAddress.Trim();
          person.ContactMeForSubscription = Convert.ToBoolean(contact.ConsentToConnect);
          person.Questions = !string.IsNullOrEmpty(contact.Question) ? contact.Question.Trim() : string.Empty;
          person.YardiID = "22222"; //contact.YardiID;
          person.PropertyName = contact.PropertyName;
          person.NonContactUsFormName = contact.PropertyName;
          person.FormTypeName = contact.ItemLanguage == "en" ? ConfigurationManager.AppSettings["PropertyContactUs"].ToString() : ConfigurationManager.AppSettings["PropertyContactUsFr"].ToString();
          person.PVDate = contact.VisitDate;
          person.PVTime = contact.VisitDate.HasValue ? contact.TimeOfDayForVisit : string.Empty;
          person.EmailSubjectLine = emailSubject;
          person.ContactLanguage = contact.ItemLanguage == "en" ? "English" : "French";

          StringBuilder LogEntry = new StringBuilder();
          LogEntry.AppendLine("[INFO] : Contact Form Submitted successfully");
          LogEntry.AppendLine("Language : (" + contact.ItemLanguage + ") " + " Form Name : " + contact.PropertyName);
          LogEntry.AppendLine();
          ContactFormLog(LogEntry.ToString());

        }
        catch (Exception ex)
        {
          StringBuilder msg = new StringBuilder();
          msg.Append("Exception Type: ");
          msg.AppendLine(ex.GetType().ToString());
          msg.AppendLine("Exception: " + ex.Message);
          msg.AppendLine("Stack Trace: ");
          if (ex.StackTrace != null)
          {
            msg.AppendLine(ex.StackTrace);
            msg.AppendLine();
          }
          msg.AppendLine("Language : (" + contact.ItemLanguage + ") " + " Form Name : " + contact.PropertyName);
          ContactFormLog(msg.ToString());
          Response.Write("Internal Error");
          contact.SendEmailError = true;
          return PartialView("ContactMsg", contact);
        };

        PersonDA Person = new PersonDA(constring);
        Person.GeneralInsertContactUs(person);
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
          msg += " " + "<a href=" + Request.UrlReferrer + ">" + Translate.Text("EmailServerErrorRefresh2") + "</a>";
          msg += " " + Translate.Text("EmailServerErrorRefresh3");
          Response.Write(msg);
          contact.SendEmailError = true;
        }
        return PartialView("RegionalContactMsg", contact);
      }
      else
      {
        ModelState.Clear();
        return View(contact);
      }
    }

    private void SendMail(RegionalContactModel contact)
    {
      StringBuilder emailBody = new StringBuilder();

      var fromAddress = contact.ItemLanguage == "en" ? ConfigurationManager.AppSettings["ContactUsEmailFrom"].ToString() : ConfigurationManager.AppSettings["ContactUsEmailFromFr"].ToString();
      var toAddress = string.Empty;
      if (contact.FirstName.Trim().ToLower() == "test")
      {
        toAddress = "patelshirin@gmail.com";
      }
      else
      {
        toAddress = ConfigurationManager.AppSettings["EMAILCOMMONCONTACTUS"].ToString();
      }

      string fromPassword = ConfigurationManager.AppSettings["ContactUsEmailPass"].ToString();

      string subject = string.Empty;
      if (contact.VisitDate.HasValue)
      {
        subject = contact.ItemLanguage == "en" ? ConfigurationManager.AppSettings["ContactUsPVEmailSubject"].ToString() + " " + contact.PropertyName.ToTitleCase() : ConfigurationManager.AppSettings["ContactUsPVEmailSubjectFr"].ToString() + " " + contact.PropertyName.ToTitleCase();
      }
      else
      {
        subject = contact.ItemLanguage == "en" ? ConfigurationManager.AppSettings["ContactUsEmailSubject"].ToString() + " " + contact.PropertyName.ToTitleCase() : ConfigurationManager.AppSettings["ContactUsEmailSubjectFr"].ToString() + " " + contact.PropertyName.ToTitleCase();
      }

      PrepareEmail(contact, emailBody);

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

    private void ContactFormLog(string ContactFormMessage)
    {
      var hostName = new DirectoryInfo(Request.PhysicalApplicationPath).Name;

      string dirName = "C:\\inetpub\\wwwroot\\" + hostName + "\\App_Data\\ContactFormsLog\\" + DateTime.Now.ToString("MMM") + DateTime.Now.Year.ToString();
      string filename = dirName + "\\ContactFormsLogEntries - " + DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt";

      if (!Directory.Exists(dirName))
      {
        Directory.CreateDirectory(dirName);
      }

      //if (!System.IO.File.Exists(filename))
      //{
      //  System.IO.File.Create(filename);
      //}

      string logFile = "~/App_Data/ContactFormsLog/" + DateTime.Now.ToString("MMM")
                                                     + DateTime.Now.Year.ToString()
                                                     + "/"
                                                     + "ContactFormsLogEntries - "
                                                     + DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt";

      logFile = HttpContext.Server.MapPath(logFile);

      // Open the log file for append and write the log  
      StreamWriter sw = new StreamWriter(logFile, true);
      sw.WriteLine("********** {0} **********", DateTime.Now);
      sw.Write(ContactFormMessage);

      sw.Close();
    }

    private static void PrepareLabelsForEmail(RegionalContactModel contact)
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

    private static void PrepareEmail(RegionalContactModel contact, StringBuilder body1)
    {
      body1.Append(contact.DisplayEmailFirstName + ": " + contact.FirstName + Environment.NewLine);
      body1.Append(contact.DisplayEmailLastName + ": " + contact.LastName + Environment.NewLine);
      body1.Append(contact.DisplayEmailPhoneNo + ": " + contact.PhoneNo + Environment.NewLine);
      body1.Append(contact.DisplayEmailId + ": " + contact.EmailAddress + Environment.NewLine);
      body1.Append(contact.DisplayEmailQuestion + ": " + Environment.NewLine + contact.Question + Environment.NewLine);
      String Consenttxt = Convert.ToBoolean(contact.ConsentToConnect) ? (contact.ItemLanguage == "en" ? "Yes" : "Oui") : (contact.ItemLanguage == "fr" ? "Non" : "No");
      body1.Append(contact.DisplayEmailConsent + ": " + Consenttxt + Environment.NewLine);

      if (contact.VisitDate.HasValue)
      {
        body1.Append(contact.DisplayEmailVisitDate + ": " + contact.VisitDate.Value.ToShortDateString() + Environment.NewLine);
        body1.Append(contact.DisplayEmailVisitTime + ": " + contact.TimeOfDayForVisit);
      }
    }
  }
}