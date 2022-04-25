using Newtonsoft.Json;

namespace CatalogManager.Models.AccessorResults
{

    public class Test
    {
        public string Id { get; set; }

        public string Title { get; set; }
        public string SchemaVersion { get; set; }
        public string TestVersion { get; set; }
        public string PreviousVersionId { get; set; }
        public string AuthorId { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public string[] Questions { get; set; }
        public DateTimeOffset  CreationTimeUTC { get; set; }
        public DateTimeOffset LastUpdateCreationTimeUTC { get; set; }
        public string Status { get; set; }
    }

    

   
}
