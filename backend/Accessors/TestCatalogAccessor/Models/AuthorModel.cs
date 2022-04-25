using Newtonsoft.Json;

namespace Spinoza.Backend.Accessor.TestCatalog.Models
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
