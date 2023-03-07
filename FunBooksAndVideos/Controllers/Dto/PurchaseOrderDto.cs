using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FunBooksAndVideos.Controllers.Dto
{
    [ExcludeFromCodeCoverage]
    public record PurchaseOrderDto
    {
        public int CustomerId { get; set; }

        public List<int> ProductIds { get; set; }
    }
}