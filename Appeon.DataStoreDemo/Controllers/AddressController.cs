using SnapObjects.Data;
using SnapObjects.Data.PowerBuilder;
using Appeon.DataStoreDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Appeon.DataStoreDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AddressController : ControllerBase
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

            packer.AddDataStore("StateProvince",
                _addressService.Retrieve("d_dddw_stateprovince"));

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

            var addressData = _addressService.Retrieve("d_address", provinceId, city);

            if (addressData.RowCount == 0)
            {
                return NotFound();
            }

            packer.AddDataStore("Address", addressData);

            return packer;
        }

        // POST api/Address/SaveChanges
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SaveChanges(IDataUnpacker unpacker)
        {
            string status = String.Empty;

            var packer = new DataPacker();

            var detail = unpacker.GetDataStore("dw1");
            
            try
            {
                status = _addressService.Update(detail);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            packer.AddDataStore("Address", detail);
            packer.AddValue("Status", status);

            return packer;
        }

        // DELETE api/Address/DeleteAddressByKey
        [HttpDelete]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> DeleteAddressByKey(IDataUnpacker unpacker)
        {
            string status = String.Empty;

            var packer = new DataPacker();
            var addressId = unpacker.GetValue<int>("arm1");

            try
            {
                status = _addressService.Delete("d_address_free", addressId);
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