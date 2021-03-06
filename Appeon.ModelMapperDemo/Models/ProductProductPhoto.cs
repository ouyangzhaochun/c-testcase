﻿using SnapObjects.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace Appeon.ModelMapperDemo.Models
{
    [SqlParameter("photoid",typeof(int))]
    [FromTable("ProductProductPhoto",Schema = "Production")]
    [SqlWhere("ProductID=:photoid")]
    public class ProductProductPhoto
    {
        [Key]
        public int ProductID { get; set; }
        public int ProductPhotoID { get; set; }
        public byte? Primary { get; set; }
        public DateTime ModifiedDate { get; set; }


    }
}
