using Microsoft.AspNetCore.Mvc;

using Application.Services;
using Application.DTO;
using Domain.Factory;
using RabbitMQ.Client;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColaboratorController : ControllerBase
    {   
        private readonly ColaboratorService _colaboratorService;
   

        List<string> _errorMessages = new List<string>();

        public ColaboratorController(ColaboratorService colaboratorService)
        {
            _colaboratorService = colaboratorService;
        }

        // GET: api/Colaborator
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ColaboratorDTO>>> GetColaborators()
        {
            IEnumerable<ColaboratorDTO> colabsDTO = await _colaboratorService.GetAllWithAddress();

            return Ok(colabsDTO);
        }


        // // GET: api/Colaborator/5
        // [HttpGet("{id}")]
        // public async Task<ActionResult<ColaboratorDTO>> GetColaboratorById(long id)
        // {
        //     var colaboratorDTO = await _colaboratorService.GetByIdWithAddress(id);

        //     if (colaboratorDTO == null)
        //     {
        //         return NotFound();
        //     }

        //     return Ok(colaboratorDTO);
        // }

        // // GET: api/Colaborator/a@bc
        // [HttpGet("email/{email}")]
        // public async Task<ActionResult<ColaboratorDTO>> GetColaboratorByEmail(string email)
        // {
        //     var colaboratorDTO= await _colaboratorService.GetByEmailWithAddress(email);

        //     if (colaboratorDTO == null)
        //     {
        //         return NotFound();
        //     }

        //     return Ok(colaboratorDTO);
        // }

    }
}
