using System.Diagnostics.CodeAnalysis;

namespace FunBooksAndVideos.Entities
{
    [ExcludeFromCodeCoverage]
    public record Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public bool IsPhysical { get; set; }
    }
}