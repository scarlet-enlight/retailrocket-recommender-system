using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RetailRocket.Application.DTOs.Request.Shop;
using RetailRocket.Application.DTOs.Response.Shop;
using RetailRocket.Application.Services.Shop;
using RetailRocket.Domain.Entities.Shop;

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
        var order = await _orderService.GetOrderAsync(id);
        if (order is null) return NotFound();
        return Ok(_mapper.Map<OrderResponseDto>(order));
    }
    
    [Authorize]
    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetAllByUser(Guid userId)
    {
        var orders = await _orderService.GetOrdersByUserAsync(userId);
        var result = _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderRequestDto requestDto)
    {
        var order = new Order(requestDto.UserId, requestDto.Total);
        await _orderService.AddOrderAsync(order);
        return CreatedAtAction(nameof(GetById), new {id = order.OrderId },  new OrderResponseDto
        {
            OrderId = order.OrderId,
            CreatedAt = order.CreatedAt,
            Total = order.Total
        });
    }

    // WIP
    [Authorize]
    [HttpPut("by-user/{userId}/price")]
    public async Task<IActionResult> UpdateTotalPrice(Guid id, [FromBody] decimal total)
    {
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var order = await _orderService.GetOrderAsync(id);
        if (order is null) return NotFound();
        await _orderService.DeleteOrderAsync(id);
        return NoContent();
    }
}