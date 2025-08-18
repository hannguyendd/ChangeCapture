# ChangeCapture

A comprehensive **Change Data Capture (CDC)** system that demonstrates real-time data synchronization between a PostgreSQL database and Elasticsearch using Debezium, RabbitMQ, and .NET 9 microservices.

## 🏗️ Architecture Overview

This project implements a complete CDC pipeline with the following components:

```mermaid
graph TD
    A[Product Service] --> B[PostgreSQL DB]
    B --> C[Debezium CDC]
    C --> D[RabbitMQ]
    D --> E[Search Service]
    E --> F[Elasticsearch]
    F --> G[Kibana Dashboard]
```

### Core Components

- **ProductService**: RESTful API for product CRUD operations using Entity Framework Core
- **SearchService**: Elasticsearch-powered search API with real-time indexing
- **Debezium**: Change data capture from PostgreSQL
- **RabbitMQ**: Message broker for event streaming
- **Elasticsearch**: Full-text search and analytics engine
- **Kibana**: Data visualization and monitoring dashboard
- **PostgreSQL**: Primary data store with logical replication

## 🚀 Features

### ProductService

- ✅ Complete CRUD operations for products
- ✅ Entity Framework Core with PostgreSQL
- ✅ Input validation with data annotations
- ✅ OpenAPI/Swagger documentation
- ✅ RESTful API design
- ✅ Error handling and logging

### SearchService

- ✅ Full-text search with fuzzy matching
- ✅ Real-time indexing via message queue
- ✅ Advanced Elasticsearch queries
- ✅ Pagination support
- ✅ Field boosting (name^2, description)
- ✅ Auto-index creation and management

### Change Data Capture Pipeline

- ✅ Real-time CDC with Debezium
- ✅ PostgreSQL logical replication
- ✅ RabbitMQ message streaming
- ✅ Automatic schema detection
- ✅ Configurable transformations

## 🛠️ Technology Stack

| Component             | Technology            | Version      |
| --------------------- | --------------------- | ------------ |
| **Backend**           | .NET                  | 9.0          |
| **Database**          | PostgreSQL            | 15+          |
| **Message Queue**     | RabbitMQ              | 3-management |
| **Search Engine**     | Elasticsearch         | 9.1.2        |
| **CDC Tool**          | Debezium              | 2.7.3.Final  |
| **Visualization**     | Kibana                | 9.1.2        |
| **ORM**               | Entity Framework Core | 9.0.8        |
| **Mapping**           | Mapster               | 7.4.0        |
| **API Documentation** | Scalar.AspNetCore     | 2.6.9        |

## 📁 Project Structure

```
ChangeCapture/
├── docker-compose.yml                 # Infrastructure services
├── ChangeCapture.sln                  # Solution file
├── Shared/                            # Shared contracts and entities
│   ├── Entities/
│   │   └── Product.cs                 # Product entity model
│   ├── Contracts/Products/
│   │   └── ProductDto.cs              # DTOs and contracts
│   └── Constants/
│       └── SearchIndex.cs             # Search index constants
├── ProductService/                    # Product management API
│   ├── Controllers/
│   │   └── ProductsController.cs      # REST API endpoints
│   ├── Services/
│   │   └── ProductService.cs          # Business logic
│   ├── Infrastructure/
│   │   └── ProductDbContext.cs        # EF Core context
│   ├── Migrations/                    # Database migrations
│   ├── Properties/launchSettings.json # Launch configuration
│   ├── ProductService.http            # HTTP test requests
│   └── appsettings.json               # Configuration
└── SearchService/                     # Search API
    ├── Controllers/
    │   └── SearchController.cs        # Search endpoints
    ├── Services/
    │   ├── SearchProductService.cs    # Search business logic
    │   ├── ElasticsearchIndexService.cs # Index management
    │   └── ProductChangesConsumer.cs  # Message consumer
    ├── Interfaces/                    # Service interfaces
    ├── Properties/launchSettings.json # Launch configuration
    ├── SearchService.http             # HTTP test requests
    └── appsettings.json               # Configuration
```

## 🔧 Setup Instructions

### Prerequisites

- **.NET 9 SDK**
- **Docker & Docker Compose**
- **Visual Studio Code** or **Visual Studio 2022**

### 1. Clone the Repository

```bash
git clone https://github.com/hannguyendd/ChangeCapture.git
cd ChangeCapture
```

### 2. Start Infrastructure Services

```bash
# Start all infrastructure services
docker-compose up -d

# Verify services are running
docker-compose ps
```

This will start:

- **PostgreSQL** on port `5433`
- **RabbitMQ** on ports `5672` (AMQP) and `15672` (Management UI)
- **Elasticsearch** on ports `9200` (HTTP) and `9300` (Transport)
- **Kibana** on port `5601`
- **Debezium Server** on port `8083`

### 3. Configure Database

```bash
# Navigate to ProductService
cd ProductService

# Update database schema
dotnet ef database update
```

### 4. Run the Services

#### Option A: Using VS Code

1. Open the workspace in VS Code
2. Use the compound launch configuration "All Services"
3. Press `F5` to start both services

#### Option B: Using Terminal

```bash
# Terminal 1 - ProductService
cd ProductService
dotnet run

# Terminal 2 - SearchService
cd SearchService
dotnet run
```

### 5. Verify the Setup

- **ProductService API**: http://localhost:5151/scalar
- **SearchService API**: http://localhost:5050/scalar
- **RabbitMQ Management**: http://localhost:15672 (user/password)
- **Kibana Dashboard**: http://localhost:5601
- **Elasticsearch**: http://localhost:9200

## 📋 API Endpoints

### ProductService (Port 5151)

| Method   | Endpoint             | Description        |
| -------- | -------------------- | ------------------ |
| `GET`    | `/api/products`      | Get all products   |
| `GET`    | `/api/products/{id}` | Get product by ID  |
| `POST`   | `/api/products`      | Create new product |
| `PUT`    | `/api/products/{id}` | Update product     |
| `DELETE` | `/api/products/{id}` | Delete product     |

### SearchService (Port 5050)

| Method | Endpoint                         | Description       |
| ------ | -------------------------------- | ----------------- |
| `GET`  | `/api/search/products?q={query}` | Search products   |
| `GET`  | `/api/search/products/all`       | Get all products  |
| `GET`  | `/api/search/products/{id}`      | Get product by ID |

## 🧪 Testing the CDC Pipeline

### 1. Create a Product

```bash
curl -X POST "http://localhost:5151/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Gaming Laptop",
    "description": "High-performance gaming laptop with RTX 4070",
    "price": 1499.99
  }'
```

### 2. Verify in Search Service

```bash
# Search for the product
curl "http://localhost:5050/api/search/products?q=gaming"

# Get all products
curl "http://localhost:5050/api/search/products/all"
```

### 3. Monitor the Pipeline

- **RabbitMQ**: Check the `product-changes` queue
- **Debezium**: Monitor CDC logs
- **Elasticsearch**: Verify document indexing
- **Kibana**: Create visualizations

## 🔍 Search Features

The SearchService provides advanced search capabilities:

### Full-Text Search

```bash
# Search across name and description
GET /api/search/products?q=gaming laptop

# Fuzzy matching (handles typos)
GET /api/search/products?q=laptoop
```

### Field Boosting

- Product **name** has 2x weight in search relevance
- **Description** provides additional context

### Pagination

```bash
# Get page 2 with 5 items
GET /api/search/products?page=2&pageSize=5
```

## 📊 Monitoring and Observability

### Kibana Dashboard

1. Open http://localhost:5601
2. Go to **Stack Management** > **Index Patterns**
3. Create index pattern for `products`
4. Explore data in **Discover** tab

### RabbitMQ Management

1. Open http://localhost:15672
2. Login with `user`/`password`
3. Monitor `product-changes` queue
4. View message flow and rates

### Application Logs

Both services provide structured logging:

- **ProductService**: CRUD operations
- **SearchService**: Search queries and indexing
- **Debezium**: CDC events and transformations

## 🛡️ Configuration

### ProductService (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5433;Database=demo-product;Username=postgres;Password=password"
  }
}
```

### SearchService (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "RabbitMQ": "amqp://user:password@localhost:5672",
    "Elasticsearch": "http://localhost:9200"
  },
  "ElasticsearchSettings": {
    "DefaultIndex": "products"
  }
}
```

## 🔄 CDC Configuration

Debezium is configured to:

- **Source**: PostgreSQL `Products` table
- **Sink**: RabbitMQ `product-changes` exchange
- **Transformations**: Extract new record state, route to specific topic
- **Format**: JSON without schemas

## 🚀 Deployment

### Docker Compose (Recommended)

All infrastructure services are containerized and can be deployed with:

```bash
docker-compose up -d
```

### Kubernetes

For production deployments, consider:

- **Helm charts** for service deployment
- **Persistent volumes** for data storage
- **Ingress controllers** for external access
- **Service mesh** for inter-service communication

## 🤝 Contributing

1. **Fork** the repository
2. **Create** a feature branch
3. **Commit** your changes
4. **Push** to the branch
5. **Create** a Pull Request

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

## 📞 Support

For questions and support:

- **Create an issue** on GitHub
- **Email**: [your-email@domain.com]
- **Documentation**: Check the `/docs` folder

---

**Built with ❤️ using .NET 9, Elasticsearch, and modern microservices architecture**
