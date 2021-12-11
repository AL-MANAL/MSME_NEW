using DataAccess.Models;
using System.Collections.Generic;

namespace DataAccess.Contract
{
    public interface IFeasibilityDAL
    {
        int InsertFeasibility(FeasibilityModel detail);

        List<FeasibilityModel> GetFeasibilities(int? feasibilityId);
    }
}