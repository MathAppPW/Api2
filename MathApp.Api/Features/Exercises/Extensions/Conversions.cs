using MathAppApi.Features.Exercises.Dtos;
using Models;

namespace MathAppApi.Features.Exercises.Extensions;

public static class Conversions
{
    public static ChapterHeaderDto ToHeaderDto(this Chapter chapter)
    {
        return new ChapterHeaderDto
        {
            Name = chapter.Name
        };
    }

    public static ChapterDto ToDto(this Chapter chapter)
    {
        return new ChapterDto()
        {
            Name = chapter.Name,
            Subjects = chapter.Subjects.Select(s => s.ToHeaderDto()).ToList(),
        };
    }

    public static SubjectHeaderDto ToHeaderDto(this Subject subject)
    {
        return new SubjectHeaderDto()
        {
            Name = subject.Name
        };
    }

    public static SubjectDto ToDto(this Subject subject)
    {
        return new SubjectDto()
        {
            Name = subject.Name,
            Lessons = subject.Lessons.Select(l => l.ToHeaderDto()).ToList()
        };
    }
    
    public static LessonHeaderDto ToHeaderDto(this Lesson lesson)
    {
        return new LessonHeaderDto()
        {
            Id = lesson.Id
        };
    }

    public static LessonDto ToLessonDto(this Lesson lesson)
    {
        return new LessonDto()
        {
            Id = lesson.Id,
            Series = lesson.Series.Select(s => s.ToHeaderDto()).ToList(),
        };
    }
    
    public static SeriesHeaderDto ToHeaderDto(this Series series)
    {
        return new SeriesHeaderDto()
        {
            Id = series.Id
        };
    }

    public static ExerciseHeaderDto ToHeaderDto(this Exercise exercise)
    {
        return new ExerciseHeaderDto()
        {
            Id = exercise.Id
        };
    }

    public static ExerciseDto ToDto(this Exercise exercise)
    {
        return new ExerciseDto()
        {
            Contents = exercise.Contents,
            Id = exercise.Id
        };
    }
}