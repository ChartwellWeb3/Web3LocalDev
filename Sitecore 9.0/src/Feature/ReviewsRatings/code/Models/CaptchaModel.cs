using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Chartwell.Feature.ReviewsRatings.Models
{
  public class CaptchaModel
  {
    public string CaptchaQuestion { get; set; }
    public string CaptchaAnswerImageName { get; set; }

    public string FakeImageName { get; set; }

    public string FakeImageNameSecond { get; set; }


  }
}