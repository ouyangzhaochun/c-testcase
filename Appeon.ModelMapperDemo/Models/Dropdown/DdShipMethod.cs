using SnapObjects.Data;
using System.ComponentModel.DataAnnotations;

namespace Appeon.ModelMapperDemo.Models
{
    [FromTable("ShipMethod", Schema = "Purchasing")]
    public class DdShipMethod
    {
        [Key]
        [Identity]
        public int Shipmethodid { get; set; }

        public string Name { get; set; }

        public decimal Shipbase { get; set; }

        public decimal Shiprate { get; set; }
    }
}
