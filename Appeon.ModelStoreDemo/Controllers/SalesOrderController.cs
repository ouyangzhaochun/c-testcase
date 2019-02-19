using SnapObjects.Data;
using Appeon.ModelStoreDemo.Models;
using Appeon.ModelStoreDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Appeon.ModelStoreDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SalesOrderController : Controller
    {
        private readonly ISalesOrderService _saleService;

        public SalesOrderController(ISalesOrderService saleService)
        {
            _saleService = saleService;
        }

        // GET api/salesorder/WinOpen
        [HttpGet]
        public ActionResult<IDataPacker> WinOpen()
        {
            var packer = new DataPacker();

            packer.AddModelStore("Customer", 
                _saleService.Retrieve<DdCustomer>());
            packer.AddModelStore("SalesPerson", 
                _saleService.Retrieve<DdSalesPerson>());
            packer.AddModelStore("SalesTerritory", 
                _saleService.Retrieve<DdSalesTerritory>());
            packer.AddModelStore("ShipMethod", 
                _saleService.Retrieve<DdShipMethod>());
            packer.AddModelStore("OrderProduct", 
                _saleService.Retrieve<DdOrderProduct>());

            return packer;
        }

        // POST api/salesorder/RetrieveSaleOrderList
        [HttpPost]
        [ProducesResponseType(404)]
        public ActionResult<IDataPacker> RetrieveSaleOrderList(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var customerId = unpacker.GetValue<int>("arm1");
            var dateFrom = DateTime.Parse(unpacker.GetValue<string>("arm2"));
            var dateto = DateTime.Parse(unpacker.GetValue<string>("arm3"));

            var orderData = _saleService.Retrieve<SalesOrderHeaderList>
                             (customerId, dateFrom, dateto);

            if (orderData.Count == 0)
            {
                return NotFound();
            }
            
            packer.AddModelStore("SalesOrderHeader", orderData);

            return packer;
        }

        // POST api/salesorder/RetrieveSaleOrderDetail
        [HttpPost]
        public ActionResult<IDataPacker> RetrieveSaleOrderDetail(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var salesOrderId = unpacker.GetValue<int>("arm1");
            var customerId = unpacker.GetValue<int>("arm2");

            packer.AddModelStore("SalesOrderDetail",
                _saleService.Retrieve<SalesOrderDetail>(salesOrderId));

            packer.AddModelStore("DddwAddress",
                _saleService.Retrieve<DdCustomerAddress>(customerId));

            packer.AddModelStore("DddwCreditcard",
                _saleService.Retrieve<DdCreditcard>(customerId));

            return packer;
        }

        // POST api/salesorder/RetrieveDropdownModel
        [HttpPost]
        public ActionResult<IDataPacker> RetrieveDropdownModel(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var dddwName = unpacker.GetValue<string>("arm1");
            var dddwCodeId = unpacker.GetValue<int>("arm2");

            switch (dddwName)
            {
                case "Customer":
                    packer.AddModelStore("DddwAddress",
                        _saleService.Retrieve<DdCustomerAddress>(dddwCodeId));
                    packer.AddModelStore("DddwCreditcard",
                        _saleService.Retrieve<DdCreditcard>(dddwCodeId));
                    break;
            }

            return packer;
        }

        // POST api/salesorder/SaveSalesOrderAndDetail
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SaveSalesOrderAndDetail(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var orderHeader = unpacker.GetModelStore<SalesOrderHeader>("dw1",
                                  ChangeTrackingStrategy.PropertyState);

            var orderDetail = unpacker.GetModelStore<SalesOrderDetail>("dw2",
                                  ChangeTrackingStrategy.PropertyState);

            try
            {
                var saleOrderId = _saleService.SaveSalesOrderAndDetail(orderHeader,
                    orderDetail);

                if (saleOrderId > 0)
                {
                    packer.AddModelStore("SalesOrderHeader",
                        _saleService.Retrieve<SalesOrderHeader>(saleOrderId));
                    packer.AddModelStore("SalesOrderHeader.SalesOrderDetail",
                        _saleService.Retrieve<SalesOrderDetail>(saleOrderId));
                }
                packer.AddValue("Status", "Success");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return packer;
        }

        // POST api/salesorder/SaveChanges
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SaveChanges(IDataUnpacker unpacker)
        {
            string status = "Success";
            var packer = new DataPacker();
            var modelname = unpacker.GetValue<string>("arm1");

            try
            {
                switch (modelname)
                {
                    case "SaleOrderHeader":
                        var orderHeader = unpacker.GetModelStore<SalesOrderHeader>
                            ("dw1", ChangeTrackingStrategy.PropertyState);
                        status = _saleService.Update(true, orderHeader);
                        packer.AddModelStore("SalesOrderHeader", orderHeader);

                        break;

                    case "SaleOrderDetail":
                        var orderDetail = unpacker.GetModelStore<SalesOrderDetail>
                            ("dw1", ChangeTrackingStrategy.PropertyState);
                        status = _saleService.Update(true, orderDetail);

                        packer.AddValue("SaleOrderDetail.SalesOrderDetail", 
                            orderDetail.Count);

                        break;
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            packer.AddValue("Status", status);

            return packer;
        }

        // DELETE api/salesorder/DeleteSalesOrderByKey
        [HttpDelete]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> DeleteSalesOrderByKey(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();
            var modelName = unpacker.GetValue<string>("arm1");
            var saleOrderId = unpacker.GetValue<int>("arm2");
            var status = "Success";

            try
            {
                switch (modelName)
                {
                    case "SaleOrder":
                        status = _saleService.DeleteSalesOrder(saleOrderId);
                        break;

                    case "OrderDetail":
                        var saleDetailId = unpacker.GetValue<int>("arm3");
                        status = _saleService.Delete<SalesOrderDetail>(true,
                            m => m.SalesOrderDetailID == saleDetailId, saleOrderId);
                        break;
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            packer.AddValue("Status", status);

            return packer;
        }
    }
}