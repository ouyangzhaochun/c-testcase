using SnapObjects.Data;
using Appeon.ModelMapperDemo.Models;
using Appeon.ModelMapperDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Appeon.ModelMapperDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderReportController : ControllerBase
    {
        private readonly IOrderReportService _reportService;
        private readonly IGenericServiceFactory _genericServices;

        public OrderReportController(IOrderReportService reportService,
                                     IGenericServiceFactory genericServiceFactory)
        {
            _reportService = reportService;
            _genericServices = genericServiceFactory;
        }

        // GET api/OrderReport/WinOpen
        [HttpGet]
        [ProducesResponseType(404)]
        public ActionResult<IDataPacker> WinOpen()
        {
            var packer = new DataPacker();
            int cateId = 0;

            var category = _genericServices.Get<Category>().Retrieve(false);
            var subCategory = _genericServices.Get<SubCategory>()
                              .Retrieve(false, cateId);

            if(category.Count == 0 || subCategory.Count == 0)
            {
                return NotFound();
            }

            packer.AddModels("Category", category);
            packer.AddModels("SubCategory", subCategory);

            return packer;
        }

        // POST api/OrderReport/CategorySalesReport
        [HttpPost]
        [ProducesResponseType(404)]
        public ActionResult<IDataPacker> CategorySalesReport(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var dataFrom = DateTime.Parse(unpacker.GetValue<string>("arm1"));
            var dataTo = DateTime.Parse(unpacker.GetValue<string>("arm2"));
            var lastDataFrom = DateTime.Parse(unpacker.GetValue<string>("arm1"))
                                       .AddYears(-1);
            var lastDataTo = DateTime.Parse(unpacker.GetValue<string>("arm2"))
                                     .AddYears(-1);
            var master = new CategorySalesReport();

            var CategoryReport = _reportService.RetrieveCategorySalesReport(
                                 master, dataFrom, dataTo,lastDataFrom, lastDataTo);

            if(CategoryReport.SalesReportByCategory.Count == 0)
            {
                return NotFound();
            }

            packer.AddModel("Category", CategoryReport)
                  .Include("SalesReport", m => m.SalesReportByCategory)
                  .Include("LastYearSalesReport", m => m.LastYearSalesReportByCategory);

            return packer;
        }

        // POST api/OrderReport/SalesReportByMonth
        [HttpPost]
        [ProducesResponseType(404)]
        public ActionResult<IDataPacker> SalesReportByMonth(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var subCategoryId = unpacker.GetValue<int>("arm1");
            var salesYear = unpacker.GetValue<string>("arm2");
            var halfYear = unpacker.GetValue<string>("arm3");

            var fromDate = DateTime.Parse(
                halfYear == "first" ? salesYear + "-01-01" : salesYear + "-07-01");
            var toDate = DateTime.Parse(
                halfYear == "first" ? salesYear + "-06-30" : salesYear + "-12-31");
            object[] yearMonth = new object[7];

            yearMonth[0] = subCategoryId;
            for (int month = 1; month < 7; month++)
            {
                yearMonth[month] = 
                    halfYear == "first" ? salesYear + string.Format("{0:00}", month)
                    : salesYear + string.Format("{0:00}",(month + 6));
            }

            var master = new SubCategorySalesReport();
            var SalesReport = _reportService
                .RetrieveSubCategorySalesReport(master, yearMonth);
            var ProductReport = _reportService
                .Retrieve<ProductSalesReport>(true, subCategoryId, fromDate, toDate);

            if(ProductReport.Count == 0)
            {
                return NotFound();
            }

            IList<SubCategorySalesReport> SalesReports = new List<SubCategorySalesReport>();
            SalesReports.Add(SalesReport);

            packer.AddModels("SalesReport", SalesReports);
            packer.AddModels("ProductReport", ProductReport);

            return packer;
        }
    }
}