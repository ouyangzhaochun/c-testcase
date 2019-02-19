using SnapObjects.Data;
using Appeon.ModelStoreDemo.Models;

namespace Appeon.ModelStoreDemo.Services
{
    public interface IPersonService : IServiceBase
    {
        int SavePerson(IModelStore<Person> person,
                   IModelStore<BusinessEntityAddress> addresses,
                   IModelStore<PersonPhone> phones,
                   IModelStore<Customer> customers);
        string DeletePerson(int personId);       
    }
}
