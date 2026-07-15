using Microsoft.AspNetCore.Mvc;
using RetailRocket.Application.DTOs.Request.Shop;
using RetailRocket.Application.DTOs.Response.Shop;
using RetailRocket.Application.Services.Shop;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.API.Controllers.Shop;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;
    
    public ProductController(ProductService productService) =>
        _productService = productService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllProductsAsync();
        var result = products.Select(p => new ProductDto
        {
            ProductId = p.ProductId,
            Name = p.Name,
            Price = p.Price,
            CategoryName = p.Category.Name,
            IsAvailable = p.Item.IsAvailable
        });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetProductAsync(id);
        if (product is null) return NotFound();
        return Ok(new ProductDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Price = product.Price,
            CategoryName = product.Category.Name,
            IsAvailable = product.Item.IsAvailable
        });
    }

    [HttpGet("by-name")]
    public async Task<IActionResult> GetByName([FromQuery] string name)
    {
        var product = await _productService.GetProductByNameAsync(name);
        if (product is null) return NotFound();
        return Ok(new ProductDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Price = product.Price,
            CategoryName = product.Category.Name,
            IsAvailable = product.Item.IsAvailable
        }); 
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var product = new Product(dto.ItemId, dto.Name, dto.Price, dto.CategoryId);
        await _productService.CreateProductAsync(product);
        return  CreatedAtAction(nameof(GetById), new { id = product.ProductId }, new ProductDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Price = product.Price,
            CategoryName = product.Category.Name,
            IsAvailable = product.Item.IsAvailable
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateProductDto dto)
    {
        var product = await _productService.GetProductAsync(id);
        if (product is null) return NotFound();
        product.UpdateItem(dto.ItemId);
        product.UpdateName(dto.Name);
        product.UpdatePrice(dto.Price);
        product.UpdateCategory(dto.CategoryId);
        await _productService.UpdateProductAsync(product);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _productService.GetProductAsync(id);
        if (product is null) return NotFound();
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}