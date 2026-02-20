using static System.Runtime.InteropServices.JavaScript.JSType;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Core.Domain.Authentication;
using TscLoanManagement.TSCDB.Core.Domain.Dealer;
using TscLoanManagement.TSCDB.Core.Domain.Loan;
using AutoMapper;
using TscLoanManagement.TSCDB.Core.Domain.LoanInstallment;
using TscLoanManagement.TSCDB.Core.Domain.DDR;
using TscLoanManagement.TSCDB.Core.Domain.Masters;
using TscLoanManagement.TSCDB.Application.DTOs.PurchaseSourceDocuments;
using TscLoanManagement.TSCDB.Application.DTOs.LoanDocuments;
using TscLoanManagement.TSCDB.Core.Domain.LoanDocuments;

namespace TscLoanManagement.TSCDB.Application.Mappings
{
    // SOLID (SRP): keeps object-object mapping concerns in one place.
    // OOP (Abstraction): decouples domain models from API DTO shapes.
    // Depends on: DTO definitions and Core domain entities.
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Mappings
            CreateMap<User, UserDto>()  ;
            CreateMap<UserDto, User>();
            CreateMap<RegisterRequestDto, User>();

            // Dealer Mappings
            CreateMap<Dealer, DealerDto>()
    .ForMember(dest => dest.ChequeDetails, opt => opt.MapFrom(src => src.ChequeDetails))
    .ForMember(dest => dest.BorrowerDetails, opt => opt.MapFrom(src => src.BorrowerDetails))
    .ForMember(dest => dest.GuarantorDetails, opt => opt.MapFrom(src => src.GuarantorDetails))
    .ForMember(dest => dest.SecurityDepositDetails, opt => opt.MapFrom(src => src.SecurityDepositDetails))
    .ForMember(dest => dest.DocumentUploads, opt => opt.MapFrom(src => src.DocumentUploads))
    .ForMember(dest => dest.RelationshipManagerName, opt =>
        opt.MapFrom(src => src.RelationshipManager != null ? src.RelationshipManager.Name : null))
    .ReverseMap()
    .ForMember(dest => dest.ChequeDetails, opt => opt.Ignore())
    .ForMember(dest => dest.BorrowerDetails, opt => opt.Ignore())
    .ForMember(dest => dest.GuarantorDetails, opt => opt.Ignore())
    .ForMember(dest => dest.SecurityDepositDetails, opt => opt.Ignore())
    .ForMember(dest => dest.DocumentUploads, opt => opt.Ignore())
    .ForMember(dest => dest.RelationshipManager, opt => opt.Ignore()) // Ignore navigation property
    .ForMember(dest => dest.RelationshipManagerId, opt => opt.MapFrom(src => src.RelationshipManagerId)) // ✅ Add this line to map FK
    .ForMember(dest => dest.Loans, opt => opt.Ignore());


            // Related entity mappings
            CreateMap<BorrowerDetails, BorrowerDetailsDto>().ReverseMap();
            CreateMap<GuarantorDetails, GuarantorDetailsDto>().ReverseMap();
            CreateMap<ChequeDetails, ChequeDetailsDto>().ReverseMap();
            CreateMap<SecurityDepositDetails, SecurityDepositDetailsDto>().ReverseMap();
            CreateMap<DocumentUpload, DocumentUploadDto>().ReverseMap();

            CreateMap<GuarantorDetailsDto, GuarantorDetails>()
           .ForMember(dest => dest.Id, opt => opt.Ignore());


            CreateMap<BorrowerDetailsDto, BorrowerDetails>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());




            CreateMap<CreateUpdateLoanDto, Loan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // if you don't want to update ID on create
                .ForMember(dest => dest.LoanNumber, opt => opt.Ignore());

            CreateMap<Loan, LoanDto>()
                //.ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer.DealershipName))
                .ForMember(dest => dest.Dealer, opt => opt.MapFrom(src => src.Dealer))
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments.Select(a => a.FilePath)))
                 .ForMember(dest => dest.VehicleInfo, opt => opt.MapFrom(src => src.VehicleInfo))
                .ForPath(dest => dest.DDR.VehicleInfo, opt => opt.MapFrom(src => src.DDR.VehicleInfo))
                //.ForMember(dest => dest.BuyerInfo, opt => opt.MapFrom(src => src.BuyerInfo))
                .ForMember(dest => dest.LoanPayments, opt => opt.MapFrom(src => src.LoanPayments))
                 .ForMember(dest => dest.LoanDetail, opt => opt.MapFrom(src => src.LoanDetail))
            .ForMember(dest => dest.LoanDetailId, opt => opt.MapFrom(src => src.LoanDetail != null ? src.LoanDetail.LoanId : (int?)null))
             .ForMember(dest => dest.DDR, opt => opt.MapFrom(src => src.DDR));

            CreateMap<LoanDto, Loan>()
                .ForMember(dest => dest.Attachments, opt => opt.Ignore())
                .ForMember(dest => dest.Dealer, opt => opt.Ignore())
               .ForPath(dest => dest.DDR.VehicleInfo, opt => opt.MapFrom(src => src.DDR.VehicleInfo))
                //.ForMember(dest => dest.BuyerInfo, opt => opt.MapFrom(src => src.BuyerInfo))
                .ForMember(dest => dest.LoanDetail, opt => opt.Ignore())
                .ForMember(dest => dest.LoanPayments, opt => opt.MapFrom(src => src.LoanPayments));

            // VehicleInfo & BuyerInfo (already exist, but reiterating)
            CreateMap<VehicleInfo, VehicleInfoDto>().ReverseMap();
            CreateMap<BuyerInfo, BuyerInfoDto>().ReverseMap();

            // LoanPayment Mapping
            CreateMap<LoanPayment, LoanPaymentDto>().ReverseMap();

            // Loan calculations and detail mapping

            // Add at the bottom of your existing MappingProfile constructor

            // LoanDetail Mappings
            CreateMap<LoanDetail, LoanDetailDto>()
                .ForMember(dest => dest.LoanFee, opt => opt.MapFrom(src => src.LoanFee))
                .ForMember(dest => dest.Installments, opt => opt.MapFrom(src => src.Installments))
                .ForMember(dest => dest.Interests, opt => opt.MapFrom(src => src.Interests));

            CreateMap<LoanDetailDto, LoanDetail>()
                .ForMember(dest => dest.LoanFee, opt => opt.MapFrom(src => src.LoanFee))
                .ForMember(dest => dest.Installments, opt => opt.MapFrom(src => src.Installments))
                .ForMember(dest => dest.Interests, opt => opt.MapFrom(src => src.Interests));

            // LoanFee Mappings
            CreateMap<LoanFee, LoanFeeDto>().ReverseMap();

            // LoanInstallment Mappings
            CreateMap<LoanInstallment, LoanInstallmentDto>().ReverseMap();

            // LoanInterest Mappings
            CreateMap<LoanInterest, LoanInterestDto>().ReverseMap();

            // Waiver Mappings
            CreateMap<Waiver, WaiverDto>().ReverseMap();

            CreateMap<DDR, Loan>()
                           .ForMember(dest => dest.Id, opt => opt.Ignore())
                           .ForMember(dest => dest.LoanNumber, opt => opt.MapFrom(src => src.DDRNumber))
                           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"))
                              //.ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments))
                              .ForMember(dest => dest.ProcessingFee, opt => opt.MapFrom(src => src.SelectedProcessingFee))
                              .ForMember(dest => dest.SuggestedProcessingFee, opt => opt.MapFrom(src => src.SuggestedProcessingFee))
                                 .ForMember(dest => dest.SelectedProcessingFee, opt => opt.MapFrom(src => src.SelectedProcessingFee))
                           .ForMember(dest => dest.VehicleInfo, opt => opt.MapFrom(src => src.VehicleInfo))
                           .ForMember(dest => dest.BuyerInfo, opt => opt.Ignore()); // Ignore BuyerInfo as it's not in DDR

            // DDR mappings - removed BuyerInfo
            CreateMap<DDR, DdrDto>().ReverseMap();
            CreateMap<DDR, DdrResponseDto>()
                 .ForMember(dest => dest.SuggestedProcessingFee, opt => opt.MapFrom(src => src.SuggestedProcessingFee))
                    .ForMember(dest => dest.SelectedProcessingFee, opt => opt.MapFrom(src => src.SelectedProcessingFee))
                        //.ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer.DealershipName))
                    .ForMember(dest => dest.Dealer, opt => opt.MapFrom(src => src.Dealer));

            // Create/Update mappings - VehicleInfo only
            CreateMap<DdrCreateDto, DDR>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.SuggestedProcessingFee, opt => opt.MapFrom(src => src.SuggestedProcessingFee))
                .ForMember(dest => dest.SelectedProcessingFee, opt => opt.MapFrom(src => src.SelectedProcessingFee))
                .ForMember(dest => dest.Dealer, opt => opt.Ignore());
                //.ForMember(dest => dest.Attachments, opt => opt.Ignore());

            CreateMap<DdrUpdateDto, DDR>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Dealer, opt => opt.Ignore())
                //.ForMember(dest => dest.Attachments, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<DDRDocument, DDRDocumentUploadDto>().ReverseMap();


            // VehicleInfo mappings - Updated to match your entity structure
            CreateMap<VehicleInfoDto, VehicleInfo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                //.ForMember(dest => dest.LoanId, opt => opt.Ignore())
                .ForMember(dest => dest.DDRId, opt => opt.Ignore())
                //.ForMember(dest => dest.Loan, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<LoanDocumentUpload, LoanDocumentUploadDto>().ReverseMap();
            CreateMap<Permission, PermissionDto>().ReverseMap();
            CreateMap<Role, RoleDto>()
                .ForMember(dest => dest.Permissions, opt =>
                    opt.MapFrom(src => src.RolePermissions.Select(rp => rp.Permission)))
                .ReverseMap()
                .ForMember(dest => dest.RolePermissions, opt =>
                    opt.MapFrom(src => src.Permissions.Select(p => new RolePermission { PermissionId = p.Id})));

            CreateMap<EntityType, MasterDto>().ReverseMap();
            CreateMap<BusinessCategory, MasterDto>().ReverseMap();
            CreateMap<BusinessType, MasterDto>().ReverseMap();
            CreateMap<AddressStatus, MasterDto>().ReverseMap();
            CreateMap<PersonType, MasterDto>().ReverseMap();
            CreateMap<BankDetail, MasterDto>().ReverseMap();
            CreateMap<PurchaseSource, MasterDto>().ReverseMap();
            CreateMap<Make, MasterDto>().ReverseMap();
            CreateMap<ChequeLocation, MasterDto>().ReverseMap();
            CreateMap<LocationDto, LocationDto>().ReverseMap();
            CreateMap<MasterDto, Designation>().ReverseMap();



            CreateMap<PurchaseSourceDocument1, PurchaseSourceDocumentDto>().ReverseMap();

            CreateMap<LoanDocumentMasterDto, LoanDocumentMaster>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<LoanDocumentActivityDto, LoanDocumentActivity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<LoanDocumentMaster, LoanDocumentMasterDto>();
            CreateMap<LoanDocumentActivity, LoanDocumentActivityDto>();
            CreateMap<BankDetail, BankDetailDto>().ReverseMap();
            CreateMap<PurchaseSource, PurchaseSourceDto>().ReverseMap();

            //CreateMap<LoanDocumentMaster, LoanDocumentMasterDto>().ReverseMap();
            //CreateMap<LoanDocumentActivity, LoanDocumentActivityDto>().ReverseMap();
        }
    }
}
