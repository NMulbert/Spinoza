using Newtonsoft.Json;

namespace Spinoza.Backend.Accessor.TestCatalog.Models.Responses
{ 
    public class TestChangeResult
    {
        public string MessageType { get; set; }
        public string Id { get; set; }
        public string SchemaVersion { get; set; } = "1.0";
        public string TestVersion { get; set; }
    }

   
}
