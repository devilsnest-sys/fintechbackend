using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.Interfaces;

namespace TscLoanManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginRequestDto request)
        {
            try
            {
                var user = await _authService.LoginAsync(request);
                return Ok(user);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        //[HttpPost("login")]
        //public async Task<IActionResult> Login(LoginRequestDto request)
        //{
        //    try
        //    {
        //        var otpSent = await _authService.LoginAsync(request);
        //        if (otpSent)
        //            return Ok(new { message = "OTP sent to your registered email" });

        //        return BadRequest(new { message = "Login failed" });
        //    }
        //    catch (ApplicationException ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterRequestDto request)
        {
            try
            {
                var user = await _authService.RegisterAsync(request);
                return CreatedAtAction(nameof(Login), new { username = user.Username }, user);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("add-representative")]
        //[Authorize(Roles = "Admin")] // make sure role-based auth is in place
        public async Task<ActionResult<UserDto>> AddRepresentative(CreateRepresentativeDto request)
        {
            try
            {
                var user = await _authService.CreateRepresentativeAsync(request);
                return CreatedAtAction(nameof(Login), new { username = user.Username }, user);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("representatives")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllRepresentatives()
        {
            var reps = await _authService.GetAllRepresentativesAsync();
            return Ok(reps);
        }

        [HttpGet("representatives/{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> GetRepresentativeById(int id)
        {
            try
            {
                var rep = await _authService.GetRepresentativeByIdAsync(id);
                return Ok(rep);
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("representatives/{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> UpdateRepresentative(int id, UpdateRepresentativeDto request)
        {
            try
            {
                var updatedUser = await _authService.UpdateRepresentativeAsync(id, request);
                return Ok(updatedUser);
            }
            catch (ApplicationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("representatives/{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRepresentative(int id)
        {
            var deleted = await _authService.DeleteRepresentativeAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = "Representative not found or already deleted" });
            }

            return NoContent();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            try
            {
                var success = await _authService.ResetPasswordAsync(request);
                if (!success)
                    return NotFound(new { message = "User not found" });

                return Ok(new { message = "Password reset successful" });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequestDto request)
        {
            try
            {
                await _authService.SendLoginOtpAsync(request);
                return Ok(new { message = "OTP sent to registered email" });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<ActionResult<UserDto>> VerifyOtp([FromBody] VerifyOtpRequestDto request)
        {
            try
            {
                var user = await _authService.VerifyOtpAsync(request);
                return Ok(user);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
