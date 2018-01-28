using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BlackBearApi.Model;
using BlackBearApi.Services;

namespace BlackBearApi.Controllers
{
    [Route("api/[controller]")]
    public class BearsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(BearService.Instance.Bears);
        }

        [HttpGet("{name}", Name = "GetBearByName")]
        public IActionResult Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest();
            var bear = BearService.Instance.Bears.FirstOrDefault(b => b.Name == name);
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
            if (BearService.Instance.Bears.Any(b => b.Name == bear.Name)) return BadRequest();
            BearService.Instance.Bears.Add(bear);
            return CreatedAtRoute("GetBearByName", new{name=bear.Name}, bear);
        }
        
        [HttpDelete("{name}")]
        public IActionResult Delete(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest();
            var bear = BearService.Instance.Bears.FirstOrDefault(b => b.Name == name);
            if (bear == null)
            {
                return NotFound();
            }

            BearService.Instance.Bears.Remove(bear);
            return NoContent();
        }

        [HttpPost("eat/{bearName}/{foodName}")]
        public IActionResult Eat(string bearName, string foodName)
        {
            if (string.IsNullOrWhiteSpace(bearName) || string.IsNullOrWhiteSpace(foodName)) return BadRequest();
            var bear = BearService.Instance.Bears.FirstOrDefault(b => b.Name == bearName);
            if (bear == null) return NotFound();
            var food = FoodService.Instance.Food.FirstOrDefault(f => f.Name == foodName);
            if (food == null) return NotFound();
            bear.Weight += food.KCal;
            return Ok(bear);
        }
        
        [HttpPost("goToToilet/{bearName}")]
        public IActionResult GoToToilet(string bearName,[FromBody] ToiletOperation operation)
        {
            if (string.IsNullOrWhiteSpace(bearName)) return BadRequest();
            var bear = BearService.Instance.Bears.FirstOrDefault(b => b.Name == bearName);
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
                BearService.Instance.Bears.Remove(bear);
                return Ok($"Your bear died with a weight of {bear.Weight}");
            }
            return Ok(bear);
        }
    }
}