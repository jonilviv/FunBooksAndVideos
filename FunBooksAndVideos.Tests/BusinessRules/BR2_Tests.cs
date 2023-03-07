using FunBooksAndVideos.BusinessRules;
using FunBooksAndVideos.Entities;
using System;
using System.Collections.Generic;
using Xunit;

namespace FunBooksAndVideos.Tests.BusinessRules
{
    public sealed class BR2_Tests
    {
        [Fact]
        public void ProcessPurchaseOrder_NoPhysicalProducts_NoShippingSlip()
        {
            // Arrange
            var purchaseOrder = new PurchaseOrder
            {
                Products = new List<Product>
                {
                    new Product { IsPhysical = false }
                }
            };
            var br2 = new BR2();

            // Act
            br2.ProcessPurchaseOrder(purchaseOrder);

            // Assert
            Assert.Null(purchaseOrder.ShippingSlip);
        }

        [Fact]
        public void ProcessPurchaseOrder_PhysicalProducts_ShippingSlipCreated()
        {
            // Arrange
            var purchaseOrder = new PurchaseOrder
            {
                Customer = new Customer { Address = "123 Main St." },
                Products = new List<Product>
                {
                    new Product
                    {
                        IsPhysical = true
                    }
                }
            };

            var br2 = new BR2();

            // Act
            br2.ProcessPurchaseOrder(purchaseOrder);

            // Assert
            Assert.NotNull(purchaseOrder.ShippingSlip);
            Assert.Equal("123 Main St.", purchaseOrder.ShippingSlip.DeliveryAddress);
            Assert.True(purchaseOrder.ShippingSlip.CreatedAt <= DateTime.UtcNow);
        }
    }
}