using SnapObjects.Data;
using Appeon.ModelStoreDemo.Models;
using Appeon.ModelStoreDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Appeon.ModelStoreDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addService)
        {
            _addressService = addService;
        }

        // GET api/Address/WinOpen
        [HttpGet]
        public ActionResult<IDataPacker> WinOpen()
        {
            var packer = new DataPacker();
            packer.AddModelStore("StateProvince", 
                _addressService.Retrieve<DdStateProvince>());
            return packer;
        }

        // POST api/Address/RetrieveAddress
        [HttpPost]
        [ProducesResponseType(404)]
        public ActionResult<IDataPacker> RetrieveAddress(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var provinceId = unpacker.GetValue<int>("arm1");
            var city = unpacker.GetValue<string>("arm2");

            var addressData = _addressService.Retrieve<AddressList>(provinceId, city);

            if (addressData.Count == 0)
            {
                return NotFound();
            }

            packer.AddModelStore("Address", addressData);

            return packer;
        }

        // POST api/Address/SaveChanges
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SaveChanges(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var addressModel = unpacker.GetModelStore<Address>("dw1",
                ChangeTrackingStrategy.PropertyState, MappingMethod.JsonKey);

            try
            {
                var status = _addressService.Update(addressModel);
                packer.AddValue("Status", status);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            packer.AddModelStore("Address", addressModel);
         
            return packer;
        }

        // DELETE api/Address/DeleteAddressByKey
        [HttpDelete]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> DeleteAddressByKey(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();
            var addressId = unpacker.GetValue<int>("arm1");

            try
            {
                var status = _addressService.Delete<Address>(addressId);
                packer.AddValue("Status", status);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return packer;
        }
    }
}