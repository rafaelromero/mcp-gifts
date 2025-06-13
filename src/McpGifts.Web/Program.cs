
using Microsoft.Extensions.Hosting;
using ModelContextProtocol;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMcpServer()
.WithStdioServerTransport()
.WithToolsFromAssembly();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Use(async (Context, next) =>
{
    // Set the content type to application/json
    Context.Response.ContentType = "application/json";
    
    // Set the server header
    Context.Response.Headers.Server = "McpGifts.Web";

    // Set the user agent header
    Context.Request.Headers.UserAgent =  new ProductInfoHeaderValue("McpGifts", "1.0").ToString();

    await next();
}); 


app.UseHttpsRedirection();

var summaries = new[]
{
    "Mongoose Bike", "Utraman", "Buzz Lightyear", "Chess Board", "Monopoly"
};



app.MapGet("/toyprices", () =>
{
    var toyPrices =  Enumerable.Range(1, 5).Select(index =>
        new ToyPrice
        (
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return toyPrices;
})
.WithName("GetToyPrices")
.WithOpenApi();

app.Run();

record ToyPrice(string Name)
{
    decimal Prices { get; init; } = (decimal)99.00;
}
