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
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IGenericServiceFactory _genericServices;

        public ProductController(IProductService proevn,
                                IGenericServiceFactory genericServiceFactory)
        {
            _productService = proevn;
            _genericServices = genericServiceFactory;
        }

        // GET api/Product/WinOpen
        [HttpGet]
        [ProducesResponseType(404)]
        public ActionResult<IDataPacker> WinOpen()
        {
            var packer = new DataPacker();

            var productCate = _genericServices.Get<Category>().Retrieve(false);

            if(productCate.Count == 0)
            {
                return NotFound();
            }

            var cateId = productCate.FirstOrDefault().Productcategoryid;

            packer.AddValue("CateId", cateId.ToString());
            packer.AddModels("Category", productCate);
            packer.AddModels("SubCategory", _genericServices
                .Get<SubCategory>().Retrieve(false, cateId));
            packer.AddModels("Units", _genericServices
                .Get<DdUnit>().Retrieve(false));
                        
            return packer;
        }

        // POST api/Product/Retrieve
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> Retrieve(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var dwname = unpacker.GetValue<string>("arm1");
            int id = unpacker.GetValue<int>("arm2");

            try
            {
                switch (dwname)
                {
                    case "d_subcategory":
                        packer.AddModels("SubCategory",
                            _genericServices.Get<SubCategory>().Retrieve(false, id));
                        break;

                    case "d_product":
                        packer.AddModels("Product",
                            _productService.Retrieve(false, id));
                        packer.AddModels("dddwSubCategory",
                            _genericServices.Get<SubCategory>().Retrieve(false, id));
                        break;

                    case "d_history_price":
                        packer.AddModels("HistoryPrice",
                            _genericServices.Get<HistoryPrice>().Retrieve(false, id));
                        packer.AddModels("dddwProduct",
                            _genericServices.Get<DdProduct>().Retrieve(false, id));
                        var photo = _genericServices
                            .Get<ViewProductPhoto>().Retrieve(false, id);

                        if (photo.Count > 0)
                        {
                            packer.AddValue("photo", photo[0].LargePhoto);
                            packer.AddValue("photoname", photo[0].LargePhotoFileName);
                        }
                        else
                        {
                            packer.AddValue("photo", "");
                            packer.AddValue("photoname", "");
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return packer;
        }

        // POST api/Product/SaveProductPhoto
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SaveProductPhoto(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var productId = unpacker.GetValue<int>("arm1");
            var photoName = unpacker.GetValue<string>("arm2");
            var productPhoto = unpacker.GetValue<string>("arm3");            
            byte[] bProductPhoto = Convert.FromBase64String(productPhoto);

            try
            {
                _productService.SaveProductPhoto(productId, photoName, bProductPhoto);
                packer.AddValue("Status", "Success");
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }            

            return packer;
        }

        // POST api/Product/SaveProductTwotier
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SaveProductTwotier(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();
            var productId = 0;

            var product = unPacker.GetModelEntries<Product>("dw1").FirstOrDefault();
            var prices = unPacker.GetModelEntries<HistoryPrice>("dw2");

            try
            {
                productId = _productService.SaveProductAndPrice(product, prices);
                packer.AddValue("Status", "Success");
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }

            packer.AddModel("Product", _productService.RetrieveByKey(false, productId));
            return packer;
        }

        // POST api/Product/SaveHistoryPrices
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SaveHistoryPrices(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();
            var subcate = unPacker
                .GetModelEntries<SubCategory>("dw1").FirstOrDefault();
            var product = unPacker
                .GetModelEntries<Product>("dw2").FirstOrDefault();
            var prices = unPacker.GetModelEntries<HistoryPrice>("dw3");

            try
            {
                var ret = _productService.SaveHistoryPrices(subcate, product, prices);
                var subcateId = subcate.GetCurrentValue("Productsubcategoryid");                
                
                packer.AddModel("SubCategory", 
                    _genericServices.Get<SubCategory>().RetrieveByKey(false, subcateId));
                packer.AddModels("Product", _productService.Retrieve(false, subcateId));
                packer.AddValue("Status", "Success");
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
            
            return packer;
        }

        // POST api/Product/SaveChanges
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SaveChanges(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();
            var modelname = unPacker.GetValue<string>("arm1");
            object modelId = 0;

            try
            {
                switch (modelname)
                {
                    case "SubCategory":
                        var subcate = unPacker.GetModelEntries<SubCategory>("dw1");
                        var sModel = _genericServices.Get<SubCategory>()
                                     .SaveChanges(subcate);
                        modelId = subcate.FirstOrDefault()
                                 .GetCurrentValue("Productsubcategoryid");

                        packer.AddModel("SubCategory", 
                            _genericServices.Get<SubCategory>()
                                            .RetrieveByKey(false, modelId));
                        break;

                    case "Product":
                        var prod = unPacker.GetModelEntries<Product>("dw1");
                        var pModel = _genericServices.Get<Product>().SaveChanges(prod);
                        modelId = prod.FirstOrDefault().GetCurrentValue("Productid");

                        packer.AddModel("Product", _productService
                              .RetrieveByKey(false, modelId));
                        break;

                }
                packer.AddValue("Status", "Success");
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return packer;
        }

        // DELETE api/Product/Delete
        [HttpDelete]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> Delete(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();
            var modelname = unPacker.GetValue<string>("arm1");

            try
            {
                switch (modelname)
                {
                    case "SubCategory":
                        var sModel = unPacker
                            .GetModelEntries<SubCategory>("dw1").FirstOrDefault();
                        var subDelete = _genericServices
                            .Get<SubCategory>().Delete(sModel);
                        break;

                    case "Product":
                        var pModel = unPacker
                            .GetModelEntries<Product>("deletedw1").FirstOrDefault();
                        var productDelete = _genericServices
                            .Get<Product>().Delete(pModel);
                        break;

                }
                packer.AddValue("Status", "Success");
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }   
            
            return packer;
        }

        // DELETE api/Product/DeleteSubcategoryByKey
        [HttpDelete]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> DeleteSubcategoryByKey(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();
            var subcateId = unPacker.GetValue<int>("arm1");

            try
            {
                var result = _genericServices.Get<SubCategory>()
                    .DeleteByKey(subcateId);
                packer.AddValue("Status", "Success");
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return packer;
        }

        // DELETE api/Product/DeleteProductByKey
        [HttpDelete]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> DeleteProductByKey(IDataUnpacker unPacker)
        {
            var packer = new DataPacker();
            var productId = unPacker.GetValue<int>("arm1");

            try
            {
                var result = _genericServices.Get<Product>().DeleteByKey(productId);
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