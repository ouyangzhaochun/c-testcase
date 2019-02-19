using SnapObjects.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Appeon.ModelStoreDemo.Models
{
    [Table("BusinessEntity", Schema = "Person")]
    public class BusinessEntity
    {
        [Key]
        [Identity]
        public Int32 BusinessEntityID { get; set; }

        public Guid Rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }

    }
}
