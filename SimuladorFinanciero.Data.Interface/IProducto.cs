using System.Collections.Generic;
using SimuladorFinanciero.Entities;

namespace SimuladorFinanciero.Data.Interface
{
    public interface IProducto : IBase<Producto>
    {
        int GetIdProducto(string Nombre);
        bool DeleteAll();
        IList<Producto> SelectByTipo(int Tipo);
    }
}
