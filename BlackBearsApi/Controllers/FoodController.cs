using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BlackBearApi.Model;
using System.Collections.Generic;
using BlackBearsApi.Repositories;

namespace BlackBearApi.Controllers
{
    [Route("api/[controller]")]
    public class FoodController : Controller
    {
        private IEnumerable<Food> _food => _repo.GetItemsFromCollectionAsync().Result;

        IDbCollectionRepository<Food> _repo;
        public FoodController(IDbCollectionRepository<Food> repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_food);
        }

        [HttpGet("{name}", Name = "GetFoodByName")]
        public IActionResult Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest();
            var food = _repo.GetItemFromCollectionAsync(name).Result;
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
            if (_food.Any(f => f.Name == food.Name)) return BadRequest();
            var result = _repo.AddDocumentIntoCollectionAsync(food).Result;
            return CreatedAtRoute("GetFoodByName", new{name=result.Name}, result);
        }

        [HttpPut("{name}")]
        public IActionResult Put(string name, [FromBody] Food food)
        {
            if (string.IsNullOrWhiteSpace(food?.Name)) return BadRequest();
            var oldFood = _food.FirstOrDefault(f => f.Name == food.Name);
            if (oldFood == null) return BadRequest();
            var updated = _repo.UpdateDocumentFromCollection(name, food).Result;
            return CreatedAtRoute("GetFoodByName", new{name=updated.Name}, updated);
        }
        
        [HttpDelete("{name}")]
        public IActionResult Delete(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest();
            var food = _food.FirstOrDefault(b => b.Name == name);
            if (food == null)
            {
                return NotFound();
            }

            _repo.DeleteDocumentFromCollectionAsync(name);
            return NoContent();
        }
    }
}