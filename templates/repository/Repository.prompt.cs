**Input**: 

- **Class**:
```csharp
public class Product
{
    public int Id { get; set; }
    public int Type { get; set; }
    public string Name { get; set; }
}
```

- **Database Table Name**: `Product`

- **Base Repository Interface**:
```csharp
public interface IRepository<T>
{
    T Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}
```

- **DbConnection Factory Interface**:
```csharp
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
```

**Expected Output Example**:

- **Product Repository Interface**:
```csharp
public interface IProductRepository : IRepository<Product>
{
 
}
```

- **Product Repository Implementation using Dapper**:
```csharp
using Dapper;
using System.Data;

public class ProductRepository : IProductRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ProductRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public Product Add(Product entity)
    {
        const string sql = "INSERT INTO [Product] (Type, Name) VALUES (@Type, @Name); SELECT CAST(SCOPE_IDENTITY() as int)";
        
        using (var connection = _connectionFactory.CreateConnection())
        {
            int id = connection.QuerySingle<int>(sql, entity);
            entity.Id = id;
            return entity;
        }
    }

    public void Update(Product entity)
    {
        const string sql = "UPDATE [Product] SET Type = @Type, Name = @Name WHERE Id = @Id";
        
        using (var connection = _connectionFactory.CreateConnection())
        {
            connection.Execute(sql, entity);
        }
    }

    public void Delete(Product entity)
    {
        const string sql = "DELETE FROM [Product] WHERE Id = @Id";
        
        using (var connection = _connectionFactory.CreateConnection())
        {
            connection.Execute(sql, new { Id = entity.Id });
        }
    }
}
```

**Notes**:
- Dapper is used to perform SQL operations (insert, update, delete) for the `Product` entity.
- `IDbConnectionFactory` is responsible for creating database connections.
- No additional functions or methods are added, following the requirements.
--- 

This structure ensures clarity and precision in the output.