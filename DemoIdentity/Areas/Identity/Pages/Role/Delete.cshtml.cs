﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DemoIdentity.Areas.Identity.Pages.Role
{
    public class DeleteModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public DeleteModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public class InputModel
        {
            [Required]
            public string ID { set; get; }
            public string Name { set; get; }

        }

        [BindProperty]
        public InputModel Input { set; get; }

        [BindProperty]
        public bool isConfirmed { set; get; }

        [TempData] // Sử dụng Session
        public string StatusMessage { get; set; }

        public IActionResult OnGet() => NotFound("Not Found");

        public async Task<IActionResult> OnPost()
        {

            if (!ModelState.IsValid)
            {
                return NotFound("Can not delete");
            }

            var role = await _roleManager.FindByIdAsync(Input.ID);
            if (role == null)
            {
                return NotFound("No role to delete");
            }

            ModelState.Clear();

            if (isConfirmed)
            {
                //Xóa
                await _roleManager.DeleteAsync(role);
                StatusMessage = "deleted " + role.Name;

                return RedirectToPage("Index");
            }
            else
            {
                Input.Name = role.Name;
                isConfirmed = true;

            }

            return Page();
        }
    }
}
