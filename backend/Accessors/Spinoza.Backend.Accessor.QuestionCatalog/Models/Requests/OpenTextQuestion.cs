namespace Spinoza.Backend.Accessor.QuestionCatalog.Models.Requests
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    // ReSharper disable once ClassNeverInstantiated.Global
    public class OpenTextQuestion : IQuestion
    {
        public string MessageType { get; set; }
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
        public string Content { get; set; }
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

}
