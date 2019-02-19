using SnapObjects.Data;
using System;

namespace Appeon.ModelStoreDemo.Services
{
    public interface IServiceBase
    {
        IModelStore<TModel> Retrieve<TModel>(params object[] keyValue) 
            where TModel : class, new();
        string Delete<TModel>(params object[] parameters) 
            where TModel : class, new();
        string Delete<TModel>(bool autoCommit, params object[] parameters) 
            where TModel : class, new();
        string Delete<TModel>(bool autoCommit, 
                              Predicate<TModel> predicate, params object[] parameters) 
            where TModel : class, new();
        string Update<TModel>(IModelStore<TModel> modelList) 
            where TModel : class, new();
        string Update<TModel>(bool autoCommit, IModelStore<TModel> modelList) 
            where TModel : class, new(); 
    }
}
