﻿using SnapObjects.Data.PowerBuilder;

namespace Appeon.DataStoreDemo.Services
{
    public interface ISalesOrderService : IServiceBase
    {
        int SaveSalesOrderAndDetail(IDataStore salesOrderHeaders, IDataStore salesOrderDetails);

        string DeleteSalesOrder(int saleOrderId);

    }
}
