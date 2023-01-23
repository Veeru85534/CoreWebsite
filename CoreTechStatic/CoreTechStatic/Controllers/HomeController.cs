using CoreTechStatic.Models;
using CoreTechStatic.Models.Banner;
using CoreTechStatic.Models.DateBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization.Metadata;

namespace CoreTechStatic.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataConn db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DataConn db)
        {
            _logger = logger;
            this.db = db;
        }
        public Object CategoryListDropDown()
        {
            return (db.Category.Select(x => new
            {
                ID = x.Ca_ID,
                Name = x.Ca_Name

            }).ToList());
        }
        public Object SocialMediaLink()
        {
            return (db.SocialMedia.OrderBy(link => link.Id).Select(x => new
            {
                SmName = x.SMName,
                Link = x.Link

            }).ToList());
        }
        public Object ProductListDropDown(int id)
        {
            if (id != 0)
            {
                return (db.Product.Where(pid => pid.Ca_Id == id).Select(x => new
                {
                    ID = x.P_Id,
                    Name = x.P_Name

                }).ToList());
            }
            else
            {
                return (db.Product.Select(x => new
                {
                    ID = x.P_Id,
                    Name = x.P_Name

                }).ToList());
            }
        }
        public List<Products> GetProducts()
        {
            List<Products> products = db.Product
                .Include(HsnMaster => HsnMaster.HsnMaster)
                .Include(PR => PR.PriceMaster)
                .Include(ca => ca.Category)
                .Where(AC => AC.Active_Id == 1)
                .OrderBy(Ac => Ac.P_Name)
                .Take(10)
                .ToList();
            return products;
        }
        public List<BannerPage> GetBanner()
        {
            return db.BannerPage.Where(img => img.BannerImage != null).OrderBy(img => img.Slide).ToList();
        }
        public List<Category> GetCategories()
        {
            return db.Category.OrderBy(ca => ca.Ca_Name)
                .Take(12)
                .ToList();
        }
        public IActionResult Index()
        {
            try
            {
                dynamic MyModel = new ExpandoObject();
                MyModel.Products = GetProducts();
                MyModel.Categories = GetCategories();
                MyModel.Banner = GetBanner();
                return View(MyModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException.Message;
                return View();
            }
        }
        public IActionResult AboutUs()
        {
            try
            {
                var data = db.AboutUs.ToList();
                return View(data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException.Message;
                return View();
            }
            
        }
        [Route("CategoryList")]
        public IActionResult CatogeryList()
        {
            try
            {
                var clist = db.Category.ToList().OrderBy(c => c.Ca_Name);
                return View(clist);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException.Message;
                return RedirectToAction("Index");
            }
        }
        [Route("ProductList/{id}")]
        public IActionResult ProductList(int id)
        {
            if (id != 0)
            {
                var list = db.Product.Include(ca => ca.Category)
                                     .Include(sp => sp.Specification)
                                      .Include(PRT => PRT.PriceMaster)
                                     .Include(HsnMaster => HsnMaster.HsnMaster)
                                     .Include(ac => ac.Active)
                                     .Include(av => av.Avalbility)
                                     .OrderBy(name => name.P_Name)
                                     .Where(ca => ca.Ca_Id == id & ca.Active_Id == 1).ToList();
                return View(list);
            }
            else
            {
                var list = db.Product.Include(ca => ca.Category)
                    .Include(PRT => PRT.PriceMaster)
                                        .Include(HsnMaster => HsnMaster.HsnMaster)
                                         .Include(PRT => PRT.PriceMaster)
                                         .Include(sp => sp.Specification)
                                         .Include(ac => ac.Active)
                                         .Include(av => av.Avalbility).ToList().OrderBy(name => name.P_Name).Where(AC => AC.Active_Id == 1);
                return View(list);
            }
        }
        [Route("ProductList")]
        [HttpPost]
        public IActionResult ProductList(string productname, int category)
        {
            var list = db.Product.Include(ca => ca.Category)
                                             .Include(PRT => PRT.PriceMaster)
                                         .Include(sp => sp.Specification)
                                            .Include(HsnMaster => HsnMaster.HsnMaster)
                                         .Include(ac => ac.Active)
                                         .Include(av => av.Avalbility).OrderByDescending(s => s.P_Name == productname).Where(s => s.P_Name.Contains(productname) || s.Ca_Id == category);
            return View(list);
        }
        public List<Products> GetProductDetails(int? id)
        {
            List<Products> products = db.Product
                                         .Include(ca => ca.Category)
                                         .Include(sp => sp.Specification)
                                         .Include(ac => ac.Active)
                                         .Include(PRT => PRT.PriceMaster)
                                         .Include(av => av.Avalbility)
                                         .Include(HSNM => HSNM.HsnMaster)
                                         .Where(m => m.P_Id == id)
                                         .ToList();
            return products;
        }
        public List<Products> GetProductDetailsRemaing(int? id, int? caid)
        {
            List<Products> products2 = db.Product
                .Include(ca => ca.Category)
                .Include(PRT => PRT.PriceMaster)
                                         .Include(sp => sp.Specification)
                                         .Include(PRT => PRT.PriceMaster)
                                         .Include(ac => ac.Active)
                                         .Include(av => av.Avalbility)
                                         .Include(HSNM => HSNM.HsnMaster)
                                         .Where(m => m.P_Id != id & m.Ca_Id == caid)
                                         .ToList();
            return products2;
        }
        public List<Category> GetCategoryDetails(int? caid)
        {
            List<Category> Category = db.Category.Where(m => m.Ca_ID == caid)
                                         .ToList();
            return Category;
        }
        [Route("ProductDetails/{id}/{category}")]
        // GET: ProductList/Details/5
        public async Task<IActionResult> ProductDetails(int? id, int? category)
        {
            if (id == null || db.Product == null)
            {
                return NotFound();
            }
            dynamic MyModel = new ExpandoObject();
            MyModel.Products = GetProductDetails(id);
            MyModel.Products2 = GetProductDetailsRemaing(id, category);
            MyModel.Category = GetCategoryDetails(category);
            return View(MyModel);
        }
        [Route("WishList")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult WishList()
        {
            int userId = int.Parse(User.Identity.Name);
            var data = db.WishList
                .Include(PT => PT.Products)
                .Where(UID => UID.UserId == userId).ToList();
            return View(data);
        }
        public JsonResult AddWishList(int ProductId)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {

                    int userId = int.Parse(User.Identity.Name);
                    var VerfiyaExists = db.WishList.Where(VF => VF.UserId == userId && VF.ProductId == ProductId);
                    if (VerfiyaExists.Any())
                    {
                        return Json(new { code = 2, message = "Alerdy Added" });

                    }
                    db.Add(new WishList
                    {
                        ProductId = ProductId,
                        UserId = userId,
                        AddedDate = DateTime.Now.Date,

                    });
                    db.SaveChanges();
                    return Json(new { code = 1, message = "Added to WishList" });
                }
                catch (Exception ex)
                {

                    return Json(new { code = -1, message = ex.InnerException.Message });
                }
            }
            else
            {
                return Json(new { code = 0, message = "Please Signin to Add Iteam To Cart" });
            }
        }
        public JsonResult RemoveWishList(int? Id)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {

                    if (Id == null)
                    {
                        return Json(new { code = 2, message = "Invalid" });
                    }

                    var data = db.WishList.Find(Id);
                    if (data != null)
                    {
                        db.WishList.Remove(data);
                        db.SaveChanges();
                        return Json(new { code = 1, message = "Removed successfully" });
                    }
                    return Json(new { code = 2, message = "Invalid" });
                }
                catch (Exception ex)
                {
                    return Json(new { code = -1, message = ex.InnerException.Message });
                }
            }
            else
            {
                return Json(new { code = 0, message = "Please Signin to Add Iteam To Cart" });
            }
        }
        public JsonResult AddCart(int Product_Id, int Quntity, int PriceId, int subid)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    int userId = int.Parse(User.Identity.Name);
                    var verifya = db.Cart.Where(pid => pid.Product_Id == Product_Id & pid.OrderStatus != 2 & pid.UserId == userId);
                    var pname = db.Product.Where(pid => pid.P_Id == Product_Id)
                        .Include(pr => pr.PriceMaster)
                        .FirstOrDefault();
                    if (verifya.Any())
                    {
                        //"Alerdy exists"
                        return Json(new { code = 2, message = "Alerdy added to cart" });
                    }
                    else
                    {
                        var SUbId = db.Cart.Where(cid => cid.OrderStatus != 2 & cid.UserId == userId).FirstOrDefault();
                        int SubID = 0;
                        if (SUbId == null)
                        {
                            var SUbId1 = db.Cart.Where(cid => cid.OrderStatus == 2 & cid.UserId == userId).OrderByDescending(sid => sid.SubItemId).FirstOrDefault();
                            if (SUbId1 == null)
                            {
                                subid = 1;
                            }
                            else
                            {
                                subid = SUbId1.SubItemId + 1;
                            }
                        }
                        else
                        {
                            subid = SUbId.SubItemId;

                        }

                        db.Add(new Cart
                        {
                            Product_Id = Product_Id,
                            SubItemId = subid,
                            Quntity = Quntity,
                            AddedDate = DateTime.Now,
                            PriceId = PriceId,
                            OrderStatus = 1,
                            UserId = userId
                        });
                        db.SaveChanges();
                        return Json(new { code = 1, message = $"Product Added " + pname.P_Name + "" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { code = 2, message = ex.InnerException.Message });
                }
            }
            else
            {
                return Json(new { code = 0, message = "Please Signin to Add Iteam To Cart" });
            }
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult CartItem()
        {
            int userId = int.Parse(User.Identity.Name);
            var Item = db.Cart.Include(pa => pa.Products)
                .Include(HSNM => HSNM.Products.HsnMaster)
                .Include(pa => pa.PriceMaster)
                .Include(us => us.User)
                .ToList().OrderBy(pn => pn.Products.P_Name).Where(os => os.OrderStatus != 2 & os.UserId == userId);
            return View(Item);
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult CartItemDelete(int id)
        {
            Cart cart = db.Cart.Find(id);
            db.Cart.Remove(cart);
            db.SaveChanges();
            return RedirectToAction("CartItem");
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult OrderList()
        {
            int userId = int.Parse(User.Identity.Name);
            var OredrList = db.Order.ToList().OrderByDescending(ID=>ID.OrderId).Where(uid => uid.CustomerId == userId);
            return View(OredrList);
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult OrderDetails(int Cart)
        {
            int userId = int.Parse(User.Identity.Name);
            dynamic OrderDetails = new ExpandoObject();
            OrderDetails.Cart = db.Cart.Where(id => id.UserId == userId & id.SubItemId == Cart)
                       .Include(p => p.Products)
                       .ToList();
            OrderDetails.Order = db.Order.Where(id => id.CustomerId == userId & id.CartSubID == Cart).ToList();
            return View(OrderDetails);            
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize(Roles = "User,Admin")]
        public JsonResult Values(Sample Input)
        {
            if (Input.SampleData != null & Input.Address != null & Input.PhoneNumber != null & Input.PinCode != null)
            {
                try
                {
                    int userId = int.Parse(User.Identity.Name);
                    foreach (var item in Input.SampleData)
                    {
                        var ChartModel = db.Cart.Single(cid => cid.Cart_Id == item.Cart_Id);
                        ChartModel.Cart_Id = item.Cart_Id;
                        ChartModel.SubTotalPrice = item.Price;
                        ChartModel.Gst = item.Gst;
                        ChartModel.UnitPrice = item.UnitPrice;
                        ChartModel.OrderStatus = 2;
                        ChartModel.CheckOutDate = DateTime.Now.Date;
                        db.SaveChanges();
                    }
                    db.Add(new Order
                    {
                        OrderStatus = 1,
                        Address = Input.Address,
                        PinCode = Input.PinCode,
                        PhoneNumber = Input.PhoneNumber,
                        OrderDate = DateTime.Now.Date,
                        TotalPrice = Input.TotalAmount,
                        CustomerId = userId,
                        CartSubID = Input.SubItemId,
                        NoOfItem = Input.TotalItem
                    });
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    return Json(new { code = 1, message = ex.InnerException.Message });
                }


                return Json(new { code = 0, message = "Order Placed" });
            }
            else
            {
                return Json(new { code = 1, message = "Iteam is Empty Or Add Shipping details" });
            }
        }
        public IActionResult B2B()
        {
            return View();
        }
        public IActionResult Invoice(int Uid, int Cart)
        {
            var data = db.Cart
                .Include(User => User.User)
                .Include(User => User.User.CityMaster)
                .Include(User => User.User.StateMaster)
                .Include(Product => Product.Products)
                .Include(Product => Product.Products.HsnMaster)
                .Include(Product => Product.Products.PriceMaster)
                .Where(ID => ID.UserId == Uid && ID.SubItemId == Cart).ToList();            
            return View(data);
        }
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}