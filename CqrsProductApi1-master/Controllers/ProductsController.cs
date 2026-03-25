using CqrsProductApi.Features.Products.Commands.CreateProduct;
using CqrsProductApi.Features.Products.Commands.DeleteProduct;
using CqrsProductApi.Features.Products.Dtos;
using CqrsProductApi.Features.Products.Queries.GetAllProducts;
using CqrsProductApi.Features.Products.Queries.GetProductById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CqrsProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductRequest request,
        [FromServices] CreateProductCommandHandler handler)
    {
        var id = await handler.Handle(new CreateProductCommand
        {
            Name = request.Name,
            Price = request.Price
        });

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] GetAllProductsQueryHandler handler)
        => Ok(await handler.Handle(new()));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromServices] GetProductByIdQueryHandler handler)
    {
        var result = await handler.Handle(new() { Id = id });
        return result == null ? NotFound() : Ok(result);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromServices] DeleteProductCommandHandler handler)
    {
        var deleted = await handler.Handle(new() { Id = id });
        return deleted ? NoContent() : NotFound();
    }
}