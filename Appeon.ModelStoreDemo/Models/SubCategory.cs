using SnapObjects.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace Appeon.ModelStoreDemo.Models
{
    [SqlParameter("cateId", typeof(int))]
    [FromTable("Productsubcategory", Schema = "production")]
    [SqlWhere("Productsubcategoryid = :cateId or :cateId = 0")]
    [SqlOrderBy("Productsubcategoryid")]
    public class SubCategory
    {
        [Key]
        [Identity]
        public int Productsubcategoryid { get; set; }

        public int Productcategoryid { get; set; }

        public string Name { get; set; }

        public DateTime Modifieddate { get; set; }
    }
}
