using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TasksManagerWeb.Data;
using TasksManagerWeb.Models;

namespace TasksManagerWeb.Controllers
{
    public class TasksController : Controller
    {
        private readonly TaskManagerDbContext _context;

        public TasksController(TaskManagerDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.Tasks.OrderBy(t => t.CreatedAt).ToListAsync();
            return View(tasks);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description")] TaskItem taskItem)
        {
            if (ModelState.IsValid)
            {
                taskItem.CreatedAt = DateTime.Now;
                taskItem.IsCompleted = false;
                _context.Add(taskItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Task added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(taskItem);
        }

        // POST: Tasks/ToggleComplete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleComplete(int id)
        {
            var taskItem = await _context.Tasks.FindAsync(id);
            if (taskItem != null)
            {
                taskItem.IsCompleted = !taskItem.IsCompleted;
                _context.Update(taskItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = taskItem.IsCompleted ? "Task marked as complete!" : "Task marked as incomplete!";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(taskItem);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskItem = await _context.Tasks.FindAsync(id);
            if (taskItem != null)
            {
                _context.Tasks.Remove(taskItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Task deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}