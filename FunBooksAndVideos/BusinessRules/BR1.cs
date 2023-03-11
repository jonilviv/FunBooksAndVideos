using FunBooksAndVideos.Entities;

namespace FunBooksAndVideos.BusinessRules
{
    public sealed class BR1 : IBusinessRule
    {
        public void ProcessPurchaseOrder(PurchaseOrder purchaseOrder)
        {
            foreach (var product in purchaseOrder.Products)
            {
                if (product.IsMembership)
                {
                    purchaseOrder.Customer.MembershipType = MembershipType.Premium;

                    var br2 = new BR2();
                    br2.ProcessPurchaseOrder(purchaseOrder);

                    return;
                }
            }
        }
    }
}