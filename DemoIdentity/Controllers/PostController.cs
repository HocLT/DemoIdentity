using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoIdentity.Data;
using DemoIdentity.Models;

namespace DemoIdentity.Controllers
{
    public class PostController : Controller
    {
        private readonly AppDbContext _context;

        public PostController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Post
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Posts.Include(p => p.Category);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // change title category 
        // thêm khoảng trắng đầu có các note con
        private void ChangeTitle(List<Category> cates, int level, List<Category> result)
        {
            // chuỗi chứa số dấu "-" theo level 
            string prefix = string.Concat(Enumerable.Repeat("-", level));
            foreach (var item in cates)
            {
                item.Title = $"{prefix} {item.Title}";  // tạo title
                result.Add(item);
                if (item.CategoryChildren != null && item.CategoryChildren.Count > 0)
                {
                    // gọi đệ qui hàm ChangeTitle() để xử lý tiếp cho mục con
                    ChangeTitle(item.CategoryChildren.ToList(), level + 1, result);
                }
            }
        }

        // build category list có phân cấp (hierarchy)
        private async Task<IEnumerable<Category>> BuildHierarchyCategory()
        {
            // load category và category children theo cách eager loading
            var list = await _context.Categories
                .Include(c => c.CategoryChildren)
                .ToListAsync();
            
            var cates = list
                .Where(c => c.ParentCategory == null)
                .ToList();

            // Add No Parent into First Position.
            List<Category> result = new List<Category>();
            result.Add(new Category
            {
                Id = -1,
                Title = "No Parent"
            });

            // xử lý tạo category phân cấp
            int level = 0;
            // gọi hàm thay đổi lại title theo kiểu phân cấp
            ChangeTitle(cates, level, result);

            return result;
        }

        // GET: Post/Create
        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(await BuildHierarchyCategory(), "Id", "Title");
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Slug,Content,CreatedAt,UpdatedAt,CategoryId")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title", post.CategoryId);
            return View(post);
        }

        // GET: Post/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title", post.CategoryId);
            return View(post);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Slug,Content,CreatedAt,UpdatedAt,CategoryId")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title", post.CategoryId);
            return View(post);
        }

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
