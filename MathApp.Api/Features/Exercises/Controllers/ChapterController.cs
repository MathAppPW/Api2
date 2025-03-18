using MathApp.Dal.Interfaces;
using MathAppApi.Features.Exercises.Dtos;
using MathAppApi.Features.Exercises.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace MathAppApi.Features.Exercises.Controllers;

[ApiController]
[Route("[controller]")]
public class ChapterController : ControllerBase
{
    private readonly IChapterRepo _chapterRepo;

    public ChapterController(IChapterRepo chapterRepo)
    {
        _chapterRepo = chapterRepo;
    }
    
    [ProducesResponseType<List<ChapterHeaderDto>>(StatusCodes.Status200OK)]
    [HttpGet("all")]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var all = await _chapterRepo.GetAllAsync();
        var result = all.Select(c => c.ToHeaderDto()).ToList();
        return Ok(result);
    }

    [ProducesResponseType<ChapterDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{name}")]
    [Authorize]
    public async Task<IActionResult> GetOne([FromRoute] string name)
    {
        var chapter = await _chapterRepo.GetAsync(name);
        if (chapter == null)
            return NotFound();
        await _chapterRepo.LoadCollectionAsync(chapter, c => c.Subjects);
        return Ok(chapter.ToDto());
    }
    
    //FIX: Should be deleted later
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Chapter chapter)
    {
        await _chapterRepo.AddAsync(chapter);
        return Ok();
    }
}