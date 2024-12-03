using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using MCMS.Models;
using MCMS.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace MCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class DeviceConsumptionController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly ILogger<DeviceConsumptionController> _logger;

        public DeviceConsumptionController(IDeviceService deviceService, ILogger<DeviceConsumptionController> logger)
        {
            _deviceService = deviceService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("{deviceId}")]
        [ProducesResponseType(typeof(DeviceConsumptionDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeviceConsumptionDTO>> GetDeviceConsumption(int deviceId)
        {
            try
            {
                _logger.LogInformation("Received request to fetch consumption data for deviceId: {DeviceId}", deviceId);

                var deviceConsumption = await _deviceService.GetDeviceConsumptionAsync(deviceId);

                if (deviceConsumption == null)
                {
                    _logger.LogWarning("No consumption data found for deviceId: {DeviceId}", deviceId);
                    return NotFound($"No consumption data found for device with ID {deviceId}");
                }

                _logger.LogInformation("Found consumption data for deviceId: {DeviceId}", deviceId);
                
                var consumptionDTO = new DeviceConsumptionDTO
                {
                    DeviceId = deviceConsumption.DeviceId,
                    MaxHourlyCons = deviceConsumption.MaxHourlyCons,
                    HourlyConsumption = deviceConsumption.HourlyConsumption
                };

                return Ok(consumptionDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while fetching consumption data for deviceId: {DeviceId}. Error: {Error}", deviceId, ex.Message);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("test")]
        public ActionResult<string> GetTestMessage()
        {
            return Ok("Testtetettettetet");
        }
    }

    public class DeviceConsumptionDTO
    {
        public int DeviceId { get; set; }
        public float MaxHourlyCons { get; set; }
        public Dictionary<long, float> HourlyConsumption { get; set; }
    }

}
