﻿using SnapObjects.Data;
using System.Collections.Generic;

namespace Appeon.ModelMapperDemo.Services
{
    public abstract class ServiceBase<TModel> 
        where TModel : class
    {
        protected readonly DataContext _context;

        protected ServiceBase(DataContext context)
        {
            _context = context;
        }

        public IList<TModel> Retrieve(bool includeEmbedded, params object[] parameters)
        {            
            if (includeEmbedded)
            {
                return _context.ModelMapper.Load<TModel>(parameters)
                                           .IncludeAll(0, cascade:true)
                                           .ToList();
            }
            else
            {
                return _context.ModelMapper.Load<TModel>(parameters)
                                           .ToList();
            }
        }

        public TModel RetrieveByKey(bool includeEmbedded, params object[] parameters)
        {
            if (includeEmbedded)
            {
                return _context.ModelMapper.LoadByKey<TModel>(parameters)
                                           .IncludeAll()
                                           .FirstOrDefault(); 
            }
            else
            {
                return _context.ModelMapper.LoadByKey<TModel>(parameters)
                                           .FirstOrDefault();
            }
        }
        
    }
}
