﻿using Newtonsoft.Json;

namespace Spinoza.Backend.Accessor.TestCatalog.Models
{
    public class TestModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public AuthorModel? Author { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
        public List<Guid> Questions { get; set; } = new List<Guid>();

        public string Status { get; set; } = string.Empty;
        public float Version { get; set; }
        [JsonProperty(PropertyName = "_eTag")]
        public string? ETag { get; set; }  

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
