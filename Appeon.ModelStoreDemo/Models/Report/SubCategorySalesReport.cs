using SnapObjects.Data;

namespace Appeon.ModelStoreDemo.Models
{
    [SqlParameter("subCategoryId", typeof(int))]
    [SqlParameter("orderMonth1", typeof(string))]
    [SqlParameter("orderMonth2", typeof(string))]
    [SqlParameter("orderMonth3", typeof(string))]
    [SqlParameter("orderMonth4", typeof(string))]
    [SqlParameter("orderMonth5", typeof(string))]
    [SqlParameter("orderMonth6", typeof(string))]
    [FromTable("ProductSubcategory", Schema = "Production")]
    [SqlWhere("ProductSubcategoryID = :subCategoryId")]
    public class SubCategorySalesReport
    {
        public string Name { get; set; }
        public int SalesqtyMonth1 { get; set; }
        public decimal SalesRoomMonth1 { get; set; }
        public int SalesqtyMonth2 { get; set; }
        public decimal SalesRoomMonth2 { get; set; }
        public int SalesqtyMonth3 { get; set; }
        public decimal SalesRoomMonth3 { get; set; }
        public int SalesqtyMonth4 { get; set; }
        public decimal SalesRoomMonth4 { get; set; }
        public int SalesqtyMonth5 { get; set; }
        public decimal SalesRoomMonth5 { get; set; }
        public int SalesqtyMonth6 { get; set; }
        public decimal SalesRoomMonth6 { get; set; }      
    }
}
