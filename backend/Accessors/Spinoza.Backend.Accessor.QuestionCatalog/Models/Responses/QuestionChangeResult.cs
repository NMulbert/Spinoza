namespace Spinoza.Backend.Accessor.QuestionCatalog.Models.Responses
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public class QuestionChangeResult
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string Id { get; set; }

        public string SchemaVersion { get; set; } = "1.0";
        public string MessageType { get; set; }
        public string ResourceType { get; set; }
        public string ActionResult { get; set; }
        public string Reason { get; init; }
        public int ReasonId { get; set; }
        public string Sender { get; set; }
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

}
