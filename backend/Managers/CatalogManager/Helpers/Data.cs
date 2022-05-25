namespace CatalogManager.Helpers
{
    public class Data
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Target { get; set; } = "SendMessage";
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Argument[] Arguments { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
