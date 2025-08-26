using Microsoft.EntityFrameworkCore;
using InventoryApi.Controllers;
using InventoryApi.Data;
using InventoryApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace InventoryApi.Tests
{
    public class ProductsControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new ProductsController(_context);

            // Seed some test data
            _context.Products.AddRange(new[]
            {
                new Product 
                { 
                    Name = "Test Laptop", 
                    Description = "Test laptop description", 
                    Price = 999.99m, 
                    StockQuantity = 10, 
                    Category = "Electronics",
                    IsActive = true
                },
                new Product 
                { 
                    Name = "Test Phone", 
                    Description = "Test phone description", 
                    Price = 599.99m, 
                    StockQuantity = 15, 
                    Category = "Electronics",
                    IsActive = true
                },
                new Product 
                { 
                    Name = "Test Chair", 
                    Description = "Test chair description", 
                    Price = 199.99m, 
                    StockQuantity = 5, 
                    Category = "Furniture",
                    IsActive = true
                }
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetProducts_ReturnsAllActiveProducts()
        {
            // Act
            var result = await _controller.GetProducts();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(3, products.Count());
        }

        [Fact]
        public async Task GetProducts_WithCategory_ReturnsFilteredProducts()
        {
            // Act
            var result = await _controller.GetProducts(category: "Electronics");

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, products.Count());
            Assert.All(products, product => Assert.Equal("Electronics", product.Category));
        }

        [Fact]
        public async Task GetProduct_WithValidId_ReturnsProduct()
        {
            // Arrange
            var testProduct = await _context.Products.FirstAsync();

            // Act
            var result = await _controller.GetProduct(testProduct.Id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Product>>(result);
            var product = Assert.IsType<Product>(actionResult.Value);
            Assert.Equal(testProduct.Id, product.Id);
            Assert.Equal(testProduct.Name, product.Name);
        }

        [Fact]
        public async Task GetProduct_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetProduct(-1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Product>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task CreateProduct_WithValidData_ReturnsCreatedProduct()
        {
            // Arrange
            var newProduct = new Product
            {
                Name = "Test Product",
                Description = "Test description",
                Price = 299.99m,
                StockQuantity = 20,
                Category = "Test Category"
            };

            // Act
            var result = await _controller.CreateProduct(newProduct);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Product>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnedProduct = Assert.IsType<Product>(createdAtActionResult.Value);
            Assert.Equal(newProduct.Name, returnedProduct.Name);
            Assert.True(returnedProduct.Id > 0);
            Assert.True(returnedProduct.IsActive);
        }

        [Fact]
        public async Task UpdateProduct_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var testProduct = await _context.Products.FirstAsync();
            var updatedProduct = new Product
            {
                Id = testProduct.Id,
                Name = "Updated Name",
                Description = testProduct.Description,
                Price = testProduct.Price,
                StockQuantity = testProduct.StockQuantity,
                Category = testProduct.Category,
                IsActive = true
            };

            // Act
            var result = await _controller.UpdateProduct(testProduct.Id, updatedProduct);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var productInDb = await _context.Products.FindAsync(testProduct.Id);
            Assert.Equal("Updated Name", productInDb.Name);
        }

        [Fact]
        public async Task DeleteProduct_ValidId_SoftDeletesProduct()
        {
            // Arrange
            var testProduct = await _context.Products.FirstAsync();

            // Act
            var result = await _controller.DeleteProduct(testProduct.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var productInDb = await _context.Products.FindAsync(testProduct.Id);
            Assert.False(productInDb.IsActive);
        }

        [Fact]
        public async Task SearchProducts_ReturnsMatchingProducts()
        {
            // Act
            var result = await _controller.SearchProducts("Laptop");

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Single(products);
            Assert.Contains(products, p => p.Name == "Test Laptop");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
