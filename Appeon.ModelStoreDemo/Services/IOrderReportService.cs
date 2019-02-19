using SnapObjects.Data;
using Appeon.ModelStoreDemo.Models;

namespace Appeon.ModelStoreDemo.Services
{
    public interface IOrderReportService : IServiceBase
    {
        IModelStore<SubCategorySalesReport> RetrieveSubCategorySalesReport(params object[] salesmonth);

    }
}

