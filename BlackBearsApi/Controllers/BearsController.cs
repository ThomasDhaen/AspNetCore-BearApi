using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using BlackBearApi.Model;
using BlackBearApi.Services;
using BlackBearsApi.Repositories;

namespace BlackBearApi.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]
    public class BearsController : Controller
    {
        private IEnumerable<Bear> _bears => _repo.GetItemsFromCollectionAsync().Result;
        private IEnumerable<Food> _food => _foodRepo.GetItemsFromCollectionAsync().Result;

        IDbCollectionRepository<Bear> _repo;
        IDbCollectionRepository<Food> _foodRepo;
        public BearsController(IDbCollectionRepository<Bear> repo, IDbCollectionRepository<Food> foodRepo)
        {
            _repo = repo;
            _foodRepo = foodRepo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var bears = _bears;
            return Ok(bears);
        }

        [HttpGet("{name}", Name = "GetBearByName")]
        public IActionResult Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest();
            var bear = _repo.GetItemFromCollectionAsync(name).Result;
            if (bear == null)
            {
                return NotFound();
            }
            return Ok(bear);
        }
        
        [HttpPost]
        public IActionResult Post([FromBody] Bear bear)
        {
            if (string.IsNullOrWhiteSpace(bear?.Name)) return BadRequest();
            
            if (_bears.Any(b => b.Name == bear.Name)) return BadRequest();
            var result = _repo.AddDocumentIntoCollectionAsync(bear).Result;
            return CreatedAtRoute("GetBearByName", new{name=result.Name}, result);
        }

        [HttpPut("{name}")]
        public IActionResult Put(string name, [FromBody] Bear bear)
        {
            if (string.IsNullOrWhiteSpace(bear?.Name)) return BadRequest();
            var oldBear =_bears.FirstOrDefault(b => b.Name == name);
            if (oldBear == null) return BadRequest();
            var updated = _repo.UpdateDocumentFromCollection(name, bear).Result;
            return CreatedAtRoute("GetBearByName", new { name = updated.Name }, updated);
        }

        [HttpDelete("{name}")]
        public IActionResult Delete(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest();
            var bear = _bears.FirstOrDefault(b => b.Name == name);
            if (bear == null)
            {
                return NotFound();
            }

            _repo.DeleteDocumentFromCollectionAsync(name);
            return NoContent();
        }

        [HttpPost("eat/{bearName}/{foodName}")]
        public IActionResult Eat(string bearName, string foodName)
        {
            if (string.IsNullOrWhiteSpace(bearName) || string.IsNullOrWhiteSpace(foodName)) return BadRequest();
            var bear = _bears.FirstOrDefault(b => b.Name == bearName);
            if (bear == null) return NotFound();
            var food = _food.FirstOrDefault(f => f.Name == foodName);
            if (food == null) return NotFound();
            bear.Weight += food.KCal;
            return Ok(bear);
        }
        
        [HttpPost("goToToilet/{bearName}")]
        public IActionResult GoToToilet(string bearName,[FromBody] ToiletOperation operation)
        {
            if (string.IsNullOrWhiteSpace(bearName)) return BadRequest();
            var bear = _bears.FirstOrDefault(b => b.Name == bearName);
            if (bear == null) return NotFound();
            switch (operation)
            {
                case ToiletOperation.Pee:
                    bear.Weight -= 5;
                    break;
                case ToiletOperation.Poo:
                    bear.Weight -= 20;
                    break;
                default:
                    return BadRequest();
            }

            if (bear.Weight < 0)
            {
                _repo.DeleteDocumentFromCollectionAsync(bearName);
                return Ok($"Your bear died with a weight of {bear.Weight}");
            }
            return Ok(bear);
        }
    }
}