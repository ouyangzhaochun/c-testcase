using SnapObjects.Data;
using Appeon.ModelMapperDemo.Models;
using Appeon.ModelMapperDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Appeon.ModelMapperDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class SalesOrderController : ControllerBase
    {
        private readonly ISalesOrderService _saleService;
        private readonly IGenericServiceFactory _genericServices;       

        public SalesOrderController(ISalesOrderService saleService,
                                    IGenericServiceFactory genericServiceFactory)
        {
            _saleService = saleService;
            _genericServices = genericServiceFactory;
        }

        // GET api/SalesOrder/WinOpen
        [HttpGet]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> WinOpen()
        {
            var packer = new DataPacker();

            try
            {
                packer.AddModels("Customer",
                _genericServices.Get<DdCustomer>().Retrieve(false));
                packer.AddModels("SalesPerson",
                    _genericServices.Get<DdSalesPerson>().Retrieve(false));
                packer.AddModels("SalesTerritory",
                    _genericServices.Get<DdSalesTerritory>().Retrieve(false));
                packer.AddModels("ShipMethod",
                    _genericServices.Get<DdShipMethod>().Retrieve(false));
                packer.AddModels("OrderProduct",
                    _genericServices.Get<DdOrderProduct>().Retrieve(false));
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }            

            return packer;
        }

        // POST api/SalesOrder/RetrieveSaleOrderList
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> RetrieveSaleOrderList(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();

            var customerId = unPacker.GetValue<int>("arm1");
            var dateFrom = DateTime.Parse(unPacker.GetValue<string>("arm2"));
            var dateto = DateTime.Parse(unPacker.GetValue<string>("arm3"));

            try
            {
                packer.AddModels("SalesOrderHeader",
                    _saleService.Retrieve(false, customerId, dateFrom, dateto));
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
            
           
            return packer;
        }

        // POST api/SalesOrder/RetrieveSaleOrderDetail
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> RetrieveSaleOrderDetail(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();

            var salesOrderId = unPacker.GetValue<int>("arm1");
            var customerId = unPacker.GetValue<int>("arm2");

            try
            {
                packer.AddModels("SalesOrderDetail",
                             _genericServices.Get<SalesOrderDetail>()
                             .Retrieve(false, salesOrderId));

                packer.AddModels("DddwAddress",
                                 _genericServices.Get<DdCustomerAddress>()
                                 .Retrieve(false, customerId));

                packer.AddModels("DddwCreditcard",
                                 _genericServices.Get<DdCreditcard>()
                                 .Retrieve(false, customerId));
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return packer;
        }

        // POST api/SalesOrder/RetrieveDropdownModel
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> RetrieveDropdownModel(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();

            var modelName = unPacker.GetValue<string>("arm1");
            var CodeId = unPacker.GetValue<int>("arm2");

            try
            {
                switch (modelName)
                {
                    case "Customer":
                        packer.AddModels("DddwAddress",
                                 _genericServices.Get<DdCustomerAddress>()
                                 .Retrieve(false, CodeId));

                        packer.AddModels("DddwCreditcard",
                                         _genericServices.Get<DdCreditcard>()
                                         .Retrieve(false, CodeId));
                        break;
                }
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return packer;
        }

        // POST api/SalesOrder/SaveSalesOrderAndDetail
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SaveSalesOrderAndDetail(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();
            var saleOrderId = 0;

            var orderHeader = unPacker.GetModelEntries<SalesOrder>("dw1")
                              .FirstOrDefault();
            var orderDetail = unPacker.GetModelEntries<SalesOrderDetail>("dw2");

            try
            {
                saleOrderId = _saleService.SaveSalesOrderAndDetail(orderHeader, orderDetail);
                packer.AddValue("Status", "Success");
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }            

            var SaleOrder = _saleService.RetrieveByKey(true, saleOrderId);
            packer.AddModel("SalesOrderHeader", SaleOrder)
                  .Include("SalesOrderDetail", m => m.OrderDetails);

            return packer;
        }

        // POST api/SalesOrder/SaveChanges
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SaveChanges(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();
            var modelname = unPacker.GetValue<string>("arm1");

            try
            {
                switch (modelname)
                {
                    case "SaleOrderHeader":
                        var orderHeader = unPacker.GetModelEntries<SalesOrder>("dw1")
                                          .FirstOrDefault();
                        var salesOrderId = _saleService.SaveSalesOrderHeader(orderHeader);

                        packer.AddModel("SalesOrderHeader", _saleService
                                        .RetrieveByKey(false, salesOrderId));
                        break;

                    case "SaleOrderDetail":
                        var orderDetail = unPacker.GetModelEntries<SalesOrderDetail>("dw1");
                        var dModel = _genericServices.Get<SalesOrderDetail>()
                                    .SaveChanges(orderDetail);

                        packer.AddValue("SalesOrderHeader.SalesOrderDetail",
                                        dModel.AffectedCount);
                        break;
                }
                packer.AddValue("Status", "Success");
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return packer;
        }

        // Delete api/SalesOrder/DeleteSalesOrderByKey
        [HttpDelete]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> DeleteSalesOrderByKey(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();
            var modelName = unPacker.GetValue<string>("arm1");
            var saleOrderId = unPacker.GetValue<int>("arm2");
            
            try
            {
                switch (modelName)
                {
                    case "SaleOrder":
                        var orderDelete = _genericServices.Get<SalesOrder>()
                                          .DeleteByKey(saleOrderId);
                        break;

                    case "OrderDetail":
                        var saleDetailId = unPacker.GetValue<int>("arm3");
                        var detailDelete = _genericServices.Get<SalesOrderDetail>()
                            .DeleteByKey(saleOrderId, saleDetailId);
                        break;
                }
                packer.AddValue("Status", "Success");
            }            
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return packer;
        }

        // Delete api/SalesOrder/DeleteSalesOrderByStatus
        [HttpDelete]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> DeleteSalesOrderByStatus(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();
            int[] status = new int[3];

            status[1] = unpacker.GetValue<int>("arm1");
            status[2] = unpacker.GetValue<int>("arm2");
            status[3] = unpacker.GetValue<int>("arm3");

            try
            {
                var ret = _saleService.DeleteBuilder(status);
                packer.AddValue("Status", "Success");
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return packer;
        }
    }
}