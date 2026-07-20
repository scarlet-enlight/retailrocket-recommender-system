using Microsoft.AspNetCore.Mvc;
using RetailRocket.Application.DTOs.Request.Shop;
using RetailRocket.Application.DTOs.Response.Shop;
using RetailRocket.Application.DTOs.Short.Shop;
using RetailRocket.Application.Services.Shop;
using RetailRocket.Domain.Entities.Shop;

namespace RetailRocket.API.Controllers.Shop;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;

    public CartController(CartService cartService, UserService userService) =>
        _cartService = cartService;


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var cart = await _cartService.GetCartAsync(id);
        if (cart is null) return NotFound();
        
        return Ok(new CartResponseDto
        {
            CartId = cart.CartId,
            User = new UserShortDto
            {
                UserId = cart.User.UserId,
                Username = cart.User.Username
            },
            Product = new ProductShortDto
            {
                Name = cart.Product.Name,
                Price = cart.Product.Price,
            },
            Quantity = cart.Quantity
        });
    }

    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetAllByUser(Guid userId)
    {
        var carts = await _cartService.GetCartsByUserAsync(userId);
        var result = carts.Select(c => new CartResponseDto
        {
            CartId = c.CartId,
            Product = new ProductShortDto
            {
                Name = c.Product.Name,
                Price = c.Product.Price,
            },
            Quantity = c.Quantity
        });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CartRequestDto requestDto)
    {
        var cart = new Cart(requestDto.UserId, requestDto.ProductId, requestDto.Quantity);
        await _cartService.AddCartAsync(cart);
        return CreatedAtAction(nameof(GetById), new { id = cart.CartId }, new CartResponseDto
        {
            CartId = cart.CartId,
            User = new UserShortDto
            {
                UserId = cart.User.UserId,
                Username = cart.User.Username
            },
            Product = new ProductShortDto
            {
                Name = cart.Product.Name,
                Price = cart.Product.Price,
            },
            Quantity = cart.Quantity
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CartRequestDto requestDto)
    {
        var cart = await _cartService.GetCartAsync(id);
        if (cart is null) return NotFound();
        cart.UpdateProduct(requestDto.ProductId);
        cart.UpdateQuantity(requestDto.Quantity);
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