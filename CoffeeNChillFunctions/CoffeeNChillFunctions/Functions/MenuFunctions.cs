using System.Net;
using System.Text.Json;
using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using CoffeeNChillFunctions.Models;

namespace CoffeeNChillFunctions.Functions;

public class MenuFunctions
{
    private readonly ILogger _logger;
    private const string TableName = "MenuItems";

    public MenuFunctions(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<MenuFunctions>();
    }

    [Function("CreateMenuItem")]
    public async Task<HttpResponseData> CreateMenuItem(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "menu")] HttpRequestData req,
        [TableInput(TableName, Connection = "AzureWebJobsStorage")] TableClient tableClient)
    {
        await tableClient.CreateIfNotExistsAsync();

        MenuItemCreateRequest? data;
        try
        {
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            data = JsonSerializer.Deserialize<MenuItemCreateRequest>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException)
        {
            return await BadRequest(req, "Invalid JSON body.");
        }

        if (data == null ||
            string.IsNullOrWhiteSpace(data.Category) ||
            string.IsNullOrWhiteSpace(data.Sku) ||
            string.IsNullOrWhiteSpace(data.Name))
        {
            return await BadRequest(req, "Category, Sku, and Name are required.");
        }

        if (data.Price < 0)
        {
            return await BadRequest(req, "Price cannot be negative.");
        }

        try
        {
            await tableClient.GetEntityAsync<MenuItem>(data.Category, data.Sku);
            return await BadRequest(req, $"A menu item with SKU '{data.Sku}' already exists in category '{data.Category}'.");
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
         
        }

        var entity = new MenuItem
        {
            PartitionKey = data.Category,
            RowKey = data.Sku,
            Name = data.Name,
            Description = data.Description,
            Price = data.Price,
            IsAvailable = data.IsAvailable
        };

        await tableClient.AddEntityAsync(entity);

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(entity);
        return response;
    }

    [Function("GetAllMenuItems")]
    public async Task<HttpResponseData> GetAllMenuItems(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu")] HttpRequestData req,
        [TableInput(TableName, Connection = "AzureWebJobsStorage")] TableClient tableClient)
    {
        await tableClient.CreateIfNotExistsAsync();

        var results = new List<MenuItem>();
        await foreach (var item in tableClient.QueryAsync<MenuItem>())
        {
            results.Add(item);
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(results);
        return response;
    }

    #region Helper Methods
    private static async Task<HttpResponseData> BadRequest(HttpRequestData req, string message)
    {
        var response = req.CreateResponse(HttpStatusCode.BadRequest);
        await response.WriteAsJsonAsync(new { error = message });
        return response;
    }
    #endregion
}