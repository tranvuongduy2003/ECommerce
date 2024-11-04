using Infrastructure.Extensions;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.DTOs.Inventory;

namespace Saga.Orchestrator.HttpRepository;

public class InventoryHttpRepository : IInventoryHttpRepository
{
    private HttpClient _client;

    public InventoryHttpRepository(HttpClient client)
    {
        _client = client;
    }

    public async Task<string> CreateSalesOrderAsync(SalesProductDto model)
    {
        var response = await _client.PostAsJsonAsync($"inventory/sales/{model.ItemNo}", model);
        if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            throw new Exception($"Create sale order for item: {model.ItemNo} not success");

        var inventory = await response.Content.ReadFromJsonAsync<InventoryEntryDto>();

        return inventory.DocumentNo;
    }

    public async Task<bool> DeleteOrderByDocumentNoAsync(string documentNo)
    {
        var response = await _client.DeleteAsync($"inventory/document-no/{documentNo}");
        if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            throw new Exception($"Delete order for Document No: {documentNo} not success");

        var result = response.IsSuccessStatusCode;

        return result;
    }
}