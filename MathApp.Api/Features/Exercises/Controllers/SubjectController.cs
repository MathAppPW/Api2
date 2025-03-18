using MathApp.Dal.Interfaces;
using MathAppApi.Features.Exercises.Dtos;
using MathAppApi.Features.Exercises.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MathAppApi.Features.Exercises.Controllers;

[ApiController]
[Route("[controller]")]
public class SubjectController : ControllerBase
{
    private readonly ISubjectRepo _subjectRepo;

    public SubjectController(ISubjectRepo subjectRepo)
    {
        _subjectRepo = subjectRepo;
    }

    [ProducesResponseType<SubjectDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetSubject([FromRoute] int id)
    {
        var subject = await _subjectRepo.GetAsync(id);
        if (subject == null)
            return NotFound();
        await _subjectRepo.LoadCollectionAsync(subject, s => s.Lessons);
        var dto = subject.ToDto();
        return Ok(dto);
    }
}