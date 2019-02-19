using SnapObjects.Data;
using Appeon.ModelStoreDemo.Models;
using System;
using System.Linq;

namespace Appeon.ModelStoreDemo.Services
{
    /// <summary>
    /// This Service needs to be injected into the ConfigureServices method of the Startup class. Sample code as follows:
    /// services.AddScoped<ISalesOrderService, SalesOrderService>();
    /// </summary>
    public class SalesOrderService : ServiceBase, ISalesOrderService
    {
        public SalesOrderService(OrderContext context)
            : base(context)
        {
        }
        public string DeleteSalesOrder(int saleOrderId)
        {
            string status = "Success";

            _context.BeginTransaction();
            status = Delete<SalesOrderDetail>(false, saleOrderId);
            status = Delete<SalesOrderHeader>(false, saleOrderId);
            _context.Commit();

            return status;

        }

        public int SaveSalesOrderAndDetail(IModelStore<SalesOrderHeader> salesOrderHeaders,
                                           IModelStore<SalesOrderDetail> salesOrderDetails)
        {
            int intSalesOrderId = 0;

            _context.BeginTransaction();

            salesOrderHeaders.SaveChanges(_context);                

            intSalesOrderId = salesOrderHeaders.FirstOrDefault().SalesOrderID;

            SetPrimaryKey(salesOrderHeaders, salesOrderDetails);
            salesOrderDetails.SaveChanges(_context);

            _context.Commit();

            return intSalesOrderId;
        }

        private void SetPrimaryKey(IModelStore<SalesOrderHeader> salesOrderHeaders,
                                   IModelStore<SalesOrderDetail> salesOrderDetails)
        {
            if (salesOrderHeaders.TrackedCount(StateTrackable.Deleted) == 0 &&
                salesOrderHeaders.Count > 0)
            {
                var salesOrderId = salesOrderHeaders.FirstOrDefault().SalesOrderID;

                for (int i = 0; i < salesOrderDetails.Count; i++)
                {
                    if (salesOrderDetails.GetModelState(i) == ModelState.NewModified)
                    {
                        salesOrderDetails.SetValue(i, "SalesOrderID", salesOrderId);
                    }
                }
            }
        }
    }
}
