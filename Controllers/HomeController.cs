using System;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;

namespace ServManager.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

}
