using SnapObjects.Data;
using Appeon.ModelStoreDemo.Models;

namespace Appeon.ModelStoreDemo.Services
{
    /// <summary>
    /// This Service needs to be injected into the ConfigureServices method of the Startup class. Sample code as follows:
    /// services.AddScoped<IOrderReportService, OrderReportService>();
    /// </summary>
    public class OrderReportService : ServiceBase, IOrderReportService

    {
        public OrderReportService(OrderContext context)
            : base(context)
        {
        }

        public IModelStore<SubCategorySalesReport> RetrieveSubCategorySalesReport(params object[] salesmonth)
        {

            var OrderReportMonth1 = Retrieve<SubCategorySalesReport_D>(salesmonth[0], salesmonth[1]);
            var OrderReportMonth2 = Retrieve<SubCategorySalesReport_D>(salesmonth[0], salesmonth[2]);
            var OrderReportMonth3 = Retrieve<SubCategorySalesReport_D>(salesmonth[0], salesmonth[3]);
            var OrderReportMonth4 = Retrieve<SubCategorySalesReport_D>(salesmonth[0], salesmonth[4]);
            var OrderReportMonth5 = Retrieve<SubCategorySalesReport_D>(salesmonth[0], salesmonth[5]);
            var OrderReportMonth6 = Retrieve<SubCategorySalesReport_D>(salesmonth[0], salesmonth[6]);

            var modelStore = new ModelStore<SubCategorySalesReport>();
            var subCategorySalesReport = new SubCategorySalesReport();

            if (OrderReportMonth1.Count > 0)
            {
                subCategorySalesReport.Name = OrderReportMonth1.GetValue<string>(0, "SubcategoryName");

                subCategorySalesReport.SalesqtyMonth1 = OrderReportMonth1.GetValue<int?>(0, "TotalSalesqty") ?? 0;
                subCategorySalesReport.SalesRoomMonth1 = OrderReportMonth1.GetValue<decimal?>(0, "TotalSaleroom") ?? 0;
            }

            if (OrderReportMonth2.Count > 0)
            {
                subCategorySalesReport.SalesqtyMonth2 = OrderReportMonth2.GetValue<int?>(0, "TotalSalesqty") ?? 0;
                subCategorySalesReport.SalesRoomMonth2 = OrderReportMonth2.GetValue<decimal?>(0, "TotalSaleroom") ?? 0;
            }

            if (OrderReportMonth3.Count > 0)
            {
                subCategorySalesReport.SalesqtyMonth3 = OrderReportMonth3.GetValue<int?>(0, "TotalSalesqty") ?? 0;
                subCategorySalesReport.SalesRoomMonth3 = OrderReportMonth3.GetValue<decimal?>(0, "TotalSaleroom") ?? 0;
            }

            if (OrderReportMonth4.Count > 0)
            {
                subCategorySalesReport.SalesqtyMonth4 = OrderReportMonth4.GetValue<int?>(0, "TotalSalesqty") ?? 0;
                subCategorySalesReport.SalesRoomMonth4 = OrderReportMonth4.GetValue<decimal?>(0, "TotalSaleroom") ?? 0;
            }

            if (OrderReportMonth5.Count > 0)
            {
                subCategorySalesReport.SalesqtyMonth5 = OrderReportMonth5.GetValue<int?>(0, "TotalSalesqty") ?? 0;
                subCategorySalesReport.SalesRoomMonth5 = OrderReportMonth5.GetValue<decimal?>(0, "TotalSaleroom") ?? 0;
            }

            if (OrderReportMonth6.Count > 0)
            {
                subCategorySalesReport.SalesqtyMonth6 = OrderReportMonth6.GetValue<int?>(0, "TotalSalesqty") ?? 0;
                subCategorySalesReport.SalesRoomMonth6 = OrderReportMonth6.GetValue<decimal?>(0, "TotalSaleroom") ?? 0;
            }

            modelStore.Add(subCategorySalesReport);

            return modelStore;
        }
    }
}
