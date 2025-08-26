using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListAPI.Exceptions;
using TodoListAPI.Models;
using TodoListAPI.Services;

namespace TodoListAPI.Controllers;

[ApiController]
[Route("todos")]
public class TodosController : ControllerBase
{
    private readonly ITasksService _tasksService;

    public TodosController(ITasksService tasksService)
    {
        _tasksService = tasksService;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(TaskDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = int.Parse(HttpContext.User.FindFirst("id")?.Value!);
        var task = await _tasksService.CreateTaskAsync(dto, userId);
        return CreatedAtAction(nameof(GetSingle), new { id = task.TaskId }, task);
    }

    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, TaskDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var task = await _tasksService.GetTaskAsync(id);
        if (task == null)
            return NotFound();

        var userId = int.Parse(HttpContext.User.FindFirst("id")?.Value!);
        if (task.Creator.UserId != userId)
            return Forbid();

        try
        {
            var updatedTask = await _tasksService.UpdateTaskAsync(id, dto);
            return Ok(updatedTask);
        }
        catch (TaskNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete(("{id}"))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var task = await _tasksService.GetTaskAsync(id);
        if (task == null)
            return NotFound();

        var userId = int.Parse(HttpContext.User.FindFirst("id")?.Value!);
        if (task.Creator.UserId != userId)
            return Forbid();
        
        return await _tasksService.DeleteTaskAsync(id) ? NoContent() : NotFound();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        var tasks = await _tasksService.GetPagedTasksAsync(page, limit);
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSingle(int id)
    {
        var task = await _tasksService.GetTaskAsync(id);
        return task != null ? Ok(task) : NotFound();
    }
}