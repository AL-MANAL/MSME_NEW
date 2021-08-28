using System.Collections.Generic;
using DataAccess.Models;

namespace DataAccess.Contract
{
    public interface IFeasibilityDAL
    {
        int InsertFeasibility(FeasibilityModel detail);
        List<FeasibilityModel> GetFeasibilities(int? feasibilityId);
    }
}
