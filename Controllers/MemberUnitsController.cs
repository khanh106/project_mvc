using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tao_project.Data;
using tao_project.Models.Entities;

namespace tao_project.Controllers
{
    public class MemberUnitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemberUnitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MemberUnits
        public async Task<IActionResult> Index()
        {
            return View(await _context.MemberUnits.ToListAsync());
        }

        // GET: MemberUnits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberUnit = await _context.MemberUnits
                .FirstOrDefaultAsync(m => m.MemberUnitId == id);
            if (memberUnit == null)
            {
                return NotFound();
            }

            return View(memberUnit);
        }

        // GET: MemberUnits/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MemberUnits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberUnitId,Name,Address,PhoneNumber,WebsiteUrl")] MemberUnit memberUnit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(memberUnit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(memberUnit);
        }

        // GET: MemberUnits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberUnit = await _context.MemberUnits.FindAsync(id);
            if (memberUnit == null)
            {
                return NotFound();
            }
            return View(memberUnit);
        }

        // POST: MemberUnits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberUnitId,Name,Address,PhoneNumber,WebsiteUrl")] MemberUnit memberUnit)
        {
            if (id != memberUnit.MemberUnitId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(memberUnit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberUnitExists(memberUnit.MemberUnitId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(memberUnit);
        }

        // GET: MemberUnits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberUnit = await _context.MemberUnits
                .FirstOrDefaultAsync(m => m.MemberUnitId == id);
            if (memberUnit == null)
            {
                return NotFound();
            }

            return View(memberUnit);
        }

        // POST: MemberUnits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var memberUnit = await _context.MemberUnits.FindAsync(id);
            if (memberUnit != null)
            {
                _context.MemberUnits.Remove(memberUnit);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberUnitExists(int id)
        {
            return _context.MemberUnits.Any(e => e.MemberUnitId == id);
        }
    }
}
