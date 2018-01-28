using Newtonsoft.Json;

namespace WebApplication1.Model
{
    public class Food
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "kCal")]
        public int KCal { get; set; }
    }
}