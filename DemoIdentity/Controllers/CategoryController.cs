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
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            //var appDbContext = _context.Categories.Include(c => c.ParentCategory);
            //return View(await appDbContext.ToListAsync());
            var list = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.CategoryChildren)
                .ToListAsync();
            var cates = list.Where(c => c.ParentCategory == null);
            return View(cates);
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
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
            var cates = await _context.Categories
                .Include(c => c.CategoryChildren)
                .Where(c => c.ParentCategory == null)
                .ToListAsync();

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

        // GET: Category/Create
        public async Task<IActionResult> Create()
        {
            /*
            var cates = await _context.Categories.ToListAsync();
            // thêm mục No Parent
            // Hàm Add thêm cuối
            // Hàm Insert thêm tại vị trí chỉ định
            cates.Insert(0, new Category
            {
                Id = -1,
                Title = "No Parent"
            });
            */
            ViewData["ParentId"] = new SelectList(await BuildHierarchyCategory(), "Id", "Title");
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParentId,Title,Slug,Content")] Category category)
        {
            if (ModelState.IsValid)
            {
                // nếu người dùng chọn No Parent, gán ParentId = null;
                if (category.ParentId.Value == -1)
                {
                    category.ParentId = null;
                }
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Title", category.ParentId);
            return View(category);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Title", category.ParentId);
            return View(category);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ParentId,Title,Slug,Content")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Title", category.ParentId);
            return View(category);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
