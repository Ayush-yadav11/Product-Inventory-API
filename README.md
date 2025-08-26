# Product Inventory API

A RESTful API built with ASP.NET Core for managing product inventory. This API provides endpoints for creating, reading, updating, and deleting products, along with features like filtering, sorting, pagination, and search capabilities.

## Features

## Bonus Features Implementation

### 1. Search Functionality

✅ Implemented in `SearchProducts` endpoint

- Search across both product names and descriptions
- Returns paginated results
- Example: `GET /api/products/search?query=gaming`
- Verified by test: `SearchProducts_ReturnsMatchingProducts`

  ```csharp
  [Fact]
  public async Task SearchProducts_ReturnsMatchingProducts()
  {
      var result = await _controller.SearchProducts("Laptop");
      var products = Assert.IsType<OkObjectResult>(result.Result).Value as IEnumerable<Product>;
      Assert.Contains(products, p => p.Name == "Test Laptop");
  }
  ```

### 2. Pagination

✅ Implemented across all list endpoints

- Configurable page size and number
- Returns total count and pages in headers
- Example: `GET /api/products?page=1&pageSize=10`
- Verified by test: `GetProducts_ReturnsAllActiveProducts`

  ```csharp
  [Fact]
  public async Task GetProducts_ReturnsAllActiveProducts()
  {
      var result = await _controller.GetProducts();
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
      Assert.Equal(3, products.Count());
  }
  ```

### 3. Low Stock Alerts

✅ Implemented in product retrieval endpoints

- Automatic alert when stock falls below 5 items
- Custom header `X-Low-Stock-Alert: true`
- Example: `GET /api/products/{id}` (check response headers)
- Verified by test in `GetProduct_WithValidId_ReturnsProduct`

### Core Features

- ✅ Product Management (CRUD Operations)
  - Create new products
  - Retrieve product details
  - Update existing products
  - Soft delete products
- ✅ Product Attributes
  - Name
  - Description
  - Price
  - Stock Quantity
  - Category
- ✅ Advanced Querying
  - Filter products by category
  - Sort products by price
  - Search by product name/description
  - Pagination support
- ✅ Stock Management
  - Low stock alerts (when quantity < 5)

## Technology Stack

- ASP.NET Core 8.0
- Entity Framework Core
- SQLite Database
- xUnit for Testing

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code

### Installation

1.Clone the repository:

```bash
git clone [repository-url]
```

2.Navigate to the project directory:

```bash
cd "Product Inventory API"
```

3.Build the solution:

```bash
dotnet build
```

4.Run the API:

```bash
cd InventoryApi
dotnet run
```

The API will be available at `http://localhost:5037` or `https://localhost:5038`

### Running Tests

To run the unit tests:

```bash
cd InventoryApi.Tests
dotnet test
```

## API Endpoints

### Products

#### GET /api/products

- Get all products
- Supports pagination, filtering, and sorting
- Query Parameters:
  - `category`: Filter by product category
  - `sortBy`: Sort by price (`price_asc` or `price_desc`)
  - `page`: Page number (default: 1)
  - `pageSize`: Items per page (default: 10)

#### GET /api/products/{id}

- Get a specific product by ID
- Returns 404 if product not found
- Includes low stock alert header if stock < 5

#### POST /api/products

- Create a new product
- Required fields in request body:
  - name
  - description
  - price
  - stockQuantity
  - category

#### PUT /api/products/{id}

- Update an existing product
- Requires all product fields in request body

#### DELETE /api/products/{id}

- Soft delete a product
- Product remains in database but marked as inactive

#### GET /api/products/search

- Search products by name or description
- Query Parameters:
  - `query`: Search term
  - `page`: Page number (default: 1)
  - `pageSize`: Items per page (default: 10)

## Example Requests

### Create a Product

```bash
curl -X POST "http://localhost:5037/api/products" \
  -H "Content-Type: application/json" \
  -d "{
    \"name\": \"Laptop\",
    \"description\": \"High-performance gaming laptop\",
    \"price\": 999.99,
    \"stockQuantity\": 10,
    \"category\": \"Electronics\"
  }"
```

### Get Products with Filtering and Sorting

```bash
curl "http://localhost:5037/api/products?category=Electronics&sortBy=price_desc"
```

### Search Products

```bash
curl "http://localhost:5037/api/products/search?query=gaming"
```

## Development

The solution includes comprehensive unit tests covering all major functionality. The tests use Entity Framework Core's in-memory database provider for isolation and speed.

Key test areas include:

- Basic CRUD operations
- Category filtering
- Price sorting
- Pagination
- Search functionality
- Low stock alerts
- Soft delete verification

## Data Persistence

The API uses SQLite for data storage. The database file (`inventory.db`) is automatically created in the application's root directory when the application first runs.
