using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailRocket.API.Mapping;
using RetailRocket.Application.DTOs.Request.Shop;
using RetailRocket.Application.DTOs.Response.Shop;
using RetailRocket.Application.DTOs.Short.Historical;
using RetailRocket.Application.Services.Shop;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.API.Controllers.Shop;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly IMapper _mapper;

    public ProductController(ProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllProductsAsync();
        var result = _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetProductAsync(id);
        if (product is null) return NotFound();
        return Ok(_mapper.Map<ProductResponseDto>(product));
    }

    [AllowAnonymous]
    [HttpGet("by-name")]
    public async Task<IActionResult> GetByName([FromQuery] string name)
    {
        var product = await _productService.GetProductByNameAsync(name);
        if (product is null) return NotFound();
        return Ok(_mapper.Map<ProductResponseDto>(product)); 
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductRequestDto requestDto)
    {
        var product = new Product(requestDto.ItemId, requestDto.Name, requestDto.Price, requestDto.CategoryId);
        await _productService.CreateProductAsync(product);
        return  CreatedAtAction(nameof(GetById), new { id = product.ProductId }, new ProductResponseDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Price = product.Price,
            Category = DtoMapping.MapCategory(product.Item.Category),
            IsAvailable = product.Item?.IsAvailable ?? false
        });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductRequestDto requestDto)
    {
        var product = await _productService.GetProductAsync(id);
        if (product is null) return NotFound();
        product.UpdateItem(requestDto.ItemId);
        product.UpdateName(requestDto.Name);
        product.UpdatePrice(requestDto.Price);
        product.UpdateCategory(requestDto.CategoryId);
        await _productService.UpdateProductAsync(product);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _productService.GetProductAsync(id);
        if (product is null) return NotFound();
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}