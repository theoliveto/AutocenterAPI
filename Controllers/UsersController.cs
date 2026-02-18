using LibraryAPI.DTOs;
using LibraryAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetById(int id) => Ok(_repo.GetById(id));

        [HttpPost]
        public IActionResult Post(UsersDTO dto) {
            _repo.Insert(dto);
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, UsersDTO dto) {
            dto.id = id;
            _repo.Update(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) {
            _repo.Delete(id);
            return Ok();
        }
    }
}