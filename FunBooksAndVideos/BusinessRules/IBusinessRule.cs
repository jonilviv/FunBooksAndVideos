using FunBooksAndVideos.Entities;

namespace FunBooksAndVideos.BusinessRules
{
    public interface IBusinessRule
    {
        void ProcessPurchaseOrder(PurchaseOrder purchaseOrder);
    }
}