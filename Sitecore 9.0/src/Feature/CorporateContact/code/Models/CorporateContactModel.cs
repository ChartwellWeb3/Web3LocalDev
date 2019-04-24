using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Chartwell.Feature.CorporateContact.Models
{
  public class CorporateContactModel
  {
    public List<CorporateEnquirySubject> SubjectList { get; set; }

    public List<CorporateContactModel> PropertyList { get; set; }

    //[LocalizedDisplayName("FirstName")]
    //[LocalizedRequired(ErrorMessage = "FirstNameValidationMsg")]

    [LocalizedDisplayName("FirstName")]
    [LocalizedRequired(ErrorMessage = "FirstNameValidationMsg")]
    public string FirstName { get; set; }

    [LocalizedDisplayName("LastName")]
    public string LastName { get; set; }

    [LocalizedDisplayName("PhoneNo")]
    public string PhoneNo { get; set; }

    [LocalizedDisplayName("Email")]
    [LocalizedRequired(ErrorMessage = "EmailValidationMsg")]
    [EmailAddress]
    public string EMailAddress { get; set; }

    [LocalizedDisplayName("Question")]
    [StringLength(1000, ErrorMessage = "Only 1000 characters allowed")]
    [RegularExpression(@"[^<>:/]*", ErrorMessage = "Characters and space only")]
    public string Questions { get; set; }

    public string PropertyID { get; set; }
    [LocalizedDisplayName("ResidenceOfInterest")]
    public string ResidenceOfInterest { get; set; }

    public string PropertyName { get; set; }

    [LocalizedDisplayName("Subject")]
    public string Subject { get; set; }

    [LocalizedDisplayName("CorpConsentToConnect")]
    [LocalizedRequired(ErrorMessage = "ConsentToConnectValidationMsg")]
    public bool? ConsentToConnect { get; set; }

    public string CorporateEnquirySubjectEmail { get; set; }

    public bool SendEmailError { get; set; }
    public string ItemLanguage { get; set; }

    public string DisplayEmailFirstName { get; set; }

    public string DisplayEmailLastName { get; set; }

    public string DisplayEmailPhoneNo { get; set; }
    public string DisplayEmailId { get; set; }

    public string DisplayEmailQuestion { get; set; }

    public string DisplayEmailConsent { get; set; }

    public string ContactUsConfirmMsg1 { get; set; }

    public string ContactUsConfirmMsg2 { get; set; }

    public string ContactUsConfirmMsg3 { get; set; }

    public string CorporateEnquirySubjectLine { get; set; }

    public string CorporateEnquirySubject { get; set; }
  }

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

  #endregion

}