using SnapObjects.Data;

namespace Appeon.ModelStoreDemo.Models
{
    [FromTable("PhoneNumberType", Schema = "Person")]
    public class DdPhoneNumberType
    {
        public int Phonenumbertypeid { get; set; }

        public string Name { get; set; }
    }
}
