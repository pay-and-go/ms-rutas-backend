using System.Text.Json.Serialization;

namespace ms_rutas_backend.Data.Models
{
    public class NodeRoute
    {
        [JsonPropertyName("idRoute")]
        public string idRoute { get; set; }
        [JsonPropertyName("startCity")]
        public string startCity{ get; set; }
        [JsonPropertyName("arrivalCity")]
        public string arrivalCity{ get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }
        [JsonPropertyName("latitudeStart")]
        public string latitudeStart { get; set; }
        [JsonPropertyName("longitudeStart")]
        public string longitudeStart { get; set; }
        [JsonPropertyName("latitudeEnd")]
        public string latitudeEnd { get; set; }
        [JsonPropertyName("longitudeEnd")]
        public string longitudeEnd { get; set; }
        public NodeRoute(string idRoute, string startCity, string arrivalCity, string description, string latitudeStart, string longitudeStart, string latitudeEnd, string longitudeEnd)
        {
            this.idRoute = idRoute;
            this.startCity = startCity;
            this.arrivalCity = arrivalCity;
            this.description = description;
            this.latitudeStart = latitudeStart;
            this.longitudeStart = longitudeStart;
            this.latitudeEnd = latitudeEnd;
            this.longitudeEnd = longitudeEnd;
        }
    }
}