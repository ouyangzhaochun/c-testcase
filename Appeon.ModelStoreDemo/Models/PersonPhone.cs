using SnapObjects.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Appeon.ModelStoreDemo.Models
{
    [SqlParameter("entityId",typeof(int))]
    [Table("PersonPhone", Schema = "Person")]
    [SqlWhere("Businessentityid = :entityId")]
    public class PersonPhone
    {
        [Key]
        public int Businessentityid { get; set; }

        [Key]
        public string Phonenumber { get; set; }

        [Key]
        public int Phonenumbertypeid { get; set; }

        public DateTime Modifieddate { get; set; }
    }
}
