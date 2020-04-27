using Newtonsoft.Json;

namespace WebApi.Web.Helpers.Exceptions
{
  public class ErrorModel
  {
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Message { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string StackTrace { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Source { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ClassName { get; set; }
  }
}
