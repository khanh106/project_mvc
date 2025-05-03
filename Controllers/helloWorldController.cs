using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using tao_project.Models;

namespace  tao_project.Controllers
{
    public class HelloWorldController : Controller
    {
        // GET: /HelloWorld/
        public IActionResult Index()
        {
            return View();
        }

        // GET: /HelloWorld/Welcome/ 
        public string Welcome()
        {
            return "this is the welcome action method";
        }
    }
}