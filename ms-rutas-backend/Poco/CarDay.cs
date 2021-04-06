using System.Text.Json.Serialization;

namespace ms_rutas_backend.Poco
{
    public class CarDay
    {
        [JsonPropertyName("idCar")]
        public int idCar { get; set; }
        [JsonPropertyName("idDay")]
        public int idDay { get; set; }
    }
}