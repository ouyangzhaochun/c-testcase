﻿using SnapObjects.Data;
using System.ComponentModel.DataAnnotations;

namespace Appeon.ModelMapperDemo.Models
{
    [FromTable(alias: "st", name: "SalesTerritory", Schema = "Sales")]
    public class DdSalesTerritory
    {
        [Key]
        [Identity]
        [SqlColumn(tableAlias: "st", column: "TerritoryID")]
        public int Territoryid { get; set; }

        [SqlColumn(tableAlias: "st", column: "Name")]
        public string Name { get; set; }

        [SqlColumn(tableAlias: "st", column: "CountryRegionCode")]
        public string Countryregioncode { get; set; }

        [SqlColumn("Group")]
        public string Group { get; set; }
    }
}
