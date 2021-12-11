using System.Collections.Generic;

namespace HA.HALoG5BWService.DatabaseAccess.Contract
{
    public interface IGenericRepository<T> where T : class
    {
        T Get(object parameters, string storedProcedure);

        List<T> GetAll(object parameters, string storedProcedure);

        long Insert(object parameters, string storedProcedure);

        int Update(object parameters, string storedProcedure);

        void ExecWithStoreProcedure(object parameters, string storedProcedure);

        T InsertEntity(object parameters, string storedProcedure);
    }
}