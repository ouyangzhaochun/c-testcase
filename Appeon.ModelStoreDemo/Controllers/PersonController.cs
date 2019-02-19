using SnapObjects.Data;
using Appeon.ModelStoreDemo.Models;
using Appeon.ModelStoreDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Appeon.ModelStoreDemo.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PersonController : Controller
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

            packer.AddModelStore("Address", 
                _personService.Retrieve<DdAddress>());
            packer.AddModelStore("AddressType", 
                _personService.Retrieve<DdAddressType>());
            packer.AddModelStore("PhonenumberType", 
                _personService.Retrieve<DdPhoneNumberType>());
            packer.AddModelStore("CustomerTerritory", 
                _personService.Retrieve<DdTerritory>());
            packer.AddModelStore("Store", 
                _personService.Retrieve<DdStore>());

            var personData = _personService.Retrieve<PersonList>("IN");

            if (personData.Count == 0)
            {
                return NotFound();
            }

            packer.AddModelStore("Person", personData);

            return packer;
        }

        // POST api/Person/RetrievePersonByKey
        [HttpPost]
        public ActionResult<IDataPacker> RetrievePersonByKey(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();
            var personId = unpacker.GetValue<int>("arm1");

            packer.AddModelStore("Person", 
                _personService.Retrieve<Person>(personId));
            packer.AddModelStore("Person.PersonAddress", 
                _personService.Retrieve<BusinessEntityAddress>(personId));
            packer.AddModelStore("Person.PersonPhone", 
                _personService.Retrieve<PersonPhone>(personId));
            packer.AddModelStore("Person.Customer", 
                _personService.Retrieve<Customer>(personId));

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

            var personData = _personService.Retrieve<PersonList>(personType);

            if (personData.Count == 0)
            {
                return NotFound();
            }
            
            packer.AddModelStore("Persons", personData);

            return packer;
        }

        // POST api/Person/SavePerson
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SavePerson(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var person = unpacker.GetModelStore<Person>("dw1",
                                  ChangeTrackingStrategy.PropertyState);

            var personAddress = unpacker.GetModelStore<BusinessEntityAddress>("dw2",
                                  ChangeTrackingStrategy.PropertyState);

            var personPhone = unpacker.GetModelStore<PersonPhone>("dw3",
                                  ChangeTrackingStrategy.PropertyState);

            var customer = unpacker.GetModelStore<Customer>("dw4",
                                 ChangeTrackingStrategy.PropertyState);
            try
            {
                var personId = _personService.SavePerson(person, personAddress, 
                    personPhone, customer);

                if (personId > 0)
                {
                    packer.AddModelStore("Person", 
                        _personService.Retrieve<Person>(personId));
                    packer.AddModelStore("Person.PersonAddress", 
                        _personService.Retrieve<BusinessEntityAddress>(personId));
                    packer.AddModelStore("Person.PersonPhone", 
                        _personService.Retrieve<PersonPhone>(personId));
                    packer.AddModelStore("Person.Customer", 
                        _personService.Retrieve<Customer>(personId));
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

            var personAddress = unpacker.GetModelStore<BusinessEntityAddress>("dw1",
                                        ChangeTrackingStrategy.PropertyState);
            var personPhone = unpacker.GetModelStore<PersonPhone>("dw2",
                                        ChangeTrackingStrategy.PropertyState);
            var customer = unpacker.GetModelStore<Customer>("dw3",
                                        ChangeTrackingStrategy.PropertyState);

            var status = "Success";
            int? intPersonId = 0;

            try
            {
                if (personAddress.Count() > 0)
                {
                    status = _personService.Update(true, personAddress);
                    intPersonId = personAddress.FirstOrDefault().Businessentityid;
                }

                if (personPhone.Count() > 0 && status == "Success")
                {
                    status = _personService.Update(true, personPhone);
                    intPersonId = personPhone.FirstOrDefault().Businessentityid;
                }

                if (customer.Count() > 0 && status == "Success")
                {
                    status = _personService.Update(true, customer);
                    intPersonId = customer.FirstOrDefault().Personid;
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            if (status == "Success")
            {
                packer.AddModelStore("Person",
                        _personService.Retrieve<Person>(intPersonId));
                packer.AddModelStore("Person.PersonAddress",
                    _personService.Retrieve<BusinessEntityAddress>(intPersonId));
                packer.AddModelStore("Person.PersonPhone",
                    _personService.Retrieve<PersonPhone>(intPersonId));
                packer.AddModelStore("Person.Customer",
                    _personService.Retrieve<Customer>(intPersonId));
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
            var status = "";

            try
            {
                switch (dwname)
                {
                    case "Person":
                        _personService.DeletePerson(personId);

                        break;

                    case "PersonAddress":
                        var addressId = unpacker.GetValue<int>("arm3");
                        var addressTypeId = unpacker.GetValue<int>("arm4");
                        status = _personService.Delete<BusinessEntityAddress>(true,
                            m => m.Addressid == addressId &&
                            m.Addresstypeid == addressTypeId, personId);

                        break;

                    case "PersonPhone":
                        var personNumber = unpacker.GetValue<string>("arm3");
                        var phonenumbertypeid = unpacker.GetValue<int>("arm4");
                        status = _personService.Delete<PersonPhone>(true,
                            m => m.Phonenumber == personNumber &&
                            m.Phonenumbertypeid == phonenumbertypeid, personId);
                        break;

                    case "Customer":
                        var customerId = unpacker.GetValue<int>("arm3");

                        status = _personService.Delete<Customer>(true, customerId);

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