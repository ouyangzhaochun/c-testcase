using SnapObjects.Data;
using Appeon.ModelStoreDemo.Models;
using System;
using System.Linq;

namespace Appeon.ModelStoreDemo.Services
{
    /// <summary>
    /// This Service needs to be injected into the ConfigureServices method of the Startup class. Sample code as follows:
    /// services.AddScoped<IPersonService, PersonService>();
    /// </summary>
    class PersonService : ServiceBase, IPersonService
    {
        public PersonService(OrderContext context) 
            : base(context)
        {
        }

        public string DeletePerson(int personId)
        {
            string status = "Success";

            _context.BeginTransaction();
            status = Delete<BusinessEntityAddress>(false, personId);
            status = Delete<PersonPhone>(false, personId);
            status = Delete<Customer>(false, personId);
            status = Delete<Person>(false, personId);
            _context.Commit();
 
            return status;
        }

        public int SavePerson(IModelStore<Person> person,
                   IModelStore<BusinessEntityAddress> addresses,
                   IModelStore<PersonPhone> phones,
                   IModelStore<Customer> customers)
        {

            int intPersonId = 0;

            _context.BeginTransaction();

            if (person.TrackedCount(StateTrackable.NewModified) == 1)
            {
                var businessEntity = new ModelStore<BusinessEntity>()
                    .TrackChanges(ChangeTrackingStrategy.PropertyState);

                businessEntity.Add(new BusinessEntity() { ModifiedDate = DateTime.Now });

                var result = businessEntity.SaveChanges(_context);

                if (result.InsertedCount == 1)
                {
                    intPersonId = businessEntity.FirstOrDefault().BusinessEntityID;
                    person.SetValue(0, "Businessentityid", intPersonId);
                }
            }
            else
            {
                intPersonId = person.FirstOrDefault().Businessentityid;

            }
               
               
            SetPrimaryKey(person, addresses, phones, customers);

            //Save person address, phone, customer
            person.SaveChanges(_context);
            addresses.SaveChanges(_context);
            phones.SaveChanges(_context);
            customers.SaveChanges(_context);

            _context.Commit();

            return intPersonId;           
                                      
        }
        private void SetPrimaryKey(IModelStore<Person> person,
                                  IModelStore<BusinessEntityAddress> addresses,
                                  IModelStore<PersonPhone> phones,
                                  IModelStore<Customer> customers)
        {
            if (person.TrackedCount(StateTrackable.Deleted) == 0 &&
                person.Count > 0)
            {
                var PersonID = person.FirstOrDefault().Businessentityid;
               

                for (int i = 0; i < addresses.Count; i++)
                {
                    if (addresses.GetModelState(i) == ModelState.NewModified)
                    {
                        addresses.SetValue(i, "Businessentityid", PersonID);
                    }
                }

                for (int i = 0; i < phones.Count; i++)
                {
                    if (phones.GetModelState(i) == ModelState.NewModified)
                    {
                        phones.SetValue(i, "Businessentityid", PersonID);
                    }
                }

                for (int i = 0; i < customers.Count; i++)
                {
                    if (customers.GetModelState(i) == ModelState.NewModified)
                    {
                        customers.SetValue(i, "Personid", PersonID);
                    }
                }
            }
        }
    }
}
