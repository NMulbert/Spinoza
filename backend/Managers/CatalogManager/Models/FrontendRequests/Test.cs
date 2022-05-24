using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CatalogManager.Models.FrontendRequests
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public class Test
    {
        [Required(ErrorMessage ="the Id is missing")]
        public string Id { get; set; }
        [Required(ErrorMessage ="The Title is missing")]
        [MaxLength(100,ErrorMessage ="The title is  bigger than 100 charecters")]
        [MinLength(3,ErrorMessage = "The title is less than 3 charecters")]
        public string Title { get; set; }
        public string SchemaVersion { get; set; }
        public string TestVersion { get; set; }
        public string? PreviousVersionId { get; set; }
        [Required(ErrorMessage = "The Author Id is missing")]
        public string AuthorId { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public string[] QuestionsRefs { get; set; }
    }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

}
