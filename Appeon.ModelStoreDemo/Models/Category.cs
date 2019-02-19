using SnapObjects.Data;
using System.ComponentModel.DataAnnotations;

namespace Appeon.ModelStoreDemo.Models
{
    [FromTable("ProductCategory", Schema = "production")]
    [SqlOrderBy("Productcategoryid ASC")]
    public class Category
    {
        [Key]
        [Identity]
        public int Productcategoryid { get; set; }

        [ConcurrencyCheck]
        public string Name { get; set; }
    }
}
