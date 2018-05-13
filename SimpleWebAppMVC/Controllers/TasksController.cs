﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleWebAppMVC.Data;

namespace SimpleWebAppMVC.Controllers
{
    /**
     * Tasks Controller
     */
    public class TasksController : Controller
    {
        private readonly AppDbContext dbContext;

        /**
         * TasksController constructor.
         * @param dbCtx Application database context
         */
        public TasksController(AppDbContext dbCtx)
        {
            this.dbContext = dbCtx;
        }

        /**
         * GET: /Tasks/Create
         */
        public IActionResult Create()
        {
            return View(new Models.Task());
        }

        /**
         * POST: /Tasks/Create
         * http://go.microsoft.com/fwlink/?LinkId=317598
         * @param taskModel Task model
         */
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,Description,Date,Status")] Models.Task taskModel)
        {
            if (ModelState.IsValid)
            {
                this.dbContext.Add(taskModel);
                await this.dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(taskModel);
        }

        /**
         * GET: /Tasks/Details/<id>
         * @param id Task ID
         */
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();

            var taskModel = await this.dbContext.Tasks.SingleOrDefaultAsync(task => task.ID == id);

            if (taskModel == null)
                return NotFound();

            return View(taskModel);
        }

        /**
         * GET: /Tasks/Edit/<id>
         * @param id Task ID
         */
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            var taskModel = await this.dbContext.Tasks.SingleOrDefaultAsync(task => task.ID == id);

            if (taskModel == null)
                return NotFound();

            return View(taskModel);
        }

        /**
         * POST: /Tasks/Edit/<id>
         * http://go.microsoft.com/fwlink/?LinkId=317598
         * @param id        Task ID
         * @param taskModel Task model
         */
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ID,Title,Description,Date,Status")] Models.Task taskModel)
        {
            if (id != taskModel.ID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try {
                    this.dbContext.Update(taskModel);
                    await this.dbContext.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!TaskModelExists(taskModel.ID))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(taskModel);
        }

        /**
         * GET: /Tasks/Delete/<id>
         * @param id Task ID
         */
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return NotFound();

            var taskModel = await this.dbContext.Tasks.SingleOrDefaultAsync(task => task.ID == id);

            if (taskModel == null)
                return NotFound();

            return View(taskModel);
        }

        /**
         * POST: /Tasks/Delete/<id>
         * @param id Task ID
         */
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var taskModel = await this.dbContext.Tasks.SingleOrDefaultAsync(task => task.ID == id);

            this.dbContext.Tasks.Remove(taskModel);
            await this.dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /**
         * GET: [ /Tasks/, /Tasks/Index ]
         */
        public async Task<IActionResult> Index(string sort)
        {
            ViewBag.TitleSortParm       = (sort == "Title"       ? "Title_desc"       : "Title");
            ViewBag.DescriptionSortParm = (sort == "Description" ? "Description_desc" : "Description");
            ViewBag.DateSortParm        = (sort == "Date"        ? "Date_desc"        : "Date");
            ViewBag.StatusSortParm      = (sort == "Status"      ? "Status_desc"      : "Status");

            IQueryable<Models.Task> tasks = from task in this.dbContext.Tasks select task;

            switch (sort) {
                case "Title":            tasks = tasks.OrderBy(s => s.Title);                 break;
                case "Title_desc":       tasks = tasks.OrderByDescending(s => s.Title);       break;
                case "Description":      tasks = tasks.OrderBy(s => s.Description);           break;
                case "Description_desc": tasks = tasks.OrderByDescending(s => s.Description); break;
                case "Date":             tasks = tasks.OrderBy(s => s.Date);                  break;
                case "Date_desc":        tasks = tasks.OrderByDescending(s => s.Date);        break;
                case "Status":           tasks = tasks.OrderBy(s => s.Status);                break;
                case "Status_desc":      tasks = tasks.OrderByDescending(s => s.Status);      break;
                default:                 tasks = tasks.OrderBy(s => s.Title);                 break;
            }

            return View(await tasks.ToListAsync());
        }

        /**
         * Returns the specified task if it exists.
         * @param id Task ID
         */
        private bool TaskModelExists(string id)
        {
            return this.dbContext.Tasks.Any(e => e.ID == id);
        }
    }
}
