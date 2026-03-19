using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinhaVidaAPI.Data;
using MinhaVidaAPI.Models;

namespace MinhaVidaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/dashboard/resumo
        // Retorna tudo que o Home.razor precisa em UMA única chamada
        [HttpGet("resumo")]
        public async Task<IActionResult> GetResumo()
        {
            // Executa as 4 queries em paralelo no banco
            var taskTransacoesEu = _context.Transacoes.AsNoTracking().Where(t => t.Responsavel == "Eu").ToListAsync();
            var taskTransacoesDela = _context.Transacoes.AsNoTracking().Where(t => t.Responsavel == "Namorada").ToListAsync();
            var taskMetas = _context.Metas.AsNoTracking().ToListAsync();
            var taskDesejos = _context.Desejos.AsNoTracking().ToListAsync();

            await Task.WhenAll(taskTransacoesEu, taskTransacoesDela, taskMetas, taskDesejos);

            return Ok(new
            {
                TransacoesEu = taskTransacoesEu.Result,
                TransacoesDela = taskTransacoesDela.Result,
                Metas = taskMetas.Result,
                Desejos = taskDesejos.Result
            });
        }
    }
}