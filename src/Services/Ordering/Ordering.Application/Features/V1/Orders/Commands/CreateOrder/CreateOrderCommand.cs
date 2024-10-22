using AutoMapper;
using EventBus.MessageComponents.Consumers.Basket;
using MediatR;
using Ordering.Application.Features.V1.Orders.Common;
using Ordering.Domain.Entities;
using Shared.SeedWork.ApiResult;

namespace Ordering.Application.Features.V1.Orders.Commands.CreateOrder;

public class CreateOrderCommand : CreateOrUpdateCommand, IRequest<ApiResult<long>>
{
    public string UserName { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CreateOrderCommand, Order>();
        profile.CreateMap<CreateOrderCommand, BasketCheckoutEvent>().ReverseMap();
    }
}