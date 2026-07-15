using Microsoft.AspNetCore.Mvc;
using RetailRocket.Application.DTOs.Request.Shop;
using RetailRocket.Application.DTOs.Response.Shop;
using RetailRocket.Application.Services.Shop;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.API.Controllers.Shop;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;
    
    public CartController(CartService cartService) => 
        _cartService = cartService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var cart = await _cartService.GetCartAsync(id);
        if (cart is null) return NotFound();
        return Ok(new CartDto
        {
            CartId = cart.CartId,
            ProductName = cart.Product.Name,
            ProductPrice =  cart.Product.Price,
            Quantity = cart.Quantity
        });
    }

    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetAllByUser(Guid userId)
    {
        var carts = await _cartService.GetCartsByUserAsync(userId);
        var result = carts.Select(c => new CartDto
        {
            CartId = c.CartId,
            ProductName = c.Product.Name,
            ProductPrice = c.Product.Price,
            Quantity = c.Quantity
        });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCartDto dto)
    {
        var cart = new Cart(dto.UserId, dto.ProductId, dto.Quantity);
        await _cartService.AddCartAsync(cart);
        return CreatedAtAction(nameof(GetById), new { id = cart.CartId }, new CartDto
        {
            CartId = cart.CartId,
            ProductName = cart.Product.Name,
            ProductPrice =  cart.Product.Price,
            Quantity = cart.Quantity
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateCartDto dto)
    {
        var cart = await _cartService.GetCartAsync(id);
        if (cart is null) return NotFound();
        cart.UpdateProduct(dto.ProductId);
        cart.UpdateQuantity(dto.Quantity);
        await _cartService.UpdateCartAsync(cart);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var cart = await _cartService.GetCartAsync(id);
        if (cart is null) return NotFound();
        await _cartService.DeleteCartAsync(id);
        return NoContent();
    }
}