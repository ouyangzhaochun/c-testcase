using SnapObjects.Data;
using SnapObjects.Data.PowerBuilder;
using Appeon.DataStoreDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Appeon.DataStoreDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderReportController : ControllerBase
    {
        private readonly IOrderReportService _reportService;

        public OrderReportController(IOrderReportService reportService)
        {
            _reportService = reportService;
        }

        // GET api/OrderReport/WinOpen
        [HttpGet]
        public ActionResult<IDataPacker> WinOpen()
        {
            var packer = new DataPacker();
            int cateId = 0;

            packer.AddDataStore("Category",
                _reportService.Retrieve("d_dddw_category"));
            packer.AddDataStore("SubCategory",
                _reportService.Retrieve("d_subcategory", cateId));

            return packer;
        }

        // POST api/OrderReport/CategorySalesReport
        [HttpPost]
        public ActionResult<IDataPacker> CategorySalesReport(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var dataFrom = DateTime.Parse(unpacker.GetValue<string>("arm1"));
            var dataTo = DateTime.Parse(unpacker.GetValue<string>("arm2"));
            var lastDataFrom = DateTime.Parse(unpacker.GetValue<string>("arm1"))
                .AddYears(-1);
            var lastDataTo = DateTime.Parse(unpacker.GetValue<string>("arm2"))
                .AddYears(-1);

            packer.AddDataStore("Category.SalesReport",
                _reportService.Retrieve("d_categorysalesreport_d", dataFrom, dataTo));

            packer.AddDataStore("Category.LastYearSalesReport",
                _reportService.Retrieve("d_categorysalesreport_d", 
                lastDataFrom, lastDataTo));

            return packer;
        }

        // POST api/OrderReport/SalesReportByMonth
        [HttpPost]
        public ActionResult<IDataPacker> SalesReportByMonth(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var subCategoryId = unpacker.GetValue<int>("arm1");
            var salesYear = unpacker.GetValue<string>("arm2");
            var halfYear = unpacker.GetValue<string>("arm3");

            var fromDate = DateTime.Parse(halfYear == "first" ? 
                salesYear + "-01-01" : salesYear + "-07-01");
            var toDate = DateTime.Parse(halfYear == "first" ? 
                salesYear + "-06-30" : salesYear + "-12-31");
            object[] yearMonth = new object[7];

            yearMonth[0] = subCategoryId;
            for (int month = 1; month < 7; month++)
            {
                yearMonth[month] = halfYear == "first" ? 
                    salesYear + string.Format("{0:00}", month) 
                    : salesYear + string.Format("{0:00}", (month + 6));
            }

            var SalesReport = _reportService.RetrieveSubCategorySalesReport(yearMonth);
            var ProductReport = _reportService.Retrieve("d_productsalesreport", 
                subCategoryId, fromDate, toDate);

            packer.AddDataStore("SalesReport", SalesReport);
            packer.AddDataStore("ProductReport", ProductReport);

            return packer;
        }
    }
}
