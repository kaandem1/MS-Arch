using AutoMapper;
using DeviceMS.Core.DomainLayer.Models;
using DeviceMS.API.DTOModels;
using DeviceMS.Logic.ServiceLayer.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DeviceMS.Logic.ServiceLayer.Helpers;

namespace DeviceMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly ILogger<DeviceController> _logger;
        private readonly IMapper _mapper;

        public DeviceController(IDeviceService deviceService, ILogger<DeviceController> logger, IMapper mapper)
        {
            _deviceService = deviceService;
            _logger = logger;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DeviceDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeviceDTO>> GetAsync([FromRoute] int id)
        {
            _logger.LogInformation("Trying to retrieve device with id {@Id}", id);
            var device = await _deviceService.GetAsync(id);

            if (device == null)
                return NotFound();

            var deviceDTO = _mapper.Map<DeviceDTO>(device);
            _logger.LogInformation("Found device: {@DeviceDTO}", deviceDTO);
            return Ok(deviceDTO);
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DeviceDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DeviceDTO>>> GetAllAsync()
        {
            var devices = await _deviceService.GetAllAsync();

            if (devices == null || !devices.Any())
                return NotFound();

            var deviceDTOs = devices.Select(device => _mapper.Map<DeviceDTO>(device)).ToList();
            _logger.LogInformation("Found devices: {@DeviceDTOs}", deviceDTOs);
            return Ok(deviceDTOs);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(DeviceDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DeviceDTO>> CreateAsync([FromBody] DeviceCreateDTO deviceCreateDTO)
        {
            _logger.LogInformation("Trying to create device.");
            var device = _mapper.Map<Device>(deviceCreateDTO);

            if (!Validator.IsValidDevice(device))
            {
                return BadRequest(device);
            }
            List<Device> allDevices = await _deviceService.GetAllAsync();
            foreach (Device d in allDevices)
            {
                if (deviceCreateDTO.Address == d.Address)
                {
                    _logger.LogError("A device with the same address already exists : {@Address}", deviceCreateDTO.Address);
                    return BadRequest();
                }
            }

            var newDevice = await _deviceService.CreateAsync(device);
            var deviceDTO = _mapper.Map<DeviceDTO>(newDevice);
            deviceDTO.Id = newDevice.Id;

            _logger.LogInformation("Device successfully created.");
            return CreatedAtAction(null, new { id = deviceDTO.Id }, deviceDTO);
        }
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DeviceDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DeviceDTO>> UpdateAsync([FromRoute] int id, [FromBody] DeviceUpdateDTO deviceUpdateDTO)
        {
            _logger.LogInformation("Trying to update device with id {@Id}", id);

            var device = await _deviceService.GetAsync(id);
            if (device == null)
            {
                _logger.LogError("Device with id {DeviceId} not found", id);
                return NotFound();
            }

            _mapper.Map(deviceUpdateDTO, device);

            if (!Validator.IsValidDevice(device))
            {
                _logger.LogError("Invalid device data for device id {DeviceId}", id);
                return BadRequest("Invalid device data");
            }

            await _deviceService.UpdateAsync(device);

            var deviceDTO = _mapper.Map<DeviceDTO>(device);
            _logger.LogInformation("Successfully updated device: {@DeviceDTO}", deviceDTO);

            return Ok(deviceDTO);
        }

        [Authorize]
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<DeviceDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DeviceDTO>>> SearchByNameAsync([FromQuery] string name)
        {
            _logger.LogInformation("Searching for devices with name containing {Name}", name);

            var devices = await _deviceService.SearchByNameAsync(name);

            if (devices == null || !devices.Any())
            {
                _logger.LogInformation("No devices found with name containing {Name}", name);
                return NotFound();
            }

            var deviceDTOs = devices.Select(device => _mapper.Map<DeviceDTO>(device)).ToList();
            _logger.LogInformation("Found devices: {@DeviceDTOs}", deviceDTOs);
            return Ok(deviceDTOs);
        }

        [Authorize]
        [HttpPut("{id}/assign")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignDeviceToUserAsync([FromRoute] int id, [FromBody] PersonReferenceDTO personReferenceDTO)
        {
            _logger.LogInformation("Assigning device with id {DeviceId} to user with id {UserId}", id, personReferenceDTO.UserId);

            var device = await _deviceService.GetAsync(id);

            if (device == null)
            {
                _logger.LogError("Device with id {DeviceId} not found", id);
                return NotFound();
            }

            try
            {
                await _deviceService.AssignDeviceToUserAsync(id, personReferenceDTO.UserId);
                _logger.LogInformation("Device with id {DeviceId} successfully assigned to user with id {UserId}", id, personReferenceDTO.UserId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning device with id {DeviceId} to user with id {UserId}", id, personReferenceDTO.UserId);
                return BadRequest("An error occurred while assigning the device to the user.");
            }
        }


        [Authorize]
        [HttpPatch("{deviceId}")]
        public async Task<IActionResult> RemoveDeviceFromUserAsync(int deviceId)
        {
            try
            {
                await _deviceService.RemoveDeviceFromUserAsync(deviceId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpPatch("clear/{userId}")]
        public async Task<IActionResult> ClearDevicesForUserAsync(int userId)
        {
            try
            {
                await _deviceService.ClearDevicesForUserAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<DeviceDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DeviceDTO>>> GetDevicesByUserIdAsync([FromRoute] int userId)
        {
            _logger.LogInformation("Retrieving devices for user with ID {UserId}", userId);
            var devices = await _deviceService.GetDevicesByUserIdAsync(userId);

            if (devices == null || !devices.Any())
            {
                _logger.LogInformation("No devices found for user with ID {UserId}", userId);
                return NotFound();
            }

            var deviceDTOs = devices.Select(device => _mapper.Map<DeviceDTO>(device)).ToList();
            _logger.LogInformation("Found devices for user ID {UserId}: {@DeviceDTOs}", userId, deviceDTOs);
            return Ok(deviceDTOs);
        }

        [Authorize]
        [HttpGet("unowned")]
        public async Task<IActionResult> GetUnownedDevices()
        {
            var devices = await _deviceService.GetUnownedDevicesAsync();
            var deviceDTOs = devices.Select(device => _mapper.Map<DeviceDTO>(device)).ToList();

            return Ok(deviceDTOs);
        }


    }
}
