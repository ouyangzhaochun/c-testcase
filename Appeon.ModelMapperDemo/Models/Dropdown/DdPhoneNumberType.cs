﻿using SnapObjects.Data;

namespace Appeon.ModelMapperDemo.Models
{
    [FromTable("PhoneNumberType", Schema = "Person")]
    public class DdPhoneNumberType
    {
        public int Phonenumbertypeid { get; set; }

        public string Name { get; set; }
    }
}
