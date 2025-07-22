using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    private readonly UserRepository _repo;

    public AuthController(UserRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("UserId") != null)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await _repo.Login(username, password);
        if (user == null)
        {
            TempData["LoginError"] = "Invalid username or password";
            return RedirectToAction("Login");
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("Username", user.Username);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.Session.GetInt32("UserId") != null)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string username, string email, string password)
    {
        if (await _repo.UserExists(username))
        {
            TempData["RegisterError"] = "User already exists";
            return RedirectToAction("Register");
        }

        var user = new User { Username = username, Email = email };
        await _repo.Register(user, password);
        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
