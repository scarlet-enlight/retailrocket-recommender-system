using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RetailRocket.Application.DTOs.Request.Shop.Orders;
using RetailRocket.Application.DTOs.Response.Shop.Orders;
using RetailRocket.Application.Services.Shop;
using RetailRocket.Domain.Entities.Shop;
using RetailRocket.Domain.Exceptions;

namespace RetailRocket.API.Controllers.Shop;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly IMapper _mapper;

    public OrderController(OrderService orderService, IMapper mapper)
    {
        _orderService = orderService;
        _mapper = mapper;
    }
    
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Order ID is required.");
        
        var order = await _orderService.GetOrderAsync(id);
        if (order is null) 
            throw new NotFoundException("Order not found.");
        return Ok(_mapper.Map<OrderResponseDto>(order));
    }
    
    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAllByUser(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("User ID is required.");
        
        var orders = await _orderService.GetOrdersByUserAsync(userId);
        if (orders is null)
            throw new NotFoundException("User does not have any orders.");
        return Ok(_mapper.Map<IEnumerable<OrderResponseDto>>(orders));
    }

    
    // we still need to think of how to make those orders for proper DTO and endpoint to work
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderCreateRequestDto createRequestDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var order = new Order(userId, 500);
        await _orderService.AddOrderAsync(order);

        var created = _orderService.GetOrderAsync(order.OrderId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            _mapper.Map<OrderResponseDto>(created));
    }

    // WIP
    [Authorize]
    [HttpPut("user/{userId}/price")]
    public async Task<IActionResult> UpdateTotalPrice(Guid id, [FromBody] decimal total)
    {
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Order ID is required.");
        
        var order = await _orderService.GetOrderAsync(id);
        if (order is null)
            throw new NotFoundException("Order not found.");
        await _orderService.DeleteOrderAsync(id);
        return NoContent();
    }
}