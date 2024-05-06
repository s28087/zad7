using zadanie.Models;

namespace zadanie.Repositories;

public interface IWarehouseService
{
    public Task<bool> PrExists(int productId);
    public Task<bool> WarExist(int warehouseId);
    public Task<int> CheckOrderExist(int idProduct, int amount, DateTime date);
    public Task<bool> CheckOrderCompletion(int idOrder);
    public Task<int> FulfilledAtUpdate(int IdOrder);
    public Task<int> AddProductToWarehouse(ProductWarehouse pr, int idOrder);
    public Task<int> GetPk(ProductWarehouse pr, int IdOrder);
}