using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ms_rutas_backend.Poco;
using Neo4j.Driver;

namespace ms_rutas_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelationDateRouteController : ControllerBase
    {
        private static IConfiguration _iConfiguration;
        private IDriver _driver;

        public RelationDateRouteController(IConfiguration iConfiguration)
        {
            _iConfiguration = iConfiguration;
            _driver = GraphDatabase.Driver(_iConfiguration.GetConnectionString("UriConnection"),
                AuthTokens.Basic(_iConfiguration.GetConnectionString("UserConnection"),
                    _iConfiguration.GetConnectionString("PasswordConnection")));
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }

        [HttpPost("createRelationDayRoute")]
        public IActionResult createRelationDayRoute([FromBody] DayRoute rel_dayroute)
        {
            using(var session = _driver.Session())
            {
                var response = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (d:Date) MATCH (r:Route) WHERE " +
                                        "id(d)=$rel_dayroute.idDay AND id(r)=$rel_dayroute.idRoute " +
                                        "CREATE (d)-[re:using]->(r) RETURN id(re)",
                        new { rel_dayroute });
                    int idRelation = 0;
                    foreach(var r in result)
                    {
                        try
                        {
                            idRelation = r["id(re)"].As<int>();
                        }
                        catch(KeyNotFoundException e)
                        {
                            idRelation = -1;
                        }
                    }
                    return idRelation;
                });
                if (response >= 0)
                {
                    Dictionary<string, int> resp = new Dictionary<string, int>();
                    resp.Add("idRelationDateRoute", response);
                    return Ok(resp);
                }
                else
                {
                    return Conflict("No se encuentran los nodos");
                }
                
            }
        }
        
        [HttpPost("searchRelationDayRoute")]
        public IActionResult searchRelationDayRoute([FromBody] DayRoute rel_dayroute)
        {
            using(var session = _driver.Session())
            {
                var response = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (d:Date) MATCH (r:Route) WHERE " +
                                        "id(d)=$rel_dayroute.idDay AND id(r)=$rel_dayroute.idRoute " +
                                        "MATCH (d)-[re:using]->(r) " +
                                        "RETURN id(re)",
                        new { rel_dayroute });
                    int idRelacion = 0;
                    foreach(var r in result)
                    {
                        try
                        {
                            idRelacion = r["id(re)"].As<int>();
                        }
                        catch(KeyNotFoundException e)
                        {
                            idRelacion = -1;
                        }
                    }
                    return idRelacion;
                });
                if(response >= 0)
                {
                    Dictionary<string, int> resp = new Dictionary<string, int>();
                    resp.Add("idRelationDateRoute", response);
                    return Ok(resp);
                }
                else
                {
                    return Conflict("No se encuentra relacion entre los nodos");
                }
            }
        }

        [HttpDelete("deleteRelationDayRoute")]
        public IActionResult deleteRelationDayRoute([FromBody] DayRoute rel_dayroute)
        {
            using(var session = _driver.Session())
            {
                var response = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (d:Date) MATCH (r:Route) WHERE " +
                                        "id(d)=$rel_dayroute.idDay AND id(r)=$rel_dayroute.idRoute " +
                                        "MATCH (d)-[re:using]->(r) DELETE re",
                        new { rel_dayroute });
                    return 0;
                });
                return Ok();
            }
        }
    }
}
