using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ms_rutas_backend.Data.Models;
using Neo4j.Driver;

namespace ms_rutas_backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class NodeDayController : ControllerBase
    {
        private IDriver _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "pass"));

        public void Dispose()
        {
            _driver?.Dispose();
        }

        [HttpPost("createDay")]
        public IActionResult createDay([FromBody] NodeDate date)
        {
            using(var session = _driver.Session())
            {
                var response = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("CREATE (d:Date) " +
                                        "SET d.dayTravel = $date.dayTravel, d.monthTravel = $date.monthTravel, d.yearTravel = $date.yearTravel " +
                                        "RETURN id(d)",
                        new { date });
                    int id_day = 0;
                    foreach(var r in result)
                    {
                        id_day = r["id(d)"].As<int>();
                    }
                    return id_day;
                });
                Dictionary<string, int> resp = new Dictionary<string, int>();
                resp.Add("idNodeDate", response);
                return Ok(resp);
            }
        }

        [HttpGet]
        public String prueba()
        {
            return "Hola";
        }
    }
}
