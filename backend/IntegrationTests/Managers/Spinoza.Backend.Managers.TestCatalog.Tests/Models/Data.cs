using System.Text.Json.Serialization;

namespace Spinoza.Backend.Managers.TestCatalog.Tests.Models
{
    public class Data
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Sender { get; set; }
        public TestChangeResult Text { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }


}