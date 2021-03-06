﻿using SnapObjects.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace Appeon.ModelStoreDemo.Models
{
    [FromTable("ProductPhoto",Schema = "Production")]
    [SqlOrderBy("ModifiedDate Desc")]
    public class ProductPhoto
    {
        [Key]
        [Identity]
        public int ProductPhotoID { get; set; }
        public byte[] LargePhoto { get; set; }
        public string LargePhotoFileName { get; set; }
        public DateTime ModifiedDate => DateTime.Now;
        
    }
}
