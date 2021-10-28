using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoIdentity.Areas.Identity.Pages.Role
{
    public class AddModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AddModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [TempData] // Sử dụng Session
        public string StatusMessage { get; set; }

        public class InputModel
        {
            public string ID { set; get; }

            [Required]
            [Display(Name = "Role name")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
            public string Name { set; get; }

        }

        [BindProperty]
        public InputModel Input { set; get; }

        [BindProperty]
        public bool IsUpdate { set; get; }

        // Không cho truy cập trang mặc định mà không có handler
        public IActionResult OnGet() => NotFound("Not found.");
        public IActionResult OnPost() => NotFound("Not found.");


        public IActionResult OnPostStartNewRole()
        {
            StatusMessage = "Please input data to create new role.";
            IsUpdate = false;
            ModelState.Clear();
            return Page();
        }

        // Truy vấn lấy thông tin Role cần cập nhật
        public async Task<IActionResult> OnPostStartUpdate()
        {
            StatusMessage = null;
            IsUpdate = true;
            if (Input.ID == null)
            {
                StatusMessage = "Error: No data for Role";
                return Page();
            }
            var result = await _roleManager.FindByIdAsync(Input.ID);
            if (result != null)
            {
                Input.Name = result.Name;
                ViewData["Title"] = "Update role : " + Input.Name;
                ModelState.Clear();
            }
            else
            {
                StatusMessage = "Error: No data for Role ID = " + Input.ID;
            }

            return Page();
        }

        // Cập nhật hoặc thêm mới tùy thuộc vào IsUpdate
        public async Task<IActionResult> OnPostAddOrUpdate()
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = null;
                return Page();
            }

            if (IsUpdate)
            {
                // CẬP NHẬT
                if (Input.ID == null)
                {
                    ModelState.Clear();
                    StatusMessage = "Error: No data for role";
                    return Page();
                }
                var result = await _roleManager.FindByIdAsync(Input.ID);
                if (result != null)
                {
                    result.Name = Input.Name;
                    // Cập nhật tên Role
                    var roleUpdateRs = await _roleManager.UpdateAsync(result);
                    if (roleUpdateRs.Succeeded)
                    {
                        StatusMessage = "Update role successfully";
                    }
                    else
                    {
                        StatusMessage = "Error: ";
                        foreach (var er in roleUpdateRs.Errors)
                        {
                            StatusMessage += er.Description;
                        }
                    }
                }
                else
                {
                    StatusMessage = "Error: No data for updated Role";
                }
            }
            else
            {
                // TẠO MỚI
                var newRole = new IdentityRole(Input.Name);
                // Thực hiện tạo Role mới
                var rsNewRole = await _roleManager.CreateAsync(newRole);
                if (rsNewRole.Succeeded)
                {
                    StatusMessage = $"Created new role successfully: {newRole.Name}";
                    return RedirectToPage("./Index");
                }
                else
                {
                    StatusMessage = "Error: ";
                    foreach (var er in rsNewRole.Errors)
                    {
                        StatusMessage += er.Description;
                    }
                }
            }

            return Page();
        }
    }
}
