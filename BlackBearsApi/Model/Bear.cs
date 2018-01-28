using Newtonsoft.Json;

namespace BlackBearApi.Model
{
    public class Bear
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "weight")]
        public int Weight { get; set; }
    }
}