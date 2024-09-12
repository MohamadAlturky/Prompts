I need help implementing a filter using Dapper. Here's the setup:

### Filter Interface
```csharp
public interface IFilter<TEntity, TFilter>
{
    PaginatedResponse<TEntity> Filter(TFilter filter);
}
```

### Entity Class (`Meal`)
```csharp
public class Meal
{
    public int Id { get; set; }
    public int Type { get; set; }
    public string Name { get; set; }
}
```

### Filter Class (`MealFilter`)
```csharp
public class MealFilter
{
    // Filtering properties
    public int? Id { get; set; }
    public int? Type { get; set; }
    public string? Name { get; set; }

    // Pagination
    public PaginatedRequest PaginatedRequest { get; set; }

    // Ordering
    public bool OrderByIdDescending { get; set; } = false;
    public bool OrderByIdAscending { get; set; } = false;
    public bool OrderByNameDescending { get; set; } = false;
    public bool OrderByNameAscending { get; set; } = false;
    public bool OrderByTypeDescending { get; set; } = false;
    public bool OrderByTypeAscending { get; set; } = false;

    // Additional filters
    public string? NameContains { get; set; }
}
```

### Database Connection Interface
```csharp
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
```

### Task
Implement the `IFilter<TEntity, TFilter>` interface using the `Meal` entity and `MealFilter` class. Use Dapper for database querying. Ensure that filtering, ordering, and pagination are handled dynamically in the SQL query, and return a `PaginatedResponse<Meal>`.

---

This version clearly defines the problem, the classes involved, and the expected outcome, while highlighting the use of Dapper for querying.