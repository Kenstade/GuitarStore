using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Validation;

public sealed class ValidationEndpointFilter<TRequest>(IValidator<TRequest> validator) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.GetArgument<TRequest>(0);
        
        var result = await validator.ValidateAsync(request);
        
        return result.IsValid 
            ? await next(context) 
            : Results.ValidationProblem(result.ToDictionary());
    }
}