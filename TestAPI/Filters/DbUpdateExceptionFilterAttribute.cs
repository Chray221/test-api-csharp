using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace TestAPI.Filters
{
    public class DbUpdateExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            //base.OnException(context);
            switch(context.Exception)
            {
                case DbUpdateException dbUpdateException:
                    return; 
            }

            SqlException sqlException = context.Exception?.InnerException as SqlException;

            if(sqlException?.Number == 2627)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    public class HttpResponseException : Exception
    {
        public HttpResponseException(int statusCode, object? value = null) =>
            (StatusCode, Value) = (statusCode, value);

        public int StatusCode { get; }

        public object Value { get; }
    }

    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                if (context.Exception is HttpResponseException httpResponseException)
                {
                    context.Result = new ObjectResult(httpResponseException.Value)
                    {
                        StatusCode = httpResponseException.StatusCode
                    };

                    context.ExceptionHandled = true;
                }
                if (context.Exception is DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException is SqlException sqlException)
                    {
                        if (sqlException?.Number == 2627)
                        {
                            context.Result = new StatusCodeResult(StatusCodes.Status409Conflict);
                            context.ExceptionHandled = true;
                            return;
                        }
                    }
                    else if(dbUpdateException.InnerException is SqliteException sqliteException)
                    {
                        if (sqliteException.SqliteErrorCode == 19)// Unique
                        {
                            //context.Result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                            //context.ExceptionHandled = true;
                            return;
                        }
                        context.Result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                        context.ExceptionHandled = true;
                    }
                    return;
                }
                context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
