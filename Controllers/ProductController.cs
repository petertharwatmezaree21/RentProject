using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyProject.Data;
using MyProject.Models;

namespace MyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext Context;
        public ProductController(ApplicationDbContext _Context)
        {
            Context = _Context;
        }
        [HttpGet("[action]")]
        [Authorize(Policy = "RequiredLoggedIn")]
        public IActionResult GetProducts()
        {

            return Ok(Context.Products.ToList());
        }

        [HttpPost("[action]")]
        [Authorize(Policy = "RequiredAdmin")]
        public async Task<IActionResult> AddProduct([FromBody] Product newProduct)
        {
            var product = new Product()
            {
                Description = newProduct.Description,
                ImageUrl = newProduct.ImageUrl,
                Name = newProduct.Name,
                OutofStock = newProduct.OutofStock,
                Price = newProduct.Price,

            };

            await Context.Products.AddAsync(product);
            await Context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]/{id}")]
        [Authorize(Policy = "RequiredAdmin")]
        public async Task<ActionResult> UpdateProduct([FromRoute] int id, [FromBody] Product formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var product = Context.Products.FirstOrDefault(x => x.ProdunctId == id);
            if (product == null)
                return NotFound();

            product.Description = formData.Description;
            product.ImageUrl = formData.ImageUrl;
            product.Name = formData.Name;
            product.OutofStock = formData.OutofStock;
            product.Price = formData.Price;

            Context.Entry(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await Context.SaveChangesAsync();

            return Ok(new JsonResult("The Product with Id: " + id + "Updated"));
        }


        [HttpDelete("[action]/{id}")]
        [Authorize(Policy = "RequiredAdmin")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = Context.Products.FirstOrDefault(x => x.ProdunctId == id);
            if (product == null)
                return NotFound();

            Context.Products.Remove(product);
            await Context.SaveChangesAsync();
            return Ok(new JsonResult("The Product with the id:"+id+"is deleted successfully"));
        }
    }
}