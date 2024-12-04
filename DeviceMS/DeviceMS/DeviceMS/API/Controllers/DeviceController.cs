using AutoMapper;
using DeviceMS.Core.DomainLayer.Models;
using DeviceMS.API.DTOModels;
using DeviceMS.Logic.ServiceLayer.IServices;
using DeviceMS.Logic.ServiceLayer.Services;
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
        private readonly RabbitMQProducer _rabbitMQProducer;

        public DeviceController(IDeviceService deviceService, ILogger<DeviceController> logger, IMapper mapper,RabbitMQProducer rabbitMQProducer)
        {
            _deviceService = deviceService;
            _logger = logger;
            _mapper = mapper;
            _rabbitMQProducer = rabbitMQProducer;
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
            _logger.LogInformation("Attempting to create a new device.");

            var device = _mapper.Map<Device>(deviceCreateDTO);

            if (!Validator.IsValidDevice(device))
            {
                _logger.LogError("Invalid device data: {@DeviceCreateDTO}", deviceCreateDTO);
                return BadRequest("Invalid device data.");
            }

            List<Device> allDevices = await _deviceService.GetAllAsync();
            if (allDevices.Any(d => d.Address == deviceCreateDTO.Address))
            {
                _logger.LogError("A device with the same address already exists: {Address}", deviceCreateDTO.Address);
                return BadRequest("A device with this address already exists.");
            }

            try
            {
                var newDevice = await _deviceService.CreateAsync(device);
                var deviceDTO = _mapper.Map<DeviceDTO>(newDevice);


                var deviceInfoDTO = new DeviceInfoDTO
                {
                    Id = newDevice.Id,
                    MaxHourlyCons = newDevice.MaxHourlyCons,
                    Operation = "CREATE"
                };
                _rabbitMQProducer.SendDeviceInfo(deviceInfoDTO);

                _logger.LogInformation("Device successfully created: {@DeviceDTO}", deviceDTO);
                return CreatedAtAction(null, new { id = deviceDTO.Id }, deviceDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create device in MCMS or DeviceMS.");
                return BadRequest("An error occurred while creating the device.");
            }
        }


        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DeviceDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeviceDTO>> UpdateAsync([FromRoute] int id, [FromBody] DeviceUpdateDTO deviceUpdateDTO)
        {
            _logger.LogInformation("Attempting to update device with ID {DeviceId}", id);

            var existingDevice = await _deviceService.GetAsync(id);
            if (existingDevice == null)
            {
                _logger.LogError("Device with ID {DeviceId} not found.", id);
                return NotFound();
            }

            _mapper.Map(deviceUpdateDTO, existingDevice);

            if (!Validator.IsValidDevice(existingDevice))
            {
                _logger.LogError("Invalid updated device data for device ID {DeviceId}", id);
                return BadRequest("Invalid device data.");
            }

            try
            {
                var deviceInfoDTO = new DeviceInfoDTO
                {
                    Id = existingDevice.Id,
                    MaxHourlyCons = deviceUpdateDTO.MaxHourlyCons,
                    Operation = "UPDATE"
                };
                _rabbitMQProducer.SendDeviceInfo(deviceInfoDTO);

                await _deviceService.UpdateAsync(existingDevice);

                var updatedDevice = _mapper.Map<DeviceDTO>(existingDevice);
                _logger.LogInformation("Device successfully updated: {@DeviceDTO}", updatedDevice);
                return Ok(updatedDevice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update device in MCMS or DeviceMS.");
                return BadRequest("An error occurred while updating the device.");
            }
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

        [AllowAnonymous]
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

        [Authorize]
        [HttpDelete("{deviceId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteByIdAsync(int deviceId)
        {
            _logger.LogInformation("Attempting to delete device with ID {DeviceId}", deviceId);

            var device = await _deviceService.GetAsync(deviceId);
            if (device == null)
            {
                _logger.LogError("Device with ID {DeviceId} not found.", deviceId);
                return NotFound();
            }

            try
            {
                var deviceInfoDTO = new DeviceInfoDTO
                {
                    Id = deviceId,
                    Operation = "DELETE"
                };
                _rabbitMQProducer.SendDeviceInfo(deviceInfoDTO);

                await _deviceService.DeleteByIdAsync(deviceId);

                _logger.LogInformation("Device successfully deleted with ID {DeviceId}", deviceId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete device in MCMS or DeviceMS.");
                return BadRequest("An error occurred while deleting the device.");
            }
        }

        [AllowAnonymous]
        [HttpGet("test")]
        public ActionResult<string> GetTestMessage()
        {
            var xd = new Blabla{bbb = 1};
            return Ok(xd);
        }

        public class Blabla 
        {
            public int bbb  { get; set; }
        }

    }
}
