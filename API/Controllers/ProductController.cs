using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ProductDto_API.Models.Product;
using Product_API.Models;
using AutoMapper;
using Product_API.Repository.IRepository;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Product_API.Models.Specifications;

namespace Product_API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]

  public class ProductController : ControllerBase
  {
    private readonly ILogger<ProductController> _logger;
    private readonly IProductRepository _productRepository;
    private readonly IKindOfProductRepository _kindOfProductRepository;
    private readonly IMapper _mapper;
    protected ApiResponse _response;

    public ProductController(
      ILogger<ProductController> logger,
      IProductRepository productRepository,
      IKindOfProductRepository kindOfProductRepository,
      IMapper mapper)
    {
      _logger = logger;
      _productRepository = productRepository;
      _kindOfProductRepository = kindOfProductRepository;
      _mapper = mapper;
      _response = new();
    }

    // Get all records =============================================================
    [HttpGet]
    // [ResponseCache(Duration = 30)]
    [ResponseCache(CacheProfileName = "Default30")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetProducts()
    {
      try
      {
        _logger.LogInformation("Obtener datos");
        IEnumerable<Product> productList = await _productRepository.GetAll();
        _response.statusCode = HttpStatusCode.OK;
        _response.Result = _mapper.Map<IEnumerable<ProductDto>>(productList);
        return Ok(_response);
      }
      catch (Exception e)
      {
        _response.isSuccessful = false;
        _response.ErrorMessages = new List<string>() { e.ToString() };
      }
      return _response;
    }

    // Get all records paginated=============================================================
    [HttpGet("PaginatedProducts")]
    // [ResponseCache(Duration = 30)]
    [ResponseCache(CacheProfileName = "Default30")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<ApiResponse> GetPaginatedProducts([FromQuery] Params param)
    {
      try
      {
        var productList = _productRepository.GetAllPaginated(param);
        _response.statusCode = HttpStatusCode.OK;
        _response.Result = _mapper.Map<IEnumerable<ProductDto>>(productList);
        _response.TotalPages = productList.MetaData.TotalPage;
        return Ok(_response);
      }
      catch (Exception e)
      {
        _response.isSuccessful = false;
        _response.ErrorMessages = new List<string>() { e.ToString() };
      }
      return _response;
    }

    // Get One record by id ==========================================================
    [HttpGet("id", Name = "GetProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetProduct(int id)
    {
      try
      {
        if (id == 0)
        {
          _logger.LogError("Error id = 0");
          _response.statusCode = HttpStatusCode.BadRequest;
          _response.isSuccessful = false;
          return BadRequest(_response);
        }
        var product = await _productRepository.Get(record => record.Id == id);
        if (product == null)
        {
          _response.statusCode = HttpStatusCode.NotFound;
          _response.isSuccessful = false;
          return NotFound(_response);
        }
        _response.statusCode = HttpStatusCode.OK;
        _response.Result = _mapper.Map<ProductDto>(product);
        return Ok(_response);
      }
      catch (Exception e)
      {
        _response.isSuccessful = false;
        _response.ErrorMessages = new List<string>() { e.ToString() };
      }
      return _response;
    }

    // Create one record ==============================================================
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> CreateProduct([FromBody] ProductCreateDto createDto)
    {
      try
      {
        // Validate model state
        if (!ModelState.IsValid) return BadRequest(ModelState);
        // Validate if record already exists
        if (await _productRepository.Get(record => record.Name.ToLower() == createDto.Name.ToLower()) != null)
        {
          ModelState.AddModelError("NameExists", "A record already exists with the same name");
          return BadRequest(ModelState);
        }
        if (await _kindOfProductRepository.Get(record => record.KindOfProductId == createDto.KindOfProductId) == null)
        {
          ModelState.AddModelError("Id error", "Id of kind od product doesn't exists");
          return BadRequest(ModelState);
        }
        if (createDto == null)
        {
          return BadRequest(createDto);
        }
        // Cearte new model
        Product model = _mapper.Map<Product>(createDto);
        model.Create = DateTime.Now;
        model.Update = DateTime.Now;
        // Insert data into db
        await _productRepository.Create(model);
        _response.statusCode = HttpStatusCode.OK;
        _response.Result = model;
        return CreatedAtRoute("GetProduct", new { id = model.Id }, _response);
      }
      catch (Exception e)
      {
        _response.isSuccessful = false;
        _response.ErrorMessages = new List<string>() { e.ToString() };
      }
      return _response;
    }

    // Delete record =============================================================
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
      try
      {
        if (id == 0)
        {
          _response.isSuccessful = false;
          _response.statusCode = HttpStatusCode.BadRequest;
          return BadRequest(_response);
        }
        var product = await _productRepository.Get(record => record.Id == id);
        if (product == null)
        {
          _response.isSuccessful = false;
          _response.statusCode = HttpStatusCode.NotFound;
          return NotFound(_response);
        }
        await _productRepository.Remove(product);
        _response.statusCode = HttpStatusCode.NoContent;
        return Ok(_response);
      }
      catch (Exception e)
      {
        _response.isSuccessful = false;
        _response.ErrorMessages = new List<string>() { e.ToString() };
      }
      return BadRequest(_response);
    }

    // Update record - PUT ====================================================
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto updateDto)
    {
      try
      {
        if (updateDto == null || id != updateDto.Id)
        {
          _response.isSuccessful = false;
          _response.statusCode = HttpStatusCode.BadRequest;
          return BadRequest(_response);
        }
        Product model = _mapper.Map<Product>(updateDto);
        await _productRepository.Update(model);
        _response.statusCode = HttpStatusCode.NoContent;
        return Ok(_response);
      }
      catch (Exception e)
      {
        _response.isSuccessful = false;
        _response.ErrorMessages = new List<string>() { e.ToString() };
      }
      return BadRequest(_response);
    }

    // Update record - PATCH ====================================================
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePartialProduct(int id, JsonPatchDocument<ProductUpdateDto> patchDto)
    {
      try
      {
        if (patchDto == null || id == 0)
        {
          _response.isSuccessful = false;
          _response.statusCode = HttpStatusCode.BadRequest;
          return BadRequest(_response);
        }
        var product = await _productRepository.Get(record => record.Id == id, tracked: false);
        if (product == null)
        {
          _response.isSuccessful = false;
          _response.statusCode = HttpStatusCode.NotFound;
          return NotFound(_response);
        }
        ProductUpdateDto productDto = _mapper.Map<ProductUpdateDto>(product);
        patchDto.ApplyTo(productDto, ModelState);

        if (!ModelState.IsValid)
        {
          _response.isSuccessful = false;
          _response.statusCode = HttpStatusCode.BadRequest;
          _response.Result = ModelState;
          return BadRequest(_response);
        }
        Product model = _mapper.Map<Product>(productDto);
        _response.statusCode = HttpStatusCode.NoContent;
        await _productRepository.Update(model);
        return Ok(_response);
      }
      catch (Exception e)
      {
        _response.isSuccessful = false;
        _response.ErrorMessages = new List<string>() { e.ToString() };
      }
      return BadRequest(_response);
    }
  }
}
