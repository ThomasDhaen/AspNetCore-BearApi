using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BlackBearApi.Model;
using System.Collections.Generic;
using BlackBearsApi.Repositories;
using System.Threading.Tasks;

namespace BlackBearApi.Controllers
{
    [Route("api/[controller]")]
    public class FoodController : Controller
    {
        readonly IDbCollectionRepository<Food> _repo;
        public FoodController(IDbCollectionRepository<Food> repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var foodList = await _repo.GetItemsFromCollectionAsync();
            return Ok(foodList);
        }

        [HttpGet("{name}", Name = "GetFoodByName")]
        public async Task<IActionResult> Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest();
            var food = await _repo.GetItemFromCollectionAsync(name);
            if (food == null)
            {
                return NotFound();
            }
            return Ok(food);
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Food food)
        {
            if (string.IsNullOrWhiteSpace(food?.Name)) return BadRequest();
            var foodList = await _repo.GetItemsFromCollectionAsync();
            if (foodList.Any(f => f.Name == food.Name)) return BadRequest();
            var result = await _repo.AddDocumentIntoCollectionAsync(food);
            return CreatedAtRoute("GetFoodByName", new{name=result.Name}, result);
        }

        [HttpPut("{name}")]
        public async Task<IActionResult> Put(string name, [FromBody] Food food)
        {
            if (string.IsNullOrWhiteSpace(food?.Name)) return BadRequest();
            var foodList = await _repo.GetItemsFromCollectionAsync();
            var oldFood = foodList.FirstOrDefault(f => f.Name == food.Name);
            if (oldFood == null) return BadRequest();
            var updated = await _repo.UpdateDocumentFromCollection(name, food);
            return Ok(updated);
        }
        
        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest();
            var foodList = await _repo.GetItemsFromCollectionAsync();
            var food = foodList.FirstOrDefault(b => b.Name == name);
            if (food == null)
            {
                return NotFound();
            }

            await _repo.DeleteDocumentFromCollectionAsync(name);
            return Ok();
        }
    }
}