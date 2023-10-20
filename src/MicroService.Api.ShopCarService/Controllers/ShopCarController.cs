using MicroService.Api.ShopCarService.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MicroService.Api.ShopCarService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShopCarController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<ShopCarController> _logger;

        public ShopCarController(ILogger<ShopCarController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new Product
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}