using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RetailRocket.Application.DTOs.Request.Shop.Carts;
using RetailRocket.Application.DTOs.Response.Shop.Carts;
using RetailRocket.Application.Services.Shop;
using RetailRocket.Domain.Entities.Shop;
using RetailRocket.Domain.Exceptions;

namespace RetailRocket.API.Controllers.Shop;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;
    private readonly IMapper _mapper;

    public CartController(CartService cartService, IMapper mapper)
    {
        _cartService = cartService;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Cart ID is required.");
        
        var cart = await _cartService.GetCartAsync(id);
        if (cart is null)
            throw new NotFoundException("Cart not found.");
        return Ok(_mapper.Map<CartResponseDto>(cart));
    }

    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAllByUser(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("User ID is required.");
        
        var carts = await _cartService.GetCartsByUserAsync(userId);
        var result = _mapper.Map<IEnumerable<CartResponseDto>>(carts);
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CartCreateRequestDto createRequestDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (createRequestDto.ProductId == Guid.Empty)
            throw new ValidationException("A selected product is required.");
        
        if (createRequestDto.Quantity <= 0)
            throw new ValidationException("Quantity of this product must be greater than 0.");
        
        var cart = new Cart(userId, createRequestDto.ProductId, createRequestDto.Quantity);
        await _cartService.AddCartAsync(cart);

        var created = await _cartService.GetCartAsync(cart.CartId);
        return CreatedAtAction(nameof(GetById), new { id = cart.CartId },
            _mapper.Map<CartResponseDto>(created));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CartUpdateRequestDto updateRequestDto)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Cart ID is required.");
        
        var cart = await _cartService.GetCartAsync(id);
        if (cart is null) 
            throw new NotFoundException("Cart not found.");
        
        if (updateRequestDto.ProductId != cart.ProductId && updateRequestDto.ProductId != Guid.Empty)
            cart.UpdateProduct(updateRequestDto.ProductId);
        if (updateRequestDto.Quantity != cart.Quantity && updateRequestDto.Quantity <= 0)
            cart.UpdateQuantity(updateRequestDto.Quantity);
        
        await _cartService.UpdateCartAsync(cart);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Cart ID is required.");
        
        var cart = await _cartService.GetCartAsync(id);
        if (cart is null)
            throw new NotFoundException("Cart not found.");
        await _cartService.DeleteCartAsync(id);
        return NoContent();
    }
}