using SnapObjects.Data.PowerBuilder;

namespace Appeon.DataStoreDemo.Services
{
    public interface IOrderReportService : IServiceBase
    {
        IDataStore RetrieveSubCategorySalesReport(params object[] salesmonth);
    }
}

