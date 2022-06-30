using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vookaba.Utils
{
    public class HttpStatusCodeDescriber
    {
        private readonly IStringLocalizer<HttpStatusCodeDescriber> localizer;

        public HttpStatusCodeDescriber(IStringLocalizer<HttpStatusCodeDescriber> localizer)
        {
            this.localizer = localizer;
        }
        public string GetStatusCodeDescription(int code) =>
         code switch
         {
             100 => localizer["Continue"],
             101 => localizer["Switching Protocols"],
             102 => localizer["Processing"],
             200 => localizer["OK"],
             201 => localizer["Created"],
             202 => localizer["Accepted"],
             203 => localizer["Non-Authoritative Information"],
             204 => localizer["No Content"],
             205 => localizer["Reset Content"],
             206 => localizer["Partial Content"],
             207 => localizer["Multi-Status"],
             300 => localizer["Multiple Choices"],
             301 => localizer["Moved Permanently"],
             302 => localizer["Found"],
             303 => localizer["See Other"],
             304 => localizer["Not Modified"],
             305 => localizer["Use Proxy"],
             307 => localizer["Temporary Redirect"],
             400 => localizer["Bad Request"],
             401 => localizer["Unauthorized"],
             402 => localizer["Payment Required"],
             403 => localizer["Forbidden"],
             404 => localizer["Not Found"],
             405 => localizer["Method Not Allowed"],
             406 => localizer["Not Acceptable"],
             407 => localizer["Proxy Authentication Required"],
             408 => localizer["Request Timeout"],
             409 => localizer["Conflict"],
             410 => localizer["Gone"],
             411 => localizer["Length Required"],
             412 => localizer["Precondition Failed"],
             413 => localizer["Request Entity Too Large"],
             414 => localizer["Request-Uri Too Long"],
             415 => localizer["Unsupported Media Type"],
             416 => localizer["Requested Range Not Satisfiable"],
             417 => localizer["Expectation Failed"],
             422 => localizer["Unprocessable Entity"],
             423 => localizer["Locked"],
             424 => localizer["Failed Dependency"],
             500 => localizer["Internal Server Error"],
             501 => localizer["Not Implemented"],
             502 => localizer["Bad Gateway"],
             503 => localizer["Service Unavailable"],
             504 => localizer["Gateway Timeout"],
             505 => localizer["Http Version Not Supported"],
             507 => localizer["Insufficient Storage"],
             _ => localizer["Unknown Error"]
         };
    }
}
