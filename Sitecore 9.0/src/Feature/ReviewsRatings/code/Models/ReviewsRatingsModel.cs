namespace Chartwell.Feature.ReviewsRatings.Models
{
  public class ReviewsRatingsModel
  {
    public string Email { get; set; }
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string RoleIdentification { get; set; }

    public string Comments { get; set; }

    public string Ratings { get; set; }

    public string TotalReviewsCnt { get; set; }
    public string OverallRatings { get; set; }

    public string CommentDate { get; set; }

    public string DisplayEmailFirstName { get; set; }
    public string DisplayEmailLastName { get; set; }
    public string DisplayEmailId { get; set; }
    public string DisplayEmailCommentDate { get; set; }
    public string DisplayEmailComment { get; set; }
    public string DisplayEmailRatings { get; set; }
  }
}