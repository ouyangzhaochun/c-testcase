using SnapObjects.Data;
using Appeon.ModelMapperDemo.Models;
using Appeon.ModelMapperDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Appeon.ModelMapperDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly IGenericServiceFactory _genericServices;

        public AddressController(IAddressService addService, 
                                 IGenericServiceFactory genericServiceFactory)
        {
            _addressService = addService;
            _genericServices = genericServiceFactory;
        }

        // GET api/Address/WinOpen
        [HttpGet]
        [ProducesResponseType(404)]
        public ActionResult<IDataPacker> WinOpen()
        {
            var packer = new DataPacker();
            var stateProvince = _genericServices.Get<DdStateProvince>()
                                .Retrieve(false);

            if (stateProvince.Count == 0)
            {
                return NotFound();
            }
            
            packer.AddModels("StateProvince", stateProvince);

            return packer;
        }

        // POST api/Address/RetrieveAddress
        [HttpPost]
        [ProducesResponseType(404)]
        public ActionResult<IDataPacker> RetrieveAddress(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();
            var provinceId = unPacker.GetValue<int>("arm1");
            var city = unPacker.GetValue<string>("arm2");

            var address = _addressService.Retrieve(false, provinceId, city);

            if (address.Count == 0)
            {
                return NotFound();
            }

            packer.AddModels("Address", address);

            return packer;
        }

        // POST api/Address/SaveChanges
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SaveChanges(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();             
            var addressModel = unPacker.GetModelEntries<Address>("dw1",
                MappingMethod.JsonKey);

            try
            {
                var pModel = _genericServices.Get<Address>()
                            .SaveChanges(addressModel);
                var addressid = addressModel.FirstOrDefault()
                                .GetCurrentValue("AddressID");

                packer.AddModel("Address", _genericServices.Get<Address>()
                      .RetrieveByKey(false, addressid));
                packer.AddValue("Status", "Success");
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
            
            return packer;
        }

        // DELETE api/Address/DeleteAddress
        [HttpDelete]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> DeleteAddress(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();
            var addressModel = unPacker.GetModelEntries<Address>("dw1")
                               .FirstOrDefault();

            try
            {
                var result = _genericServices.Get<Address>()
                             .Delete(addressModel);
                packer.AddValue("Status", "Success");
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return packer;
        }

        // DELETE api/Address/DeleteAddressByKey
        [HttpDelete]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> DeleteAddressByKey(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();
            var addressId = unPacker.GetValue<int>("arm1");

            try
            {
                var result = _genericServices.Get<Address>().DeleteByKey(addressId);
                packer.AddValue("Status", "Success");
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }   

            return packer;
        }
    }
}