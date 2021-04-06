using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Net.Http.Headers;
using ms_rutas_backend.Data.Models;
using Neo4j.Driver;

namespace ms_rutas_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodeRouteController : ControllerBase
    {
        private IDriver _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "pass"));
        public void Dispose()
        {
            _driver?.Dispose();
        }

        [HttpPost ("createRoute")]
        public IActionResult createRoute([FromBody] NodeRoute route)
        {
            using(var session = _driver.Session())
            {
                var response = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("CREATE (r:Route) " +
                                        "SET r.idRoute = $route.idRoute , r.startCity = $route.startCity, r.arrivalCity = $route.arrivalCity, r.description = $route.description, " +
                                        "r.latitudeStart = $route.latitudeStart, r.longitudeStart = $route.longitudeStart , r.latitudeEnd = $route.latitudeEnd, r.longitudeEnd = $route.longitudeEnd " +
                                        "RETURN id(r)",
                        new { route });
                    int route_id = 0;
                    foreach(var r in result)
                    {
                        try
                        {
                            route_id = r["id(r)"].As<int>();
                        }
                        catch (KeyNotFoundException e)
                        {
                            route_id = -1;
                        }
                    }
                    return route_id;
                });
                if (response >= 0)
                {
                    Dictionary<string, int> resp = new Dictionary<string, int>();
                    resp.Add("idNodeRoute", response);
                    return Ok(resp);
                }
                else
                {
                    return Conflict("Esta ruta ya existe");
                }
                
            }
        }

        
        [HttpPost("getRouteById")]
        public IActionResult getRouteById([FromBody] NodeRoute route)
        {
            using(var session = _driver.Session())
            {
                var routes = session.ReadTransaction(tx => tx.Run("MATCH (r:Route) WHERE r.idRoute = $route.idRoute RETURN r", new { route }).ToList());
                if(routes.Count > 0)
                {
                    return Ok(routes);
                }
                else
                {
                    return Conflict("No existe la ruta");
                }
            }
        }
        //Falla
        [HttpPost("getRouteIdById")]
        public IActionResult getRouteIdById([FromBody] NodeRoute route)
        {
            using(var session = _driver.Session())
            {
                var routes = session.ReadTransaction(tx => tx.Run("MATCH (r:Route) WHERE r.idRoute = $route.idRoute RETURN id(r)", new { route }));
                
                int idRoute = 0;
                foreach(var r in routes)
                {
                    try
                    {
                        idRoute = r["id(r)"].As<int>();
                    }
                    catch(KeyNotFoundException e)
                    {
                        idRoute = -1;
                    }
                }
                if(idRoute >= 0)
                {
                    Dictionary<string, int> resp = new Dictionary<string, int>();
                    resp.Add("idNodeRoute", idRoute);
                    return Ok(resp);
                }
                else
                {
                    return Conflict("La ruta no existe");
                }
            }
        }

        [HttpGet("getRoutes")]
        public IActionResult getRoutes()
        {
            using(var session = _driver.Session())
            {
                var routes = session.ReadTransaction(tx => tx.Run("MATCH (r:Route) RETURN r").ToList());
                if (routes.Count > 0)
                {
                    return Ok(routes);
                }
                else
                {
                    return Conflict("No hay rutas disponibles");
                }
                
            }
        }

        [HttpPut("modifyRoute")]
        public IActionResult modifyRoute([FromBody] NodeRoute route)
        {
            String query = "";
            if (route.startCity != null) query += "r.startCity = $route.startCity ,";
            if (route.arrivalCity != null) query += "r.arrivalCity = $route.arrivalCity ,";
            if (route.description != null) query += "r.description = $route.description ,";
            if (route.latitudeStart != null ) query += "r.latitudeStart = $route.latitudeStart ,";
            if (route.longitudeStart != null) query += "r.longitudeStart = $route.longitudeStart ,";
            if (route.latitudeEnd != null) query += "r.latitudeEnd = $route.latitudeEnd ,";
            if (route.longitudeEnd != null) query += "r.longitudeEnd = $route.longitudeEnd ";

            query = query.TrimEnd(',');

            using(var session = _driver.Session())
            {
                var response = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (r:Route) " + "WHERE r.idRoute = $route.idRoute SET " +
                                        query +
                                        "RETURN r.message + ', from node ' + id(r)",
                        new { route });
                    return result.Single()[0].As<string>();
                });
            }
            return Ok("Ruta cambiada correctamente");
        }

        [HttpDelete("deleteRoute")]
        public IActionResult deleteRoute([FromBody] NodeRoute route)
        {
            using(var session = _driver.Session())
            {
                var response = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (r:Route) " + "WHERE r.idRoute = $route.idRoute " +
                                        "DELETE r",
                        new { route });
                    return true;
                });
            }
            return Ok();
        }

    }
}
