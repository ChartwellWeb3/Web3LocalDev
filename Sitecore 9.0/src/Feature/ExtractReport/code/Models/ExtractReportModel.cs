using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Chartwell.Feature.ExtractReport.Models
{
  public class ExtractReportModel
  {
    [DisplayName("User Name")]
    [Required]
    public string Username { get; set; }
    [DisplayName("Password")]
    [Required]
    [PasswordPropertyText]
    public string Password { get; set; }

    public bool LoginSuccess { get; set; }

    [DisplayName("From")]
    public DateTime? FromDate { get; set; }

    [DisplayName("To")]
    public DateTime? ToDate { get; set; }

  }
}