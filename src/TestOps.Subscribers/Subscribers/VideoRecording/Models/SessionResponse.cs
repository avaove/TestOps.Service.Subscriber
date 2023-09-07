using Newtonsoft.Json;


namespace TestOps.Subscribers.VideoRecording.Models
{
    public class ResponseData
    {
        [JsonProperty("data")]
        public DataObject? Data { get; set; }
    }

    public class DataObject
    {
        [JsonProperty("session")]
        public SessionData? Session { get; set; }
    }

    public class SessionData
    {
        [JsonProperty("nodeUri")]
        public string? NodeUri { get; set; }
    }
}
