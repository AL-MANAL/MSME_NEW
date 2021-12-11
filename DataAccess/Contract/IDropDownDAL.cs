using DataAccess.Models;
using System.Collections.Generic;

namespace DataAccess.Contract
{
    public interface IDropDownDAL
    {
        List<DropDownModel> GetDepartment(string type);
    }
}