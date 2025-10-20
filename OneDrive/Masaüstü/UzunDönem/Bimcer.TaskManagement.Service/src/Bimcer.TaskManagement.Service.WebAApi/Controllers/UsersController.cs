using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bimcer.TaskManagement.Service.Business.Abstractions;
using Bimcer.TaskManagement.Service.Contracts.Users;

namespace Bimcer.TaskManagement.Service.WebAApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IUserService users) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll()
        => Ok(await users.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetById(string id)
    {
        var user = await users.GetByIdAsync(id);
        if (user is null)
            return NotFound();
        return Ok(user);
    }
}

