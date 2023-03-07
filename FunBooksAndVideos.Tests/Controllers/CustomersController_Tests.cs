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
    public sealed class CustomersController_Tests
    {
        private static CustomersController BuildController(out List<Customer> customers, out Mock<DbSet<Customer>> mockDbSet)
        {
            var customer1 = new Customer
            {
                Id = 1,
                Name = "John Doe",
                Email = "jd@test.com",
                MembershipType = MembershipType.Regular,
                Address = "123 Main St, Anytown, USA"
            };
            var customer2 = new Customer
            {
                Id = 2,
                Name = "Kate Doe",
                Email = "kd@test.com",
                MembershipType = MembershipType.Premium,
                Address = "456 Elm Avenue, Springfield, IL 67890"
            };
            customers = new List<Customer>
            {
                customer1,
                customer2
            };
            mockDbSet = new Mock<DbSet<Customer>>();

            mockDbSet.As<IAsyncEnumerable<Customer>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Customer>(customers.GetEnumerator()));

            mockDbSet.As<IQueryable<Customer>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Customer>(customers.AsQueryable().Provider));

            mockDbSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customers.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customers.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(customers.AsQueryable().GetEnumerator());

            var mockContext = new Mock<IFunDbContext>();
            mockContext.Setup(x => x.Customers).Returns(mockDbSet.Object);

            var controller = new CustomersController(mockContext.Object);

            return controller;
        }

        #region GET: api/customers

        [Fact]
        public async Task GetCustomers_ReturnsListOfCustomers()
        {
            // Arrange
            CustomersController controller = BuildController(out List<Customer> customers, out _);

            // Act
            var result = await controller.GetCustomers();

            // Assert
            Assert.Null(result.Result);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());

            Customer[] customersResult = (Customer[])result.Value;

            Assert.Equal(customers[0], customersResult[0]);
            Assert.Equal(customers[1], customersResult[1]);
        }


        #endregion GET: api/customers

        #region GET: api/customers/5

        [Fact]
        public async Task GetCustomer_ReturnsNotFound_WhenIdNotFound()
        {
            // Arrange
            CustomersController controller = BuildController(out _, out _);
            const int id = -1;

            // Act
            ActionResult<Customer> result = await controller.GetCustomer(id);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCustomer_ReturnsCustomer_WhenIdFound()
        {
            // Arrange
            const int id = 1;
            CustomersController controller = BuildController(out List<Customer> customers, out Mock<DbSet<Customer>> mockCustomerDbSet);
            mockCustomerDbSet.Setup(x => x.FindAsync(id)).ReturnsAsync(customers[0]);

            // Act
            ActionResult<Customer> result = await controller.GetCustomer(id);

            // Assert
            Assert.Null(result.Result);
            Assert.Equal(customers[0], result.Value);
        }

        #endregion GET: api/customers/5

        #region POST: api/customers

        [Fact]
        public async Task PostCustomer_ReturnsCreatedAtActionResult()
        {
            // Arrange
            CustomersController controller = BuildController(out _, out _);
            var customer = new Customer
            {
                Name = "John Smith",
                Email = "john.smith@example.com",
                MembershipType = MembershipType.Premium,
                Address = "123 Main St, Anytown, USA"
            };

            // Act
            ActionResult<Customer> result = await controller.PostCustomer(customer);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Null(result.Value);

            var rs = (CreatedAtActionResult)result.Result;
            Assert.Equal(customer, rs.Value);
        }

        #endregion POST: api/customers

        #region PUT: api/customers/5

        [Fact]
        public async Task PutCustomer_WhenIdDoesNotMatch_ReturnsBadRequest()
        {
            // Arrange
            var customer = new Customer { Id = 1 };
            CustomersController controller = BuildController(out _, out _);

            // Act
            var result = await controller.PutCustomer(2, customer);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        #endregion PUT: api/customers/5

        #region DELETE: api/customers/5

        [Fact]
        public async Task DeleteCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            CustomersController controller = BuildController(out _, out _);
            const int customerId = 1;

            // Act
            IActionResult result = await controller.DeleteCustomer(customerId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsNoContent_WhenCustomerExists()
        {
            // Arrange
            CustomersController controller = BuildController(out List<Customer> customers, out Mock<DbSet<Customer>> mockCustomerDbSet);
            const int customerId = 1;
            mockCustomerDbSet.Setup(x => x.FindAsync(customerId)).ReturnsAsync(customers[0]);

            // Act
            IActionResult result = await controller.DeleteCustomer(customerId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }


        #endregion DELETE: api/customers/5
    }
}