using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BlackBearApi.Model;
using BlackBearApi.Services;

namespace BlackBearApi.Controllers
{
    [Route("api/[controller]")]
    public class FoodController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(FoodService.Instance.Food);
        }

        [HttpGet("{name}", Name = "GetFoodByName")]
        public IActionResult Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest();
            var food = FoodService.Instance.Food.FirstOrDefault(b => b.Name == name);
            if (food == null)
            {
                return NotFound();
            }
            return Ok(food);
        }
        
        [HttpPost]
        public IActionResult Post([FromBody] Food food)
        {
            if (string.IsNullOrWhiteSpace(food?.Name)) return BadRequest();
            if (FoodService.Instance.Food.Any(b => b.Name == food.Name)) return BadRequest();
            FoodService.Instance.Food.Add(food);
            return CreatedAtRoute("GetFoodByName", new{name=food.Name}, food);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Food food)
        {
            if (string.IsNullOrWhiteSpace(food?.Name)) return BadRequest();
            var oldFood = FoodService.Instance.Food.FirstOrDefault(f => f.Name == food.Name);
            if (oldFood == null) return BadRequest();
            FoodService.Instance.Food.Remove(oldFood);
            FoodService.Instance.Food.Add(food);
            return CreatedAtRoute("GetFoodByName", new{name=food.Name}, food);
        }
        
        [HttpDelete("{name}")]
        public IActionResult Delete(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest();
            var food = FoodService.Instance.Food.FirstOrDefault(b => b.Name == name);
            if (food == null)
            {
                return NotFound();
            }

            FoodService.Instance.Food.Remove(food);
            return NoContent();
        }
    }
}