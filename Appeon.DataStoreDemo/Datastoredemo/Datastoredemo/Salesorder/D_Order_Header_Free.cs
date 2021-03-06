using SnapObjects.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Appeon.DataStoreDemo
{
    [Table("SalesOrderHeader", Schema = "Sales")]
    [UpdateWhereStrategy(UpdateWhereStrategy.KeyAndConcurrencyCheckColumns)]
    public class D_Order_Header_Free
    {
        [Key]
        [Identity]
        public Int32 Salesorderid { get; set; }

        [ConcurrencyCheck]
        public Byte Revisionnumber { get; set; }

        [ConcurrencyCheck]
        public DateTime Orderdate { get; set; }

        [ConcurrencyCheck]
        public DateTime Duedate { get; set; }

        [ConcurrencyCheck]
        public DateTime? Shipdate { get; set; }

        [ConcurrencyCheck]
        public Byte Status { get; set; }

        [ConcurrencyCheck]
        public Boolean Onlineorderflag { get; set; }

        public String Salesordernumber { get; set; }

        [ConcurrencyCheck]
        public String Purchaseordernumber { get; set; }

        [ConcurrencyCheck]
        public String Accountnumber { get; set; }

        [ConcurrencyCheck]
        public Int32 Customerid { get; set; }

        [ConcurrencyCheck]
        public Int32? Salespersonid { get; set; }

        [ConcurrencyCheck]
        public Int32? Territoryid { get; set; }

        [ConcurrencyCheck]
        public Int32 Billtoaddressid { get; set; }

        [ConcurrencyCheck]
        public Int32 Shiptoaddressid { get; set; }

        [ConcurrencyCheck]
        public Int32 Shipmethodid { get; set; }

        [ConcurrencyCheck]
        public Int32? Creditcardid { get; set; }

        [ConcurrencyCheck]
        public String Creditcardapprovalcode { get; set; }

        [ConcurrencyCheck]
        public Int32? Currencyrateid { get; set; }

        [ConcurrencyCheck]
        public Decimal Subtotal { get; set; }

        [ConcurrencyCheck]
        public Decimal Taxamt { get; set; }

        [ConcurrencyCheck]
        public Decimal Freight { get; set; }

        public Decimal? Totaldue { get; set; }

        [ConcurrencyCheck]
        public String Comment { get; set; }

        [ConcurrencyCheck]
        public DateTime? Modifieddate { get; set; }

    }
}
