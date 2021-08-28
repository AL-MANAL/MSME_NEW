using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Contract
{
    public interface IDropDownDAL
    {
        List<DropDownModel> GetDepartment(string type);
    }
}
