using System;

namespace DataAccess.Models
{
    public class FeasibilityModel
    {
        public int Id { get; set; }
        public int EnquiryId { get; set; }
        public YesNo TechnicalFeasibility { get; set; }
        public string TechnicalFeasibilityReason { get; set; }

        public YesNo OperationalFeasibility { get; set; }
        public string OperationalFeasibilityReason { get; set; }

        public YesNo CommercialFeasibility { get; set; }
        public string CommercialFeasibilityReason { get; set; }

        public YesNo PaymentFeasibility { get; set; }
        public string PaymentFeasibilityReason { get; set; }

        public YesNo CustomerReputation { get; set; }
        public string CustomerReputationReason { get; set; }
        public YesNo FinalFeasibility { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public enum YesNo
    {
        No,
        Yes
    }
}