﻿using SnapObjects.Data.PowerBuilder;

namespace Appeon.DataStoreDemo.Services
{
    public interface IPersonService : IServiceBase
    {
        int SavePerson(IDataStore person, IDataStore addresses, IDataStore phones, IDataStore customers);

        string DeletePerson(int personId);

    }
}
