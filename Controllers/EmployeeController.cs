using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tao_project.Models;

 namespace Employee.Controllers
 {
     public class EmployeeController : Controller
     {
        public IActionResult view_employee()
         {
             return View();
         }
 
         public IActionResult add_employee()
         {
             return View();
         }
 
     }
 }