using SnapObjects.Data;
using SnapObjects.Data.PowerBuilder;
using Appeon.DataStoreDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Appeon.DataStoreDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SalesOrderController : ControllerBase
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

            packer.AddDataStore("Customer", 
                _saleService.Retrieve("d_dddw_customer_individual"));
            packer.AddDataStore("SalesPerson", 
                _saleService.Retrieve("d_dddw_salesperson"));
            packer.AddDataStore("SalesTerritory", 
                _saleService.Retrieve("d_dddw_salesterritory"));
            packer.AddDataStore("ShipMethod", 
                _saleService.Retrieve("d_dddw_shipmethod"));
            packer.AddDataStore("OrderProduct", 
                _saleService.Retrieve("d_dddw_order_production"));

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

            var orderData = _saleService
                .Retrieve("d_order_header_grid", dateFrom, dateto, customerId);
            if (orderData.RowCount == 0)
            {
                return NotFound();
            }
            packer.AddDataStore("SalesOrderHeader", orderData);

            return packer;
        }

        // POST api/salesorder/RetrieveSaleOrderDetail
        [HttpPost]
        public ActionResult<IDataPacker> RetrieveSaleOrderDetail(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var salesOrderId = unpacker.GetValue<int>("arm1");
            var customerId = unpacker.GetValue<int>("arm2");

            packer.AddDataStore("SalesOrderDetail",
                _saleService.Retrieve("d_order_detail_list", salesOrderId));

            packer.AddDataStore("DddwAddress",
                _saleService.Retrieve("d_dddw_customer_address", customerId));

            packer.AddDataStore("DddwCreditcard",
                _saleService.Retrieve("d_dddw_customer_creditcard", customerId));

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
                    packer.AddDataStore("DddwAddress", 
                        _saleService.Retrieve("d_dddw_customer_address", dddwCodeId));
                    packer.AddDataStore("DddwCreditcard", 
                        _saleService.Retrieve("d_dddw_customer_creditcard", dddwCodeId));
                    break;
            }

            return packer;
        }

        // POST api/salesorder/SaveSalesOrderAndDetail
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SaveSalesOrderAndDetail(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();
            var orderHeader = unPacker.GetDataStore("dw1", "d_order_header_free");
            var orderDetail = unPacker.GetDataStore("dw2", "d_order_detail_list");

            try
            {
                var saleOrderId = _saleService.SaveSalesOrderAndDetail(orderHeader, 
                    orderDetail);

                if (saleOrderId > 0)
                {
                    packer.AddDataStore("SalesOrderHeader", 
                        _saleService.Retrieve("d_order_header_free", saleOrderId));
                    packer.AddDataStore("SalesOrderHeader.SalesOrderDetail", 
                        _saleService.Retrieve("d_order_detail_list", saleOrderId));
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
                        var orderHeader = unpacker.GetDataStore("dw1", "d_order_header_free");
                        status = _saleService.Update(true, orderHeader);

                        packer.AddDataStore("SalesOrderHeader", orderHeader);

                        break;

                    case "SaleOrderDetail":
                        var orderDetail = unpacker.GetDataStore("dw1", "d_order_detail_list");
                        status = _saleService.Update(true, orderDetail);

                        packer.AddValue("SaleOrderDetail.SalesOrderDetail",
                            orderDetail.RowCount);

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
                        status = _saleService.Delete("d_order_detail_list", true,
                            "SalesOrderDetailID = " + saleDetailId.ToString(), saleOrderId);
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