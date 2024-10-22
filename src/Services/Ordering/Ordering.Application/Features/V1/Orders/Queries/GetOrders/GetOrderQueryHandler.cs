using AutoMapper;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Serilog;
using Shared.SeedWork.ApiResult;

namespace Ordering.Application.Features.V1.Orders.Queries.GetOrders;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, ApiResult<List<OrderDto>>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private const string MethodName = nameof(GetOrderQueryHandler);

    public GetOrderQueryHandler(IOrderRepository orderRepository, IMapper mapper, ILogger logger)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApiResult<List<OrderDto>>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        _logger.Information($"BEGIN: {MethodName} - Username: {request.UserName}");

        var orders = await _orderRepository.GetOrdersByUserName(request.UserName);
        var orderDtoList = _mapper.Map<List<OrderDto>>(orders);

        _logger.Information($"END: {MethodName} - Username: {request.UserName}");

        return new ApiSuccessResult<List<OrderDto>>(orderDtoList);
    }
}