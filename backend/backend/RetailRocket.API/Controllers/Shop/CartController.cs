using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
    private readonly UserService _userService;
    private readonly IMapper _mapper;

    public CartController(CartService cartService, UserService userService, IMapper mapper)
    {
        _cartService = cartService;
        _userService = userService;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var cart = await _cartService.GetCartAsync(id);
        if (cart is null) return NotFound();
        return Ok(_mapper.Map<CartResponseDto>(cart));
    }

    [Authorize]
    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetAllByUser(Guid userId)
    {
        var carts = await _cartService.GetCartsByUserAsync(userId);
        var result = _mapper.Map<IEnumerable<CartResponseDto>>(carts);
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CartRequestDto requestDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var cart = new Cart(userId, requestDto.ProductId, requestDto.Quantity);
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

    [Authorize]
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

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var cart = await _cartService.GetCartAsync(id);
        if (cart is null) return NotFound();
        await _cartService.DeleteCartAsync(id);
        return NoContent();
    }
}