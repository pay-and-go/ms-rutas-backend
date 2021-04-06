using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using ms_rutas_backend.Data.Models;
using Neo4j.Driver;

namespace ms_rutas_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodeCarController : ControllerBase
    {
        private IDriver _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "pass"));
        
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
                if(cars.Count > 0)
                {
                    return Ok(cars);
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
                int idCar = 0;
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
                var cars = session.ReadTransaction(tx => tx.Run("MATCH (c:Car) WHERE c.licenseCar = $car.licenseCar RETURN c", new { car }).ToList());
                if(cars.Count > 0)
                {
                    return Ok(cars);
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
                var greeting = session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MATCH (c:Car) " + "WHERE c.licenseCar = $car.licenseCar SET " +
                                        query +
                                        "RETURN id(c)",
                        new { car });
                    return 0;
                });
            }
            return Ok("Auto modificado correctamente");
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



    }


}
