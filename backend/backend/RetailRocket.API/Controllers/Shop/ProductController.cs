using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailRocket.Application.DTOs.Request.Shop.Products;
using RetailRocket.Application.DTOs.Response.Shop.Products;
using RetailRocket.Application.Services.Shop;
using RetailRocket.Domain.Entities.Shop;
using RetailRocket.Domain.Exceptions;

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
        if (id == Guid.Empty)
            throw new ValidationException("Product ID is required.");
        
        var product = await _productService.GetProductAsync(id);
        if (product is null) 
            throw new NotFoundException("Product not found.");
        return Ok(_mapper.Map<ProductResponseDto>(product));
    }

    [AllowAnonymous]
    [HttpGet("name")]
    public async Task<IActionResult> GetByName([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Product's name is required.");
        
        var product = await _productService.GetProductByNameAsync(name);
        if (product is null)
            throw new NotFoundException($"Product {name} not found.");
        return Ok(_mapper.Map<ProductResponseDto>(product)); 
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateRequestDto createRequestDto)
    {
        // we need to create new services for items and categories if we want to keep in check product entities checks
        if (createRequestDto.ItemId < 1)
            throw new ValidationException("Item ID must be greater than zero.");
        if (string.IsNullOrWhiteSpace(createRequestDto.Name))
            throw new ValidationException("Product's name is required.");
        if (createRequestDto.Price <= 0)
            throw new ValidationException("Price must be greater than zero.");
        if (createRequestDto.CategoryId < 1)
            throw new ValidationException("Category ID must be greater than zero.");
        
        var product = new Product(createRequestDto.ItemId, createRequestDto.Name, createRequestDto.Price, createRequestDto.CategoryId);
        await _productService.CreateProductAsync(product);
        
        var created = await _productService.GetProductAsync(product.ProductId);
        return CreatedAtAction(nameof(GetById), new { id = created!.ProductId },
            _mapper.Map<ProductResponseDto>(created));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateRequestDto updateRequestDto)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Product ID is required.");
        
        var product = await _productService.GetProductAsync(id);
        if (product is null)
            throw new NotFoundException("Product not found.");
        
        // we need to create new services for items and categories if we want to keep in check product entities checks
        if (updateRequestDto.ItemId < 1)
            throw new ValidationException("Item ID must be greater than zero.");
        product.UpdateItem(updateRequestDto.ItemId);
        if (string.IsNullOrWhiteSpace(updateRequestDto.Name))
            throw new ValidationException("Product's name is required.");
        product.UpdateName(updateRequestDto.Name);
        if (updateRequestDto.Price <= 0)
            throw new ValidationException("Price must be greater than zero.");
        product.UpdatePrice(updateRequestDto.Price);
        if (updateRequestDto.CategoryId < 1)
            throw new ValidationException("Category ID must be greater than zero.");
        product.UpdateCategory(updateRequestDto.CategoryId);
        
        await _productService.UpdateProductAsync(product);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Product ID is required.");
        
        var product = await _productService.GetProductAsync(id);
        if (product is null)
            throw new NotFoundException("Product not found.");
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}