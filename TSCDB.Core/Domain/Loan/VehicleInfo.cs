namespace TscLoanManagement.TSCDB.Core.Domain.Loan
{
    public class VehicleInfo
    {
        public int Id { get; set; }
        public int? LoanId { get; set; }
        public string Make { get; set; }     // Newly mapped
        public string Model { get; set; }    // Newly mapped
        public string RegistrationNumber { get; set; }
        public int? Year { get; set; }       // Newly mapped
        public string ChassisNumber { get; set; } // Newly mapped
        public string EngineNumber { get; set; }  // Newly mapped
        public decimal? Value { get; set; }  // Newly mapped

        //public virtual Loan Loan { get; set; }
        //public int? LoanId { get; set; }  // For Loan relationship
        public int? DDRId { get; set; }
        public string? Color { get; set; }
        public string? ApplicationNumber { get; set; }
    }
}
