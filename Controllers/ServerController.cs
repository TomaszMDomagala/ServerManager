using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServManager.Data;
using ServManager.Models;

namespace ServManager.Controllers
{
    public class ServerController : Controller
    {
        private readonly ServerContext _context;

        public ServerController(ServerContext context)
        {
            _context = context;
        }

        // GET: Server
        public async Task<IActionResult> Index()
        {
              return _context.Server != null ? 
                          View(await _context.Server.ToListAsync()) :
                          Problem("Entity set 'ServerContext.Server'  is null.");
        }

        // GET: Server/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Server == null)
            {
                return NotFound();
            }

            var server = await _context.Server
                .FirstOrDefaultAsync(m => m.ID == id);
            if (server == null)
            {
                return NotFound();
            }

            return View(server);
        }

        // GET: Server/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Server/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Address,Username,Password,Available,Apps")] Server server)
        {
            if (ModelState.IsValid)
            {
                _context.Add(server);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(server);
        }

        // GET: Server/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Server == null)
            {
                return NotFound();
            }

            var server = await _context.Server.FindAsync(id);
            if (server == null)
            {
                return NotFound();
            }
            return View(server);
        }

        // POST: Server/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Address,Username,Password,Available")] Server server)
        {
            if (id != server.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(server);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServerExists(server.ID))
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
            return View(server);
        }

        // GET: Server/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Server == null)
            {
                return NotFound();
            }

            var server = await _context.Server
                .FirstOrDefaultAsync(m => m.ID == id);
            if (server == null)
            {
                return NotFound();
            }

            return View(server);
        }

        // POST: Server/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Server == null)
            {
                return Problem("Entity set 'ServerContext.Server'  is null.");
            }
            var server = await _context.Server.FindAsync(id);
            if (server != null)
            {
                _context.Server.Remove(server);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServerExists(int id)
        {
          return (_context.Server?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
