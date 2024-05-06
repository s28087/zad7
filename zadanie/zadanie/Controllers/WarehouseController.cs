using Microsoft.AspNetCore.Mvc;
using zadanie.Models;
using zadanie.Repositories;

namespace zadanie.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }
    
    
    //[HttpGet]
    //public IActionResult GetWares()
    
    [HttpPost]
    public async Task<IActionResult> AddProductWarehouse(ProductWarehouse productWarehouse)
    {
        bool prExist = await _warehouseService.PrExists(productWarehouse.IdProduct);
        bool warExist = await _warehouseService.WarExist(productWarehouse.IdWarehouse);
        
        if(!prExist)
        {
            return NotFound("brak produktu z id " + productWarehouse.IdProduct);
        }
        if (!warExist)
        {
            return NotFound("brak magazynu z id " + productWarehouse.IdWarehouse);
        }

        int orderId = await _warehouseService.CheckOrderExist(productWarehouse.IdProduct, productWarehouse.Amount, productWarehouse.CreatedAt);

        if(orderId == -99)
        {
            return NotFound("brak zamowienia");
        }

        bool orderComp = await _warehouseService.CheckOrderCompletion(orderId);

        if(!orderComp)
        {
            return NotFound("zamowienie z id " + orderId + " jest zrealizowane");
        }

        await _warehouseService.FulfilledAtUpdate(orderId);
        await _warehouseService.AddProductToWarehouse(productWarehouse, orderId);
        int pk = await _warehouseService.GetPk(productWarehouse, orderId);

        if (pk == 0)
        {
            return NotFound("brak zamowienia");
        }

        return Ok("klucz gł zamowienia " + pk);
    }
}