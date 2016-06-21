using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
