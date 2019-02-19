using SnapObjects.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Appeon.ModelStoreDemo.Models
{
    [SqlParameter("saleId",typeof(int))]
    [Table("SalesOrderDetail", Schema = "Sales")]
    [SqlWhere("Salesorderid=:saleId")]
    public class SalesOrderDetail
    {
        [Key]
        public int SalesOrderID { get; set; }

        [Key]
        [Identity]
        public int SalesOrderDetailID { get; set; }

        public string CarrierTrackingNumber { get; set; }

        public int OrderQty { get; set; }

        public int ProductID { get; set; }

        public int SpecialOfferID { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal UnitPriceDiscount { get; set; }

        [Identity]
        public decimal LineTotal { get; set; }       

        public DateTime ModifiedDate { get; set; }
    }
}
