namespace TscLoanManagement.TSCDB.Application.DTOs
{
    public class ForecastLoanResponseDto
    {
        public decimal DisbursementAmount { get; set; }
        public decimal InterestCharge { get; set; }
        public decimal ProcessingFee { get; set; }
        public decimal ProcessingFeeWithGST { get; set; }
        public decimal gst { get; set; }
        public decimal TotalPayableAmount { get; set; }

        public decimal ROI { get; set; }             // New
        public decimal DelayROI { get; set; }
    }

}
