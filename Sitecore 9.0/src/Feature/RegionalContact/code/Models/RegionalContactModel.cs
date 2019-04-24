using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.RegionalContact.Models
{
  public class RegionalContactModel
  {
    public string PropertyName { get; set; }

    public string PhoneNo { get; set; }

    [LocalizedDisplayName("FirstName")]
    [LocalizedRequired(ErrorMessage = "FirstNameValidationMsg")]
    [RegularExpression(@"[^<>]*", ErrorMessage = "Characters and space only")]
    public string FirstName { get; set; }

    [LocalizedDisplayName("LastName")]
    [RegularExpression(@"[^<>]*", ErrorMessage = "Characters and space only")]
    public string LastName { get; set; }

    [LocalizedDisplayName("PhoneNo")]
    //[RegularExpression(@"[^<>]*", ErrorMessage = "Numbers and space only")]
    public string ContactPhoneNo { get; set; }

    [LocalizedDisplayName("Email")]
    [LocalizedRequired(ErrorMessage = "EmailValidationMsg")]
    [EmailAddress]
    public string EmailAddress { get; set; }

    [LocalizedDisplayName("ConsentToConnect")]
    [LocalizedRequired(ErrorMessage = "ConsentToConnectValidationMsg")]
    public bool? ConsentToConnect { get; set; }

    [LocalizedDisplayName("Question")]
    [StringLength(1000, ErrorMessage = "Only 1000 characters allowed")]
    [RegularExpression(@"[^<>:/]*", ErrorMessage = "Characters and space only")]
    public string Question { get; set; }

    public string ContactPropertyName { get; set; }

    public DateTime? VisitDate { get; set; }

    public string TimeOfDayForVisit { get; set; }

    public IEnumerable<SelectListItem> TimeOfDayOfVisitList { get; set; }

    public string PropertyPhoneNo { get; set; }

    public string City { get; set; }

    public bool SendEmailError { get; set; }

    public string EmailSubjectLine { get; set; }

    public string ItemLanguage { get; set; }

    public Language ContextLanguage { get; set; }

    public string NonContactUsFormName { get; set; }

    public string DisplayEmailFirstName { get; set; }

    public string DisplayEmailLastName { get; set; }

    public string DisplayEmailPhoneNo { get; set; }
    public string DisplayEmailId { get; set; }

    public string DisplayEmailQuestion { get; set; }

    public string DisplayEmailVisitDate { get; set; }

    public string DisplayEmailVisitTime { get; set; }

    public string DisplayEmailConsent { get; set; }

    public string ContactUsConfirmMsg1 { get; set; }

    public string ContactUsConfirmMsg2 { get; set; }

    public string PVConfirmationMsg { get; set; }

    public string YardiID { get; set; }
  }

  #region Class for overriding the Required attribute (Client / Server) 

  /// <summary>
  /// Overrides Required attribute to read and display validation error messages from the Sitecore dictionary
  /// </summary>
  public class LocalizedRequiredAttribute : RequiredAttribute, IClientValidatable
  {
    public override string FormatErrorMessage(string name)
    {
      return Translate.Text(base.FormatErrorMessage(name));
    }

    IEnumerable<ModelClientValidationRule> IClientValidatable.GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
      return new[] { new ModelClientValidationRequiredRule(Translate.Text(ErrorMessage)) };
    }
  }

  #endregion

  #region Class for overriding the DisplayName attribute

  /// <summary>
  /// Overrides DisplayName attribute to read and display the column title from the Sitecore dictionary
  /// </summary>
  public class LocalizedDisplayNameAttribute : DisplayNameAttribute
  {
    private readonly string resourceName;
    public LocalizedDisplayNameAttribute(string resourceName) : base()
    {
      this.resourceName = resourceName;
    }
    public override string DisplayName
    {
      get
      {
        return Translate.Text(resourceName);
      }
    }
  }
  #endregion

  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public sealed class EmailAddressAttribute : DataTypeAttribute, IClientValidatable
  {
    private static Regex _regex = new Regex(@"^[\w-\._\+%]+@(?:[\w-]+\.)+[\w]{2,6}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public EmailAddressAttribute()
        : base(DataType.EmailAddress)
    {
      ErrorMessage = "{0}";
    }


    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
      yield return new ModelClientValidationRule
      {
        ValidationType = "email",
        ErrorMessage = FormatErrorMessage(Translate.Text("EmailInvalidError"))
      };
    }

    public override bool IsValid(object value)
    {
      if (value == null)
      {
        return true;
      }

      string valueAsString = value as string;

      return valueAsString != null && _regex.Match(valueAsString).Length > 0;
    }
  }

}