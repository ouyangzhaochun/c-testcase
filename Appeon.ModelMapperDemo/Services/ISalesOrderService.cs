using SnapObjects.Data;
using Appeon.ModelMapperDemo.Models;
using System.Collections.Generic;

namespace Appeon.ModelMapperDemo.Services
{
    public interface ISalesOrderService
    {
        IList<SalesOrder> Retrieve(bool includeEmbedded, params object[] parameters);
        
        SalesOrder RetrieveByKey(bool includeEmbedded, params object[] parameters);

        IDbResult DeleteBuilder(params object[] parameters);

        int SaveSalesOrderHeader(IModelEntry<SalesOrder> header);

        int SaveSalesOrderAndDetail(IModelEntry<SalesOrder> header,  
                                    IEnumerable<IModelEntry<SalesOrderDetail>> details);

    }
}
