using System;
using System.Text.Json.Serialization;

namespace ms_rutas_backend.Data.Models
{
    public class NodeDate
    {
        [JsonPropertyName("dayTravel")]
        public int dayTravel { get; set;}
        [JsonPropertyName("monthTravel")]
        public int monthTravel { get; set; }
        [JsonPropertyName("yearTravel")]
        public int yearTravel { get; set; }

        public NodeDate(int dayTravel, int monthTravel, int yearTravel)
        {
            this.dayTravel = dayTravel;
            this.monthTravel = monthTravel;
            this.yearTravel = yearTravel;
        }
    }
}