using Newtonsoft.Json;

namespace CallApp.Models
{
    public class MakeCall
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }
    }
}