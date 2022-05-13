using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestApp.Data;
using TestApp.Models;

namespace TestApp.Controllers
{
    public class PersonController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public PersonController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Person
        [HttpGet]
        public async Task<IActionResult> Index()
        {
              return _context.Persons != null ? 
                          View(await _context.Persons.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Persons'  is null.");
        }
        
        [HttpPost]
        public async Task<IActionResult> Index(IFormFile? file)
        {
            //if (!_context.Persons.Any())
            {
                #region UploadCSV

                var name = Path.Combine(_webHostEnvironment.WebRootPath + "\\files", Path.GetFileName(file.FileName));
                FileStream f = new FileStream(name, FileMode.Create);
                await file.CopyToAsync(f);
                f.Close();

                using (FileStream fileStream = System.IO.File.Create(name))
                {
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                }

                #endregion
                
                _context.RemoveRange(_context.Persons);
                _context.Persons.AddRange(GetPersonLists(file.FileName));
                await _context.SaveChangesAsync();
            
                System.IO.File.Delete(name);
            }

            return RedirectToAction(nameof(Index));
        }

        private List<Person> GetPersonLists(string fileName)
        {
            List<Person> persons = new List<Person>();
                
            //read csv    
            string path = $"wwwroot/files/{fileName}";
            using (StreamReader reader = new StreamReader(path))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    Person person = csv.GetRecord<Person>();
                    persons.Add(person);
                }
            }

            return persons;
        }

        // GET: Person/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Person/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Date,Married,Salary")] Person person)
        {
           _context.Add(person);
           await _context.SaveChangesAsync();
           return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        public async Task Edit([FromBody] Person person)
        {
            if (ModelState.IsValid)
            {
                
                _context.Persons.Update(person);
                await _context.SaveChangesAsync();
            }

        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Persons == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Persons'  is null.");
            }
            var person = await _context.Persons
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (person != null)
            {
                _context.Persons.Remove(person);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
          return (_context.Persons?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
