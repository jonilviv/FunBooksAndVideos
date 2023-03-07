using FunBooksAndVideos.Entities;

namespace FunBooksAndVideos.Services
{
    public interface IPurchaseOrderProcessor
    {
        ProcessResult Process(PurchaseOrder purchaseOrder);
    }
}