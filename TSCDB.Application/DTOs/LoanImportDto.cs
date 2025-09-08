namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class LoanImportDto
    {
        public int RowNumber { get; set; }
        public string LoanId { get; set; }
        public string LoanNumber { get; set; }
        public string DealerCode { get; set; }
        public string DealershipName { get; set; }
        public string Status { get; set; }
        public DateTime DateOfWithdraw { get; set; }
        public decimal Amount { get; set; }
        public string UtrNumber { get; set; }
        public decimal InterestRate { get; set; }
        public decimal DelayedROI { get; set; }
        public decimal ProcessingFee { get; set; }
        public decimal? GSTOnProcessingFee { get; set; }
        public DateTime? ProcessingFeeReceivedDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? SettledDate { get; set; }


        // Vehicle Information
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleRegistrationNumber { get; set; }
        public int VehicleYear { get; set; }
        public string VehicleChassisNumber { get; set; }
        public string VehicleEngineNumber { get; set; }
        public decimal VehicleValue { get; set; }

        // Purchase Information
        public string PurchaseSource { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? InterestWaiver { get; set; }
        public string? WaiverType { get; set; } = string.Empty;
        public List<PaymentRecordDto>? Payments { get; set; } = new();

        // Payment 1
        //public DateTime? Payment1Date { get; set; }
        //public decimal? PaymentRecieved1 { get; set; }
        //public decimal? InterestEarned1 { get; set; }
        //public decimal? DelayedInterestEarned1 { get; set; }
        //public decimal? PrincipalRecieved1 { get; set; }

        //// Payment 2
        //public DateTime? Payment2Date { get; set; }
        //public decimal? PaymentRecieved2 { get; set; }
        //public decimal? InterestEarned2 { get; set; }
        //public decimal? DelayedInterestEarned2 { get; set; }
        //public decimal? PrincipalRecieved2 { get; set; }

        //// Payment 3
        //public DateTime? Payment3Date { get; set; }
        //public decimal? PaymentRecieved3 { get; set; }
        //public decimal? InterestEarned3 { get; set; }
        //public decimal? DelayedInterestEarned3 { get; set; }
        //public decimal? PrincipalRecieved3 { get; set; }
    }

}
