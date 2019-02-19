using SnapObjects.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Appeon.ModelStoreDemo.Models
{
    [SqlParameter("prodid", typeof(int))]
    [Table("ProductProductPhoto", Schema = "production")]
    [SqlWhere("ProductID = :prodid")]
    public class ProductProductPhoto
    {
        [Key]
        public int ProductID { get; set; }

        public int ProductPhotoID { get; set; }

        public byte? Primary { get; set; }

        public DateTime ModifiedDate { get; set; }
       
    }
}
