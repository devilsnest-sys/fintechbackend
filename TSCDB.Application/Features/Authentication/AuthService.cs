using AutoMapper;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.Authentication;
using TscLoanManagement.TSCDB.Core.Domain.Dealer;
using TscLoanManagement.TSCDB.Core.Interfaces.Repositories;
using TscLoanManagement.TSCDB.Infrastructure.Data.Context;
using TscLoanManagement.TSCDB.Infrastructure.Repositories;

namespace TscLoanManagement.TSCDB.Application.Features.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly TSCDbContext _context;
        private readonly IDealerRepository _dealerRepository;
        private readonly IDealerService _dealerService;
        private readonly IEmailService _emailService;

        private static readonly ConcurrentDictionary<string, (string Otp, DateTime Expiry)> _otpStore = new ConcurrentDictionary<string, (string, DateTime)>();
        public AuthService(IUserRepository userRepository, IMapper mapper, IJwtService jwtService, TSCDbContext context, IDealerRepository dealerRepository, IDealerService dealerService, IEmailService emailService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtService = jwtService;
            _context = context;
            _dealerRepository = dealerRepository;
            _dealerService = dealerService;
            _emailService = emailService;
        }

        public async Task<UserDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetUserByUsernameAsync(request.Username);

            if (user == null || user.PasswordHash != request.Password)
                throw new ApplicationException("Invalid username or password");

            // Build full user DTO with role & permissions
            var userDto = await BuildUserDtoAsync(user);

            // Add JWT token
            userDto.Token = _jwtService.GenerateToken(user);

            return userDto;
        }

        //public async Task<bool> LoginAsync(LoginRequestDto request)
        //{
        //    // ✅ Authenticate using email + password
        //    var user = await _userRepository.GetUserByEmailAsync(request.Username);
        //    if (user == null || user.PasswordHash != request.Password)
        //        throw new ApplicationException("Invalid email or password");

        //    // ✅ Generate OTP & send to email
        //    var otp = new Random().Next(100000, 999999).ToString();
        //    var expiry = DateTime.UtcNow.AddMinutes(5);

        //    _otpStore[user.Email] = (otp, expiry);

        //    await _emailService.SendOtpEmailAsync(user.Email, otp);

        //    // Instead of returning JWT, just confirm OTP sent
        //    return true;
        //}



        public async Task<UserDto> RegisterAsync(RegisterRequestDto request)
        {
            var existingUser = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (existingUser != null)
                throw new ApplicationException("Username already exists");

            var existingEmail = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingEmail != null)
                throw new ApplicationException("Email already exists");

            var newUser = new User
            {
                Name = request.Name,
                Username = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = request.Password,
                UserType = request.UserType,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            // 👇 Add Dealer if UserType is Dealer
            if (request.UserType?.ToLower() == "dealer")
            {
                var dealer = new Dealer
                {
                    //UserId = newUser.Id,
                    DealershipName = request.Name,
                    ContactNo = request.PhoneNumber,
                    EmailId = request.Email,
                    RegisteredDate = DateTime.UtcNow,
                    IsActive = true,
                    DealerCode = await _dealerService.GenerateDealerCodeAsync(),
                    LoanProposalNo = await _dealerService.GenerateLoanProposalNoAsync(),
                    Status = 0 ,// or initial status
                    UserId = newUser.Id
                };
                await _dealerRepository.AddAsync(dealer);
                //await _dealerRepository.SaveChangesAsync();
            }

            await _emailService.SendWelcomeEmailAsync(
                newUser.Email,
                newUser.Username,
                request.Password // ⚠️ plain password – only if you’re okay with emailing it
            );

            var userDto = _mapper.Map<UserDto>(newUser);
            userDto.Token = _jwtService.GenerateToken(newUser);

            return userDto;
        }


        public async Task<UserDto> CreateRepresentativeAsync(CreateRepresentativeDto request)
        {
            var existingUser = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                throw new ApplicationException("Username already exists");
            }

            var newUser = new User
            {
                Name = request.Name,
                Username = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = request.Password,
                UserType = "representative",
                IsActive = true,
                IsRepresentative = request.IsRepresentative,
                RoleId = request.RoleId,
                Designation = request.Designation
            };

            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(newUser);
            userDto.Token = _jwtService.GenerateToken(newUser);

            return userDto;
        }

        public async Task<IEnumerable<UserDto>> GetAllRepresentativesAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var reps = users.Where(u => u.UserType == "representative");

            var result = new List<UserDto>();
            foreach (var rep in reps)
            {
                result.Add(await BuildUserDtoAsync(rep));
            }

            return result;
        }


        public async Task<UserDto> GetRepresentativeByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.UserType?.ToLower() != "representative")
                throw new ApplicationException("Representative not found");

            return await BuildUserDtoAsync(user);
        }


        public async Task<UserDto> UpdateRepresentativeAsync(int id, UpdateRepresentativeDto request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.UserType != "representative")
                throw new ApplicationException("Representative not found");

            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.IsActive = request.IsActive;
            user.RoleId = request.RoleId;
            user.IsRepresentative = request.IsRepresentative;
            user.Designation = request.Designation;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> DeleteRepresentativeAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.UserType != "representative")
                return false;

            await _userRepository.DeleteAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ForgotPasswordRequestDto request)
        {
            var user = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (user == null)
                throw new ApplicationException("User not found");

            user.PasswordHash = request.NewPassword; // In production, hash the password properly

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        private async Task<UserDto> BuildUserDtoAsync(User user)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == user.RoleId);

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserType = user.UserType,
                IsActive = user.IsActive,
                IsRepresentative = user.IsRepresentative,
                Role = _mapper.Map<RoleDto>(role),
                Designation = user.Designation
            };
        }

        public async Task<bool> SendLoginOtpAsync(OtpRequestDto request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
                throw new ApplicationException("User not found");

            // Generate 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(5);

            _otpStore[user.Email] = (otp, expiry);

            await _emailService.SendOtpEmailAsync(user.Email, otp);

            return true;
        }

        public async Task<UserDto> VerifyOtpAsync(VerifyOtpRequestDto request)
        {
            if (!_otpStore.TryGetValue(request.Email, out var otpData))
                throw new ApplicationException("OTP not requested");

            if (otpData.Expiry < DateTime.UtcNow)
                throw new ApplicationException("OTP expired");

            if (otpData.Otp != request.Otp)
                throw new ApplicationException("Invalid OTP");

            // OTP valid -> login user
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null) throw new ApplicationException("User not found");

            var userDto = await BuildUserDtoAsync(user);
            userDto.Token = _jwtService.GenerateToken(user);

            // remove OTP after success
            _otpStore.TryRemove(request.Email, out _);

            return userDto;
        }


    }
}
