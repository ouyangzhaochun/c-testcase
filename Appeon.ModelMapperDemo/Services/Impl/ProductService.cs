using SnapObjects.Data;
using Appeon.ModelMapperDemo.Models;
using System.Collections.Generic;

namespace Appeon.ModelMapperDemo.Services
{
    public class ProductService : ServiceBase<Product>, IProductService
    {
        public ProductService(OrderContext context)
            : base(context)
        {
        }
                
        public void SaveProductPhoto(int productId, string photoName, byte[] photo)
        { 

            var productPhoto = new ProductPhoto()
            {
                LargePhotoFileName = photoName,
                LargePhoto = photo
            };
            
            _context.ModelMapper.TrackCreate(productPhoto);

            var pordProdPhoto = new ProductProductPhoto()
            {
                Primary = 1,
                ProductID = productId               
            };

            _context.ModelMapper.Track(
                (savecontext) => {
                    pordProdPhoto.ProductPhotoID = productPhoto.ProductPhotoID;
                });

            _context.ModelMapper.TrackCreate(pordProdPhoto)
                                .SaveChanges();
        }       

        public int SaveProductAndPrice(IModelEntry<Product> prod, 
                                       IEnumerable<IModelEntry<HistoryPrice>> price)
        {
            var master = _context.ModelMapper.TrackMaster(prod)
                                             .TrackDetails(hp => hp.HistoryPrices, price)
                                             .MasterModel;

            _context.ModelMapper.SaveChanges();

             return master.Productid;
        }

        public int SaveSubCateAndProduct(IModelEntry<SubCategory> subcate, 
                                         IModelEntry<Product> prod)
        {
            return _context.ModelMapper.TrackMaster(subcate)
                                       .TrackDetail(mapper => mapper.Products, prod)
                                       .SaveChanges()
                                       .AffectedCount;
        }

        public int SaveHistoryPrices(IModelEntry<SubCategory> subcate, 
                                     IModelEntry<Product> prod, 
                                     IEnumerable<IModelEntry<HistoryPrice>> price)
        {
            return _context.ModelMapper.TrackMaster(subcate)
                              .TrackDetail(pro => pro.Products, prod)
                              .TrackGrandDetails<Product, HistoryPrice>(pro => pro.Products, 
                                         hprice => hprice.HistoryPrices, price)
                              .SaveChanges()
                              .AffectedCount;
        }
    }
}
