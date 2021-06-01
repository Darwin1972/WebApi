using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            var groupList = new List<string>();
            Console.WriteLine("----- HttpGet ----------------");

            /*
            var userClaims = HttpContext.User.Claims;

            foreach (var claim in userClaims)
            {
                Console.WriteLine($"{claim.Type}\t{claim.Value}");
            }*/

            ClaimsPrincipal principal = HttpContext.User as ClaimsPrincipal;
            if (null != principal)
            {
                

                foreach (Claim claim in principal.Claims)
                {
                    Console.WriteLine("TYPE: " + claim.Type + "; VALUE: " + claim.Value );
                    if(claim.Type == "groups")
                    {
                        groupList.Add(claim.Value);                        
                    }
                }
            }


            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                Group = groupList[0]
            })
            .ToArray();
            
        }
    }
}
