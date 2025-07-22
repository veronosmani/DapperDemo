using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DapperDemo.Models;
using DapperDemo.Repository;
using Microsoft.AspNetCore.Authorization;

namespace DapperDemo.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ICompanyRepository _compRepo;

        public CompaniesController(ICompanyRepository compRepo)
        {
            _compRepo = compRepo;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            return View(_compRepo.GetAll());
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var company = _compRepo.Find(id.Value);
            if (company == null)
                return NotFound();

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,Name,Address,City,State,PostalCode")] Company company)
        {
            _compRepo.Add(company);
            return RedirectToAction(nameof(Index));
        }

        // GET: Companies/Edit/5
        public IActionResult Edit(int? id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return View("RedirectToLogin");
            }

            if (id == null)
            {
                return NotFound();
            }

            var company = _compRepo.Find(id.Value);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompanyId,Name,Address,City,State,PostalCode")] Company company)
        {

            if (id != company.CompanyId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _compRepo.Update(company);
                return RedirectToAction(nameof(Index));
            }

            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            _compRepo.Remove(id.Value);
            return RedirectToAction(nameof(Index));
        }
    }
}
