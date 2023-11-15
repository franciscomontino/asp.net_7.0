using Product_API.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ProductDto_API.Models.Product;
using Product_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Product_API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]

  public class ProductController : ControllerBase
  {
    // Implementar logger
    private readonly ILogger<ProductController> _logger;
    private readonly AppDbContext _db;

    public ProductController(ILogger<ProductController> logger, AppDbContext db)
    {
      _logger = logger;
      _db = db;
    }

    // Get all records
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<ProductDto>> GetProducts()
    {
      _logger.LogInformation("Obtener datos");
      return Ok(_db.Products.ToList());
    }

    // Get One record by id
    [HttpGet("id", Name = "GetProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProductDto> GetProduct(int id)
    {
      if (id == 0)
      {
        _logger.LogError("Error id = 0");
        return BadRequest();
      }

      var data = _db.Products.FirstOrDefault(record => record.Id == id);

      if (data == null) return NotFound();

      return Ok(data);
    }

    // Create one record
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<ProductDto> CreateProduct([FromBody] ProductDto productDto)
    {
      // Validate model state
      if (!ModelState.IsValid) return BadRequest(ModelState);

      // Validate if record already exists
      if (ProductStore.productList.FirstOrDefault(record => record.Name.ToLower() == productDto.Name.ToLower()) != null)
      {
        ModelState.AddModelError("NameExists", "A record already exists with the same name");
      }

      if (productDto == null) return BadRequest(productDto);

      if (productDto.Id > 0) return StatusCode(StatusCodes.Status500InternalServerError);

      // Cearte new model
      Product model = new()
      {
        Name = productDto.Name,
        Detail = productDto.Detail,
        Price = productDto.Price,
        ImageUrl = productDto.ImageUrl,
        Create = DateTime.Now,
        Update = DateTime.Now,
      };

      // Insert data into db
      _db.Products.Add(model);
      _db.SaveChanges();

      return CreatedAtRoute("GetProduct", new { id = productDto.Id }, productDto);
    }

    // Delete record
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteProduct(int id)
    {
      if (id == 0) return BadRequest();

      var product = _db.Products.FirstOrDefault(record => record.Id == id);
      
      if (product == null) return NotFound();

      _db.Products.Remove(product);
      _db.SaveChanges();

      return NoContent();
    }

    // Update record - PUT
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateProduct(int id, [FromBody] ProductDto productDto)
    {
      if (productDto == null || id != productDto.Id) return BadRequest();

      Product model = new()
      {
        Id = productDto.Id,
        Name = productDto.Name,
        Detail = productDto.Detail,
        Price = productDto.Price,
        ImageUrl = productDto.ImageUrl,
        Update = DateTime.Now,
      };

      _db.Products.Update(model);
      _db.SaveChanges();

      return NoContent();
    }

    // Update record - PATCH
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdatePartialProduct(int id, JsonPatchDocument<ProductDto> patchDto)
    {
      if (patchDto == null || id == 0) return BadRequest();

      var product = _db.Products.AsNoTracking().FirstOrDefault(record => record.Id == id);

      if (product == null) return BadRequest();

      ProductDto productDto = new()
      {
        Id = product.Id,
        Name = product.Name,
        Detail = product.Detail,
        Price = product.Price,
        ImageUrl = product.ImageUrl,
      };

      patchDto.ApplyTo(productDto, ModelState);

      if (!ModelState.IsValid) return BadRequest(ModelState);

      Product model = new()
      {
        Id = productDto.Id,
        Name = productDto.Name,
        Detail = productDto.Detail,
        Price = productDto.Price,
        ImageUrl = productDto.ImageUrl,
        Update = DateTime.Now,
      };

      _db.Products.Update(model);
      _db.SaveChanges();

      return NoContent();
    }
  }
}
