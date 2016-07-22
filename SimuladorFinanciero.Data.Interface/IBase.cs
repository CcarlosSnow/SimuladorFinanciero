using System;
using System.Collections.Generic;

namespace SimuladorFinanciero.Data.Interface
{
    public interface IBase<T> : IDisposable
    {
        IList<T> SelectAll();
        T Select(int id);
        bool Insert(T entidad);
        bool Update(T entidad);
        bool Delete(T entidad);
    }
}
