using MediatR;
using Ordering.Application.Common.Models;
using Shared.SeedWork.ApiResult;

namespace Ordering.Application.Features.V1.Orders.Queries.GetOrders;

public class GetOrderQuery : IRequest<ApiResult<List<OrderDto>>>
{
    public string UserName { get; set; } = string.Empty;

    public GetOrderQuery(string userName)
    {
        UserName = userName;
    }
}