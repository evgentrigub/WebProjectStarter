using System.Collections.Generic;
using System.Net;
using AutoMapper;
using System.Threading.Tasks;
using API.Dtos;
using API.Helpers;
using API.Models;
using API.Models.Base;
using API.Models.ViewModels;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    //[Authorize]
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private IUserService _userService;
        private IMapper _mapper;

        public UsersController(
            IUserService userService,
            IMapper mapper
        )
        {
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        /// <response code="202">Returns authenticated user</response>
        /// <response code="400">If something wrong with authenticate</response>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Authenticate([FromBody] UserDto userDto)
        {
            try
            {
                var result = await _userService.Authenticate(userDto.Username, userDto.Password);
                if (!result.IsSuccess)
                    return NotFound(result.Message);

                var token = _userService.CreateToken(result.Data.Id);
                var user = _mapper.Map<UserAuthenticateModel>(result.Data);
                user.Token = token;

                return Accepted(new Result<UserAuthenticateModel>(message: "Authenticate successful!", isSuccess: true,
                    data: user));

            }
            catch (AppException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        /// <response code="201">Returns registered user</response>
        /// <response code="400">If username exist or password null</response>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            try
            {
                var result = await _userService.Create(user, userDto.Password);
                if (!result.IsSuccess)
                    return BadRequest(result.Message);

                return Created("", result);
            }
            catch (AppException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll().ToListAsync();
            var userViewModels = _mapper.Map<IList<UserViewModel>>(users);
            return Ok(userViewModels);
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Returns user</response>
        /// <response code="400">If username is empty</response>
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var result = await _userService.FindById(id);
                if (!result.IsSuccess)
                    return BadRequest(result.Message);

                var userViewModel = _mapper.Map<UserViewModel>(result.Data);
                return Ok(userViewModel);
            }
            catch (AppException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userDto"></param>
        /// <returns></returns>
        /// <response code="200">Updated successful</response>
        /// <response code="400">If error while updating</response>
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult Update(string id, [FromBody] UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            try
            {
                var result = _userService.Update(id, user);
                if (!result.IsSuccess)
                    return BadRequest(result.Message);
                return Ok(result);
            }
            catch (AppException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Returns registered user</response>
        /// <response code="400">If error while deleting</response>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult Delete(string id)
        {
            _userService.Delete(id);
            return Ok();
        }
    }
}
