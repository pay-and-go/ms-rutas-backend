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
        public double latitudeStart { get; set; }
        [JsonPropertyName("longitudeStart")]
        public double longitudeStart { get; set; }
        [JsonPropertyName("latitudeEnd")]
        public double latitudeEnd { get; set; }
        [JsonPropertyName("longitudeEnd")]
        public double longitudeEnd { get; set; }

        public NodeRoute(string idRoute, string startCity, string arrivalCity, string description, double latitudeStart, double longitudeStart, double latitudeEnd, double longitudeEnd)
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

        public override string ToString()
        {
            return idRoute + " " + startCity + " " + arrivalCity + " " + description + " " + longitudeStart + " " +
                   latitudeStart;
        }
    }
}