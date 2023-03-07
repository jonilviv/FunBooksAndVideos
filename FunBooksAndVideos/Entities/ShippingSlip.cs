using System;
using System.Diagnostics.CodeAnalysis;

namespace FunBooksAndVideos.Entities
{
    [ExcludeFromCodeCoverage]
    public record ShippingSlip
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string DeliveryAddress { get; set; }
    }
}