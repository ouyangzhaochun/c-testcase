using SnapObjects.Data;
using System.ComponentModel.DataAnnotations;

namespace Appeon.ModelMapperDemo.Models
{
    [FromTable("StateProvince", Schema = "Person")]
    [SqlOrderBy("StateProvinceID")]
    public class DdStateProvince
    {
        [Key]
        [Identity]
        public int StateProvinceID { get; set; }

        public string Name { get; set; }

    }
}
