using Contratos.Login;
using Infraestructura.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _cfg;
    private readonly ApplicationDbContext _db;

    public AuthController(IConfiguration cfg, ApplicationDbContext db)
    {
        _cfg = cfg;
        _db = db;
    }

    public record LoginRequest(string Usuario, string Clave);

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Usuario) || string.IsNullOrWhiteSpace(req.Clave))
            return BadRequest(new LoginResponse(false, null, null, "Usuario y clave son obligatorios"));

        // Buscar usuario en BD
        var usuario = await _db.Usuarios
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.usuario == req.Usuario);

        if (usuario is null)
            return Unauthorized(new LoginResponse(false, null, null, "Credenciales inválidas"));

        // Verificar contraseña
        if (!VerificarPassword(req.Clave, usuario.clavehash, usuario.clavesalt, usuario.clavealgoritmo, usuario.claveiteraciones))
            return Unauthorized(new LoginResponse(false, null, null, "Credenciales inválidas"));

        // Generar JWT
        var token = CreateToken(usuario);
        return Ok(new LoginResponse(true, token, usuario.nombre ?? usuario.usuario, null));

    }

    private bool VerificarPassword(string passwordPlano, byte[] hashAlmacenado, byte[]? saltBytes,
        string? algoritmo, int? iteraciones)
    {
        if (saltBytes is null ||
            string.IsNullOrWhiteSpace(algoritmo) ||
            iteraciones is null or <= 0)
        {
            return false; // datos incompletos en BD
        }

        var salt = saltBytes;

        // Asumo que usaste PBKDF2 (Rfc2898DeriveBytes) para generar la clave
        using var pbkdf2 = new Rfc2898DeriveBytes(
            passwordPlano,
            salt,
            iteraciones.Value,
            HashAlgorithmName.SHA256);

        var hashCalculado = pbkdf2.GetBytes(hashAlmacenado.Length);

        // Comparación constante para evitar timing attacks
        var diff = 0;
        for (int i = 0; i < hashAlmacenado.Length; i++)
        {
            diff |= hashAlmacenado[i] ^ hashCalculado[i];
        }

        return diff == 0;
    }

    private string CreateToken(Dominio.Usuarios.Usuario usuario)
    {
        var issuer = _cfg.GetSection("Jwt").GetValue<string>("Issuer") ?? string.Empty;
        var audience = _cfg.GetSection("Jwt").GetValue<string>("Audience") ?? string.Empty;
        var secret = _cfg.GetSection("Jwt").GetValue<string>("Secret") ?? string.Empty;
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.idus.ToString()),
            new(ClaimTypes.Name, usuario.usuario),
            new("display_name", usuario.nombre ?? string.Empty),
            // new(ClaimTypes.Role, "Admin")
        };

        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}