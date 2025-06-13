using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;

namespace McpGifts.Tools;

[McpServerToolType]
public static class ToysTools
{
    [McpServerTool, Description("Get toy prices.")]
    public static async Task<string> GetToyPrices(
        HttpClient client,
        [Description("The type of toy.")] string state)
    {
        return $"""
                    Name: "bike"
                    Price: 890.00"
                    """;
    }

    [McpServerTool, Description("Get weather forecast for a location.")]
    public static async Task<string> GetForecast(
        HttpClient client,
        [Description("Latitude of the location.")] double latitude,
        [Description("Longitude of the location.")] double longitude)
    {
        var pointUrl = string.Create(CultureInfo.InvariantCulture, $"/points/{latitude},{longitude}");
        using var jsonDocument = await client.ReadJsonDocumentAsync(pointUrl);
        var forecastUrl = jsonDocument.RootElement.GetProperty("properties").GetProperty("forecast").GetString()
            ?? throw new Exception($"No forecast URL provided by {client.BaseAddress}points/{latitude},{longitude}");

        using var forecastDocument = await client.ReadJsonDocumentAsync(forecastUrl);
        var periods = forecastDocument.RootElement.GetProperty("properties").GetProperty("periods").EnumerateArray();

        return string.Join("\n---\n", periods.Select(period => $"""
                {period.GetProperty("name").GetString()}
                Temperature: {period.GetProperty("temperature").GetInt32()}°F
                Wind: {period.GetProperty("windSpeed").GetString()} {period.GetProperty("windDirection").GetString()}
                Forecast: {period.GetProperty("detailedForecast").GetString()}
                """));
    }
}