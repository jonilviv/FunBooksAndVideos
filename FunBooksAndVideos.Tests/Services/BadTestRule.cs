using FunBooksAndVideos.BusinessRules;
using FunBooksAndVideos.Entities;
using System;

namespace FunBooksAndVideos.Tests.Services
{
    internal class BadTestRule : IBusinessRule
    {
        public void ProcessPurchaseOrder(PurchaseOrder purchaseOrder)
        {
            throw new NotImplementedException();
        }
    }
}