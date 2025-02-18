using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Services;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/fournisseurs")] // URL de base : http://localhost:5000/api/fournisseurs
    public class FournisseurController : ControllerBase
    {
        private readonly DatabaseService _dbService;

        public FournisseurController(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        /// <summary>
        /// Endpoint GET qui retourne tous les fournisseurs
        /// Route : GET /api/fournisseurs
        /// </summary>
        /// <returns>Une liste de tous les fournisseurs au format JSON</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllFournisseurs()
        {
            var fournisseurs = await _dbService.GetFournisseursAsync();
            return Ok(fournisseurs);
        }
    }
}