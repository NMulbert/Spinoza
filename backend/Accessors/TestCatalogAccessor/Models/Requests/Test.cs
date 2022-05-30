namespace Spinoza.Backend.Accessor.TestCatalog.Models.Requests
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public class Test
    {
        public string MessageType { get; set; } = "";
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Id { get; set; }
        public string Title { get; set; }
        public string SchemaVersion { get; set; }
        public string TestVersion { get; set; }
        public string PreviousVersionId { get; set; }
        public string AuthorId { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public string[] QuestionsRefs { get; set; }

        public string Status { get; set; }
    }


#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
