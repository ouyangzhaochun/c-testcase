using SnapObjects.Data;
using System;

namespace Appeon.ModelStoreDemo.Models
{
    [SqlParameter("fromDate", typeof(DateTime))]
    [SqlParameter("toDate", typeof(DateTime))]
    [FromTable("SalesOrderDetail", Schema = "Sales")]
    [FromTable(alias: "LastSalesOrderDetail", name: "SalesOrderDetail", Schema ="Sales")]
    [JoinTable("SalesOrderHeader", Schema = "Sales",
                OnRaw = "SalesOrderDetail.SalesOrderID = SalesOrderHeader.SalesOrderID")]
    [JoinTable(alias: "LastSalesOrderHeader", name: "SalesOrderHeader", Schema = "Sales",
                OnRaw = "LastSalesOrderDetail.SalesOrderID = LastSalesOrderHeader.SalesOrderID")]
    [JoinTable("Product", Schema = "Production",
                OnRaw = "SalesOrderDetail.ProductID = Product.ProductID")]
    [JoinTable("ProductSubcategory", Schema = "Production",
                OnRaw = "Product.ProductSubcategoryID = ProductSubcategory.ProductSubcategoryID")]
    [JoinTable("ProductCategory", Schema = "Production",
                OnRaw = "ProductSubcategory.ProductCategoryID = ProductCategory.ProductCategoryID")]
    [SqlWhere("SalesOrderHeader.Status in(1,2,5) and " +
                "SalesOrderHeader.OrderDate between :fromDate to :toDate")]
    [SqlGroupBy("ProductCategory.Name")]
    [SqlOrderBy("Sum(SalesOrderDetail.orderqty) Desc")]
    public class SalesReport_byCategory
    {
        [SqlColumn(tableAlias: "ProductCategory", column: "Name")]
        public string ProductCategoryName { get; }

        [SqlColumn(tableAlias: "SalesOrderDetail", column: "Sum(orderqty)")]
        public int Totalsales { get; }

    }
}
