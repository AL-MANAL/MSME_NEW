using DataAccess.Contract;
using DataAccess.Models;
using HA.HALoG5BWService.DatabaseAccess.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Impl
{
    public class DropDownDAL : GenericRepository<DropDownModel>, IDropDownDAL
    {
        private readonly string _connectionString;

        #region Constructor

        public DropDownDAL() : base(Constants.DbConnectionString)
        {
            this._connectionString = Constants.DbConnectionString;
        }
        #endregion

        public List<DropDownModel> GetDepartment(string type)
        {
            object parameters = new
            {
                PFilterType = type,
            };
            var transaction = base.GetAll(parameters, "get_expeditor_departments");

            return transaction;
        }
    }
}
