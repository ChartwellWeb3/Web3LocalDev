using Chartwell.Feature.ExtractReport.Models;
using Sitecore.Security.Domains;
using SitecoreOLP.OP;
using SitecoreOLP.OP.DA;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace Chartwell.Feature.ExtractReport.Controllers
{
  public class ExtractReportController : Controller
  {
    string constring = ConfigurationManager.ConnectionStrings["SitecoreOLP"].ToString();
    private SqlCommand cmd;
    private SqlConnection con;

    // GET: ExtractReport
    public ActionResult Index()
    {
      //var user = System.Web.Security.Membership.GetUser(@"sitecore\Shirin");
      //reset Password
      //user.ChangePassword("prachis23", "gopal2412");
      return View();
    }

    [HttpPost]
    public ActionResult Index(ExtractReportModel report)
    {
      var domainUserList = Domain.GetDomain("sitecore").GetUsers().ToList();
      var user = domainUserList.Where(x => x.LocalName == report.Username).FirstOrDefault();
      string domainUser = string.Empty;
      if(user == null)
      {
        ModelState.AddModelError("Password", "Please Try Again");
        return View(report);
        //return PartialView("LoginStatusMsg", report);
      }
      else
      {
        domainUser = user.Domain.ToString() + @"\" + report.Username;
      }

      if (!System.Web.Security.Membership.ValidateUser(domainUser, report.Password))
      {
        report.LoginSuccess = false;
        ModelState.AddModelError("Password", "Invalid Username / Passowrd. Please Try Again");

        return View(report);
      }
      else
      {
        report.LoginSuccess = true;
        return PartialView("ExtractReport", report);
        //return RedirectToAction("ExtractReport");
      }
    }

    public ActionResult DisplayReport()
    {
      con = new SqlConnection(constring);
      con.Open();
      cmd = new SqlCommand("sp_GetExtractFromDate", con);
      cmd.Parameters.AddWithValue("@startdate", Request.Form["FromDate"]);
      cmd.Parameters.AddWithValue("@enddate", Request.Form["ToDate"]);

      cmd.CommandType = System.Data.CommandType.StoredProcedure;
      SqlDataReader dr = cmd.ExecuteReader();

      while (dr.Read())
      {
        var t = Convert.ToDateTime(dr["SubmissionDateAndTime"]).ToString("dd/MM/yyyy");
      };

      con.Close();

      return PartialView("DisplayReport");
    }
  }
}