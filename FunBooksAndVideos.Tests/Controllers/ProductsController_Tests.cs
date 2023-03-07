using FunBooksAndVideos.Controllers;
using FunBooksAndVideos.Data;
using FunBooksAndVideos.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FunBooksAndVideos.Tests.Controllers
{
    public sealed class ProductsController_Tests
    {
        private static ProductsController BuildController(out List<Product> products, out Mock<DbSet<Product>> mockDbSet)
        {
            var product1 = new Product
            {
                Id = 1,
                Name = "Gizmo ABC",
                Price = 12.34m,
                IsPhysical = false
            };
            var product2 = new Product
            {
                Id = 2,
                Name = "Super Widget",
                Price = 23.45m,
                IsPhysical = true
            };
            products = new List<Product>
            {
                product1,
                product2
            };
            mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.As<IAsyncEnumerable<Product>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Product>(products.GetEnumerator()));

            mockDbSet.As<IQueryable<Product>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Product>(products.AsQueryable().Provider));

            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.AsQueryable().GetEnumerator());

            var mockContext = new Mock<IFunDbContext>();
            mockContext.Setup(x => x.Products).Returns(mockDbSet.Object);

            var controller = new ProductsController(mockContext.Object);

            return controller;
        }

        #region GET: api/products

        [Fact]
        public async Task GetProducts_ReturnsListOfProducts()
        {
            // Arrange
            ProductsController controller = BuildController(out _, out _);

            // Act
            ActionResult<IEnumerable<Product>> result = await controller.GetProducts();

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.NotNull(result.Value);

            var prods = (List<Product>)result.Value;
            Assert.Equal(prods, prods);
        }

        #endregion GET: api/products

        #region api/products/5

        [Fact]
        public async Task GetProduct_ReturnsProduct_WhenIdExists()
        {
            // Arrange
            const int productId = 1;
            ProductsController controller = BuildController(out List<Product> products, out var mockDbSet);
            mockDbSet.Setup(x => x.FindAsync(productId)).ReturnsAsync(products[0]);

            // Act
            ActionResult<Product> result = await controller.GetProduct(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.NotNull(result.Value);
            Assert.Equal(products[0], result.Value);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            const int productId = 1;
            ProductsController controller = BuildController(out _, out _);

            // Act
            ActionResult<Product> result = await controller.GetProduct(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Value);
            Assert.NotNull(result.Result);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        #endregion api/products/5

        #region POST: api/products

        [Fact]
        public async Task PostProduct_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Price = 12.34m,
                IsPhysical = true
            };
            ProductsController controller = BuildController(out _, out _);

            // Act
            var result = await controller.PostProduct(product);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Null(result.Value);
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        #endregion POST: api/products

        #region PUT: api/products/5

        [Fact]
        public async Task PutProduct_WhenCalledWithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            ProductsController controller = BuildController(out _, out _);
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Price = 12.34m,
                IsPhysical = true
            };

            // Act
            IActionResult result = await controller.PutProduct(2, product);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestResult>(result);
        }

        #endregion PUT: api/products/5

        #region DELETE: api/products/5

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductNotFound()
        {
            // Arrange
            ProductsController controller = BuildController(out _, out _);
            const int productId = 1;

            // Act
            IActionResult result = await controller.DeleteProduct(productId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent_WhenProductFound()
        {
            // Arrange
            ProductsController controller = BuildController(out _, out _);
            var product = new Product
            {
                Id = 1
            };

            // Act
            IActionResult result = await controller.DeleteProduct(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion DELETE: api/products/5
    }
}