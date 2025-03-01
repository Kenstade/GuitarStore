namespace GuitarStore.Common.Web;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}
