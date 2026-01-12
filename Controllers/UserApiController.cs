using Microsoft.AspNetCore.Mvc;
using Web_App_UserManagement.Models;
using Web_App_UserManagement.Services;

namespace Web_App_UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserApiController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserApiController> _logger;

        public UserApiController(IUserRepository userRepository, ILogger<UserApiController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng
        /// </summary>
        /// <returns>Danh sách người dùng</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        public ActionResult<List<User>> GetAll()
        {
            try
            {
                var users = _userRepository.GetAll();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách người dùng", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thông tin người dùng theo mã
        /// </summary>
        /// <param name="code">Mã người dùng</param>
        /// <returns>Thông tin người dùng</returns>
        [HttpGet("{code}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<User> GetByCode(string code)
        {
            try
            {
                var user = _userRepository.GetByCode(code);
                if (user == null)
                {
                    return NotFound(new { message = $"Không tìm thấy người dùng với mã: {code}" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by code: {Code}", code);
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin người dùng", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo người dùng mới
        /// </summary>
        /// <param name="user">Thông tin người dùng</param>
        /// <returns>Người dùng đã được tạo</returns>
        [HttpPost]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<User> Create([FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _userRepository.Add(user);
                _logger.LogInformation("User created: {Code}", user.Code);
                return CreatedAtAction(nameof(GetByCode), new { code = user.Code }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {Code}", user?.Code);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="code">Mã người dùng</param>
        /// <param name="user">Thông tin người dùng cần cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{code}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update(string code, [FromBody] User user)
        {
            try
            {
                if (code != user.Code)
                {
                    return BadRequest(new { message = "Mã người dùng không khớp" });
                }

                var existingUser = _userRepository.GetByCode(code);
                if (existingUser == null)
                {
                    return NotFound(new { message = $"Không tìm thấy người dùng với mã: {code}" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _userRepository.Update(user);
                _logger.LogInformation("User updated: {Code}", code);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {Code}", code);
                return StatusCode(500, new { message = "Lỗi khi cập nhật người dùng", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa người dùng
        /// </summary>
        /// <param name="code">Mã người dùng</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{code}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(string code)
        {
            try
            {
                var user = _userRepository.GetByCode(code);
                if (user == null)
                {
                    return NotFound(new { message = $"Không tìm thấy người dùng với mã: {code}" });
                }

                _userRepository.Delete(code);
                _logger.LogInformation("User deleted: {Code}", code);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {Code}", code);
                return StatusCode(500, new { message = "Lỗi khi xóa người dùng", error = ex.Message });
            }
        }
    }
}
