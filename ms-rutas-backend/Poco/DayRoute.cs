using System.Text.Json.Serialization;

namespace ms_rutas_backend.Poco
{
    public class DayRoute
    {
        [JsonPropertyName("idRoute")]
        public int idRoute { get; set; }
        [JsonPropertyName("idDay")]
        public int idDay { get; set; }
    }
}