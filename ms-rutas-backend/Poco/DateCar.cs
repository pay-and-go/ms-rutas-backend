using System.Text.Json.Serialization;

namespace ms_rutas_backend.Poco
{
    public class DateCar
    {
        [JsonPropertyName("licenseCar")]
        public string licenseCar { get; set; }
        [JsonPropertyName("dayTravel")]
        public int dayTravel { get; set; }
        [JsonPropertyName("monthTravel")]
        public int monthTravel { get; set; }
        [JsonPropertyName("yearTravel")]
        public int yearTravel { get; set; }
    }
}