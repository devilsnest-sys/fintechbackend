using Microsoft.EntityFrameworkCore;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Services;
using TscLoanManagement.TSCDB.Core.Domain;
using TscLoanManagement.TSCDB.Infrastructure;
using AutoMapper;
using System;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.LoanInstallment;
using TscLoanManagement.TSCDB.Infrastructure.Data.Context;
using Humanizer;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DocumentFormat.OpenXml.Office2010.Excel;
using TscLoanManagement.TSCDB.Application.Features.Loans;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;
using TscLoanManagement.TSCDB.Core.Domain.Loan;
using DocumentFormat.OpenXml.InkML;

namespace TscLoanManagement.TSCDB.Application.Services.Implementation
{
    public class LoanCalculationService : ILoanCalculationService
    {
        private readonly TSCDbContext _dbContext;
        private readonly IMapper _mapper;
        //private readonly ILoanService _loanService;
        private readonly ILoanRepository _repo;



        public LoanCalculationService(TSCDbContext context, IMapper mapper, ILoanRepository repo)
        {
            _dbContext = context;
            _mapper = mapper;
            //_loanService = loanService;
            _repo = repo;
        }

        public async Task<LoanDetail> CreateLoanWithInstallmentsAsync(LoanDetail loanDetail, List<(DateTime PaidDate, decimal AmountPaid)> installments)
        {
            // Calculate processing fee + GST
            var fee = loanDetail.PrincipalAmount * loanDetail.ProcessingFeeRate;
            var gst = fee * (loanDetail.GSTPercent / 100);
            var totalFee = fee + gst;

            var loanFee = new LoanFee
            {
                FeeAmount = fee,
                GSTAmount = gst,
                TotalFeeWithGST = totalFee
            };
            loanDetail.LoanFee = loanFee;

            var principalLeft = loanDetail.PrincipalAmount;
            var lastPaidDate = loanDetail.StartDate;
            var feeRemaining = totalFee;

            foreach (var (paidDate, amountPaid) in installments.OrderBy(i => i.PaidDate))
            {
                var installment = new LoanInstallment
                {
                    PaidDate = paidDate,
                    AmountPaid = amountPaid,
                    Loan = loanDetail
                };

                var daysSinceLast = (paidDate - lastPaidDate).Days;
                var dueDate = loanDetail.StartDate.AddDays(loanDetail.DueDays);
                decimal regularInterest = 0;
                decimal delayInterest = 0;

                // Regular interest
                regularInterest = principalLeft * loanDetail.InterestRate * daysSinceLast / (365 * 100);
                loanDetail.Interests.Add(new LoanInterest
                {
                    FromDate = lastPaidDate,
                    ToDate = paidDate <= dueDate ? paidDate : dueDate,
                    InterestType = "Regular",
                    InterestAmount = Math.Round(regularInterest, 2)
                });

                installment.AdjustedToInterest = Math.Round(regularInterest, 2);

                var remainingAmount = amountPaid;

                // Deduct Fee + GST if still pending
                if (feeRemaining > 0)
                {
                    var feeAdjusted = Math.Min(feeRemaining, remainingAmount);
                    installment.AdjustedToFee = feeAdjusted;
                    feeRemaining -= feeAdjusted;
                    remainingAmount -= feeAdjusted;
                }

                // Deduct Interest
                var interestAdjusted = Math.Min(regularInterest, remainingAmount);
                installment.AdjustedToInterest = interestAdjusted;
                remainingAmount -= interestAdjusted;

                // Delay Interest if overdue
                if (paidDate > dueDate)
                {
                    var overdueDays = (paidDate - dueDate).Days;
                    delayInterest = principalLeft * loanDetail.DelayInterestRate * overdueDays / (365 * 100);
                    loanDetail.Interests.Add(new LoanInterest
                    {
                        FromDate = dueDate.AddDays(1),
                        ToDate = paidDate,
                        InterestType = "Delay",
                        InterestAmount = Math.Round(delayInterest, 2)
                    });

                    var delayAdjusted = Math.Min(delayInterest, remainingAmount);
                    installment.AdjustedToDelayInterest = delayAdjusted;
                    remainingAmount -= delayAdjusted;
                }

                // Principal
                installment.AdjustedToPrincipal = remainingAmount;
                principalLeft -= installment.AdjustedToPrincipal;

                loanDetail.Installments.Add(installment);
                lastPaidDate = paidDate;
                Console.WriteLine(principalLeft);
            }

            _dbContext.LoanDetails.Add(loanDetail);
            await _dbContext.SaveChangesAsync();

            return loanDetail;
        }

        public async Task<LoanSummaryDto> GetLoanSummaryAsync(int loanId)
        {
            var loan = await _dbContext.LoanDetails
                .Include(x => x.Installments)
                .Include(x => x.Interests)
                .Include(x => x.LoanFee)
                .FirstOrDefaultAsync(x => x.LoanId == loanId);

            if (loan == null) return null;

            // Load waivers related to this loan
            var waivers = await _dbContext.Waivers
                .Where(w => w.LoanId == loanId)
                .ToListAsync();

            var waivedPrincipal = waivers
                .Where(w => w.WaiverType.Equals("Principal", StringComparison.OrdinalIgnoreCase))
                .Sum(w => w.Amount);

            var waivedInterest = waivers
                .Where(w => w.WaiverType.Equals("Interest", StringComparison.OrdinalIgnoreCase))
                .Sum(w => w.Amount);

            var waivedDelayInterest = waivers
                .Where(w => w.WaiverType.Equals("DelayInterest", StringComparison.OrdinalIgnoreCase))
                .Sum(w => w.Amount);

            var waivedProcessingFee = waivers
                .Where(w => w.WaiverType.Equals("ProcessingFee", StringComparison.OrdinalIgnoreCase))
                .Sum(w => w.Amount);

            // Calculate totals - waivers are already deducted from the loan balances by the waiver method
            var principalPaid = loan.Installments.Sum(i => i.AdjustedToPrincipal) - waivedPrincipal;
            var totalPaid = loan.Installments.Sum(i => i.AmountPaid);
            var interestPaid = loan.Installments.Sum(i => i.AdjustedToInterest) - waivedInterest;
            var delayInterestPaid = loan.Installments.Sum(i => i.AdjustedToDelayInterest) - waivedDelayInterest;
            var processingFeePaid = loan.Installments.Sum(i => i.AdjustedToFee) - waivedProcessingFee;

            var gstAmount = loan.LoanFee?.GSTAmount ?? 0;
            var totalFeeWithGst = loan.LoanFee?.TotalFeeWithGST ?? 0;

            // Calculate LastPaidDate
            var lastPaidDate = loan.Installments.Any()
                ? loan.Installments.Max(i => i.PaidDate)
                : loan.StartDate;

            var actualInterestPaid = loan.Installments.Sum(i => i.AdjustedToInterest);
            var actualDelayInterestPaid = loan.Installments.Sum(i => i.AdjustedToDelayInterest);

            var totalPrincipalPaid = loan.Installments?.Sum(i => i.AdjustedToPrincipal) ?? 0;
            var totalPrincipalWaived = await _dbContext.Waivers
                .Where(w => w.LoanId == loanId && w.WaiverType.ToLower() == "principal")
                .SumAsync(w => w.Amount);

            //var outstandingPrincipal = loan.PrincipalAmount - totalPrincipalPaid - totalPrincipalWaived;


            // Outstanding amounts are based on current loan balances (already adjusted by waivers)
            var outstandingPrincipal = loan.PrincipalAmount - totalPrincipalPaid - totalPrincipalWaived - waivedInterest - waivedDelayInterest - waivedProcessingFee;
            var processingFeeOutstanding = totalFeeWithGst - processingFeePaid;

            // Initialize accrued interest values
            decimal accruedRegularInterest = 0;
            decimal accruedDelayInterest = 0;
            DateTime accruedTillDate = DateTime.Today;

            // Only calculate accrued interest if loan is not closed
            bool isLoanClosed = string.Equals(loan.Status, "Closed", StringComparison.OrdinalIgnoreCase);

            if (!isLoanClosed)
            {
                accruedTillDate = DateTime.Today;

                // Calculate Accrued Regular Interest
                accruedRegularInterest = CalculateAccruedRegularInterest(
                    loan, outstandingPrincipal, lastPaidDate, accruedTillDate);

                // Calculate Accrued Delay Interest
                accruedDelayInterest = CalculateAccruedDelayInterest(
                    loan, outstandingPrincipal, lastPaidDate, accruedTillDate);
            }
            else
            {
                accruedTillDate = lastPaidDate;
            }


            await TryMarkLoanAsClosedAsync(loan, outstandingPrincipal, accruedRegularInterest, accruedDelayInterest, processingFeeOutstanding);

            return new LoanSummaryDto
            {
                LoanId = loan.LoanId,
                DisbursedAmount = loan.PrincipalAmount,
                PrincipalReceived = principalPaid,
                OutstandingPrincipal = outstandingPrincipal,
                TotalInstallmentReceived = totalPaid,

                RegularInterestReceived = interestPaid,
                DelayInterestReceived = delayInterestPaid,

                ActualRegularInterestReceived = actualInterestPaid,
                ActualDelayInterestReceived = actualDelayInterestPaid,

                ProcessingFee = loan.LoanFee?.FeeAmount ?? 0,
                PaidProcessingFee = processingFeePaid,
                GST = gstAmount,
                TotalProcessingFeeWithGST = totalFeeWithGst,
                LastPaidDate = lastPaidDate,
                AccruedTillDate = accruedTillDate,
                AccruedRegularInterest = accruedRegularInterest,
                AccruedDelayInterest = accruedDelayInterest,

                WaivedPrincipal = waivedPrincipal,
                WaivedInterest = waivedInterest,
                WaivedDelayInterest = waivedDelayInterest,
                WaivedProcessingFee = waivedProcessingFee,

                Installments = loan.Installments.Select(i => new InstallmentDto
                {
                    PaidDate = i.PaidDate,
                    AmountPaid = i.AmountPaid,
                    AdjustedToPrincipal = i.AdjustedToPrincipal,
                    AdjustedToInterest = i.AdjustedToInterest,
                    AdjustedToDelayInterest = i.AdjustedToDelayInterest,
                    AdjustedToFee = i.AdjustedToFee,
                    DueFee = CalculateDueFeeAtDate(loan, i.PaidDate),
                    DueInterest = CalculateDueInterestAtDate(loan, outstandingPrincipal, i.PaidDate),
                    DueDelayInterest = CalculateDueDelayInterestAtDate(loan, outstandingPrincipal, i.PaidDate)
                }).ToList()
            };
        }



        public async Task<LoanDetail> CreateLoanAsync(CreateLoanRequestDto dto)
        {
            //var fee = dto.PrincipalAmount * dto.ProcessingFeeRate / 100;
            var fee = dto.ProcessingFeeRate;
            var gst = fee * (dto.GSTPercent / 100);
            var totalFee = fee + gst;

            var loan = new LoanDetail
            {
                CustomerId = dto.CustomerId,
                PrincipalAmount = dto.PrincipalAmount,
                InterestRate = dto.InterestRate,
                ProcessingFeeRate = dto.ProcessingFeeRate,
                GSTPercent = dto.GSTPercent,
                StartDate = dto.StartDate,
                DueDays = dto.DueDays,
                DelayInterestRate = dto.DelayInterestRate,
                Status = dto.Status ?? "Active",
                LoanFee = new LoanFee
                {
                    FeeAmount = fee,
                    GSTAmount = gst,
                    TotalFeeWithGST = totalFee
                },
                Interests = new List<LoanInterest>(),
                Installments = new List<LoanInstallment>()
            };

            _dbContext.LoanDetails.Add(loan);
            await _dbContext.SaveChangesAsync();
            return loan;
        }

        public async Task<LoanInstallment> AddBulkInstallmentAsync(BulkInstallmentDto dto)
        {
            var loan = await _dbContext.LoanDetails
                .Include(l => l.Installments)
                .Include(l => l.Interests)
                .Include(l => l.LoanFee)
                .FirstOrDefaultAsync(l => l.LoanId == dto.LoanId);

            if (loan == null) return null;

            var installment = new LoanInstallment
            {
                LoanId = dto.LoanId,
                PaidDate = dto.PaidDate,
                AmountPaid = dto.AmountPaid,
                AdjustedToPrincipal = dto.AdjustedToPrincipal,
                AdjustedToInterest = dto.AdjustedToInterest,
                AdjustedToDelayInterest = dto.AdjustedToDelayInterest,
                AdjustedToFee = dto.AdjustedToFee,
                DueInterest = dto.DueInterest,
                DueFee = dto.DueFee,
            };

            loan.Installments.Add(installment);
            await _dbContext.SaveChangesAsync();

            return installment;
        }

        public async Task<bool> AddBulkLoanFeeAsync(BulkLoanFeeDto dto)
        {
            var loan = await _dbContext.LoanDetails
                .Include(l => l.LoanFee)
                .FirstOrDefaultAsync(l => l.LoanId == dto.LoanId);

            if (loan == null) return false;

            if (loan.LoanFee != null)
            {
                loan.LoanFee.FeeAmount = dto.FeeAmount;
                loan.LoanFee.GSTAmount = dto.GSTAmount;
                loan.LoanFee.TotalFeeWithGST = dto.TotalFeeWithGST;
            }
            else
            {
                var loanFee = new LoanFee
                {
                    LoanId = dto.LoanId,
                    FeeAmount = dto.FeeAmount,
                    GSTAmount = dto.GSTAmount,
                    TotalFeeWithGST = dto.TotalFeeWithGST,
                };

                loan.LoanFee = loanFee;
                _dbContext.LoanFees.Add(loanFee);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }



        public async Task<LoanInstallment> AddInstallmentAsync(AddInstallmentRequestDto dto)
        {
            var loan = await _dbContext.LoanDetails
                .Include(l => l.Installments)
                .Include(l => l.Interests)
                .Include(l => l.LoanFee)
                .FirstOrDefaultAsync(l => l.LoanId == dto.LoanId);

            if (loan == null) return null;

            var loanStart = loan.StartDate.Date;
            var dueDate = loanStart.AddDays(loan.DueDays - 1);
            var paidDate = dto.PaidDate.Date;
            var amountLeft = dto.AmountPaid;

            var totalPrincipalPaid = loan.Installments.Sum(i => i.AdjustedToPrincipal);
            var totalFeePaid = loan.Installments.Sum(i => i.AdjustedToFee);
            var totalDueInterest = loan.Installments.Sum(i => i.DueInterest); // previous due interest

            var principalRemaining = loan.PrincipalAmount - totalPrincipalPaid;
            var feeRemaining = loan.LoanFee.TotalFeeWithGST - totalFeePaid;

            var lastPaidDate = loan.Installments
                .OrderByDescending(i => i.PaidDate)
                .FirstOrDefault()?.PaidDate.Date ?? loanStart;

            var installment = new LoanInstallment
            {
                LoanId = loan.LoanId,
                PaidDate = paidDate,
                AmountPaid = dto.AmountPaid
            };

            // Step 1: Adjust Processing Fee
            if (feeRemaining > 0)
            {
                installment.AdjustedToFee = Math.Min(feeRemaining, amountLeft);
                installment.DueFee = Math.Max(0, feeRemaining - installment.AdjustedToFee);
                amountLeft -= installment.AdjustedToFee;
            }

            int interestDays = (paidDate - lastPaidDate).Days;
            if (lastPaidDate == loanStart) interestDays++;

            decimal totalRegularInterest = 0;
            decimal totalDelayInterest = 0;
            int delayDays = 0;

            if (paidDate <= dueDate)
            {
                // Case A: On-time or early

                // Step 2: Adjust prior Due Interest
                var unpaidInterest = totalDueInterest;
                var adjustedDueInterest = Math.Min(unpaidInterest, amountLeft);
                installment.AdjustedToInterest = adjustedDueInterest;
                installment.DueInterest = -adjustedDueInterest;
                amountLeft -= adjustedDueInterest;

                // Step 3: Calculate and adjust current Regular Interest
                totalRegularInterest = Math.Round((principalRemaining * loan.InterestRate * interestDays) / (365 * 100), 2);

                var adjustedCurrentInterest = Math.Min(totalRegularInterest, amountLeft);
                installment.AdjustedToInterest += adjustedCurrentInterest;
                amountLeft -= adjustedCurrentInterest;

                // Remaining regular interest becomes new Due Interest
                installment.DueInterest += (totalRegularInterest - adjustedCurrentInterest);

                // Step 4: Remaining goes to Principal
                if (amountLeft > 0)
                {
                    installment.AdjustedToPrincipal = amountLeft;
                    amountLeft = 0;
                }
            }
            else
            {
                // Case B: Late Payment

                // Step 2: Calculate Regular and Delay Interest
                int regularInterestDays = 0;
                if (lastPaidDate > dueDate)
                {
                    regularInterestDays = (dueDate - dueDate).Days;
                }
                else
                { regularInterestDays = (dueDate - lastPaidDate).Days; }
                if (lastPaidDate == loanStart) regularInterestDays++;

                if (regularInterestDays > 0)
                {
                    totalRegularInterest = Math.Round(
                        (principalRemaining * loan.InterestRate * regularInterestDays) / (365 * 100),
                        2
                    );
                }

                delayDays = interestDays - regularInterestDays;
                if (delayDays > 0)
                {
                    // Split the delayed interest calculation
                    // Regular interest portion (28% in your example)
                    decimal delayRegularInterest = Math.Round(
                        (principalRemaining * loan.InterestRate * delayDays) / (365 * 100),
                        2
                    );

                    // Pure delay interest portion (7% in your example = 35% - 28%)
                    decimal pureDelayInterestRate = loan.DelayInterestRate - loan.InterestRate;
                    decimal pureDelayInterest = Math.Round(
                        (principalRemaining * pureDelayInterestRate * delayDays) / (365 * 100),
                        2
                    );

                    // Add the regular interest portion from delay period to total regular interest
                    totalRegularInterest += delayRegularInterest;

                    // Only the pure delay interest portion goes to delay interest
                    totalDelayInterest = pureDelayInterest;
                }

                // Step 3: Adjust previous Due Interest
                var unpaidInterest = totalDueInterest;
                var adjustedDueInterest = Math.Min(unpaidInterest, amountLeft);
                installment.AdjustedToInterest = adjustedDueInterest;
                installment.DueInterest = unpaidInterest - adjustedDueInterest;
                amountLeft -= adjustedDueInterest;

                // Step 4: Adjust Delay Interest (only the pure delay portion)
                var adjustedDelayInterest = Math.Min(totalDelayInterest, amountLeft);
                installment.AdjustedToDelayInterest = adjustedDelayInterest;
                amountLeft -= adjustedDelayInterest;

                // Step 5: Adjust Current Regular Interest (now includes regular portion from delay period)
                var adjustedCurrentInterest = Math.Min(totalRegularInterest, amountLeft);
                installment.AdjustedToInterest += adjustedCurrentInterest;
                amountLeft -= adjustedCurrentInterest;

                // Any leftover Regular Interest becomes Due
                installment.DueInterest += (totalRegularInterest - adjustedCurrentInterest);

                // Step 6: Remaining goes to Principal
                if (amountLeft > 0)
                {
                    installment.AdjustedToPrincipal = amountLeft;
                    amountLeft = 0;
                }
            }

            loan.Installments.Add(installment);
            await _dbContext.SaveChangesAsync();

            var updatedPrincipalPaid = loan.Installments.Sum(i => i.AdjustedToPrincipal);
            var updatedFeePaid = loan.Installments.Sum(i => i.AdjustedToFee);
            var updatedInterestPaid = loan.Installments.Sum(i => i.AdjustedToInterest);
            var updatedDelayInterestPaid = loan.Installments.Sum(i => i.AdjustedToDelayInterest);

            var updatedPrincipalRemaining = loan.PrincipalAmount - updatedPrincipalPaid;
            var updatedFeeRemaining = loan.LoanFee.TotalFeeWithGST - updatedFeePaid;

            var unpaidRegularInterest = totalRegularInterest - (installment.AdjustedToInterest);
            var unpaidDelayInterest = totalDelayInterest - (installment.AdjustedToDelayInterest);

            await TryMarkLoanAsClosedAsync(
                loan,
                updatedPrincipalRemaining,
                unpaidRegularInterest,
                unpaidDelayInterest,
                updatedFeeRemaining
            );
            //await TryMarkLoanAsClosedAsync(loan, principalRemaining, totalRegularInterest - installment.AdjustedToInterest, totalDelayInterest - installment.AdjustedToDelayInterest, feeRemaining);

            return installment;
        }

        public async Task<WaiverDto> WaiveLoanComponentAsync(int loanId, WaiverRequestDto request)
        {
            var loan = await _dbContext.LoanDetails
                .Include(l => l.Installments)
                .Include(l => l.Interests)
                .Include(l => l.LoanFee)
                .FirstOrDefaultAsync(l => l.LoanId == loanId);

            if (loan == null)
                throw new Exception("Loan not found");

            var allowedTypes = new[] { "Interest", "DelayInterest", "Principal", "ProcessingFee" };
            if (!allowedTypes.Contains(request.WaiverType, StringComparer.OrdinalIgnoreCase))
                throw new Exception("Invalid waiver type");

            var waiver = new Waiver
            {
                LoanId = loanId,
                WaiverType = request.WaiverType,
                Amount = request.Amount,
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Waivers.Add(waiver);

            // Apply waiver to respective balances
            switch (request.WaiverType.ToLower())
            {
                case "principal":
                    //loan.PrincipalAmount -= request.Amount;
                    break;
                case "interest":
                    // Apply to oldest regular interest record
                    var regularInterest = loan.Interests
                        .Where(i => i.InterestType == "Regular")
                        .OrderBy(i => i.FromDate)
                        .FirstOrDefault(i => i.InterestAmount >= request.Amount);
                    if (regularInterest != null)
                        regularInterest.InterestAmount -= request.Amount;
                    break;
                case "delayinterest":
                    // Apply to oldest delay interest record
                    var delayInterest = loan.Interests
                        .Where(i => i.InterestType == "Delay")
                        .OrderBy(i => i.FromDate)
                        .FirstOrDefault(i => i.InterestAmount >= request.Amount);
                    if (delayInterest != null)
                        delayInterest.InterestAmount -= request.Amount;
                    break;
                case "processingfee":
                    if (loan.LoanFee != null)
                    {
                        loan.LoanFee.FeeAmount -= request.Amount;
                        loan.LoanFee.TotalFeeWithGST = loan.LoanFee.FeeAmount + loan.LoanFee.GSTAmount;
                    }
                    break;
            }

            await _dbContext.SaveChangesAsync();

            return new WaiverDto
            {
                WaiverId = waiver.WaiverId,
                LoanId = waiver.LoanId,
                WaiverType = waiver.WaiverType,
                Amount = waiver.Amount,
                Reason = waiver.Reason,
                CreatedAt = waiver.CreatedAt
            };
        }


        private async Task TryMarkLoanAsClosedAsync(LoanDetail loan,
            decimal outstandingPrincipal,
            decimal accruedRegularInterest,
            decimal accruedDelayInterest,
            decimal processingFeeOutstanding)
        {
            bool shouldCloseLoan =
                outstandingPrincipal <= 0 &&
                accruedRegularInterest <= 0 &&
                accruedDelayInterest <= 0 &&
                processingFeeOutstanding <= 0;

            if (shouldCloseLoan && !string.Equals(loan.Status, "Closed", StringComparison.OrdinalIgnoreCase))
            {
                loan.Status = "Closed";
                //loan.SettledDate = DateTime.UtcNow;

                // Update parent Loan entity if exists
                var parentLoan = await _dbContext.Loans.FirstOrDefaultAsync(l => l.LoanDetailId == loan.LoanId);
                if (parentLoan != null)
                {
                    parentLoan.IsActive = false;
                    parentLoan.SettledDate = DateTime.UtcNow;
                }

                await _dbContext.SaveChangesAsync();
            }

        }


        private decimal CalculateAccruedRegularInterest(LoanDetail loan, decimal outstandingPrincipal,
            DateTime fromDate, DateTime toDate)
        {
            if (outstandingPrincipal <= 0 || fromDate >= toDate)
                return 0;

            // Calculate days between last payment and accrued till date
            var daysDiff = (toDate - fromDate).Days + 1;

            // Calculate daily interest rate (annual rate / 365)
            var dailyRate = loan.InterestRate / 100 / 365;

            // Calculate accrued interest: Principal × Daily Rate × Days
            var accruedInterest = outstandingPrincipal * (decimal)dailyRate * daysDiff;

            return Math.Round(accruedInterest, 2);
        }

        private decimal CalculateAccruedDelayInterest(LoanDetail loan, decimal outstandingPrincipal,
    DateTime lastPaidDate, DateTime accruedTillDate)
        {
            if (outstandingPrincipal <= 0)
                return 0;

            var dueDate = loan.StartDate.AddDays(loan.DueDays);

            if (accruedTillDate <= dueDate)
                return 0;

            var delayDays = (accruedTillDate - dueDate).Days + 1;

            if (lastPaidDate > dueDate)
            {
                delayDays = Math.Max(0, (lastPaidDate - dueDate).Days);
            }

            if (delayDays <= 0)
                return 0;

            // FIXED: Use only the penalty portion (not full 48%)
            var penaltyRate = loan.DelayInterestRate - loan.InterestRate;
            var dailyPenaltyRate = penaltyRate / 100 / 365;

            var delayInterest = outstandingPrincipal * (decimal)dailyPenaltyRate * delayDays;

            return Math.Round(delayInterest, 2);
        }


        private decimal CalculateDueFeeAtDate(LoanDetail loan, DateTime paymentDate)
        {
            // Processing fee is typically due at loan disbursement
            // You can adjust this logic based on your business rules
            if (paymentDate >= loan.StartDate)
            {
                return loan.LoanFee?.TotalFeeWithGST ?? 0;
            }
            return 0;
        }

        private decimal CalculateDueInterestAtDate(LoanDetail loan, decimal principalAmount, DateTime paymentDate)
        {
            // Calculate interest due from loan start date to payment date
            var daysDiff = (paymentDate - loan.StartDate).Days;
            if (daysDiff <= 0) return 0;

            var dailyRate = loan.InterestRate / 100 / 365;
            var dueInterest = principalAmount * (decimal)dailyRate * daysDiff;

            return Math.Round(dueInterest, 2);
        }

        private decimal CalculateDueDelayInterestAtDate(LoanDetail loan, decimal principalAmount, DateTime paymentDate)
        {
            var dueDate = loan.StartDate.AddDays(loan.DueDays);

            // No delay interest if payment is before or on due date
            if (paymentDate <= dueDate)
                return 0;

            var delayDays = (paymentDate - dueDate).Days;
            var dailyDelayRate = loan.DelayInterestRate / 100 / 365;
            var delayInterest = principalAmount * (decimal)dailyDelayRate * delayDays;

            return Math.Round(delayInterest, 2);
        }

        public async Task<PendingInstallment> AddPendingInstallmentAsync(PendingInstallmentDto dto)
        {
            var pending = new PendingInstallment
            {
                LoanId = dto.LoanId,
                PaidDate = dto.PaidDate,
                AmountPaid = dto.AmountPaid,
                Remarks = dto.Remarks
            };

            _dbContext.PendingInstallments.Add(pending);
            await _dbContext.SaveChangesAsync();
            return pending;
        }

        public async Task<LoanInstallment?> ApprovePendingInstallmentAsync(int pendingInstallmentId)
        {
            var pending = await _dbContext.PendingInstallments
                .FirstOrDefaultAsync(p => p.Id == pendingInstallmentId && p.Status == "Pending");

            if (pending == null)
                return null;

            // Create AddInstallmentRequestDto
            var dto = new AddInstallmentRequestDto
            {
                LoanId = pending.LoanId ?? 0,
                PaidDate = pending.PaidDate ?? DateTime.UtcNow,
                AmountPaid = pending.AmountPaid ?? 0,
                //Remarks = pending.Remarks
            };

            // Process actual installment logic
            var installment = await AddInstallmentAsync(dto); // Use your existing logic

            if (installment != null)
            {
                pending.Status = "Approved";
                await _dbContext.SaveChangesAsync();
            }

            return installment;
        }

        public async Task<List<PendingInstallmentWithLoanFlatDto>> GetAllPendingInstallmentsAsync(int? loanId = null)
        {
            var query = _dbContext.PendingInstallments.AsQueryable();
            
            if (loanId.HasValue)
                query = query.Where(p => p.LoanId == loanId.Value);

            //List<PendingInstallment> p = await query
            //    .Where(p => p.Status == "Pending")
            //    .OrderByDescending(p => p.CreatedAt)
            //    .ToListAsync();

            //var loan = await _loanService.GetLoanByIdAsync(id);

            //return await query
            //    .Where(p => p.Status == "Pending")
            //    .OrderByDescending(p => p.CreatedAt)
            //    .ToListAsync();

            var installments = await query
        .Where(p => p.Status == "Pending")
        .OrderByDescending(p => p.CreatedAt)
        .ToListAsync();

            var result = new List<PendingInstallmentWithLoanFlatDto>();

            foreach (var inst in installments)
            {
                Loan loanDto = null;

                var loanDetail = await _dbContext.LoanDetails
                    .AsNoTracking()
                    .FirstOrDefaultAsync(ld => ld.LoanId == inst.LoanId.Value);

                if (inst.LoanId.HasValue)
                {
                    loanDto = await _dbContext.Loans
                    .Include(l => l.Dealer)
                    .Include(l => l.VehicleInfo)
                    .Include(l => l.DDR.VehicleInfo)
                    .Include(l => l.BuyerInfo)
                    .Include(l => l.DDR)
                    .Include(l => l.Attachments)
                    .FirstOrDefaultAsync(l => l.LoanDetailId == inst.LoanId.Value);

                }

                result.Add(new PendingInstallmentWithLoanFlatDto
                {
                    Id = inst.Id,
                    LoanId = inst.LoanId,
                    PaidDate = inst.PaidDate,
                    AmountPaid = inst.AmountPaid,
                    Remarks = inst.Remarks,
                    Status = inst.Status,
                    CreatedAt = inst.CreatedAt,
                    Loan = loanDto
                });
            }
            return result;
        }


    }
}