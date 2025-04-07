using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Validation;

public sealed class ValidationEndpointFilter<TRequest> : IEndpointFilter
{
    private readonly IValidator<TRequest> _validator;
    public ValidationEndpointFilter(IValidator<TRequest> validator)
    {
        _validator = validator;
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.GetArgument<TRequest>(0);
        
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid) return TypedResults.ValidationProblem(result.ToDictionary());
        
        return await next(context);    
    }
}