FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["src/E-Commerce.OrderManagement.Api/E-Commerce.OrderManagement.Api.csproj", "src/E-Commerce.OrderManagement.Api/"]
COPY ["src/E-Commerce.OrderManagement.Application/E-Commerce.OrderManagement.Application.csproj", "src/E-Commerce.OrderManagement.Application/"]
COPY ["src/E-Commerce.OrderManagement.Domain/E-Commerce.OrderManagement.Domain.csproj", "src/E-Commerce.OrderManagement.Domain/"]
COPY ["src/E-Commerce.OrderManagement.Infrastructure/E-Commerce.OrderManagement.Infrastructure.csproj", "src/E-Commerce.OrderManagement.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/E-Commerce.OrderManagement.Api/E-Commerce.OrderManagement.Api.csproj"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/src/E-Commerce.OrderManagement.Api"
RUN dotnet build "E-Commerce.OrderManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "E-Commerce.OrderManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "E-Commerce.OrderManagement.Api.dll"]
