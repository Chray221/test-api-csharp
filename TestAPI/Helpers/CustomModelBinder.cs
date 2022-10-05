using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using TestAPI.Models;

namespace TestAPI.Helpers
{
    public class CustomModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(SignUpUserRequestDto))
                return new CustomModelBinder();
            return null;
        }
    }

    public class CustomModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));
            switch(bindingContext.HttpContext.Request.ContentType)
            {
                case "application/x-www-form-urlencoded":
                    break;
                case "multipart/form-data":
                    Logger.Log(JsonConvert.SerializeObject(bindingContext.HttpContext.Request.Form));
                    //var student = new Student();
                    //if (bindingContext.HttpContext.Request.Form.ContainsKey("StuId"))
                    //    student.StuId = Convert.ToInt32(bindingContext.HttpContext.Request.Form["StuId"].ToString());
                    //if (bindingContext.HttpContext.Request.Form.ContainsKey("StuName"))
                    //    student.StuName = bindingContext.HttpContext.Request.Form["StuName"].ToString();
                    //bindingContext.Result = ModelBindingResult.Success(student);
                    break;
                case "application/json":
                    string valueFromBody = string.Empty;
                    using (var sr = new StreamReader(bindingContext.HttpContext.Request.Body))
                    {
                        valueFromBody = sr.ReadToEnd();
                        if (string.IsNullOrEmpty(valueFromBody))
                        {
                            return Task.CompletedTask;
                        }
                        
                        var data = JsonConvert.DeserializeObject(valueFromBody, bindingContext.ModelMetadata.ModelType);
                        if (data != null)
                        {
                            bindingContext.Result = ModelBindingResult.Success(data);
                        }
                    }
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
