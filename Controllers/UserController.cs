using Microsoft.AspNetCore.Mvc;
using Web_App_UserManagement.Models;
using Web_App_UserManagement.Services;

namespace Web_App_UserManagement.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var users = _userRepository.GetAll();
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _userRepository.Add(user);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(user);
        }

        public IActionResult Edit(string id) // id corresponds to Code
        {
            var user = _userRepository.GetByCode(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                _userRepository.Update(user);
                return RedirectToAction("Index");
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            _userRepository.Delete(id);
            return RedirectToAction("Index");
        }

        // ========== API ENDPOINTS ==========
        // Các endpoint này sẽ trả về JSON cho Swagger và API calls

        /// <summary>
        /// [API] Lấy danh sách tất cả người dùng
        /// </summary>
        [HttpGet("api/User")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        public ActionResult<List<User>> GetAllApi()
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
        /// [API] Lấy thông tin người dùng theo mã
        /// </summary>
        [HttpGet("api/User/{code}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<User> GetByCodeApi(string code)
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
        /// [API] Tạo người dùng mới
        /// </summary>
        [HttpPost("api/User")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<User> CreateApi([FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _userRepository.Add(user);
                _logger.LogInformation("User created via API: {Code}", user.Code);
                return CreatedAtAction(nameof(GetByCodeApi), new { code = user.Code }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user via API: {Code}", user?.Code);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// [API] Cập nhật thông tin người dùng
        /// </summary>
        [HttpPut("api/User/{code}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateApi(string code, [FromBody] User user)
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
                _logger.LogInformation("User updated via API: {Code}", code);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user via API: {Code}", code);
                return StatusCode(500, new { message = "Lỗi khi cập nhật người dùng", error = ex.Message });
            }
        }

        /// <summary>
        /// [API] Xóa người dùng
        /// </summary>
        [HttpDelete("api/User/{code}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteApi(string code)
        {
            try
            {
                var user = _userRepository.GetByCode(code);
                if (user == null)
                {
                    return NotFound(new { message = $"Không tìm thấy người dùng với mã: {code}" });
                }

                _userRepository.Delete(code);
                _logger.LogInformation("User deleted via API: {Code}", code);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user via API: {Code}", code);
                return StatusCode(500, new { message = "Lỗi khi xóa người dùng", error = ex.Message });
            }
        }
    }
}
