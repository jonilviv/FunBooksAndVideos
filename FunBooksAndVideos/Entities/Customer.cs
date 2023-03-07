using System.Diagnostics.CodeAnalysis;

namespace FunBooksAndVideos.Entities
{
    [ExcludeFromCodeCoverage]
    public record Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public MembershipType MembershipType { get; set; }

        public string Address { get; set; }
    }
}