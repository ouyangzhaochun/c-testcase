﻿using SnapObjects.Data;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Appeon.ModelMapperDemo.Models
{
    [Table("BusinessEntity", Schema = "Person")]
    public class BusinessEntity
    { 
        [Key]
        [Identity]
        public int Businessentityid { get; set; }
        public DateTime ModifiedDate { get; set; }

        [JsonIgnore]
        [SetValue("$Businessentityid", "$Businessentityid", SetValueStrategy.Always)]
        [ModelEmbedded(typeof(Person), ParamValue = "$Businessentityid",
            CascadeCreate = true, CascadeDelete = true)]
        public Person mPerson { get; set; }

        
    }
}
