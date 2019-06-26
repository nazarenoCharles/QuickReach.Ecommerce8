using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickReach.ECommerce.Domain;
using QuickReach.ECommerce.Domain.Models;
using QuickReach.ECommerce.Infra.Data;
using QuickReach.ECommerce.Infra.Data.Repositories;

namespace QuickReach.ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository repository;
        public CategoriesController(ICategoryRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public ActionResult Get(string search = "" ,int skip = 0, int count = 10)
        {
            var categories = repository.Retrieve(search, skip, count);
            return Ok(categories);

        }
        [HttpGet("{id}")]
        public ActionResult GetCategoryWithProducts(int id)
        {
            var category = repository.Retrieve(id);
            return Ok(category);

        }
        [HttpPost]
        public IActionResult Post([FromBody] Category newCategory)
        {
            if(!this.ModelState.IsValid)
            {
                return BadRequest(this);
            }
            this.repository.Create(newCategory);
            return CreatedAtAction(nameof(this.Get), new { id = newCategory.ID }, newCategory);
        }
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Category category)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            this.repository.Update(id, category);
            return Ok(category);
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            this.repository.Delete(id);
            return Ok();
        }


    }

}