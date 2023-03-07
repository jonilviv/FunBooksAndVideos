using System.Diagnostics.CodeAnalysis;

namespace FunBooksAndVideos.Services
{
    [ExcludeFromCodeCoverage]
    public record ProcessResult
    {
        public bool Successful { get; set; }

        public string ErrorMessage { get; set; }
    }
}