using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OakChan.Controllers
{
    public class BoardController : Controller
    {
        public string Index() => $"Board {RouteData.Values["board"]}";
    }
}
