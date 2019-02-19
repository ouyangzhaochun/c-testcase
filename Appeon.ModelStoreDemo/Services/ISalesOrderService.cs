using SnapObjects.Data;
using Appeon.ModelStoreDemo.Models;

namespace Appeon.ModelStoreDemo.Services
{
    public interface ISalesOrderService : IServiceBase
    {
        int SaveSalesOrderAndDetail(IModelStore<SalesOrderHeader> salesOrderHeaders,
                                    IModelStore<SalesOrderDetail> salesOrderDetails);

        string DeleteSalesOrder(int saleOrderId);
    }
}
