using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinhaVidaAPI.Data;
using MinhaVidaAPI.Models;
using MinhaVidaAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhaVidaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransacoesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly WhatsAppService _waService;

        // CONSTRUTOR CORRIGIDO: Agora atribuímos o waService corretamente
        public TransacoesController(AppDbContext context, WhatsAppService waService)
        {
            _context = context;
            _waService = waService;
        }

        // GET: api/transacoes/{responsavel} (Busca transações de Eu ou Namorada)
        [HttpGet("{responsavel}")]
        public async Task<ActionResult<IEnumerable<Transacao>>> GetTransacoes(string responsavel)
        {
            return await _context.Transacoes
                .Where(t => t.Responsavel == responsavel)
                .ToListAsync();
        }

        // POST: api/transacoes (Lançamento individual manual)
        [HttpPost]
        public async Task<ActionResult<Transacao>> PostTransacao(Transacao transacao)
        {
            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();
            return Ok(transacao);
        }

        // GET: api/transacoes/resumo (Somas básicas)
        [HttpGet("resumo")]
        public async Task<IActionResult> GetResumo()
        {
            var transacoes = await _context.Transacoes.ToListAsync();
            var resumo = new
            {
                TotalMeu = transacoes.Where(t => t.Responsavel == "Eu").Sum(t => t.Valor),
                TotalDela = transacoes.Where(t => t.Responsavel == "Namorada").Sum(t => t.Valor)
            };
            return Ok(resumo);
        }

        // POST: api/transacoes/lote (IMPORTAÇÃO DO CSV)
        [HttpPost("lote")]
        public async Task<IActionResult> PostTransacoesLote([FromBody] List<Transacao> transacoes)
        {
            if (transacoes == null || !transacoes.Any())
                return BadRequest("A lista de transações está vazia.");

            try
            {
                // Garantir que o banco crie novos IDs para cada item do CSV
                foreach (var t in transacoes)
                {
                    t.Id = 0;
                }

                _context.Transacoes.AddRange(transacoes);
                await _context.SaveChangesAsync();

                // Lógica de Notificação WhatsApp
                var entradas = transacoes.Where(t => t.Valor > 0).Sum(t => t.Valor);
                var saidas = transacoes.Where(t => t.Valor < 0).Sum(t => t.Valor);

                await _waService.EnviarMensagemParaCasal(
                    $"📊 *EXTRATO IMPORTADO!*\n\n" +
                    $"Processamos *{transacoes.Count}* novas transações no Dashboard.\n" +
                    $"💰 Ganhos: {entradas:C}\n" +
                    $"💸 Gastos: {Math.Abs(saidas):C}"
                );

                return Ok(new { mensagem = $"{transacoes.Count} transações importadas com sucesso!" });
            }
            catch (Exception ex)
            {
                // Log detalhado no console da API para debug
                Console.WriteLine($"ERRO NO LOTE: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"DETALHE: {ex.InnerException.Message}");

                return StatusCode(500, $"Erro no banco de dados: {ex.Message}");
            }
        }
    }
}