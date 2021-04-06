using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ms_rutas_backend.Data.Models
{    public class NodeCar
    {
        [JsonPropertyName("idOwner")]
        public string idOwner { get; set; }
        [JsonPropertyName("nameOwner")]
        public string nameOwner { get; set; }
        [JsonPropertyName("licenseCar")]
        public string licenseCar { get; set; }
        public NodeCar(string idOwner, string nameOwner, string licenseCar)
        {
            this.idOwner = idOwner;
            this.nameOwner = nameOwner;
            this.licenseCar = licenseCar;
        }
    }
}
