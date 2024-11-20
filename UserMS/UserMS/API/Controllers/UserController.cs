using UserMS.API.DTOModels;
using AutoMapper;
using UserMS.Core.DomainLayer.Enums;
using UserMS.Core.DomainLayer.Models;
using UserMS.Logic.ServiceLayer.IServices;
using UserMS.Logic.ServiceLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserMS.Logic.ServiceLayer.Helpers;
using System.Security.Claims;

namespace UserMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        readonly IUserService _userService;
        readonly ILogger<UserController> _logger;
        readonly IMapper _mapper;
        readonly ITokenService _tokenService;

        public UserController(IUserService userService, ILogger<UserController> logger, IMapper mapper, ITokenService tokenService)
        {
            _userService = userService;
            _logger = logger;
            _mapper = mapper;
            _tokenService = tokenService;
        }
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetAsync([FromRoute] int id)
        {
            _logger.LogInformation("Trying to retrieve user with id {@Id}", id);
            UserDTO userDTO;
            var user = await _userService.GetAsync(id);

            if (user == null)
                return NotFound();

            userDTO = _mapper.Map<UserDTO>(user);
            _logger.LogInformation("Found user: {@UserDTO}", userDTO);
            return Ok(userDTO);
        }
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetAllAsync()
        {

            var users = await _userService.GetAllAsync();
            List<UserDTO> userDTOs = new List<UserDTO>();
            foreach (var user in users)
            {
                userDTOs.Add(_mapper.Map<UserDTO>(user));
            }

            _logger.LogInformation("Found user: {@UserDTO}", userDTOs);
            return Ok(userDTOs);
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDTO>> CreateAsync([FromBody] UserCreateDTO userCreateDTO)
        {
            _logger.LogInformation("Trying to create user.");
            var user = _mapper.Map<User>(userCreateDTO);
            if (!Validator.IsValidUser(user))
            {
                return BadRequest(user);
            }
            var newUser = await _userService.CreateAsync(user);

            if (newUser.Id < 0)
            {
                _logger.LogError("Invalid request");
                return BadRequest();
            }
            UserDTO userDTO = _mapper.Map<UserDTO>(newUser);
            userDTO.Id = newUser.Id;
            _logger.LogInformation("User succesfuly created.");
            return CreatedAtAction(null, new { id = userDTO.Id }, userDTO);
        }

        [Authorize]
        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDTO>> UpdateAsync(int userId, [FromBody] UserUpdateDTO userUpdateDTO)
        {
            _logger.LogInformation("Trying to update user with ID: {UserId}", userId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.GetAsync(userId);

            if (user == null)
            {
                _logger.LogInformation("User with ID {UserId} not found", userId);
                return NotFound();
            }

            var userClone = user.Clone();

            if (!Validator.IsValid(userUpdateDTO.FirstName) ||
                !Validator.IsValid(userUpdateDTO.LastName) ||
                !Validator.IsValid(userUpdateDTO.Country))
            {
                _logger.LogError("Update failed - invalid user data: {@UserUpdateDTO}", userUpdateDTO);
                return BadRequest("Invalid user data");
            }

            user = _mapper.Map(userUpdateDTO, user);
            await _userService.UpdateAsync(user);

            var userDTO = _mapper.Map<UserDTO>(user);
            var originalUserDTO = _mapper.Map<UserDTO>(userClone);

            var response = new
            {
                OriginalUser = originalUserDTO,
                ModifiedUser = userDTO,
            };

            _logger.LogInformation("Successfully updated: {@Response}", response);
            return Ok(response);
        }




        [AllowAnonymous]
        [HttpPatch("change-password")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ChangePasswordAsync([FromBody] UserChangePasswordDTO userChangePasswordDTO)
        {
            _logger.LogInformation("Trying to change the password of user with id ");

            if (!Validator.IsValidPassword(userChangePasswordDTO.NewPassword))
            {
                return BadRequest(userChangePasswordDTO.NewPassword);
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userService.GetAsync(int.Parse(userId));
            var userClone = user.Clone();
            if (user == null)
                return NotFound();

            var userFound = await _userService.FindByCredentialsAsync(user.Email, userChangePasswordDTO.OldPassword);
            if (userFound == null)
            {
                return BadRequest("credentials not ok");
            }
            user = await _userService.ChangePasswordAsync(user, userChangePasswordDTO.NewPassword);



            var userDTO = _mapper.Map<UserDTO>(user);
            var originalUserDTO = _mapper.Map<UserDTO>(userClone);
            var response = new
            {
                OriginalUser = originalUserDTO,
                OldPassword = userClone.Password,
                ModifiedUser = userDTO,
                NewPassword = user.Password,
            };
            _logger.LogInformation("Succesfuly updated the password: {@Response}", response);
            return Ok(response);

        }
        [AllowAnonymous]
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ResetPasswordAsync([FromBody] UserResetPasswordDTO userResetPasswordDTO)
        {
            _logger.LogInformation("Trying to reset the password of user with id ");

            if (!Validator.IsValidPassword(userResetPasswordDTO.NewPassword))
            {
                return BadRequest(userResetPasswordDTO.NewPassword);
            }
            User user = await _userService.FindByEmailAsync(userResetPasswordDTO.Email);
            if (user == null)
                return NotFound();

            var userFound = await _userService.FindByEmailAsync(user.Email);
            if (userFound == null)
            {
                return BadRequest("credentials not ok");
            }
            user = await _userService.ChangePasswordAsync(user, userResetPasswordDTO.NewPassword);



            var userDTO = _mapper.Map<UserDTO>(user);
            var originalUserDTO = _mapper.Map<UserDTO>(user);
            var response = new
            {
                OriginalUser = originalUserDTO,
                ModifiedUser = userDTO,
                NewPassword = user.Password,
            };
            _logger.LogInformation("Succesfuly updated the password: {@Response}", response);
            return Ok(response);

        }



        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/change-role")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeRoleAsync([FromRoute] int id, [FromBody] int role)
        {
            _logger.LogInformation("Trying to change the role of user with id {@Id}", id);
            var user = await _userService.GetAsync(id);
            var userClone = user.Clone();
            if (user == null)
                return NotFound();


            user.Role = (UserRole)role;
            await _userService.UpdateAsync(user);
            if (user.Role == userClone.Role)
            {
                _logger.LogError("Change Role failed {@Role}", role);
                return BadRequest("Change Role failed");
            }


            var userDTO = _mapper.Map<UserDTO>(user);
            var originalUserDTO = _mapper.Map<UserDTO>(userClone);
            var response = new
            {
                OriginalUser = originalUserDTO,
                ModifiedUser = userDTO,
            };
            _logger.LogInformation("Successfuly changed the role : {@Response}", response);
            return Ok(response);

        }
        [Authorize]
        [HttpPatch("{id}/inactivate")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> InactivateAsync([FromRoute] int id, [FromBody] int status)
        {
            _logger.LogInformation("Trying to inactivate the user with id {@Id}", id);
            var user = await _userService.GetAsync(id);
            var userClone = user.Clone();
            if (user == null)
                return NotFound();

            user.Status = (UserStatus)status;
            await _userService.UpdateAsync(user);
            if (user.Status == userClone.Status)
            {
                _logger.LogError("Inactivate failed {@Status}", status);
                return BadRequest("Inactivate failed");
            }


            var userDTO = _mapper.Map<UserDTO>(user);
            var originalUserDTO = _mapper.Map<UserDTO>(userClone);
            var response = new
            {
                User = userDTO,
                OriginalStatus = userClone.Status,
                ModifiedStatus = user.Status,
            };
            _logger.LogInformation("Successfully inactivated: {@Response}", response);
            return Ok(response);


        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> LoginAsync([FromBody] UserLoginDTO userLoginDTO)
        {
            _logger.LogInformation("Trying to login...");

            var user = await _userService.FindByCredentialsAsync(userLoginDTO.Username, userLoginDTO.Password);
            _logger.LogInformation("User found: {@user}", user);
            _logger.LogInformation("Password: {Password}", userLoginDTO.Password);


            if (user == null || user.Status == (UserStatus)1)
            {
                return BadRequest("login failed");
            }

            string token = await _tokenService.GenerateTokenAsync(user);

            return Ok(token);

        }
        [Authorize]
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> SearchByNameAsync([FromQuery] string name)
        {
            _logger.LogInformation("Searching for users with name containing {Name}", name);

            var users = await _userService.SearchByNameAsync(name);
            if (users == null || !users.Any())
            {
                _logger.LogInformation("No users found with name containing {Name}", name);
                return NotFound();
            }

            var userDTOs = users.Select(user => _mapper.Map<UserDTO>(user)).ToList();
            _logger.LogInformation("Found users: {@UserDTOs}", userDTOs);
            return Ok(userDTOs);
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task DeleteAsync([FromRoute] int id)
        {
            if (id == 0)
            {
                _logger.LogError("Invalid id (id = 0).");
                throw new ArgumentException("id");
            }
            await _userService.DeleteAsync(id);
            _logger.LogInformation("Deleted album with {@Id}", id);
        }

    }
}
