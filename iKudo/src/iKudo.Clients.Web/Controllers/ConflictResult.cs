using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace iKudo.Controllers
{
    public class ConflictResult : IActionResult
    {
        public ConflictResult(string message)
        {
            Error = message;
            StatusCode = HttpStatusCode.Conflict;
        }

        public string Error { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public Task ExecuteResultAsync(ActionContext context)
        {
            var response = new HttpResponseMessage()
            {
                Content = new StringContent(Error),
                StatusCode = HttpStatusCode.Conflict
            };
            return Task.FromResult(response);
        }
    }
}
