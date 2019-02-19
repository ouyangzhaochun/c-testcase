using SnapObjects.Data;
using Appeon.ModelMapperDemo.Models;
using System;
using System.Collections.Generic;

namespace Appeon.ModelMapperDemo.Services
{
    public class SalesOrderService : ServiceBase<SalesOrder>, ISalesOrderService
    {
        public SalesOrderService(OrderContext context)
            :base(context)
        {
        }
        
        public IDbResult DeleteBuilder(params object[] parameters)
        {
            SalesOrder header = new SalesOrder();

            SqlDeleteBuilder DeleteSql = new SqlDeleteBuilder();
            DeleteSql.Delete("Sales.SalesOrderHeader")
                     .Where("status", SqlBinaryOperator.In,
                      SqlBuilder.Parameter(typeof(int[])));

            return _context.ModelMapper.TrackDelete(header, 
                (saveContext) => {  })
                                       .SaveChanges();

        }

        public int SaveSalesOrderHeader(IModelEntry<SalesOrder> header)
        {
            var master = _context.ModelMapper.TrackMaster(header)
                                             .MasterModel;

            _context.ModelMapper.SaveChanges();

            return master.SalesOrderID;
        }

        public int SaveSalesOrderAndDetail(IModelEntry<SalesOrder> header,
                        IEnumerable<IModelEntry<SalesOrderDetail>> details)
        {
            var master = _context.ModelMapper.TrackMaster(header)
                                             .TrackDetails(m => m.OrderDetails, details)
                                             .MasterModel;

            _context.ModelMapper.SaveChanges();

            return master.SalesOrderID;
        }

    }
}
