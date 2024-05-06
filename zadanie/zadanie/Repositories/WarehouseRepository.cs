using Microsoft.Data.SqlClient;
using zadanie.Models;

namespace zadanie.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private IConfiguration _configuration;

    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    //1.Sprawdzamy, czy produkt o podanym identyfikatorze istnieje.
    
    public async Task<bool> PrExists(int IdProduct)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        
        
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        
        cmd.CommandText = "SELECT * FROM Product WHERE IdProduct = @productId";
        cmd.Parameters.AddWithValue("@productId", IdProduct);

        if (await cmd.ExecuteScalarAsync() is not null)
        {
            return true;
        }

        return false;
    }
    
    //Następnie sprawdzamy, czy magazyn o podanym identyfikatorze istnieje.
    public async Task<bool> WarExist(int Idwarehouse)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM Warehouse WHERE IdWarehouse = @warehouseId";
        cmd.Parameters.AddWithValue("@warehouseId", Idwarehouse);

        if (await cmd.ExecuteScalarAsync() is not null)
        {
            return true;
        }

        return false;
    }
    
    
    //2.Możemy dodać produkt do magazynu tylko wtedy, gdy istnieje
    //zamówienie zakupu produktu w tabeli Order. Dlatego sprawdzamy, czy w
    // tabeli Order istnieje rekord z IdProduktu i Ilością (Amount), które
    //odpowiadają naszemu żądaniu. Data utworzenia zamówienia powinna
    //być wcześniejsza niż data utworzenia w żądaniu
    public async Task<int> CheckOrderExist(int idProduct, int amount, DateTime date)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        //[Order] <- slowo kluczowe
        cmd.CommandText = "SELECT IdOrder FROM [Order] WHERE IdProduct = @IdProduct and Amount = @Amount and CreatedAt < @date";

        cmd.Parameters.AddWithValue("@IdProduct", idProduct);
        cmd.Parameters.AddWithValue("@Amount", amount);
        cmd.Parameters.AddWithValue("@date", date);

        
        var res = await cmd.ExecuteScalarAsync();
        if (await cmd.ExecuteScalarAsync() is not null)
        {
            return (int)res;
        }

        return -99;
    }
    
    //3.Sprawdzamy, czy to zamówienie zostało przypadkiem zrealizowane.
    //Sprawdzamy, czy nie ma wiersza z danym IdOrder w tabeli Product_Warehouse.
    public async Task<bool> CheckOrderCompletion(int idOrder)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM Product_Warehouse where IdOrder = @idOrder";

        cmd.Parameters.AddWithValue("@idOrder", idOrder);

        if (await cmd.ExecuteScalarAsync() is not null)
        {
            return false;
        }

        return true;
    }
    
    
    
    //4. Aktualizujemy kolumnę FullfilledAt zamówienia na aktualną datę i godzinę. (UPDATE)
    public async Task<int> FulfilledAtUpdate(int IdOrder)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "UPDATE [Order] SET FulfilledAt = @Date WHERE IdOrder = @orderId";
        cmd.Parameters.AddWithValue("@Date", DateTime.Now.AddHours(2));
        cmd.Parameters.AddWithValue("@orderId", IdOrder);

        return await cmd.ExecuteNonQueryAsync();
    }


    //5.Wstawiamy rekord do tabeli Product_Warehouse.
    //Kolumna Price powinna odpowiadać cenie produktu pomnożonej
    //przez kolumnę Amount z naszego zamówienia. Ponadto wstawiamy wartość
    //CreatedAt zgodnie z aktualnym czasem. (INSERT)
    
    
    public async Task<decimal> GetPrice(int IdProduct)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
        cmd.Parameters.AddWithValue("@IdProduct", IdProduct);

        var res = await cmd.ExecuteScalarAsync();

        if (res is not null)
        {
            return (decimal)res;
        }

        return 0;
    }
    
    public async Task<int> AddProductToWarehouse(ProductWarehouse pr, int idOrder)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        decimal price = await GetPrice(pr.IdProduct);
        decimal priceAmount =price * pr.Amount;
        cmd.Connection = con;
        cmd.CommandText =
            "INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) VALUES(@IdWar, @IdPr, @IdOr, @amount, @price, @created)";
        cmd.Parameters.AddWithValue("@IdWar", pr.IdWarehouse);
        cmd.Parameters.AddWithValue("@IdPr", pr.IdProduct);
        cmd.Parameters.AddWithValue("@IdOr", idOrder);
        cmd.Parameters.AddWithValue("@amount", pr.Amount);
        cmd.Parameters.AddWithValue("@price", priceAmount);
        cmd.Parameters.AddWithValue("@created", pr.CreatedAt);
        
        return await cmd.ExecuteNonQueryAsync();
    }
    
    
    //6.W wyniku operacji zwracamy wartość klucza głównego wygenerowanega
    //dla rekordu wstawionego do tabeli Product_Warehouse.
    public async Task<int> GetPk(ProductWarehouse pr, int IdOrder)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT IdProductWarehouse FROM Product_Warehouse where IdWarehouse = @IdWar AND IdProduct = @IdPr AND IdOrder = @IdOr";
        
        
        cmd.Parameters.AddWithValue("@IdWar", pr.IdWarehouse);
        cmd.Parameters.AddWithValue("@IdPr", pr.IdProduct);
        cmd.Parameters.AddWithValue("@IdOr", IdOrder);

        var res = await cmd.ExecuteScalarAsync();
        if (res is not null)
        {
            return (int)res;
        }

        return 0;
    }
    
    
}