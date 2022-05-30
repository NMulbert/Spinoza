namespace CatalogManager.Models.AccessorSubmits
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class MultipleChoiceQuestion
    {
        public string MessageType { get; set; } = "";
        public string Id { get; set; }
        public string SchemaVersion { get; set; }
        public string QuestionVersion { get; set; }
        public string PreviousVersionId { get; set; }
        public string AuthorId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string DifficultyLevel { get; set; }
        public string[] Tags { get; set; }
        public string Status { get; set; }
        public Content Content { get; set; }
    }

    public class Content
    {
        public string QuestionText { get; set; }
        public AnswerOption[] AnswerOptions { get; set; }
    }

    public class AnswerOption
    {
        public string Description { get; set; }
        public bool IsCorrect { get; set; }
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

}
