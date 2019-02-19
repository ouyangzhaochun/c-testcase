using SnapObjects.Data;
using System;

namespace Appeon.ModelStoreDemo.Services
{
    public abstract class ServiceBase : IServiceBase
    {
        protected readonly OrderContext _context;

        protected ServiceBase(OrderContext context)
        {
            _context = context;
        }
        public string Delete<TModel>(params object[] parameters) 
            where TModel : class, new()
        {
            return Delete<TModel>(true, parameters);

        }
        public string Delete<TModel>(bool autoCommit,
            params object[] parameters) where TModel : class, new()
        {
            return Delete<TModel>(autoCommit, m => true, parameters);

        }
        public string Delete<TModel>(bool autoCommit,
            Predicate<TModel> predicate, params object[] parameters) 
            where TModel : class, new()
        {

            var modelList = Retrieve<TModel>(parameters)
                            .TrackChanges(ChangeTrackingStrategy.PropertyState);

            modelList.RemoveAll(predicate);

            if (autoCommit)
            {
                _context.BeginTransaction();
                modelList.SaveChanges(_context);
                _context.Commit();
            }
            else
            {
                modelList.SaveChanges(_context);
            }

            return "Success";

        }
        public string Update<TModel>(IModelStore<TModel> modelList) 
            where TModel : class, new()
        {
            return Update(true, modelList);
        }
        public string Update<TModel>(bool autoCommit,
                                     IModelStore<TModel> modelList) 
            where TModel : class, new()
        {

            if (autoCommit)
            {
                _context.BeginTransaction();
                modelList.SaveChanges(_context);
                _context.Commit();
            }
            else
            {
                modelList.SaveChanges(_context);
            }

            return "Success";
        }
        public IModelStore<TModel> Retrieve<TModel>(params object[] keyValue) 
            where TModel : class, new()
        {
            var modelStore = new ModelStore<TModel>();
            modelStore.Retrieve(_context, keyValue);
            return modelStore;

        }
    }
}
