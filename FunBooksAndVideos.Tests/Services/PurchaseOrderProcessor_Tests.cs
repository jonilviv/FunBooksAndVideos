using FunBooksAndVideos.Entities;
using FunBooksAndVideos.Services;
using Xunit;

namespace FunBooksAndVideos.Tests.Services
{
    public sealed class PurchaseOrderProcessor_Tests
    {
        [Fact]
        public void Process_WhenCalledWithInvalidPurchaseOrder_ShouldReturnErrorMessage()
        {
            // Arrange
            var purchaseOrder = new PurchaseOrder();
            var sut = new PurchaseOrderProcessor();

            // Act
            ProcessResult result = sut.Process(purchaseOrder);

            // Assert
            Assert.False(result.Successful);
            Assert.Equal("The method or operation is not implemented.", result.ErrorMessage);
        }
    }
}