using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ProductDto_API.Models.Product;
using ProductDto_API.Models.KindOfProduct;
using Product_API.Models;
using AutoMapper;
using Product_API.Repository.IRepository;
using System.Net;

namespace Product_API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]

  public class KindOfProductController : ControllerBase
  {
    // Implementar logger
    private readonly ILogger<KindOfProductController> _logger;
    private readonly IKindOfProductRepository _kindOfProductRepository;
    private readonly IMapper _mapper;
    protected ApiResponse _response;

    public KindOfProductController(ILogger<KindOfProductController> logger, IKindOfProductRepository kindOfProductRepository, IMapper mapper)
    {
      _logger = logger;
      _kindOfProductRepository = kindOfProductRepository;
      _mapper = mapper;
      _response = new();
    }

    // Get all records =============================================================
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetKindOfProducts()
    {
      try
      {
        _logger.LogInformation("Get data");
        IEnumerable<KindOfProduct> kindOfProductList = await _kindOfProductRepository.GetAll();
        _response.statusCode = HttpStatusCode.OK;
        _response.Result = _mapper.Map<IEnumerable<KindOfProductDto>>(kindOfProductList);
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
    [HttpGet("id", Name = "GetKindOfProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetKindOfProduct(int id)
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
        var kindOfProduct = await _kindOfProductRepository.Get(record => record.KindOfProductId == id);
        if (kindOfProduct == null)
        {
          _response.statusCode = HttpStatusCode.NotFound;
          _response.isSuccessful = false;
          return NotFound(_response);
        }
        _response.statusCode = HttpStatusCode.OK;
        _response.Result = _mapper.Map<ProductDto>(kindOfProduct);
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
    public async Task<ActionResult<ApiResponse>> CreateKindOfProduct([FromBody] KindOfProductCreateDto createDto)
    {
      try
      {
        // Validate model state
        if (!ModelState.IsValid) return BadRequest(ModelState);
        // Validate if record already exists
        if (await _kindOfProductRepository.Get(record => record.Name.ToLower() == createDto.Name.ToLower()) != null)
        {
          ModelState.AddModelError("NameExists", "A record already exists with the same name");
          _response.statusCode = HttpStatusCode.BadRequest;
          _response.isSuccessful = false;
          _response.Result = ModelState;
          return BadRequest(_response);
        }
        if (createDto == null){
          _response.statusCode = HttpStatusCode.BadRequest;
          _response.isSuccessful = false;
          return BadRequest(_response);
        }
        // Cearte new model
        KindOfProduct model = _mapper.Map<KindOfProduct>(createDto);
        model.Create = DateTime.Now;
        model.Update = DateTime.Now;
        // Insert data into db
        await _kindOfProductRepository.Create(model);
        _response.statusCode = HttpStatusCode.OK;
        _response.Result = model;
        return CreatedAtRoute("GetKindOfProduct", new { id = model.KindOfProductId }, _response);
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
    public async Task<IActionResult> DeleteKindOfProduct(int id)
    {
      try
      {
        if (id == 0)
        {
          _response.isSuccessful = false;
          _response.statusCode = HttpStatusCode.BadRequest;
          return BadRequest(_response);
        }
        var kindOfProduct = await _kindOfProductRepository.Get(record => record.KindOfProductId == id);
        if (kindOfProduct == null)
        {
          _response.isSuccessful = false;
          _response.statusCode = HttpStatusCode.NotFound;
          return NotFound(_response);
        }
        await _kindOfProductRepository.Remove(kindOfProduct);
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
    public async Task<IActionResult> UpdateKindOfProduct(int id, [FromBody] KindOfProductUpdateDto updateDto)
    {
      try
      {
        if (updateDto == null || id != updateDto.KindOfProductId)
        {
          _response.isSuccessful = false;
          _response.statusCode = HttpStatusCode.BadRequest;
          return BadRequest(_response);
        }
        KindOfProduct model = _mapper.Map<KindOfProduct>(updateDto);
        await _kindOfProductRepository.Update(model);
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
    public async Task<IActionResult> UpdatePartialKindOfProduct(int id, JsonPatchDocument<KindOfProductUpdateDto> patchDto)
    {
      try
      {
        if (patchDto == null || id == 0)
        {
          _response.isSuccessful = false;
          _response.statusCode = HttpStatusCode.BadRequest;
          return BadRequest(_response);
        }
        var kindOfProduct = await _kindOfProductRepository.Get(record => record.KindOfProductId == id, tracked: false);
        if (kindOfProduct == null)
        {
          _response.isSuccessful = false;
          _response.statusCode = HttpStatusCode.NotFound;
          return NotFound(_response);
        }
        KindOfProductUpdateDto kindOfProductDto = _mapper.Map<KindOfProductUpdateDto>(kindOfProduct);
        patchDto.ApplyTo(kindOfProductDto, ModelState);

        if (!ModelState.IsValid)
        {
          _response.isSuccessful = false;
          _response.statusCode = HttpStatusCode.BadRequest;
          _response.Result = ModelState;
          return BadRequest(_response);
        }
        KindOfProduct model = _mapper.Map<KindOfProduct>(kindOfProductDto);
        _response.statusCode = HttpStatusCode.NoContent;
        await _kindOfProductRepository.Update(model);
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
