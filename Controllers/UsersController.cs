using LibraryAPI.DTOs;
using LibraryAPI.Helpers;
using LibraryAPI.Interfaces;
using LibraryAPI.Repositories;
using LibraryAPI.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using LibraryAPI.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;

namespace LibraryAPI.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase {
        private readonly IUsersRepository _repo;

        public UsersController(IUsersRepository repo) {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Get() => Ok(_repo.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id) {
            var user = _repo.GetById(id);

            if (user == null) { 
                return NotFound();
            }

            var response = new UsersResponse {
                id = user.id,
                name = user.name,
                login = user.login,
                email = user.email,
                profile = user.profile != null ? $"data:image/png;base64,{Convert.ToBase64String(user.profile)}" : null
            };

            return Ok(response);
        }

        [HttpPost]
        public IActionResult Post([FromForm] UsersRequest request) {
            var dto = ToDto(request);

            try {
                if (_repo.LoginExists(dto.login)) { 
                    return Conflict(new { field = "login", message = "Login já existe" });
                }

                if (_repo.EmailExists(dto.email)) { 
                    return Conflict(new { field = "email", message = "E-mail já existe" });
                }

                _repo.Insert(dto);
                return Ok();
            } catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) {
                return Conflict(new { message = "Login ou e-mail já cadastrado" });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromForm] UsersRequest request) {
            var dto = ToDto(request);
            dto.id = id;

            try {
                if (_repo.LoginExists(dto.login, id))
                    return Conflict(new { field = "login", message = "Login já existe" });

                if (_repo.EmailExists(dto.email, id))
                    return Conflict(new { field = "email", message = "E-mail já existe" });

                _repo.Update(dto);
                return Ok();
            } catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) {
                return Conflict(new { message = "Login ou e-mail já cadastrado" });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) {
            _repo.Delete(id);
            return Ok();
        }

        private UsersDTO ToDto(UsersRequest request) {
            return new UsersDTO {
                name = request.name,
                login = request.login,
                password = string.IsNullOrWhiteSpace(request.password) ? null : HashHelper.CreateSha256(request.password),
                email = request.email,
                role = request.role,
                observations = request.observations ?? string.Empty,
                active = request.active,
                profile = ConversionsHelper.ConvertToBytes(request.profile)
            };
        }
    }
}