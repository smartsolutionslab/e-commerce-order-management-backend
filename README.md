# E-Commerce Order Management Backend

Microservice for order processing and management with Clean Architecture, CQRS, and Event Sourcing.

## Features

- Order Lifecycle Management (Draft → Confirmed → Processing → Shipped → Delivered)
- Order Item Management
- Payment Integration
- Inventory Reservation
- Shipping Management
- Multi-Tenancy Support
- Event Sourcing
- RESTful API with Versioning

## API Endpoints

### v1 Endpoints
- `GET /api/v1/orders` - List orders
- `GET /api/v1/orders/{id}` - Get order by ID
- `POST /api/v1/orders` - Create order
- `PUT /api/v1/orders/{id}/confirm` - Confirm order
- `PUT /api/v1/orders/{id}/ship` - Ship order
- `PUT /api/v1/orders/{id}/cancel` - Cancel order
- `GET /api/v1/orders/{id}/items` - Get order items

## Domain Events

- OrderCreated, OrderConfirmed, OrderShipped, OrderDelivered
- OrderCancelled, PaymentProcessed, InventoryReserved

## Run Locally

```bash
dotnet run
```

Access: https://localhost:7003
