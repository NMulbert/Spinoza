namespace Spinoza.Backend.Accessor.TagCatalog.Models.DB;

public class Tag
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public string Id { get; set; }
    public string TagName { get; set; }
    public string _etag { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}