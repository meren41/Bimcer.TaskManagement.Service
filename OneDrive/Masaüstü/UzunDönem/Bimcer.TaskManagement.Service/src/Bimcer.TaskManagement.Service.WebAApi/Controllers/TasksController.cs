using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bimcer.TaskManagement.Service.Business.Abstractions;
using Bimcer.TaskManagement.Service.Contracts;
using Bimcer.TaskManagement.Service.Contracts.Tasks;

namespace Bimcer.TaskManagement.Service.WebAApi.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize] // ⬅️ tüm metotlar korumalı
public class TasksController(ITaskService tasks) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskResponse>>> GetMine()
        => Ok(await tasks.GetMyAsync(GetUserId()));

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create([FromBody] TaskCreateRequest request)
    {
        var created = await tasks.CreateAsync(GetUserId(), request);
        return CreatedAtAction(nameof(GetMine), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<TaskResponse>> Update(long id, [FromBody] TaskUpdateRequest request)
        => Ok(await tasks.UpdateAsync(GetUserId(), id, request));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await tasks.DeleteAsync(GetUserId(), id);
        return NoContent();
    }

    private string GetUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("sub");
        return sub!;
    }
}
