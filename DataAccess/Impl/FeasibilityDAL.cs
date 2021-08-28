using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using DataAccess.Contract;
using DataAccess.Models;
using HA.HALoG5BWService.DatabaseAccess.Impl;
using MySql.Data.MySqlClient;

namespace DataAccess.Impl
{
    public class FeasibilityDAL : GenericRepository<FeasibilityModel>, IFeasibilityDAL
    {
        private readonly string _connectionString;

        #region Constructor

        public FeasibilityDAL() : base(Constants.DbConnectionString)
        {
            this._connectionString = Constants.DbConnectionString;
        }
        #endregion

        //#region DB methods

        public List<FeasibilityModel> GetFeasibilities(int? feasibilityId)
        {
            object parameters = new
            {
                pfeasibilityId = feasibilityId
            };
            var transaction = base.GetAll(parameters, "");

            return transaction;
        }

        public int InsertFeasibility(FeasibilityModel detail)
        {
            object parameters = new
            {
                pEnquiryId = detail.EnquiryId,
                pTechnicalFeasibility = detail.TechnicalFeasibility,
                pTechnicalFeasibilityReason = detail.TechnicalFeasibilityReason,
                pOperationalFeasibility = detail.OperationalFeasibility,
                pOperationalFeasibilityReason = detail.OperationalFeasibilityReason,
                pCommercialFeasibility = detail.CommercialFeasibility,
                pCommercialFeasibilityReason = detail.CommercialFeasibilityReason,
                pPaymentFeasibility = detail.PaymentFeasibility,
                pPaymentFeasibilityReason = detail.PaymentFeasibilityReason,
                pCustomerReputation = detail.CustomerReputation,
                pCustomerReputationReason = detail.CustomerReputationReason,
                pFinalFeasibility = detail.FinalFeasibility,
                pCreatedBy = detail.CreatedBy,
                pCreatedDate = detail.CreatedDate
            };

            return (int)base.Insert(parameters, "insert_enquiries_feasibility");
        }
        //#endregion
    }
}
