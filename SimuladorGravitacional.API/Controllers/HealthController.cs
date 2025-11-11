using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramacaoAvancada.Data;  // ✅ ADICIONE ESTA LINHA

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _context;

    public HealthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            var canConnect = _context.Database.CanConnect();
            return Ok(new
            {
                Status = "✅ API Online",
                Database = canConnect ? "✅ Conectado" : "❌ Desconectado",
                Timestamp = DateTime.Now,
                Port = 5214
            });
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                Status = "✅ API Online",
                Database = "❌ Erro: " + ex.Message,
                Timestamp = DateTime.Now,
                Port = 5214
            });
        }
    }
}