using FunBooksAndVideos.Controllers;
using FunBooksAndVideos.Data;
using FunBooksAndVideos.Entities;
using FunBooksAndVideos.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.AspNetCore.Routing;

namespace FunBooksAndVideos.Tests.Controllers
{
    public sealed class PurchaseOrdersController_Tests
    {
        private static PurchaseOrdersController BuildController(out List<PurchaseOrder> purchaseOrders, out Mock<DbSet<PurchaseOrder>> purchaseOrderMmockDbSet)
        {
            var customer1 = new Customer
            {
                Id = 1,
                Name = "Customer One",
                Email = "cutomer1@test.com",
                MembershipType = MembershipType.Regular,
                Address = "123 Main St, Anytown, USA"
            };

            var customer2 = new Customer
            {
                Id = 2,
                Name = "Customer Two",
                Email = "cutomer2@test.com",
                MembershipType = MembershipType.Premium,
                Address = "456 Elm Avenue, Springfield, IL 67890"
            };

            Product product11 = null;
            Product product12 = null;
            var products1 = new List<Product>
             {
                 product11,
                  product12
             };

            var purchaseOrder1 = new PurchaseOrder
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                TotalPrice = 12.34m,
                Customer = customer1,
                Products = products1,
                //ShippingSlip  =     ,
            };

            List<Product> products2 = null;
            var purchaseOrder2 = new PurchaseOrder
            {
                Id = 2,
                CreatedAt = DateTime.UtcNow,
                TotalPrice = 23.45m,
                Customer = customer2,
                Products = products2,
                //ShippingSlip  =     ,
            };
            purchaseOrders = new List<PurchaseOrder>
            {
                purchaseOrder1,
                purchaseOrder2
            };
            purchaseOrderMmockDbSet = new Mock<DbSet<PurchaseOrder>>();

            purchaseOrderMmockDbSet.As<IAsyncEnumerable<PurchaseOrder>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<PurchaseOrder>(purchaseOrders.GetEnumerator()));

            purchaseOrderMmockDbSet.As<IQueryable<PurchaseOrder>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<PurchaseOrder>(purchaseOrders.AsQueryable().Provider));

            purchaseOrderMmockDbSet.As<IQueryable<PurchaseOrder>>().Setup(m => m.Expression).Returns(purchaseOrders.AsQueryable().Expression);
            purchaseOrderMmockDbSet.As<IQueryable<PurchaseOrder>>().Setup(m => m.ElementType).Returns(purchaseOrders.AsQueryable().ElementType);
            purchaseOrderMmockDbSet.As<IQueryable<PurchaseOrder>>().Setup(m => m.GetEnumerator()).Returns(purchaseOrders.AsQueryable().GetEnumerator());

            var mockContext = new Mock<IFunDbContext>();
            mockContext.Setup(x => x.PurchaseOrders).Returns(purchaseOrderMmockDbSet.Object);
            // mockContext.Setup(x=> x.Customers).Returns();
            // mockContext.Setup(x=> x.Products).Returns();

            var mockProcessor = new Mock<IPurchaseOrderProcessor>();

            var controller = new PurchaseOrdersController(mockContext.Object, mockProcessor.Object);

            return controller;
        }

        #region GET: api/purchase-orders

        [Fact]
        public async Task GetPurchaseOrders_ShouldReturnPurchaseOrders()
        {
            // Arrange
            PurchaseOrdersController controller = BuildController(out List<PurchaseOrder> purchaseOrders, out _);

            // Act
            ActionResult<IEnumerable<PurchaseOrder>> result = await controller.GetPurchaseOrders();

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.NotNull(result.Value);
            Assert.Equal(purchaseOrders, result.Value);
        }

        #endregion GET: api/purchase-orders

        #region GET: api/purchase-orders/5

        [Fact]
        public async Task GetPurchaseOrder_ReturnsNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            PurchaseOrdersController controller = BuildController(out List<PurchaseOrder> purchaseOrders, out _);
            int id = 0;

            // Act
            var result = await controller.GetPurchaseOrder(id);

            // Assertl
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetPurchaseOrder_ReturnsPurchaseOrder_WhenIdExists()
        {
            // Arrange
            PurchaseOrdersController controller = BuildController(out List<PurchaseOrder> purchaseOrders, out _);
            int id = 1;

            // Act
            var result = await controller.GetPurchaseOrder(id);

            // Assert
            Assert.IsType<PurchaseOrder>(result);
        }

        #endregion GET: api/purchase-orders/5
    }
}