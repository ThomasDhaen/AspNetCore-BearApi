using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using BlackBearApi.Model;
using BlackBearsApi.Repositories;

namespace BlackBearApi.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]
    public class BearsController : Controller
    {
        readonly IDbCollectionRepository<Bear> _repo;
        readonly IDbCollectionRepository<Food> _foodRepo;
        public BearsController(IDbCollectionRepository<Bear> repo, IDbCollectionRepository<Food> foodRepo)
        {
            _repo = repo;
            _foodRepo = foodRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var bears = await _repo.GetItemsFromCollectionAsync();
            return Ok(bears);
        }

        [HttpGet("{name}", Name = "GetBearByName")]
        public async Task<IActionResult> Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest();
            var bear = await _repo.GetItemFromCollectionAsync(name);
            if (bear == null)
            {
                return NotFound();
            }
            return Ok(bear);
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Bear bear)
        {
            if (string.IsNullOrWhiteSpace(bear?.Name)) return BadRequest();
            var bears = await _repo.GetItemsFromCollectionAsync();
            if (bears.Any(b => b.Name == bear.Name)) return BadRequest();
            var result = await _repo.AddDocumentIntoCollectionAsync(bear);
            return CreatedAtRoute("GetBearByName", new{name=result.Name}, result);
        }

        [HttpPut("{name}")]
        public async Task<IActionResult> Put(string name, [FromBody] Bear bear)
        {
            if (string.IsNullOrWhiteSpace(bear?.Name)) return BadRequest();
            var bears = await _repo.GetItemsFromCollectionAsync();
            var oldBear = bears.FirstOrDefault(b => b.Name == name);
            if (oldBear == null) return BadRequest();
            var updated = await _repo.UpdateDocumentFromCollection(name, bear);
            return Ok(updated);
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest();
            var bears = await _repo.GetItemsFromCollectionAsync();
            var bear = bears.FirstOrDefault(b => b.Name == name);
            if (bear == null)
            {
                return NotFound();
            }

            await _repo.DeleteDocumentFromCollectionAsync(name);
            return Ok();
        }

        [HttpGet("{bearName}/eat/{foodName}")]
        public async Task<IActionResult> Eat(string bearName, string foodName)
        {
            if (string.IsNullOrWhiteSpace(bearName) || string.IsNullOrWhiteSpace(foodName)) return BadRequest();
            var bears = await _repo.GetItemsFromCollectionAsync();
            var bear = bears.FirstOrDefault(b => b.Name == bearName);
            if (bear == null) return NotFound();
            var foodList = await _foodRepo.GetItemsFromCollectionAsync();
            var food = foodList.FirstOrDefault(f => f.Name == foodName);
            if (food == null) return NotFound();
            bear.Weight += food.KCal;
            var updated = await _repo.UpdateDocumentFromCollection(bearName, bear);
            return Ok(updated);
        }
        
        [HttpGet("{bearName}/goToToilet/{operation}")]
        public async Task<IActionResult> GoToToilet(string bearName, ToiletOperation operation)
        {
            if (string.IsNullOrWhiteSpace(bearName)) return BadRequest();
            var bears = await _repo.GetItemsFromCollectionAsync();
            var bear = bears.FirstOrDefault(b => b.Name == bearName);
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
                await _repo.DeleteDocumentFromCollectionAsync(bearName);
                return Ok($"{bear.Name} died with a weight of {bear.Weight}");
            }
            var updated = await _repo.UpdateDocumentFromCollection(bearName, bear);
            return Ok(updated);
        }
    }
}