using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Application.Services;
using FuarYonetimSistemi.Domain.Entities;
using FuarYonetimSistemi.Infrastructure.Data;
using FuarYonetimSistemi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FuarYonetimSistemi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public AuthController(AppDbContext context, IPasswordHasher passwordHasher, ITokenService tokenService, IEmailService emailService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Bu e-posta zaten kayıtlı.");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Role = request.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _tokenService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Token = token,
            FullName = user.FullName,
            Role = user.Role
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
            return Unauthorized("Geçersiz e-posta veya şifre.");

        var token = _tokenService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Token = token,
            FullName = user.FullName,
            Role = user.Role
        });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
            return BadRequest("Bu e-posta adresi ile kayıtlı kullanıcı bulunamadı.");

        var resetToken = Guid.NewGuid().ToString(); // basit token üretimi
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);

        await _context.SaveChangesAsync();

        var resetLink = $"{Request.Scheme}://{Request.Host}/reset-password?token={resetToken}";
        await _emailService.SendEmailAsync(
            user.Email,
            "Şifre Sıfırlama",
            $"Şifrenizi sıfırlamak için aşağıdaki linke tıklayın:<br/><a href='{resetLink}'>{resetLink}</a>"
        );

        return Ok("Şifre sıfırlama e-postası gönderildi.");
    }


    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token && u.PasswordResetTokenExpires > DateTime.UtcNow);
        if (user == null)
            return BadRequest("Geçersiz veya süresi dolmuş token.");

        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpires = null;

        await _context.SaveChangesAsync();

        return Ok("Şifreniz başarıyla güncellendi.");
    }

   
}
