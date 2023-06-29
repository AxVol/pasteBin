﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pasteBin.Areas.AdminPanel.ViewModels;
using pasteBin.Areas.Home.Models;
using pasteBin.Database;

namespace pasteBin.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class PasteController : Controller
    {
        private readonly DBContext db;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public PasteController(DBContext contex, UserManager<IdentityUser> uManager, RoleManager<IdentityRole> rManager)
        {
            db = contex;
            userManager = uManager;
            roleManager = rManager;
        }

        [Route("Admin/Paste")]
        [HttpGet]
        public IActionResult Paste()
        {
            ViewBag.Host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            return View(db.pasts.Include(a => a.Author));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePaste(int id)
        {
            PasteModel paste = new PasteModel { Id = id };

            IEnumerable<CommentModel> comments = db.comments.Where(c => c.Paste.Id == paste.Id).ToList();
            IEnumerable<LikesModel> likes = db.likes.Where(l => l.Paste.Id == paste.Id).ToList();
            IEnumerable<ReportModel> reports = db.reports.Where(r => r.Paste.Id == paste.Id).ToList();

            await db.UpdateTables(comments, likes, reports, new List<PasteModel> { paste });

            return RedirectToAction("Paste");
        }
    }
}