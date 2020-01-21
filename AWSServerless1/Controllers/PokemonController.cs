using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AWSServerless1.Models;

/*
namespace AWSServerless1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        // GET: api/Pokemon
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Pokemon/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Pokemon
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Pokemon/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
*/


using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AWSServerless1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly IConfiguration _config;

        public PokemonController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection("Server=tcp:kwnss.database.windows.net,1433;Initial Catalog=NSSDatabase;Persist Security Info=False;User ID=gkwilliamson;Password=Yerbamate19$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, PokemonSpecies, Nickname, PokedexId, PictureUrl, KeyCaught, DateCaught FROM Pokemon";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Pokemon> pokemonList = new List<Pokemon>();

                    while (reader.Read())
                    {
                        Pokemon pokemon = new Pokemon
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PokemonSpecies = reader.GetString(reader.GetOrdinal("PokemonSpecies")),
                            Nickname = reader.GetString(reader.GetOrdinal("Nickname")),
                            PokedexId = reader.GetInt32(reader.GetOrdinal("PokedexId")),
                            PictureUrl = reader.GetString(reader.GetOrdinal("PictureUrl")),
                            KeyCaught = reader.GetInt32(reader.GetOrdinal("KeyCaught")),
                            DateCaught = reader.GetDateTime(reader.GetOrdinal("DateCaught")),
                        };

                        pokemonList.Add(pokemon);
                    }
                    reader.Close();
                    Response.Headers.Add("X-Requested-With", "*");
                    Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type,X-Amz-Date,Authorization,X-Api-Key,x-requested-with");
                    Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,OPTIONS");
                   // Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                    return Ok(pokemonList);
                }
            }
        }

        /*

        [HttpGet("{id}", Name = "GetCoffee")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, Title, BeanType
                        FROM Coffee
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Coffee coffee = null;

                    if (reader.Read())
                    {
                        coffee = new Coffee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            BeanType = reader.GetString(reader.GetOrdinal("BeanType"))
                        };
                    }
                    reader.Close();

                    return Ok(coffee);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Coffee coffee)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Coffee (Title, BeanType)
                                        OUTPUT INSERTED.Id
                                        VALUES (@title, @beanType)";
                    cmd.Parameters.Add(new SqlParameter("@title", coffee.Title));
                    cmd.Parameters.Add(new SqlParameter("@beanType", coffee.BeanType));

                    int newId = (int)await cmd.ExecuteScalarAsync();
                    coffee.Id = newId;
                    return CreatedAtRoute("GetCoffee", new { id = newId }, coffee);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Coffee coffee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Coffee
                                            SET Title = @title,
                                                BeanType = @beanType
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@title", coffee.Title));
                        cmd.Parameters.Add(new SqlParameter("@beanType", coffee.BeanType));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!CoffeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Coffee WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!CoffeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CoffeeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Title, BeanType
                        FROM Coffee
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }

    */

    }
}
