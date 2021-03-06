﻿using SnapObjects.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Appeon.ModelMapperDemo.Models
{
    [SqlParameter("personId", typeof(int))]
    [Table("BusinessEntityAddress", Schema = "Person")]
    [SqlWhere("Businessentityid = :personId")]
    public class BusinessentityAddress
    {
        [Key]
        public int Businessentityid { get; set; }

        [Key]
        public int Addressid { get; set; }

        [Key]
        public int Addresstypeid { get; set; }

        public DateTime Modifieddate { get; set; }
    }
}
