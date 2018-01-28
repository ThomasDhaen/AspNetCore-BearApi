using Newtonsoft.Json;

namespace BlackBearApi.Model
{
    public class Food
    {
        [JsonProperty(PropertyName = "id")]
        private string Id => Name;

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "kCal")]
        public int KCal { get; set; }
    }
}