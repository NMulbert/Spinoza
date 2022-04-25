using Newtonsoft.Json;

namespace CatalogManager.Models.FrontendResponses
{

    public class Question
    {
        public string MessageType { get; set; }
        public string Id { get; set; }
        public string SchemaVersion { get; set; }
        public string QuestionVersion { get; set; }
    }

   
}
