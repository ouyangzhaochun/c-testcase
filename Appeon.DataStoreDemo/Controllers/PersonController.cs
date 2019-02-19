using SnapObjects.Data;
using SnapObjects.Data.PowerBuilder;
using Appeon.DataStoreDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Appeon.DataStoreDemo.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService perService)
        {
            _personService = perService;
        }

        // GET api/Person/WinOpen
        [HttpGet]
        [ProducesResponseType(404)]
        public ActionResult<IDataPacker> WinOpen()
        {
            var packer = new DataPacker();

            packer.AddDataStore("Address",
                _personService.Retrieve("d_dddw_address"));
            packer.AddDataStore("AddressType",
                _personService.Retrieve("d_dddw_addresstype"));
            packer.AddDataStore("PhonenumberType",
                _personService.Retrieve("d_dddw_phonenumbertype"));
            packer.AddDataStore("CustomerTerritory",
                _personService.Retrieve("d_dddw_territory"));
            packer.AddDataStore("Store",
                _personService.Retrieve("d_dddw_store"));

            var personData = _personService.Retrieve("d_person_list", "IN");

            if (personData.RowCount == 0)
            {
                return NotFound();
            }

            packer.AddDataStore("Person", personData);

            return packer;
        }

        // POST api/Person/RetrievePersonByKey
        [HttpPost]
        public ActionResult<IDataPacker> RetrievePersonByKey(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var personId = unpacker.GetValue<int>("arm1");

            packer.AddDataStore("Person",
                _personService.Retrieve("d_person", personId));
            packer.AddDataStore("Person.PersonAddress",
                _personService.Retrieve("d_businessentityaddress", personId));

            packer.AddDataStore("Person.PersonPhone",
                _personService.Retrieve("d_personphone", personId));

            packer.AddDataStore("Person.Customer",
                _personService.Retrieve("d_customer", personId));

            return packer;
        }

        // POST api/Person/RetrievePerson
        [HttpPost]
        [ProducesResponseType(404)]
        public ActionResult<IDataPacker> RetrievePerson(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            // only retrieve customer(personType = "IN") from person
            var personType = "IN";

            var personData = _personService.Retrieve("d_person_list", personType);

            if (personData.RowCount == 0)
            {
                return NotFound();
            }

            packer.AddDataStore("Persons", personData);

            return packer;
        }

        // POST api/Person/SavePerson
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SavePerson(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            try
            {
                var person = unpacker.GetDataStore("dw1", "d_person");
                var personAddress = unpacker.GetDataStore("dw2", "d_address");
                var personPhone = unpacker.GetDataStore("dw3", "d_personphone");
                var customer = unpacker.GetDataStore("dw4", "d_customer");

                var personId = _personService.SavePerson(person, personAddress, 
                    personPhone, customer);

                if (personId > 0)
                {
                    packer.AddDataStore("Person", 
                        _personService.Retrieve("d_person", personId));
                    packer.AddDataStore("Person.PersonAddress", 
                        _personService.Retrieve("d_businessentityaddress", personId));
                    packer.AddDataStore("Person.PersonPhone", 
                        _personService.Retrieve("d_personphone", personId));
                    packer.AddDataStore("Person.Customer", 
                        _personService.Retrieve("d_customer", personId));
                }
                packer.AddValue("Status", "Success");
            }

            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return packer;
        }

        // POST api/Person/Savechanges
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> Savechanges(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var personAddress = unpacker.GetDataStore("dw1", "d_businessentityaddress");
            var personPhone = unpacker.GetDataStore("dw2", "d_personphone");
            var customer = unpacker.GetDataStore("dw3", "d_customer");
            var status = "Success";
            int? intPersonId = 0;

            try
            {
                if (personAddress.RowCount > 0)
                {
                    status = _personService.Update(true, personAddress);
                    intPersonId = personAddress.GetItem<int?>(0, "businessentityid");
                }

                if (personPhone.RowCount > 0 && status == "Success")
                {
                    status = _personService.Update(true, personPhone);
                    intPersonId = personPhone.GetItem<int?>(0, "businessentityid");
                }

                if (customer.RowCount > 0 && status == "Success")
                {
                    status = _personService.Update(true, customer);
                    intPersonId = customer.GetItem<int?>(0, "personid");

                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            if (status == "Success")
            {
                packer.AddDataStore("Person", 
                    _personService.Retrieve("d_person", intPersonId));
                packer.AddDataStore("Person.PersonAddress", 
                    _personService.Retrieve("d_businessentityaddress", intPersonId));
                packer.AddDataStore("Person.PersonPhone", 
                    _personService.Retrieve("d_personphone", intPersonId));
                packer.AddDataStore("Person.Customer", 
                    _personService.Retrieve("d_customer", intPersonId));
            }

            packer.AddValue("Status", status);

            return packer;
        }

        // Delete api/Person/DeleteByKey
        [HttpDelete]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> DeleteByKey(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var dwname = unpacker.GetValue<string>("arm1");
            var personId = unpacker.GetValue<int>("arm2");
            var status = "Success";

            try
            {
                switch (dwname)
                {
                    case "Person":
                        status = _personService.DeletePerson(personId);

                        break;
                    case "PersonAddress":
                        var addressId = unpacker.GetValue<int>("arm3");
                        var addressTypeId = unpacker.GetValue<int>("arm4");
                        status = _personService.Delete("d_businessentityaddress", true,
                            "Addressid = " + addressId.ToString() + " And " +
                            "Addresstypeid = " + addressTypeId.ToString(),
                            personId);

                        break;
                    case "PersonPhone":
                        var personNumber = unpacker.GetValue<string>("arm3");
                        var phonenumbertypeid = unpacker.GetValue<int>("arm4");
                        status = _personService.Delete("d_personphone", true,
                            "Phonenumber = " + personNumber.ToString() + " And " +
                            "Phonenumbertypeid = " + phonenumbertypeid.ToString(),
                            personId);

                        break;
                    case "Customer":
                        var customerId = unpacker.GetValue<int>("arm3");

                        status = _personService.Delete("d_customer", true, customerId);

                        break;
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            packer.AddValue("Status", status);

            return packer;
        }
    }
}