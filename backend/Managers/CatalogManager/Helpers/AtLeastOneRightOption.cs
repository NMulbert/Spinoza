using System.ComponentModel.DataAnnotations;

namespace CatalogManager.Helpers
{
    public class AtLeastOneRightOption : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Models.FrontendRequests.AnswerOption[] answerOptions = (Models.FrontendRequests.AnswerOption[])value;
            foreach (var answerOption in answerOptions)
            {
                if (answerOption.IsCorrect == true)
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("there is no correct answers please correct you multipalcechoise question");
        }
    }
}
