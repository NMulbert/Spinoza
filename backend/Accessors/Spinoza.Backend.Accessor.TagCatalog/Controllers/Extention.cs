using System.Net;

namespace Spinoza.Backend.Accessor.TagCatalog.Controllers;

public static class Extention
{
    public static bool IsOk(this HttpStatusCode httpStatusCode) => (int)httpStatusCode >= 200 && (int)httpStatusCode <= 299;  

}