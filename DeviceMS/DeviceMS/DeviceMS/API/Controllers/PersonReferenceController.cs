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
            _logger.LogDebug("Received request to create PersonReference with UserId: {@UserId}", personReferenceDTO.UserId);

            try
            {
                _logger.LogDebug("Fetching all existing person references...");
                List<PersonReference> allPersonReferences = await _personReferenceService.GetAllAsync();
                _logger.LogDebug("Total person references fetched: {Count}", allPersonReferences.Count);

                foreach (PersonReference pr in allPersonReferences)
                {
                    _logger.LogDebug("Checking if UserId: {UserId} already exists in the database...", pr.UserId);
                    if (personReferenceDTO.UserId == pr.UserId)
                    {
                        _logger.LogError("A person reference with the same UserId already exists: {@UserId}", personReferenceDTO.UserId);
                        return BadRequest("Person reference with this UserId already exists.");
                    }
                }

                _logger.LogDebug("Mapping PersonReferenceDTO to PersonReference entity...");
                var personReference = _mapper.Map<PersonReference>(personReferenceDTO);

                _logger.LogDebug("Saving new person reference to the database...");
                await _personReferenceService.CreateAsync(personReference);

                _logger.LogDebug("Person reference created successfully for UserId: {@UserId}", personReferenceDTO.UserId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a person reference for UserId: {@UserId}", personReferenceDTO.UserId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }

        [AllowAnonymous]
        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeletePersonReferenceAsync([FromRoute] int userId)
        {
            var personReference = new PersonReference { UserId = userId };

            await _personReferenceService.DeleteAsync(personReference);

            return Ok();
        }


        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(List<PersonReferenceDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<PersonReferenceDTO>>> GetAllPersonReferencesAsync()
        {
            List<PersonReferenceDTO> personReferenceDTOList = new List<PersonReferenceDTO>();

            List<PersonReference> allPersonReferences = await _personReferenceService.GetAllAsync();

            if (allPersonReferences == null)
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
