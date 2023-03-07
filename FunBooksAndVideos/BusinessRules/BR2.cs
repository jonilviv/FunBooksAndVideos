using FunBooksAndVideos.Entities;
using System;
using System.Linq;

namespace FunBooksAndVideos.BusinessRules
{
    public sealed class BR2 : IBusinessRule
    {
        public void ProcessPurchaseOrder(PurchaseOrder purchaseOrder)
        {
            bool needsShippingSlip = purchaseOrder.Products.Any(product => product.IsPhysical);

            if (!needsShippingSlip)
            {
                return;
            }

            var shippingSlip = new ShippingSlip
            {
                CreatedAt = DateTime.UtcNow,
                DeliveryAddress = purchaseOrder.Customer.Address
            };

            purchaseOrder.ShippingSlip = shippingSlip;
        }
    }
}