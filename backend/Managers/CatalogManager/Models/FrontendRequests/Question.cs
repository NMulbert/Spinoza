using CatalogManager.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CatalogManager.Models.FrontendRequests
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public interface IQuestion
    {
        string Type { get; set; }
        string MessageType { get; set; }
        string QuestionVersion { get; set; }
        string[] Tags { get; set; }
        string Status { get; set; }
    }
    public class CommonQuestion : IQuestion
    {
        public string Type { get; set; }
        public string MessageType { get; set; }
        public string QuestionVersion { get; set; }
        public string[] Tags { get; set; }
        public string Status { get; set; }
    }

    // ReSharper disable ClassNeverInstantiated.Global
    public class MultipleChoiceQuestion : IQuestion
    {
        [ValidateNever]
        public string MessageType { get; set; }

        [Required(ErrorMessage = "the Id is missing")]
        public string Id { get; set; }
        [ValidateNever]
        public string SchemaVersion { get; set; }
        [ValidateNever]
        public string QuestionVersion { get; set; }
        [ValidateNever]
        public string PreviousVersionId { get; set; }

        [Required(ErrorMessage = "The Author Id is missing")]
        public string AuthorId { get; set; }

        [Required(ErrorMessage = "The question name is missing")]
        [MaxLength(100, ErrorMessage = "The title is  bigger than 100 characters")]
        [MinLength(3, ErrorMessage = "The title is less than 3 characters")]
        public string Name { get; set; }
        public string Type { get; set; }
        [ValidateNever]
        public string Status { get; set; }
        [Required(ErrorMessage = "Please enter question difficulty")]
        [Range(typeof(int), "1", "5", ErrorMessage = "Please enter question difficulty btewwen 1-5")]
        public string DifficultyLevel { get; set; }
        [ValidateNever]
        public string[] Tags { get; set; }
        public Content Content { get; set; }
    }

    public class Content
    {
        [Required(ErrorMessage = "Please write your question")]
        public string QuestionText { get; set; }

        [MinLength(2, ErrorMessage = "Please enter more than one option answer")]
        [AtLeastOneRightOption]
        public AnswerOption[] AnswerOptions { get; set; }
    }

    public class AnswerOption
    {
        public string Description { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class OpenTextQuestion : IQuestion
    {
        [ValidateNever]
        public string MessageType { get; set; }

        [Required(ErrorMessage = "the Id is missing")]
        public string Id { get; set; }
        [ValidateNever]
        public string SchemaVersion { get; set; }
        [ValidateNever]
        public string QuestionVersion { get; set; }
        [ValidateNever]
        public string PreviousVersionId { get; set; }
        [Required(ErrorMessage = "The Author Id is missing")]
        public string AuthorId { get; set; }

        [Required(ErrorMessage = "The question name is missing")]
        [MaxLength(100, ErrorMessage = "The title is  bigger than 100 characters")]
        [MinLength(3, ErrorMessage = "The title is less than 3 characters")]
        public string Name { get; set; }
        public string Type { get; set; }
        [Required(ErrorMessage = "Please enter question difficulty")]
        [Range(typeof(int), "1", "5", ErrorMessage = "Please enter question difficulty btewwen 1-5")]
        public string DifficultyLevel { get; set; }
        [ValidateNever]
        public string Status { get; set; }
        [ValidateNever]
        public string[] Tags { get; set; }

        [Required(ErrorMessage = "Please write your question content")]
        public string Content { get; set; }
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
