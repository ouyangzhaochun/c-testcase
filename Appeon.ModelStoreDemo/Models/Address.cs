using SnapObjects.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Appeon.ModelStoreDemo.Models
{
    [SqlParameter(name: "addressId", dataType: typeof(Int32))]
    [Table("Address", Schema = "Person")]
    [SqlWhere("AddressID = :addressId")]
    public class Address
    {
        [Key]
        [Identity]
        [Column("AddressID")]
        public int AddressID { get; set; }

        [Column("AddressLine1")]
        [Required]
        [MaxLength(60)]
        public String AddressLine1 { get; set; }

        [Column("AddressLine2")]
        [MaxLength(60)]
        public String AddressLine2 { get; set; }

        [Column("City")]
        [Required]
        [MaxLength(30)]
        public String City { get; set; }

        [Column("StateProvinceID")]
        [Required]
        public int StateProvinceID { get; set; }

        [Column("PostalCode")]
        [Required]
        [MaxLength(60)]
        public String PostalCode { get; set; }

        [Column("ModifiedDate")]
        public DateTime ModifiedDate { get; set; }
    }
}
