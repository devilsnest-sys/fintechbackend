using System.Xml.Linq;

namespace TscLoanManagement.TSCDB.Core.Domain.Masters
{
    public interface IMasterEntity
    {
        int Id { get; set; }
        string Name { get; set; }
    }

    public class EntityType : IMasterEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BusinessCategory : IMasterEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BusinessType : IMasterEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AddressStatus : IMasterEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PersonType : IMasterEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BankDetail : IMasterEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }



        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string IFSC { get; set; }
        public string BranchName { get; set; }
        public bool is_credit_account { get; set; }
    }

    public class PurchaseSource : IMasterEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? LoanBidAmtPercentage { get; set; }
        public bool? IsApplicationNoRequired { get; set; }
    }

    public class Make : IMasterEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class DocumentType : IMasterEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., RC, PAN, Aadhar, ITR, etc.
    }

    public class ChequeLocation : IMasterEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., RC, PAN, Aadhar, ITR, etc.
    }
    public class Designation : IMasterEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


    public class PurchaseSourceDocument1    
    {
        public int Id { get; set; }
        public int PurchaseSourceId { get; set; }
        public string DocumentName { get; set; }

        public PurchaseSource PurchaseSource { get; set; }
        public bool is_mandatory { get; set; }
    }

}
