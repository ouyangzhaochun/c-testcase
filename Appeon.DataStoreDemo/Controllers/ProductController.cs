using SnapObjects.Data;
using SnapObjects.Data.PowerBuilder;
using Appeon.DataStoreDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Appeon.DataStoreDemo.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
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
            int intCateId = 0;

            var productCate = _productService.Retrieve("d_dddw_category");

            if (productCate.RowCount > 0)
            {
                intCateId = productCate.GetItem<int>(0, "productcategoryid");
            }      

            packer.AddValue("CateId", intCateId.ToString());
            packer.AddDataStore("Category", productCate);
            packer.AddDataStore("SubCategory", 
                _productService.Retrieve("d_subcategory_list", intCateId));
            packer.AddDataStore("Units", _productService.Retrieve("d_dddw_unit"));

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
                    var subData = _productService.Retrieve("d_subcategory_list", id);
                    if (subData.RowCount == 0)
                    {
                        return NotFound();
                    }
                    packer.AddDataStore("SubCategory", subData);

                    break;

                case "d_product":
                    packer.AddDataStore("Product", 
                        _productService.Retrieve("d_product", id));

                    packer.AddDataStore("dddwSubCategory", 
                        _productService.Retrieve("d_subcategory_list", id));

                    break;

                case "d_history_price":
                    packer.AddDataStore("HistoryPrice", 
                        _productService.Retrieve("d_history_price", id));
                    packer.AddDataStore("dddwProduct", 
                        _productService.Retrieve("d_dddw_product"));

                    if (_productService.retrieveProductPhote(id, out string photoname, 
                        out byte[] largePhoto))
                    {
                        packer.AddValue("photo", largePhoto);
                        packer.AddValue("photoname", photoname);
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

            var product = unpacker.GetDataStore("dw1", "d_product");
            var prices = unpacker.GetDataStore("dw2", "d_history_price");

            try
            {
                var productId = _productService.SaveProductAndPrice(product, prices);
                
                packer.AddDataStore("Product", 
                    _productService.Retrieve("d_product_detail", productId));
                packer.AddDataStore("Product.HistoryPrice", 
                    _productService.Retrieve("d_history_price", productId));

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

            var subcate = unpacker.GetDataStore("dw1", "d_subcategory_list");
            var product = unpacker.GetDataStore("dw2", "d_product");
            var prices = unpacker.GetDataStore("dw3", "d_history_price");
            try
            {

                var ret = _productService.SaveHistoryPrices(subcate, product, prices);

                packer.AddDataStore("SubCategory", subcate);
                packer.AddDataStore("Product", product);
                packer.AddDataStore("Product.HistoryPrice", prices);
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
                        var subcate = unpacker.GetDataStore("dw1", "d_subcategory_list");
                        status = _productService.Update(true, subcate);

                        packer.AddDataStore("SubCategory", subcate);

                        break;

                    case "Product":
                        var prod = unpacker.GetDataStore("dw1", "d_product");
                        status = _productService.Update(true, prod);

                        if (prod.RowCount > 0)
                        {
                            var productId = prod.GetItem<int>(0, "productid");
                            packer.AddDataStore("Product",
                                _productService.Retrieve("d_product", productId));
                            packer.AddDataStore("Product.HistoryPrice",
                                _productService.Retrieve("d_history_price", productId));
                        }

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
                        var subCateId = unpacker.GetDataStore("dw1", "d_subcategory")
                                                .FirstOrDefault<D_Subcategory>()
                                                .Productsubcategoryid;

                        status = _productService.Delete("d_subcategory", subCateId);

                        break;

                    case "Product":
                        var productId = unpacker.GetDataStore("deletedw1", "d_product")
                                                .FirstOrDefault<D_Product>()
                                                .Productid;

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
                var status = _productService.Delete("d_subcategory", subCateId);
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