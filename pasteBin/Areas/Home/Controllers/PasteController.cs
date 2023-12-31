﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pasteBin.Database;
using pasteBin.Areas.Home.Models;
using pasteBin.Areas.Home.ViewModels;
using pasteBin.Services.Interfaces;

namespace pasteBin.Areas.Home.Controllers
{
    [Area("Home")]
    public class PasteController : Controller
    {
        private readonly DBContext dataBase;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IRedisCache redis;

        public PasteController(DBContext context, SignInManager<IdentityUser> sign, 
            UserManager<IdentityUser> user, IRedisCache cacheDistributed)
        {
            dataBase = context;
            signInManager = sign;
            userManager = user;
            redis = cacheDistributed;
        }

        [Route("Paste/{hash}")]
        public async Task<IActionResult> Paste(string hash)
        {
            PasteModel? paste = redis.Get(hash);
            string? cacheStatus = paste == null ? null : "cache";

            if (paste == null)
            {
                paste = await dataBase.pasts.Include(a => a.Author).FirstOrDefaultAsync(p => p.Hash == hash);

                if (paste == null)
                    return NotFound();
            }

            IEnumerable<CommentModel> comments = await dataBase.comments.Include(a => a.Author).Where(c => c.Paste.Equals(paste)).ToListAsync();
            IEnumerable<LikesModel> likes = await dataBase.likes.Include(p => p.Paste).Include(u => u.User).Where(p => p.Paste == paste).ToListAsync();

            PasteViewModel viewModel = new (paste, comments, likes);

            await AddViews(paste, cacheStatus);

            return View(viewModel);
        }

        [Route("Paste/Popular")]
        public IActionResult Popular()
        {
            IEnumerable<PasteModel> pasts = redis.GetAll().OrderByDescending(p => p.View);

            ViewBag.Host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            return View(pasts);
        }

        public async Task<IActionResult> AddComment(string com, string hash)
        {
            if (com == null)
            {
                return RedirectToAction("Paste", new { hash });
            }

            IdentityUser user = await userManager.GetUserAsync(HttpContext.User);
            PasteModel paste = await dataBase.pasts.FirstAsync(p => p.Hash == hash);

            CommentModel comment = new()
            {
                Author = user,
                Paste = paste,
                Comment = com
            };

            dataBase.comments.Add(comment);
            await dataBase.SaveChangesAsync();

            return RedirectToAction("Paste", new { hash });
        }

        public async Task<IActionResult> AddLike(string hash)
        {
            IdentityUser user = await userManager.GetUserAsync(HttpContext.User);
            PasteModel paste = await dataBase.pasts.FirstAsync(p => p.Hash == hash);

            LikesModel like = new()
            {
                Paste = paste,
                User = user
            };

            dataBase.likes.Add(like);
            await dataBase.SaveChangesAsync();

            return RedirectToAction("Paste", new { hash });
        }

        public async Task<IActionResult> RemoveLike(string hash)
        {
            IdentityUser user = await userManager.GetUserAsync(HttpContext.User);
            PasteModel paste = await dataBase.pasts.FirstAsync(p => p.Hash == hash);
            LikesModel like = await dataBase.likes.FirstAsync(l => l.User == user && l.Paste == paste);

            dataBase.Entry(like).State = EntityState.Deleted;
            dataBase.SaveChanges();

            return RedirectToAction("Paste", new { hash });
        }

        public async Task<IActionResult> SendReport(string reportText, string hash)
        {
            if (reportText == null)
                return RedirectToAction("Paste", new { hash });

            PasteModel paste = await dataBase.pasts.Include(a => a.Author).FirstAsync(p => p.Hash == hash);

            ReportModel report = new()
            {
                ReportText = reportText,
                User = paste.Author,
                Paste = paste
            };

            dataBase.reports.Add(report);
            await dataBase.SaveChangesAsync();

            return RedirectToAction("Paste", new { hash });
        }

        // кеш-статус нужен для определения где обнавлять данные, в кеше или базе данных
        private async Task AddViews(PasteModel paste, string cacheStatus)
        {
            IdentityUser user = await userManager.GetUserAsync(HttpContext.User);

            if (signInManager.IsSignedIn(User))
            {
                IEnumerable<ViewCheatModel> cheat = await dataBase.viewCheats.Include(u => u.User).
                    Where(c => c.Paste == paste && c.User == user).ToListAsync();

                if (!cheat.Any())
                {
                    paste.View++;

                    ViewCheatModel viewCheat = new()
                    {
                        Paste = paste,
                        User = user
                    };

                    dataBase.Entry(viewCheat).State = EntityState.Added;
                    await dataBase.SaveChangesAsync();

                    if (cacheStatus != null)
                    {
                        redis.Update(paste);
                    }
                }
            }
        }
    }
}
