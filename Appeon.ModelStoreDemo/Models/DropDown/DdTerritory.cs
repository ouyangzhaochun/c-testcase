using SnapObjects.Data;

namespace Appeon.ModelStoreDemo.Models
{
    [FromTable(alias: "st", name: "SalesTerritory", Schema = "Sales")]
    [SqlOrderBy("st.TerritoryID")]
    public class DdTerritory
    {
        [Identity]
        [SqlColumn(tableAlias: "st", column: "TerritoryID")]
        public int Territoryid { get; set; }

        [SqlColumn(tableAlias: "st", column: "Name")]
        public string Name { get; set; }
    }
}
