using portfolio_todo_rest_api.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace portfolio_todo_rest_api.Controllers;

public static class ControllerResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.Succeeded && result.Value != null)
        {
            return result.Value;
        }

        return result.ErrorCode switch
        {
            Errors.NotFound => controller.NotFound(CreateProblemDetails(StatusCodes.Status404NotFound, result)),
            Errors.Conflict => controller.Conflict(CreateProblemDetails(StatusCodes.Status409Conflict, result)),
            _ => controller.BadRequest(CreateProblemDetails(StatusCodes.Status400BadRequest, result))
        };
    }

    public static IActionResult ToActionResult(this ControllerBase controller, Result result)
    {
        if (result.Succeeded)
        {
            return controller.NoContent();
        }

        return result.ErrorCode switch
        {
            Errors.NotFound => controller.NotFound(CreateProblemDetails(StatusCodes.Status404NotFound, result)),
            Errors.Conflict => controller.Conflict(CreateProblemDetails(StatusCodes.Status409Conflict, result)),
            _ => controller.BadRequest(CreateProblemDetails(StatusCodes.Status400BadRequest, result))
        };
    }

    private static ProblemDetails CreateProblemDetails(int statusCode, Result result) =>
        new()
        {
            Status = statusCode,
            Title = result.ErrorCode,
            Detail = result.ErrorMessage
        };
}
