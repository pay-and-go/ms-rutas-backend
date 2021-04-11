using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ms_rutas_backend.Data.Models;
using ms_rutas_backend.Poco;
using Neo4j.Driver;

namespace ms_rutas_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelationCarDateController : ControllerBase
    {
        private static IConfiguration _iConfiguration;
        private IDriver _driver;

        public RelationCarDateController(IConfiguration iConfiguration)
        {
            _iConfiguration = iConfiguration;
            _driver = GraphDatabase.Driver(_iConfiguration.GetConnectionString("UriConnection"),
                AuthTokens.Basic(_iConfiguration.GetConnectionString("UserConnection"),
                    _iConfiguration.GetConnectionString("PasswordConnection")));
        }

        [HttpPost("createRelationCarDay")] 
        public IActionResult createRelationCarDay([FromBody] CarDay rel_carday)
        {
            using(var session = _driver.Session())
            {
                var response = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (c:Car) MATCH (d:Date) " +
                                        "WHERE id(c)=$rel_carday.idCar AND id(d)=$rel_carday.idDay " +
                                        "CREATE (c)-[re:travel_in]->(d) RETURN id(re)",
                        new { rel_carday });
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
                if(response >= 0)
                {
                    Dictionary<string, int> resp = new Dictionary<string, int>();
                    resp.Add("idRelationCarDate", response);
                    return Ok(resp);
                }
                else
                {
                    return Conflict("No se encuentran los nodos");
                }
            }
        }

        [HttpPost("getRelationByDate")]
        public IActionResult getRelationByDate([FromBody] DateCar datecar)
        {
            using(var session = _driver.Session())
            {
                var response = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (c:Car {licenseCar : $datecar.licenseCar}) MATCH (d:Date {dayTravel : $datecar.dayTravel, monthTravel : $datecar.monthTravel, yearTravel : $datecar.yearTravel})" +
                                        "MATCH (c)-[re:travel_in]->(d) RETURN id(re)",
                        new { datecar });
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
                if(response >= 0)
                {
                    Dictionary<string, int> resp = new Dictionary<string, int>();
                    resp.Add("idRelationCarDate", response);
                    return Ok(resp);
                }
                else
                {
                    return Conflict("No existe relacion entre la fecha y el auto");
                }

            }
        }


        [HttpPost("getIdRelationCarDate")]
        public IActionResult getIdRelationCarDate([FromBody] CarDay rel_carday)
        {
            using(var session = _driver.Session())
            {
                var response = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (c:Car) MATCH (d:Date) MATCH(c:Car) " +
                                                            "WHERE id(c)=$rel_carday.idCar AND id(d)=$rel_carday.idDay " +
                                                            "MATCH (c)-[re:travel_in]->(d) RETURN id(re)",
                        new { rel_carday });
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
                if(response >= 0)
                {
                    Dictionary<string, int> resp = new Dictionary<string, int>();
                    resp.Add("idRelationCarDate", response);
                    return Ok(resp);
                }
                else
                {
                    return Conflict("No existe relacion entre la fecha y el auto");
                }

            }
        }

        [HttpGet("getIdDatebyIdRelation/{id}")]
        public IActionResult getDatebyIdRelation(int id)
        {
            using(var session = _driver.Session())
            {
                var response = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (c:Car)-[re:travel_in]->(d:Date) WHERE id(re)=$id RETURN id(d)",
                        new { id });
                    int idDate = 0;
                    foreach(var r in result)
                    {
                        try
                        {
                            idDate = r["id(d)"].As<int>();
                        }
                        catch(KeyNotFoundException e)
                        {
                            idDate = -1;
                        }
                    }
                    return idDate;
                });
                if(response != 0)
                {
                    Dictionary<string, int> resp = new Dictionary<string, int>();
                    resp.Add("idNodeDate", response);
                    return Ok(resp);
                }
                else
                {
                    return Conflict("No existe relacion entre la fecha y el auto");
                }

            }
        }

        [HttpDelete("deleteRelationCarDay")]
        public IActionResult deleteRelationCarDay([FromBody] CarDay rel_carday)
        {
            using(var session = _driver.Session())
            {
                var response = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (c:Car) MATCH (d:Date) WHERE " +
                                        "id(c)=$rel_carday.idCar AND id(d)=$rel_carday.idDay " +
                                        "MATCH (c)-[r:travel_in]->(d) DELETE r",
                        new { rel_carday });

                    return 0;
                });
                return Ok();
            }
        }

    }
}
