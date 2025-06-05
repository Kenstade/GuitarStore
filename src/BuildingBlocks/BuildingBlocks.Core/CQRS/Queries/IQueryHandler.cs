using BuildingBlocks.Core.ErrorHandling;

namespace BuildingBlocks.Core.CQRS.Queries;

public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery request, CancellationToken cancellationToken = default);
}