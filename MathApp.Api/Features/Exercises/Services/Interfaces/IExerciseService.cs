using MathAppApi.Features.Exercise.Dtos;
using Models;

namespace MathAppApi.Features.Exercise.Services.Interfaces;

public interface IExerciseService
{
    Task<ExerciseResponse> GetExercises(Models.UserProfile userProfile, ExerciseDto dto);
}
