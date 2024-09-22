# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["RiverBooks.SharedKernel/Directory.Build.props", "RiverBooks.SharedKernel/"]
COPY ["RiverBooks.Web/RiverBooks.Web.csproj", "RiverBooks.Web/"]
COPY ["RiverBooks.SharedKernel/RiverBooks.SharedKernel.csproj", "RiverBooks.SharedKernel/"]
COPY ["RiverBooks.Books.Api/RiverBooks.Books.Api.csproj", "RiverBooks.Books.Api/"]
COPY ["RiverBooks.Books.Contracts/RiverBooks.Books.Contracts.csproj", "RiverBooks.Books.Contracts/"]
COPY ["RiverBooks.Books/RiverBooks.Books.csproj", "RiverBooks.Books/"]
COPY ["RiverBooks.EmailSending.Api/RiverBooks.EmailSending.Api.csproj", "RiverBooks.EmailSending.Api/"]
COPY ["RiverBooks.EmailSending.Contracts/RiverBooks.EmailSending.Contracts.csproj", "RiverBooks.EmailSending.Contracts/"]
COPY ["RiverBooks.EmailSending/RiverBooks.EmailSending.csproj", "RiverBooks.EmailSending/"]
COPY ["RiverBooks.EventsProcessing.Api/RiverBooks.EventsProcessing.Api.csproj", "RiverBooks.EventsProcessing.Api/"]
COPY ["RiverBooks.EventsProcessing/RiverBooks.EventsProcessing.csproj", "RiverBooks.EventsProcessing/"]
COPY ["RiverBooks.EventsProcessing.Contracts/RiverBooks.EventsProcessing.Contracts.csproj", "RiverBooks.EventsProcessing.Contracts/"]
COPY ["RiverBooks.OrderProcessing.Api/RiverBooks.OrderProcessing.Api.csproj", "RiverBooks.OrderProcessing.Api/"]
COPY ["OrderProcessing.Contracts/RiverBooks.OrderProcessing.Contracts.csproj", "OrderProcessing.Contracts/"]
COPY ["RiverBooks.OrderProcessing/RiverBooks.OrderProcessing.csproj", "RiverBooks.OrderProcessing/"]
COPY ["RiverBooks.Users.Contracts/RiverBooks.Users.Contracts.csproj", "RiverBooks.Users.Contracts/"]
COPY ["RiverBooks.Reporting.Api/RiverBooks.Reporting.Api.csproj", "RiverBooks.Reporting.Api/"]
COPY ["RiverBooks.Reporting.Contracts/RiverBooks.Reporting.Contracts.csproj", "RiverBooks.Reporting.Contracts/"]
COPY ["RiverBooks.Reporting/RiverBooks.Reporting.csproj", "RiverBooks.Reporting/"]
COPY ["RiverBooks.Users.Api/RiverBooks.Users.Api.csproj", "RiverBooks.Users.Api/"]
COPY ["RiverBooks.Users/RiverBooks.Users.csproj", "RiverBooks.Users/"]
RUN dotnet restore "./RiverBooks.Web/RiverBooks.Web.csproj"
COPY . .
WORKDIR "/src/RiverBooks.Web"
RUN dotnet build "./RiverBooks.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RiverBooks.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "RiverBooks.Web.dll"]