using SnapObjects.Data;

namespace Appeon.ModelStoreDemo.Models
{
    [FromTable(alias: "st", name: "SalesTerritory", Schema = "Sales")]
    [FromTable(alias: "cr", name: "CountryRegion", Schema = "Person")]
    [SqlWhere("cr.CountryRegionCode = st.CountryRegionCode")]
    public class DdSalesTerritory
    {
        [Identity]
        [SqlColumn(tableAlias: "st", column: "TerritoryID")]
        public int Salesterritory_Territoryid { get; set; }

        [SqlColumn(tableAlias: "st", column: "Name")]
        public string Salesterritory_Name { get; set; }

        [SqlColumn(tableAlias: "st", column: "CountryRegionCode")]
        public string Salesterritory_Countryregioncode { get; set; }

        [SqlColumn("Group")]
        public string Salesterritory_Group { get; set; }

        [SqlColumn(tableAlias: "cr", column: "Name")]
        public string Countryregion_Name { get; set; }
    }
}
