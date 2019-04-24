using Chartwell.Feature.ReviewsRatings.Models;
using Sitecore.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Chartwell.Feature.ReviewsRatings.ViewModel
{
  public class ReviewsRatingViewModel
  {
    public PropertyModel Property { get; set; }

    public List<ReviewsRatingsModel> Reviews { get; set; }

    public List<RoleModel> RoleList { get; set; }

    public CaptchaModel Captcha { get; set; }

    public ReviewsRatingsModel Review { get; set; }

    //[Required(ErrorMessage = "Please select the correct fruit.")]
    [LocalizedRequired(ErrorMessage = "ReviewCaptchaError")]
    //[Remote("ValidateCaptcha", "ReviewsRatings", ErrorMessage = "Wrong Captcha. Please try again")]
    public List<string> CaptchaImagesList { get; set; }

    [EmailAddress]
    [LocalizedRequired(ErrorMessage = "ReviewsEmailError")]
    public string Email { get; set; }

    [LocalizedDisplayName("ReviewsFirstName")]
    [LocalizedRequired(ErrorMessage = "ReviewsFirstNameError")]

    public string FirstName { get; set; }

    [LocalizedDisplayName("ReviewsLasttName")]
    [LocalizedRequired(ErrorMessage = "ReviewsLastNameError")]

    public string LastName { get; set; }

    [LocalizedDisplayName("ReviewsRole")]
    public string RoleIdentification { get; set; }

    [LocalizedDisplayName("ReviewsCommentTitle")]
    public string Comments { get; set; }

    [LocalizedDisplayName("ReviewsContentTitle")]
    [LocalizedRequired(ErrorMessage = "ReviewsRatingsError")]
    public string Ratings { get; set; }

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

  #endregion


}