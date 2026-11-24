using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Code123.Controllers
{
    [Authorize(Roles ="Student,Admin")]
    public class StudentController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
