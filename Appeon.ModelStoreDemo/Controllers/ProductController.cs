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
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET api/product/WinOpen
        [HttpGet]
        public ActionResult<IDataPacker> WinOpen()
        {
            var packer = new DataPacker();
                        
            var productCate = _productService.Retrieve<Category>();
            var cateId = productCate.FirstOrDefault().Productcategoryid;            

            packer.AddValue("CateId", cateId.ToString());
            packer.AddModelStore("Category", productCate);
            packer.AddModelStore("SubCategory", 
                _productService.Retrieve<SubCategoryList>(cateId));
            packer.AddModelStore("Units", _productService.Retrieve<DdUnit>());

            return packer;
        }

        // POST api/product/Retrieve
        [HttpPost]
        [ProducesResponseType(404)]
        public ActionResult<IDataPacker> Retrieve(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var dwname = unpacker.GetValue<string>("arm1");
            int id = int.Parse(unpacker.GetValue<string>("arm2"));

            switch (dwname)
            {
                case "d_subcategory":
                    var subData = _productService.Retrieve<SubCategoryList>(id);
                    if (subData.Count == 0)
                    {
                        return NotFound();
                    }
                    packer.AddModelStore("SubCategory", subData);

                    break;

                case "d_product":
                    packer.AddModelStore("Product", 
                        _productService.Retrieve<ProductList>(id));
                    packer.AddModelStore("dddwSubCategory", 
                        _productService.Retrieve<SubCategoryList>(id));

                    break;

                case "d_history_price":
                    packer.AddModelStore("HistoryPrice", _productService.Retrieve<HistoryPrice>(id));
                    packer.AddModelStore("dddwProduct", _productService.Retrieve<DdProduct>(id));
                    var photo = _productService.Retrieve<ViewProductPhoto>(id);
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

            return packer;
        }

        // POST api/product/SaveProductPhoto
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
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return packer;
        }

        // POST api/product/SaveProductTwotier
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SaveProductTwotier(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var product = unpacker.GetModelStore<Product>("dw1",
                                       ChangeTrackingStrategy.PropertyState);
            var prices = unpacker.GetModelStore<HistoryPrice>("dw2",
                                       ChangeTrackingStrategy.PropertyState);

            try
            {
                var productId = _productService.SaveProductAndPrice(product, prices);
                
                packer.AddModelStore("Product", 
                    _productService.Retrieve<Product>(productId));
                packer.AddModelStore("Product.HistoryPrice", 
                    _productService.Retrieve<HistoryPrice>(productId));

                packer.AddValue("Status", "Success");
                
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            
            return packer;
        }

        // POST api/product/SaveHistoryPrices
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SaveHistoryPrices(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var subcate = unpacker.GetModelStore<SubCategory>("dw1",
                                       ChangeTrackingStrategy.PropertyState);
            var product = unpacker.GetModelStore<Product>("dw2",
                                       ChangeTrackingStrategy.PropertyState);
            var prices = unpacker.GetModelStore<HistoryPrice>("dw3",
                                       ChangeTrackingStrategy.PropertyState);
            try
            {

                _productService.SaveHistoryPrices(subcate, product, prices);

                packer.AddModelStore("SubCategory", subcate);
                packer.AddModelStore("Product", product);
                packer.AddModelStore("Product.HistoryPrice", prices);
                packer.AddValue("Status", "Success");
            }

            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }           

            return packer;
        }

        // POST api/product/SaveChanges
        [HttpPost]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> SaveChanges(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();
            var modelname = unpacker.GetValue<string>("arm1");
            var status = "Success";

            try
            {
                switch (modelname)
                {
                    case "SubCategory":
                        var subcate = unpacker.GetModelStore<SubCategoryList>("dw1",
                                           ChangeTrackingStrategy.PropertyState);
                        status = _productService.Update(true, subcate);

                        var modelId = subcate.FirstOrDefault().Productcategoryid;

                        packer.AddModelStore("SubCategory", subcate);

                        break;

                    case "Product":
                        var prod = unpacker.GetModelStore<Product>("dw1",
                                           ChangeTrackingStrategy.PropertyState);
                        status = _productService.Update(true, prod);

                        var productId = prod.FirstOrDefault().Productid;
                        packer.AddModelStore("Product", 
                            _productService.Retrieve<Product>(productId));
                        packer.AddModelStore("Product.HistoryPrice",
                            _productService.Retrieve<HistoryPrice>(productId));

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

        // DELETE api/product/Delete
        [HttpDelete]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> Delete(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var status = "Success";
            var modelname = unpacker.GetValue<string>("arm1");

            try
            {
                switch (modelname)
                {
                    case "SubCategory":
                        var subCateId = unpacker.GetModelStore<SubCategory>("dw1",
                                           ChangeTrackingStrategy.PropertyState)
                                           .FirstOrDefault().Productsubcategoryid;
                        status = _productService.Delete<SubCategory>(subCateId);

                        break;

                    case "Product":
                        var productId = unpacker.GetModelStore<ProductList>("deletedw1",
                                           ChangeTrackingStrategy.PropertyState)
                                           .FirstOrDefault().Productid;
                        status = _productService.DeleteProduct(productId);

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

        // DELETE api/product/DeleteSubcategoryByKey
        [HttpDelete]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> DeleteSubcategoryByKey(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();
            var subCateId = unpacker.GetValue<int>("arm1");

            try
            {
                var status = _productService.Delete<SubCategory>(subCateId);
                packer.AddValue("Status", status);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return packer;
        }

        // DELETE api/product/DeleteProductByKey
        [HttpDelete]
        [ProducesResponseType(500)]
        public ActionResult<IDataPacker> DeleteProductByKey(IDataUnpacker unpacker)
        {
            var packer = new DataPacker();

            var productId = unpacker.GetValue<int>("arm1");
            try
            {
                var status = _productService.DeleteProduct(productId);

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