using Newtonsoft.Json;

namespace WebApplication1.Model
{
    public class Bear
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "weight")]
        public int Weight { get; set; }
    }
}