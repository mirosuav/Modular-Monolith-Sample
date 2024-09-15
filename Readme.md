
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
- Use client generate incrementa UUIDv7 as guids see `RiverBooks.SharedKernel.Extensions.Uuid7` class

TODO:
- Separate Modules IoC i.e. each modules has its own ServiceProvider, Logger etc
- Use strongly typed Ids, so instead `Guid UserId` define a typed `record struct UserId(Guid Value)` and use `UserId Id`
- Simplify events, no need to have separate DomainEvents and IntegrationEvents as we are not using DDD
- Use MediatR in modules internally but Service Bus to communicate between modules
- Use https://www.keycloak.org/ as IdentityProvider
- Check Seq for OpenTelemetry
- Check Azure ApplicationInsights for OpenTelemetry
- Check Jaeger for OpenTelemetry
- Check Grafana for OpenTelemetry
- Implement Unit tests
- Implement Integration tests with Docker db
- ...

# Features

## Order processing module

### Materialized view for order addresses

- User module sends `NewUserAddressAddedIntegrationEvent` to notify Order processing module about new addresses
- Customer order addressess are stored in `SqlServerOrderAddressCache` implementing `IDistributedCache`
- When order is created user addresses are taken from cache if exists or fetched by...
- `ReadThroughOrderAddressCache` is used to fetch user addresses from cache or send direct query to the User module and store the result in cache

### Outbox pattern for sending emails

- EmailSending module uses Outbox pattern for gathering and sending emails
- `SendEmailCommand` is received and queued in db in `QueueEmailInOutboxSendEmailCommandHandler`
- Then hosted service: `EmailSendingBackgroundService` calls `ISendEmailsFromOutboxService` implementation every second to process emails
- `DefaultSendEmailsFromOutboxService` implementing `ISendEmailsFromOutboxService` picks up emails and attempts to sends them

### Domain events Transactional Outbox pattern

- Domain entities are enriched with domain events to perform **eventual consistency** operations
- Domain events are persisted in `EventsOutbox` table of each module's DbContext
- `EventProcessing` periodically triggers a `ProcessDomainEventsCommand` command on each module to asynchronously process its domain events
- For example a ``User`` entity generates the ``AddressAddedDomainEvent`` which is translated to ``NewUserAddressAddedIntegrationEvent`` to notify the ``OrderProcessing`` module to updates its cache of users adresses
- Or ``Order`` domain entity generates the ``OrderCreatedDomainEvent`` to trigger the ``OrderCreatedIntegrationEvent`` to notify the ``Reporting`` module to store the order reporting tailored data in its table.


# Operations

### Generate EF migrations

```
dotnet ef migrations add Initial -p RiverBooks.EmailSending -c EmailSendingDbContext -s RiverBooks.Web -o Infrastructure\Migrations
dotnet ef migrations add Initial -p RiverBooks.Users -c UsersDbContext -s RiverBooks.Web -o Infrastructure\Migrations
dotnet ef migrations add Initial -p RiverBooks.Reporting -c ReportingDbContext -s RiverBooks.Web -o Infrastructure\Migrations
dotnet ef migrations add Initial -p RiverBooks.OrderProcessing -c OrderProcessingDbContext -s RiverBooks.Web -o Infrastructure\Migrations
dotnet ef migrations add Initial -p RiverBooks.Books -c BookDbContext -s RiverBooks.Web -o Infrastructure\Migrations
```

### Update databases in each module
```
dotnet ef database update -p RiverBooks.Users -c UsersDbContext -s RiverBooks.Web
dotnet ef database update -p RiverBooks.OrderProcessing -c OrderProcessingDbContext -s RiverBooks.Web
dotnet ef database update -p RiverBooks.Reporting -c ReportingDbContext -s RiverBooks.Web
dotnet ef database update -p RiverBooks.EmailSending -c EmailSendingDbContext -s RiverBooks.Web
dotnet ef database update -p RiverBooks.Books -c BookDbContext -s RiverBooks.Web

Below is optional - currently handled by Initial migration in OrderProcessing module
dotnet sql-cache create "Server=(local);Integrated Security=true;Initial Catalog=RiverBooks;Trust Server Certificate=True" OrderProcessing UserAddressesCache
```


# Todo

- Implement Domain events to be dispatched *offline* when client is waiting online, inspired by Amichai Mantinband approach


## Idea: EventsModule - Asynchronously processes all domain event by sending `ProcessDomainEvents` command to each individual module

EventsModule periodically sends commands to individual modules for processing their own domain events:

### Advantages:
- Transactional: Each module saves its DomainEvents in one transaction along with the associated business process
- Modularity: Each module remains responsible for its own domain events, which aligns with the modular monolith architecture.
- Decoupling: The EventsModule doesn’t need to know the specifics of event processing in each module. It simply triggers the process.
- Scalability: As your application grows, you can add more modules without affecting the EventsModule.
### Implementation:
- Define a ProcessDomainEvents command (or similar) that the EventsModule sends to each module.
- Each module (e.g., OrderManagementModule, InventoryModule) should handle this command and process its own domain events.
- The EventsModule can schedule these commands periodically (e.g., every minute) or based on specific triggers (e.g., after a batch of events is persisted).
### Considerations:
- Idempotency: Ensure that processing domain events remains idempotent, even if the command is sent multiple times.
- Retry Mechanism: Handle transient failures during command execution (e.g., network issues, database unavailability).
