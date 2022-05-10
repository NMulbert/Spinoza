using System.Net;

namespace Spinoza.Backend.Accessor.QuestionCatalog.Controllers
{
    public static class Extention
    {
        public static bool IsOk(this HttpStatusCode httpStatusCode) => (int)httpStatusCode >= 200 && (int)httpStatusCode <= 299;
    }
}
