using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders.Commands.CreateOrder;
using Ordering.Application.Features.V1.Orders.Commands.DeleteOrder;
using Ordering.Application.Features.V1.Orders.Commands.UpdateOrder;
using Ordering.Application.Features.V1.Orders.Queries.GetOrders;
using Shared.SeedWork.ApiResult;

namespace Ordering.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    private static class RouteNames
    {
        public const string GetOrders = nameof(GetOrders);
        public const string CreateOrder = nameof(CreateOrder);
        public const string UpdateOrder = nameof(UpdateOrder);
        public const string DeleteOrder = nameof(DeleteOrder);
    }

    [HttpGet("{userName}", Name = RouteNames.GetOrders)]
    public async Task<ActionResult<ApiResult<List<OrderDto>>>> GetOrdersByUserName(string userName)
    {
        var query = new GetOrderQuery(userName);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    #region CRUD

    [HttpPost(Name = RouteNames.CreateOrder)]
    public async Task<ActionResult<ApiResult<long>>> CreateOrder(CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id:long}", Name = RouteNames.UpdateOrder)]
    public async Task<ActionResult<ApiResult<OrderDto>>> UpdateOrder(long id, UpdateOrderCommand command)
    {
        command.SetId(id);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:long}", Name = RouteNames.DeleteOrder)]
    public async Task<ActionResult> DeleteOrder(long id)
    {
        var command = new DeleteOrderCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }

    #endregion
}