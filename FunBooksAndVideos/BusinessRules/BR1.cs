using FunBooksAndVideos.Entities;

namespace FunBooksAndVideos.BusinessRules
{
    public sealed class BR1 : IBusinessRule
    {
        public void ProcessPurchaseOrder(PurchaseOrder purchaseOrder)
        {
            if (purchaseOrder.Customer.MembershipType == MembershipType.Premium)
            {
                //TODO: implement logic, that I don't understand
            }
        }
    }
}