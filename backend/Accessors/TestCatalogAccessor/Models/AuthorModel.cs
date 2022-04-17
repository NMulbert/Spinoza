using Newtonsoft.Json;

namespace TestCatalogAccessor.Models
{
    public class AuthorModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
