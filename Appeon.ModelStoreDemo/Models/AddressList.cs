using SnapObjects.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Appeon.ModelStoreDemo.Models
{
    [SqlParameter("stateId",typeof(int))]
    [SqlParameter("city",typeof(string))]
    [Table("Address", Schema = "Person")]
    [SqlWhere("(StateProvinceID = :stateId OR :stateId = 0) " +
              " And (City like '%' + :city + '%')")]
    public class AddressList
    {
        [Key]
        [Identity]
        public int AddressID { get; set; }
        
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public int StateProvinceID { get; set; }

        public string PostalCode { get; set; }

        public Guid Rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
