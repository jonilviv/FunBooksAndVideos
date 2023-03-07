using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FunBooksAndVideos.Entities
{
    [ExcludeFromCodeCoverage]
    public record PurchaseOrder
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public decimal TotalPrice { get; set; }

        public Customer Customer { get; set; }

        public List<Product> Products { get; set; }

        public ShippingSlip ShippingSlip { get; set; }
    }
}