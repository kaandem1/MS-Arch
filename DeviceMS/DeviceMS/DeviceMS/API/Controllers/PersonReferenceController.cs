using AutoMapper;
using DeviceMS.Core.DomainLayer.Models;
using DeviceMS.API.DTOModels;
using DeviceMS.Logic.ServiceLayer.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DeviceMS.Logic.ServiceLayer.Helpers;
using DeviceMS.Logic.ServiceLayer.Services;

namespace DeviceMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PersonReferenceController : ControllerBase
    {
        private readonly IPersonReferenceService _personReferenceService;
        private readonly ILogger<PersonReferenceController> _logger;
        private readonly IMapper _mapper;

        public PersonReferenceController(IPersonReferenceService personReferenceService, ILogger<PersonReferenceController> logger, IMapper mapper)
        {
            _personReferenceService = personReferenceService;
            _logger = logger;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(PersonReferenceDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PersonReferenceDTO>> CreatePersonReferenceAsync([FromBody] PersonReferenceDTO personReferenceDTO)
        {

            List<PersonReference> allPersonReferences = await _personReferenceService.GetAllAsync();
            foreach (PersonReference pr in allPersonReferences)
            {
                if (personReferenceDTO.UserId == pr.UserId)
                {
                    _logger.LogError("A person reference with the same id already exists : {@UserId}", personReferenceDTO.UserId);
                    return BadRequest();
                }
            }

            var personReference = _mapper.Map<PersonReference>(personReferenceDTO);
            await _personReferenceService.CreateAsync(personReference);

            return Ok();
        }

        [Authorize]
        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeletePersonReferenceAsync([FromRoute] int userId)
        {
            var personReference = new PersonReference { UserId = userId };

            await _personReferenceService.DeleteAsync(personReference);

            return Ok();
        }


        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(List<PersonReferenceDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<PersonReferenceDTO>>> GetAllPersonReferencesAsync()
        {
            List<PersonReferenceDTO> personReferenceDTOList = new List<PersonReferenceDTO>();

            List<PersonReference> allPersonReferences = await _personReferenceService.GetAllAsync();

            if(allPersonReferences == null)
            {
                return NotFound();
            }

            foreach (PersonReference person in allPersonReferences)
            {
                PersonReferenceDTO personReferenceDTO = _mapper.Map<PersonReferenceDTO>(person);
                personReferenceDTOList.Add(personReferenceDTO);
            }

            return Ok(personReferenceDTOList);

        }
    }
}
