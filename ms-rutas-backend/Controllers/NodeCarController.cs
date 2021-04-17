using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using ms_rutas_backend.Data.Models;
using ms_rutas_backend;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace ms_rutas_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodeCarController : ControllerBase
    {
        private static IConfiguration _iConfiguration;
        private IDriver _driver;
        public NodeCarController(IConfiguration iConfiguration)
        {
            _iConfiguration = iConfiguration;
            _driver = GraphDatabase.Driver(_iConfiguration.GetConnectionString("UriConnection"), AuthTokens.Basic(_iConfiguration.GetConnectionString("UserConnection"), _iConfiguration.GetConnectionString("PasswordConnection")));
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }

        [HttpPost("createCar")]
        public IActionResult createCar([FromBody] NodeCar car)
        {
            using(var session = _driver.Session())
            {
                var response = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("CREATE (c:Car) " +
                                        "SET c.idOwner = $car.idOwner , c.nameOwner = $car.nameOwner, c.licenseCar = $car.licenseCar " +
                                        "RETURN id(c)",
                        new { car });
                    int idCar = 0;
                    try
                    {
                        foreach (var r in result)
                        {
                            try
                            {
                                idCar = r["id(c)"].As<int>();
                            }
                            catch (KeyNotFoundException e)
                            {
                                idCar = -1;
                            }
                        }
                    }
                    catch (ClientException e)
                    {
                        idCar = -1;
                    }
                    return idCar;
                });
                if(response >= 0)
                {
                    Dictionary<string, int> resp = new Dictionary<string, int>();
                    resp.Add("idNodeCar", response);
                    return Ok(resp);
                }
                else
                {
                    return Conflict("El auto ya existe");
                }
            }

        }

        [HttpGet("getCars")]
        public IActionResult getCars()
        {
            using(var session = _driver.Session())
            {
                var cars = session.ReadTransaction(tx => tx.Run("MATCH (c:Car) RETURN c").ToList());
                List<NodeCar> list_cars = new List<NodeCar>();
                foreach (var record in cars)
                {
                    var nodeProps = JsonConvert.SerializeObject(record[0].As<INode>().Properties);
                    Debug.WriteLine(nodeProps);
                    list_cars.Add(JsonConvert.DeserializeObject<NodeCar>(nodeProps));
                }

                if(list_cars.Count > 0)
                {
                    return Ok(list_cars);
                }
                else
                {
                    return Conflict("No hay autos");
                }
            }
        }
        
        [HttpPost("getIdCarbyLicense")]
        public IActionResult getIdCarbyLicense([FromBody] NodeCar car)
        {
            using(var session = _driver.Session())
            {
                var cars = session.ReadTransaction(tx => tx.Run("MATCH (c:Car) WHERE c.licenseCar = $car.licenseCar RETURN id(c)", new { car }).ToList());
                int idCar = -1;
                foreach(var r in cars)
                {
                    try
                    {
                        idCar = r["id(c)"].As<int>();
                    }
                    catch(KeyNotFoundException e)
                    {
                        idCar = -1;
                    }
                }

                if(idCar >= 0)
                {
                    Dictionary<string, int> resp = new Dictionary<string, int>();
                    resp.Add("idNodeCar", idCar);
                    return Ok(resp);
                }
                else
                {
                    return Conflict("El auto no existe");
                }
            }
        }

        [HttpPost("getCarbyLicense")]
        public IActionResult getCarbyLicense([FromBody] NodeCar car)
        {
            using(var session = _driver.Session())
            {
                var car_query = session.ReadTransaction(tx => tx.Run("MATCH (c:Car) WHERE c.licenseCar = $car.licenseCar RETURN c", new { car }).ToList());
                List<NodeCar> list_cars = new List<NodeCar>();
                foreach(var record in car_query)
                {
                    var nodeProps = JsonConvert.SerializeObject(record[0].As<INode>().Properties);
                    list_cars.Add(JsonConvert.DeserializeObject<NodeCar>(nodeProps));
                }

                if(list_cars.Count > 0)
                {
                    return Ok(list_cars[0]);
                }
                else
                {
                    return Conflict("El auto no existe");
                }
            }
        }

        [HttpPut("modifyCar")]
        public IActionResult modifyCar([FromBody] NodeCar car)
        {
            String query = "";
            if(car.nameOwner != null) query += "c.nameOwner = $car.nameOwner ,";
            if(car.idOwner != null) query += "c.idOwner = $car.idOwner ,";
            query = query.TrimEnd(',');

            using(var session = _driver.Session())
            {
                var response = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (c:Car) " + "WHERE c.licenseCar = $car.licenseCar SET " +
                                        query +
                                        "RETURN id(c)",
                        new { car });
                    int idCar = -1;
                    try
                    {
                        foreach(var r in result)
                        {
                            try
                            {
                                idCar = r["id(c)"].As<int>();
                            }
                            catch(KeyNotFoundException e)
                            {
                                idCar = -1;
                            }
                        }
                    }
                    catch(ClientException e)
                    {
                        idCar = -1;
                    }
                    return idCar;
                });
                if(response >= 0)
                {
                    return Ok("Auto modificado correctamente");
                }
                else
                {
                    return Conflict("El auto no existe");
                }
            }
            
        }

        [HttpDelete("deleteCar")]
        public IActionResult deleteCar([FromBody] NodeCar car)
        {
            using(var session = _driver.Session())
            {
                var greeting = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (c:Car) " + "WHERE c.licenseCar = $car.licenseCar " +
                                        "DELETE c",
                        new { car });
                    return true;
                });
            }
            return Ok("Auto eliminado correctamente");
        }
        }
       

}
