using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TscLoanManagement.Migrations
{
    /// <inheritdoc />
    public partial class Baseline_20260220 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddressStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IFSC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_credit_account = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChequeLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChequeLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DDRDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DdrId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UploadedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DDRDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Designations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanAccrual",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanId = table.Column<int>(type: "int", nullable: true),
                    LoanNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SnapshotDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PrincipalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DelayInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccruedInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AccruedDelayedInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalPrincipalPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalInterestPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalDelayedInterestPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OutstandingPrincipal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OutstandingInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OutstandingDelayedInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProcessingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GSTOnProcessingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalFeeWithGST = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalWaivedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanAccrual", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanDetails",
                columns: table => new
                {
                    LoanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    PrincipalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProcessingFeeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GSTPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDays = table.Column<int>(type: "int", nullable: false),
                    DelayInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanDetails", x => x.LoanId);
                });

            migrationBuilder.CreateTable(
                name: "Makes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Makes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OtpVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Otp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpVerifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PendingInstallments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanId = table.Column<int>(type: "int", nullable: true),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingInstallments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    category = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoanBidAmtPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsApplicationNoRequired = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanFees",
                columns: table => new
                {
                    LoanFeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanId = table.Column<int>(type: "int", nullable: false),
                    FeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GSTAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalFeeWithGST = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanFees", x => x.LoanFeeId);
                    table.ForeignKey(
                        name: "FK_LoanFees_LoanDetails_LoanId",
                        column: x => x.LoanId,
                        principalTable: "LoanDetails",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanInstallments",
                columns: table => new
                {
                    InstallmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanId = table.Column<int>(type: "int", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdjustedToFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdjustedToInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdjustedToPrincipal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdjustedToDelayInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DueFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDelayInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanInstallments", x => x.InstallmentId);
                    table.ForeignKey(
                        name: "FK_LoanInstallments_LoanDetails_LoanId",
                        column: x => x.LoanId,
                        principalTable: "LoanDetails",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanInterests",
                columns: table => new
                {
                    InterestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanId = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InterestType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InterestAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanInterests", x => x.InterestId);
                    table.ForeignKey(
                        name: "FK_LoanInterests_LoanDetails_LoanId",
                        column: x => x.LoanId,
                        principalTable: "LoanDetails",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Waivers",
                columns: table => new
                {
                    WaiverId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanId = table.Column<int>(type: "int", nullable: false),
                    InstallmentId = table.Column<int>(type: "int", nullable: true),
                    WaiverType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Waivers", x => x.WaiverId);
                    table.ForeignKey(
                        name: "FK_Waivers_LoanDetails_LoanId",
                        column: x => x.LoanId,
                        principalTable: "LoanDetails",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseSourceDocuments1",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseSourceId = table.Column<int>(type: "int", nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_mandatory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseSourceDocuments1", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseSourceDocuments1_PurchaseSources_PurchaseSourceId",
                        column: x => x.PurchaseSourceId,
                        principalTable: "PurchaseSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsRepresentative = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dealers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DealerCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoanProposalNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DealershipName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DealershipPAN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GSTNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GSTRegStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MSMERegistrationNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MSMEType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MSMEStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlternativeContactNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlternativeEmailId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopPinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParkingYardAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParkingYardPinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParkingYardState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParkingYardCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pincode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficeStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgreementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AgreementExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ParkingStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParkingAgreementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ParkingAgreementExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateOfIncorporation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateOfFacilityAgreement = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CIBILOfEntity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalSanctionLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ROI = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ROIPerLakh = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DelayROI = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProcessingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProcessingCharge = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GSTOnProcessingCharge = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DocumentationCharge = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GSTOnDocCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RelationshipManagerId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    RegisteredDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dealers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dealers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BorrowerDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DealerId = table.Column<int>(type: "int", nullable: false),
                    PersonType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PAN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AadharNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FatherName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelationshipWithEntity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BorrowerCPinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BorrowerCState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermanentAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BorrowerPPincode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BorrowerPState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressAgreementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AddressAgreementExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CIBILScore = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BorrowerType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowerDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BorrowerDetails_Dealers_DealerId",
                        column: x => x.DealerId,
                        principalTable: "Dealers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChequeDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DealerId = table.Column<int>(type: "int", nullable: false),
                    BeneficiaryStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BeneficiaryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChequeNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IFSCCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateHandover = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsENach = table.Column<bool>(type: "bit", nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MandateType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxDebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MandateAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MandateStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MandateEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChequeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChequeDetails_Dealers_DealerId",
                        column: x => x.DealerId,
                        principalTable: "Dealers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DDRs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DDRNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateOfWithdraw = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    GSTOnProcessingFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ProcessingFeeReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SettledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UtrNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProcessingFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DealerId = table.Column<int>(type: "int", nullable: false),
                    PurchaseSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InterestWaiver = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DelayedROI = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsSigned = table.Column<bool>(type: "bit", nullable: false),
                    SuggestedProcessingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SelectedProcessingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HoldReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HoldBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HoldDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RejectedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RejectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DDRs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DDRs_Dealers_DealerId",
                        column: x => x.DealerId,
                        principalTable: "Dealers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GuarantorDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DealerId = table.Column<int>(type: "int", nullable: true),
                    PersonType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PAN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AadharNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FatherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelationshipWithBorrower = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuarantorCPinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuarantorCState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermanentAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuarantorPPincode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuarantorPState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressAgreementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AddressAgreementExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CIBILScore = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GuarantorType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuarantorDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuarantorDetails_Dealers_DealerId",
                        column: x => x.DealerId,
                        principalTable: "Dealers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LoanDocumentMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DealerCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DealerId = table.Column<int>(type: "int", nullable: false),
                    LoanNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VehicleNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelationshipManager = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MakeModel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcuredFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfDisbursement = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Tenure = table.Column<int>(type: "int", nullable: true),
                    DocumentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanDocumentMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanDocumentMasters_Dealers_DealerId",
                        column: x => x.DealerId,
                        principalTable: "Dealers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecurityDepositDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DealerId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UTRNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateReceived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateRefunded = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CIBILOfEntity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalSanctionLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ROI = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ROIPerLakh = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DelayROI = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AgreementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProcessingCharge = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GSTOnProcessingCharge = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DocumentationCharge = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GSTOnDocCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RegisteredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityDepositDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecurityDepositDetails_Dealers_DealerId",
                        column: x => x.DealerId,
                        principalTable: "Dealers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfWithdraw = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GSTOnProcessingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProcessingFeeReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SettledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SuggestedProcessingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SelectedProcessingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UtrNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DealerId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchaseSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InterestWaiver = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DelayedROI = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoanDetailId = table.Column<int>(type: "int", nullable: true),
                    DDRId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loans_DDRs_DDRId",
                        column: x => x.DDRId,
                        principalTable: "DDRs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Loans_Dealers_DealerId",
                        column: x => x.DealerId,
                        principalTable: "Dealers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Loans_LoanDetails_LoanDetailId",
                        column: x => x.LoanDetailId,
                        principalTable: "LoanDetails",
                        principalColumn: "LoanId");
                });

            migrationBuilder.CreateTable(
                name: "DocumentUpload",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DealerId = table.Column<int>(type: "int", nullable: true),
                    BorrowerDetailsId = table.Column<int>(type: "int", nullable: true),
                    GuarantorDetailsId = table.Column<int>(type: "int", nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentUpload", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentUpload_BorrowerDetails_BorrowerDetailsId",
                        column: x => x.BorrowerDetailsId,
                        principalTable: "BorrowerDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentUpload_Dealers_DealerId",
                        column: x => x.DealerId,
                        principalTable: "Dealers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentUpload_GuarantorDetails_GuarantorDetailsId",
                        column: x => x.GuarantorDetailsId,
                        principalTable: "GuarantorDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LoanDocumentActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanDocumentMasterId = table.Column<int>(type: "int", nullable: false),
                    ActivityType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromPerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToPerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryNoteNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanDocumentActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanDocumentActivities_LoanDocumentMasters_LoanDocumentMasterId",
                        column: x => x.LoanDocumentMasterId,
                        principalTable: "LoanDocumentMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuyerInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentificationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentificationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuyerSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DDRId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuyerInfos_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanAttachments_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanDocumentUploads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    UploadedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanDocumentUploads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanDocumentUploads_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanId = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentReceived = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestEarned = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DelayedInterestEarned = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrincipalReceived = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanPayments_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanId = table.Column<int>(type: "int", nullable: true),
                    Make = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: true),
                    ChassisNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EngineNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DDRId = table.Column<int>(type: "int", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleInfos_DDRs_DDRId",
                        column: x => x.DDRId,
                        principalTable: "DDRs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleInfos_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BorrowerDetails_DealerId",
                table: "BorrowerDetails",
                column: "DealerId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerInfos_LoanId",
                table: "BuyerInfos",
                column: "LoanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChequeDetails_DealerId",
                table: "ChequeDetails",
                column: "DealerId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_StateId",
                table: "Cities",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_DDRs_DealerId",
                table: "DDRs",
                column: "DealerId");

            migrationBuilder.CreateIndex(
                name: "IX_Dealers_UserId",
                table: "Dealers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentUpload_BorrowerDetailsId",
                table: "DocumentUpload",
                column: "BorrowerDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentUpload_DealerId",
                table: "DocumentUpload",
                column: "DealerId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentUpload_GuarantorDetailsId",
                table: "DocumentUpload",
                column: "GuarantorDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_GuarantorDetails_DealerId",
                table: "GuarantorDetails",
                column: "DealerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanAttachments_LoanId",
                table: "LoanAttachments",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanDocumentActivities_LoanDocumentMasterId",
                table: "LoanDocumentActivities",
                column: "LoanDocumentMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanDocumentMasters_DealerId",
                table: "LoanDocumentMasters",
                column: "DealerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanDocumentUploads_LoanId",
                table: "LoanDocumentUploads",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanFees_LoanId",
                table: "LoanFees",
                column: "LoanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanInstallments_LoanId",
                table: "LoanInstallments",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanInterests_LoanId",
                table: "LoanInterests",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanPayments_LoanId",
                table: "LoanPayments",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_DDRId",
                table: "Loans",
                column: "DDRId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_DealerId",
                table: "Loans",
                column: "DealerId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_LoanDetailId",
                table: "Loans",
                column: "LoanDetailId",
                unique: true,
                filter: "[LoanDetailId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseSourceDocuments1_PurchaseSourceId",
                table: "PurchaseSourceDocuments1",
                column: "PurchaseSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDepositDetails_DealerId",
                table: "SecurityDepositDetails",
                column: "DealerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInfos_DDRId",
                table: "VehicleInfos",
                column: "DDRId",
                unique: true,
                filter: "[DDRId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInfos_LoanId",
                table: "VehicleInfos",
                column: "LoanId",
                unique: true,
                filter: "[LoanId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Waivers_LoanId",
                table: "Waivers",
                column: "LoanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressStatuses");

            migrationBuilder.DropTable(
                name: "BankDetails");

            migrationBuilder.DropTable(
                name: "BusinessCategories");

            migrationBuilder.DropTable(
                name: "BusinessTypes");

            migrationBuilder.DropTable(
                name: "BuyerInfos");

            migrationBuilder.DropTable(
                name: "ChequeDetails");

            migrationBuilder.DropTable(
                name: "ChequeLocations");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "DDRDocuments");

            migrationBuilder.DropTable(
                name: "Designations");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "DocumentUpload");

            migrationBuilder.DropTable(
                name: "EntityTypes");

            migrationBuilder.DropTable(
                name: "LoanAccrual");

            migrationBuilder.DropTable(
                name: "LoanAttachments");

            migrationBuilder.DropTable(
                name: "LoanDocumentActivities");

            migrationBuilder.DropTable(
                name: "LoanDocumentUploads");

            migrationBuilder.DropTable(
                name: "LoanFees");

            migrationBuilder.DropTable(
                name: "LoanInstallments");

            migrationBuilder.DropTable(
                name: "LoanInterests");

            migrationBuilder.DropTable(
                name: "LoanPayments");

            migrationBuilder.DropTable(
                name: "Makes");

            migrationBuilder.DropTable(
                name: "OtpVerifications");

            migrationBuilder.DropTable(
                name: "PendingInstallments");

            migrationBuilder.DropTable(
                name: "PersonTypes");

            migrationBuilder.DropTable(
                name: "PurchaseSourceDocuments1");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "SecurityDepositDetails");

            migrationBuilder.DropTable(
                name: "VehicleInfos");

            migrationBuilder.DropTable(
                name: "Waivers");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "BorrowerDetails");

            migrationBuilder.DropTable(
                name: "GuarantorDetails");

            migrationBuilder.DropTable(
                name: "LoanDocumentMasters");

            migrationBuilder.DropTable(
                name: "PurchaseSources");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "DDRs");

            migrationBuilder.DropTable(
                name: "LoanDetails");

            migrationBuilder.DropTable(
                name: "Dealers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
