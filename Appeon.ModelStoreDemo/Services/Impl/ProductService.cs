using SnapObjects.Data;
using Appeon.ModelStoreDemo.Models;
using System;
using System.Linq;

namespace Appeon.ModelStoreDemo.Services
{
    /// <summary>
    /// This Service needs to be injected into the ConfigureServices method of the Startup class. Sample code as follows:
    /// services.AddScoped<IProductService, ProductService>();
    /// </summary>
    public class ProductService : ServiceBase, IProductService
    {
        public ProductService(OrderContext context)
            : base(context)
        {
        }

        public void SaveProductPhoto(int productId, string photoname, byte[] photo)
        {

            _context.BeginTransaction();

            var productPhoto = new ModelStore<ProductPhoto>()
                                    .TrackChanges(ChangeTrackingStrategy.PropertyState);
            productPhoto.Add(new ProductPhoto()
            {
                ProductPhotoID = productId,
                LargePhotoFileName = photoname,
                LargePhoto = photo
            });

            var result = productPhoto.SaveChanges(_context);
            if (result.InsertedCount == 1)
            {
                var productPhotoID = productPhoto.FirstOrDefault().ProductPhotoID;

                var pordProdPhoto = new ModelStore<ProductProductPhoto>()
                                        .TrackChanges(ChangeTrackingStrategy.PropertyState);
                pordProdPhoto.Add(new ProductProductPhoto()
                {
                    ProductID = productId,
                    ProductPhotoID = productPhotoID,
                    Primary = 1

                });

                pordProdPhoto.SaveChanges(_context);
            }

            _context.Commit();
        }

        public string DeleteProduct(int productId)
        {
            string status = "Success";

            _context.BeginTransaction();
            status = Delete<ProductProductPhoto>(false, productId);
            status = Delete<HistoryPrice>(false, productId);
            status = Delete<Product>(false, productId);
            _context.Commit();

            return status;

        }

        public int SaveHistoryPrices(IModelStore<SubCategory> subcate,
                                     IModelStore<Product> product,
                                     IModelStore<HistoryPrice> prices)
        {
            int intSubCateId = 0;
            int intProductId = 0;

            _context.BeginTransaction();

            subcate.SaveChanges(_context);

            intSubCateId = subcate.FirstOrDefault().Productsubcategoryid;

            SetProductPrimaryKey(subcate, product);
            product.SaveChanges(_context);
            intProductId = product.FirstOrDefault().Productid;

            SetPricePrimaryKey(product, prices);
            prices.SaveChanges(_context);

            _context.Commit();
            
            return intSubCateId;
        }

        public int SaveProductAndPrice(IModelStore<Product> product,
                                       IModelStore<HistoryPrice> prices)
        {
            int intProductId = 0;

            _context.BeginTransaction();

            product.SaveChanges(_context);

            intProductId = product.FirstOrDefault().Productid;

            SetPricePrimaryKey(product, prices);
            prices.SaveChanges(_context);

            _context.Commit();

            return intProductId;
        }

        private void SetPricePrimaryKey(IModelStore<Product> product,
                                   IModelStore<HistoryPrice> prices)
        {
            if (product.TrackedCount(StateTrackable.Deleted) == 0 &&
                product.Count > 0)
            {
                var productId = product.FirstOrDefault().Productid;

                for (int i = 0; i < prices.Count; i++)
                {
                    if (prices.GetModelState(i) == ModelState.NewModified)
                    {
                        prices.SetValue(i, "Productid", productId);
                    }
                }

            }
        }

        private void SetProductPrimaryKey(IModelStore<SubCategory> subcate,
                                   IModelStore<Product> product)
        {
            if (subcate.TrackedCount(StateTrackable.Deleted) == 0 &&
                subcate.Count > 0)
            {
                var subCateId = subcate.FirstOrDefault().Productsubcategoryid;

                for (int i = 0; i < product.Count; i++)
                {
                    if (product.GetModelState(i) == ModelState.NewModified)
                    {
                        product.SetValue(i, "Productsubcategoryid", subCateId);
                    }
                }
            }
        }
    }
}
