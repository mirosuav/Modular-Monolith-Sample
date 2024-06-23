
# Modular monolith WebAPI sample

Inspired by 
Steven Ardalis Smith 
Modular Monoliths course

### Modifications

- Use Clean Architecture on module level
- Replace FastEndpoints with Asp.NET Core minimal Apis
- Replace Redis cache with Distributed SQL Server Cache [Distributed caching in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-8.0)
- Replace Ardalis.Result with custom tailored implementation of Result pattern that supports ASP.Net Core http result types
- Replace Ardalis.Guard with custom guard helper class
- Use MediatR
- Uses Registration/Login with JWT Bearer authentication scheme
- Replaced MongoDB based outbox pattern with SqlServer based
- Use client generate incrementa UUIDv7 as guids

TODO:
- Use strongly typed Ids


### Update databases in each module
```
dotnet ef database update -p RiverBooks.Users -c UsersDbContext -s RiverBooks.Web
dotnet ef database update -p RiverBooks.OrderProcessing -c OrderProcessingDbContext -s RiverBooks.Web
dotnet ef database update -p RiverBooks.EmailSending -c EmailSendingDbContext -s RiverBooks.Web
dotnet sql-cache create "Server=(local);Integrated Security=true;Initial Catalog=RiverBooks;Trust Server Certificate=True" OrderProcessing UserAdressesCache
dotnet ef database update -p RiverBooks.Books -c BookDbContext -s RiverBooks.Web`
```

### Generate EF migrations

```
dotnet ef migrations add Initial -p RiverBooks.EmailSending -c EmailSendingDbContext -s RiverBooks.Web -o Infrastructure\Migrations
dotnet ef migrations add Initial -p RiverBooks.Users -c UsersDbContext -s RiverBooks.Web -o Infrastructure\Migrations
dotnet ef migrations add Initial -p RiverBooks.OrderProcessing -c OrderProcessingDbContext -s RiverBooks.Web -o Infrastructure\Migrations
dotnet ef migrations add Initial -p RiverBooks.Books -c BookDbContext -s RiverBooks.Web -o Infrastructure\Migrations
```







