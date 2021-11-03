using DemoIdentity.Data;
using DemoIdentity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoIdentity.Controllers
{
    [Route("/cart")]
    public class CartController : Controller
    {

        // Session sẽ quản lý dữ liệu dưới dạng Hash table => định nghĩa Key cho Cart trong session
        public const string CARTKEY = "cart";

        private readonly AppDbContext _context;
        private readonly ILogger<CartController> _logger;

        // Dùng DI để đưa context và logger vào
        public CartController(AppDbContext context, ILogger<CartController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // một số hàm hổ trợ thao tác với session
        // Lấy cart từ session
        List<CartItem> GetCartItems()
        {
            // lấy đối tượng session từ context
            var session = HttpContext.Session;
            // đọc chuỗi cart json từ session dựa vào key CARTKEY
            string cartJson = session.GetString(CARTKEY);
            if (cartJson == null)
            {
                return new List<CartItem>();
            }
            // chuyển chuỗi json thành List<CartItem>
            return JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        }

        // xóa cart khỏi session
        void ClearCart()
        {
            // lấy đối tượng session từ context
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        // lưu cart vào session
        void SaveCartSession(List<CartItem> ls)
        {
            // lấy đối tượng session từ context
            var session = HttpContext.Session;
            string json = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, json);
        }

        // end - một số hàm hổ trợ

        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        [Route("/addcart/{productid:int}", Name = "addcart")]
        public async Task<IActionResult> AddCart([FromRoute] int productid)
        {
            // tìm thông tin của product trong db
            var product = await _context.Products.SingleOrDefaultAsync(p => p.Id == productid);

            if (product == null)
            {
                return NotFound("Can not found Product");
            }

            // tìm thấy sản phẩm => đưa vào giỏ hàng
            var cart = GetCartItems();
            var item = cart.Find(p => p.Product.Id == productid);
            if (item == null)
            {
                // trường hợp sản phẩm chưa có trong giỏ hàng
                cart.Add(new CartItem { Product = product, Quantity = 1 });
            }
            else
            {
                item.Quantity++;
            }
            // lưu giỏ hàng (cart) vào lại session
            SaveCartSession(cart);

            // chuyển đến trang hiển thị giỏ hàng
            return RedirectToAction(nameof(ViewCart));
        }

        [Route("/viewcart", Name = "viewcart")]
        public IActionResult ViewCart()
        {
            return View(GetCartItems());
        }

        [Route("/removecart/{productid:int}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute] int productid)
        {
            // lấy cart từ Session
            var cart = GetCartItems();
            // tìm CartItem từ cart (giỏ hàng)
            var item = cart.Find(p => p.Product.Id == productid);
            if (item != null)
            {
                cart.Remove(item);
            }
            // lưu lại cart vào session sau khi xử lý xong
            SaveCartSession(cart);

            // chuyển đến trang hiển thị giỏ hàng
            return RedirectToAction(nameof(ViewCart));
        }

        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        // attribute FromForm: dữ liệu này được truyền lên từ Form
        public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            // lấy cart từ Session
            var cart = GetCartItems();
            // tìm CartItem từ cart (giỏ hàng)
            var item = cart.Find(p => p.Product.Id == productid);
            if (item != null)
            {
                item.Quantity = quantity;
            }
            // lưu lại cart vào session sau khi xử lý xong
            SaveCartSession(cart);

            // chuyển đến trang hiển thị giỏ hàng
            return RedirectToAction(nameof(ViewCart));
        }
    }
}
