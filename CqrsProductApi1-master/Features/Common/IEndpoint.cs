using Microsoft.AspNetCore.Routing;
namespace CqrsProductApi1.Features.Common;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
