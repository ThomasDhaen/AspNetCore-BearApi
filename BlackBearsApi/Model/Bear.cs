using Newtonsoft.Json;
using System;

namespace BlackBearApi.Model
{
    public class Bear
    {
        [JsonProperty(PropertyName = "id")]
        private string Id => Name;

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "weight")]
        public int Weight { get; set; }
    }
}