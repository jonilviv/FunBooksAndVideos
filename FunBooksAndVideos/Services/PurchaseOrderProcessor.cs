using FunBooksAndVideos.BusinessRules;
using FunBooksAndVideos.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FunBooksAndVideos.Services
{
    public sealed class PurchaseOrderProcessor : IPurchaseOrderProcessor
    {
        public ProcessResult Process(PurchaseOrder purchaseOrder)
        {
            var result = new ProcessResult();

            Assembly assembly = Assembly.GetCallingAssembly();
            IEnumerable<Type> ruleTypes = assembly
                .GetTypes()
                .Where(t => typeof(IBusinessRule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            try
            {
                foreach (Type ruleType in ruleTypes)
                {
                    var rule = (IBusinessRule)Activator.CreateInstance(ruleType);
                    rule.ProcessPurchaseOrder(purchaseOrder);
                }

                result.Successful = true;
            }
            catch (Exception exception)
            {
                result.Successful = false;
                result.ErrorMessage = exception.Message;
            }

            return result;
        }
    }
}