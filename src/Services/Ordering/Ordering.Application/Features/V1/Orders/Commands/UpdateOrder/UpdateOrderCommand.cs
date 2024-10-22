using AutoMapper;
using Infrastructure.Extensions;
using MediatR;
using Ordering.Application.Common.Mapping;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders.Common;
using Ordering.Domain.Entities;
using Shared.SeedWork.ApiResult;

namespace Ordering.Application.Features.V1.Orders.Commands.UpdateOrder;

public class UpdateOrderCommand : CreateOrUpdateCommand, IRequest<ApiResult<OrderDto>>, IMapFrom<Order>
{
    public long Id { get; private set; }

    public void SetId(long id)
    {
        Id = id;
    }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<UpdateOrderCommand, Order>()
            .ForMember(dest => dest.Status, opts => opts.Ignore())
            .IgnoreAllNonExisting();
    }
}