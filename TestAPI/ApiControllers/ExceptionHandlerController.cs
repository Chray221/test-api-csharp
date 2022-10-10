using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TestAPI.Filters;

namespace TestAPI.ApiControllers
{
    //[ApiController]
    [ApiVersion("1.0")]
    [Route("error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ExceptionHandlerController : MyControllerBase
    {

        public ExceptionHandlerController(IWebHostEnvironment environment)
            : base(environment)
        {
        }

        [Route("development")]
        public IActionResult HandleDevelopmentError()
        {
            var exceptionHandlerFeature =
                HttpContext.Features.Get<IExceptionHandlerFeature>()!;

            Exception exception = exceptionHandlerFeature.Error;
            if (exception != null)
            {
#if DEBUG
                object content = exception;
#else
            object content = new { message = "Oops, something unexpected went wrong" };
#endif

                if (exception is HttpResponseException httpResponseException)
                {
                    return StatusCode(httpResponseException.StatusCode, httpResponseException.Value);
                }
                else if (exception is DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException is SqlException sqlException)
                    {
                        if (sqlException?.Number == 2627)
                        {
                            return Conflict(content);
                        }
                    }
                    else if (dbUpdateException.InnerException is SqliteException sqliteException)
                    {
                        if (sqliteException.SqliteErrorCode == 19)// Unique
                        {
                            return BadRequest(content);
                        }
                        return BadRequest(content);
                    }

                }

                return StatusCode(StatusCodes.Status500InternalServerError, content);
            }
            return Ok();
        }
    }
}
