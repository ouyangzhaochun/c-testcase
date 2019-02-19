using SnapObjects.Data;
using Appeon.ModelStoreDemo.Models;
using Appeon.ModelStoreDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Appeon.ModelStoreDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderReportController : Controller
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

            packer.AddModelStore("Category", 
                _reportService.Retrieve<Category>());
            packer.AddModelStore("SubCategory", 
                _reportService.Retrieve<SubCategory>(cateId));

            return packer;
        }

        // POST api/OrderReport/CategorySalesReport
        [HttpPost]
        public ActionResult<IDataPacker> CategorySalesReport(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var dataFrom = DateTime.Parse(unpacker.GetValue<string>("arm1"));
            var dataTo = DateTime.Parse(unpacker.GetValue<string>("arm2"));
            var lastDataFrom = DateTime.Parse(unpacker
                .GetValue<string>("arm1")).AddYears(-1);
            var lastDataTo = DateTime.Parse(unpacker
                .GetValue<string>("arm2")).AddYears(-1);

            packer.AddModelStore("Category.SalesReport",
                _reportService.Retrieve<CategorySalesReport_D>(dataFrom, dataTo));

            packer.AddModelStore("Category.LastYearSalesReport",
                _reportService.Retrieve<CategorySalesReport_D>(lastDataFrom, 
                lastDataTo));

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

            var SalesReport = _reportService
                .RetrieveSubCategorySalesReport(yearMonth);
            var ProductReport = _reportService
                .Retrieve<ProductSalesReport>(subCategoryId, fromDate, toDate);

            packer.AddModelStore("SalesReport", SalesReport);
            packer.AddModelStore("ProductReport", ProductReport);

            return packer;
        }
    }
}
