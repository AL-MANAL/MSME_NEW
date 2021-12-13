using DataAccess.Models;
using System;
using System.Collections.Generic;

namespace DataAccess.Contract
{
    public interface IExpeditorDAL /*: IGenericRepository<ExpeditorTemplateModel>*/
    {
        int InsertOrderLineItem(ExpeditorTemplateModel detail);

        List<ExpeditorTemplateModel> GetOrderLineItem(DateTime? fromDate, DateTime? toDate, string dep, string status, string customerPo, string supplierPo, string salesman);

        int InsertOrderItem(ExpeditorOrderItemModel detail);
    }

    public interface IExpeditorOrderDAL
    {
        List<ExpeditorOrderItemModel> GetOrderLineItemsById(int id);
    }
}