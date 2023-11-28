using Microsoft.AspNetCore.Mvc;

using SharingIsCaring.Core.Extensions;
using SharingIsCaring.Core.Models.Search;
using SharingIsCaring.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder
    .ConfigureMicrosoftLearnHttpClients()
    .ConfigureMicrosoftLearnSearchService();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    // TODO: uncomment and change after tunnel has been setup

    // opt.AddServer(new Microsoft.OpenApi.Models.OpenApiServer
    // {
    //     Url = "https://<dev.tunnels.url.here>",
    //     Description = "Microsoft Learn OpenAPI server"
    // });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwagger().UseSwaggerUI();

var msLearn = app.MapGroup("/search");
msLearn.MapGet("/", async (
    [FromServices] IMicrosoftLearnSearchService service,
    [FromQuery] string searchQuery = "") =>
        await service.SearchAsync(new SearchRequest
        {
            Query = searchQuery,
            Scope = ".Net",
            Locale = "en-us",
            Facets = [ "category", "products", "tags" ],
            Filter = "(scopes/any(s: s eq '.Net'))",
            Take = 10,
            ExpandScope = true,
            PartnerId = "LearnSite"
        }) is SearchResponse response
            ? Results.Ok(response)
            : Results.NotFound())
    .Produces<SearchResponse>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("MicrosoftLearn")
    .WithOpenApi();

app.Run();