using zadanie.Models;

namespace zadanie.Repositories;

public class WarehouseService : IWarehouseService
{

    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }
    
    public async Task<bool> PrExists(int productId)
    {
        return await _warehouseRepository.PrExists(productId);
    }

    public async Task<bool> WarExist(int warehouseId)
    {
        return await _warehouseRepository.WarExist(warehouseId);
    }

    public async Task<int> CheckOrderExist(int idProduct, int amount, DateTime date)
    {
        return await _warehouseRepository.CheckOrderExist(idProduct, amount, date);
    }

    public async Task<bool> CheckOrderCompletion(int idOrder)
    {
        return await _warehouseRepository.CheckOrderCompletion(idOrder);
    }

    public async Task<int> FulfilledAtUpdate(int IdOrder)
    {
        return await _warehouseRepository.FulfilledAtUpdate(IdOrder);
    }

    public async Task<int> AddProductToWarehouse(ProductWarehouse pr, int idOrder)
    {
        return await _warehouseRepository.AddProductToWarehouse(pr, idOrder);
    }

    public async Task<int> GetPk(ProductWarehouse pr, int IdOrder)
    {
        return await _warehouseRepository.GetPk(pr, IdOrder);
    }
}