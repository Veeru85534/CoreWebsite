using CoreTechStatic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CoreTechStatic.Models.DateBase;
using System.Dynamic;
using CoreTechStatic.Models.Banner;
using CoreTechStatic.Models.Static;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace CoreTechStatic.Controllers
{

    public class AdminController : Controller
    {
        private readonly DataConn db;
        public AdminController(DataConn db)
        {
            this.db = db;
        }
        public Object HsnMasterDropDown()
        {
            return (db.HsnMaster.Select(x => new
            {
                ID = x.HsnCode,
                Name = x.HsnCode

            }).ToList().OrderBy(sn => sn.Name));
        }
        public Object CategoryDropDown()
        {
            return (db.Category.Select(x => new
            {
                ID = x.Ca_ID,
                Name = x.Ca_Name

            }).ToList().OrderBy(sn => sn.Name));
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index(string Eidtor)
        {
            TempData["View"] = Eidtor;
            return View();
        }
        public IActionResult Invoice(int Uid,int Cart)
        {
            var data = db.Cart
                .Include(User =>User.User)
                .Include(User =>User.User.CityMaster)                
                .Include(User =>User.User.StateMaster)
                .Include(Product => Product.Products)
                .Include(Product => Product.Products.HsnMaster)
                .Include(Product => Product.Products.PriceMaster)
                .Where(ID => ID.UserId == Uid && ID.SubItemId == Cart).ToList();
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult SocialMedia(int? Id, string LinkStatus, string? Link)
        {
            if (Id == null)
            {
                TempData["Error"] = "In-valid";
                return RedirectToAction("Banner");
            }
            try
            {
                if (Link == "RemoveLink")
                {
                    var RemoveLink = db.SocialMedia.Single(id => id.Id == Id);
                    RemoveLink.Link = "#";
                    RemoveLink.UpdatedDate = DateTime.Now.Date;
                    db.SaveChanges();
                    TempData["Error"] = "link Removed.";
                    return RedirectToAction("Banner");
                }
                if (LinkStatus == "#")
                {
                    var RemoveLink = db.SocialMedia.Single(id => id.Id == Id);
                    RemoveLink.Link = Link;
                    RemoveLink.UpdatedDate = DateTime.Now.Date;
                    db.SaveChanges();
                    TempData["Message"] = "Link Updated.";
                    return RedirectToAction("Banner");
                }

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException.Message;
            }
            return View();
        }
        public List<BannerPage> GetBannerPages()
        {
            return db.BannerPage.ToList();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AboutUsPage()
        {
            try
            {
                var data = db.AboutUs.FirstOrDefault();
                return View(data);
            }
            catch (Exception ex)
            {

            }
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AboutUsPage(AboutUs Model, IFormFile file)
        {
            try
            {
                var Verifya = db.AboutUs.Where(img => img.ID == 1).FirstOrDefault();
                if (Model.Image != null && file != null)
                {
                    var ImagePathToDelete = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images\AboutUs", Verifya.Image); ;
                    System.IO.File.Delete(ImagePathToDelete);
                }
                if (Model != null)
                {
                    string FileName = "AboutUs" + Model.ID + ".jpg";
                    string filepath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images\AboutUs", FileName);
                    using (var filestram = new FileStream(filepath, FileMode.Create))
                    {
                        file.CopyTo(filestram);
                    }
                    var UpdateAboutUs = db.AboutUs.Single(ID => ID.ID == 1);
                    UpdateAboutUs.Title = Model.Title;
                    UpdateAboutUs.Description = Model.Description;
                    UpdateAboutUs.Image = FileName;
                    UpdateAboutUs.LastUpdate = DateTime.Now.Date;
                    db.SaveChanges();
                    TempData["Message"] = "Updated About Us.";
                    return RedirectToAction("AboutUsPage");
                }

            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("AboutUsPage");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Banner()
        {
            dynamic BannerModel = new ExpandoObject();
            BannerModel.Banner = GetBannerPages();
            BannerModel.SocialMedia = db.SocialMedia.ToList();
            //var data = 
            return View(BannerModel);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateSlide(int? UpdateId, string? Slide, string? ImagePath, IFormFile file)
        {
            if (UpdateId == null)
            {
                TempData["Error"] = "In-valid";
            }
            try
            {
                if (Slide == "RemoveSlide" && ImagePath != null)
                {
                    var ImagePathToDelete = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images\Banner", ImagePath); ;
                    var UpdateSlide = db.BannerPage.Single(Id => Id.Slide == UpdateId);
                    UpdateSlide.BannerImage = null;
                    UpdateSlide.AddedDate = null;
                    db.SaveChanges();
                    System.IO.File.Delete(ImagePathToDelete);
                    TempData["Error"] = "Banner Removed.";
                    return RedirectToAction("Banner");
                }
                if (ImagePath == null && file != null)
                {
                    string FileName = "Slide" + UpdateId + ".jpg";
                    string filepath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images\Banner", FileName);
                    using (var filestram = new FileStream(filepath, FileMode.Create))
                    {
                        file.CopyTo(filestram);
                    }
                    var Update = db.BannerPage.Single(Id => Id.Slide == UpdateId);
                    Update.BannerImage = FileName;
                    Update.AddedDate = DateTime.Now.Date;
                    db.SaveChanges();
                    TempData["Message"] = "Banner Updated.";
                    return RedirectToAction("Banner");
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DashBoard()
        {
            ViewBag.TotalProduct = db.Product.ToList().Count();
            ViewBag.OutOfStock = db.Product.Where(OST => OST.Avalbility_Id == 2).ToList().Count();
            ViewBag.Active = db.Product.Where(OST => OST.Active_Id == 1).ToList().Count();
            ViewBag.TotalCatgory = db.Category.ToList().Count();
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult SpecificationList()
        {
            var list = db.Specification.ToList();
            return View(list);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult SpecificationEdit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var editspecfication = db.Specification
                .Where(sid => sid.S_Id == id).FirstOrDefault();
            if (editspecfication == null)
            {
                return NotFound();
            }
            return View(editspecfication);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult SpecificationEdit(int? id, Specification specification)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                db.Update(specification);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return NotFound(ex.InnerException.Message);
            }
            return RedirectToAction("SpecificationList");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult CategoryList()
        {
            var Products = db.Category.ToList().OrderByDescending(c => c.Ca_Name);
            return View(Products);
        }
        public IActionResult AddCategory()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddCategory(Category category, IFormFile file)
        {
            if (file != null)
            {
                try
                {
                    string FileName = "Category" + DateTime.Now.ToString("ssffff") + ".jpg";
                    string filepath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images\Category", FileName);
                    using (var filestram = new FileStream(filepath, FileMode.Create))
                    {
                        file.CopyTo(filestram);
                    }
                    db.Add(new Category
                    {
                        Ca_Name = category.Ca_Name,
                        Ca_Image = FileName,
                        Ca_Description = category.Ca_Description,
                    });
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return View(ex.InnerException.Message);
                }
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AddProduct()
        {
            List<Avalbility> avalbilities = new List<Avalbility>();
            List<Active> actives = new List<Active>();
            actives = (from a in db.Active select a).ToList();
            actives.Insert(0, new Active { Ac_Id = 0, Ac_Name = "- Select -" });
            avalbilities = (from av in db.Avalbility select av).ToList();
            avalbilities.Insert(0, new Avalbility { Av_Id = 0, Av_Name = "- Select -" });
            ViewBag.Avalbility = avalbilities;
            ViewBag.Active = actives;
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddProduct(Products products, Specification specification, IFormFile file, IFormFile file2)
        {
            string numbers = "0123456789";
            Random random = new Random();
            string s_id = string.Empty;
            for (int i = 0; i < 5; i++)
            {
                int tempval = random.Next(0, numbers.Length);
                s_id += tempval;
            }
            int id = int.Parse(s_id);
            if (file != null || file2 != null)
            {
                try
                {
                    string FileName = "Product" + DateTime.Now.ToString("HHmmssffff") + ".jpg";
                    if (file != null)
                    {
                        string filepath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images\Products", FileName);
                        using (var filestram = new FileStream(filepath, FileMode.Create))
                        {
                            file.CopyTo(filestram);
                        }
                    }
                    string FileName2 = null;
                    if (file2 != null)
                    {
                        FileName2 = "Product" + DateTime.Now.ToString("HHmmssffff") + (2) + ".jpg";
                        string filepath2 = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images\Products", FileName2);
                        using (var filestram = new FileStream(filepath2, FileMode.Create))
                        {
                            file2.CopyTo(filestram);
                        }
                    }
                    if (specification.S_Id == 0)
                    {
                        db.Add(new Specification
                        {
                            S_Id = id,
                            Name = products.P_Name,
                            S_type1 = specification.S_type1,
                            S_type2 = specification.S_type2,
                            S_type3 = specification.S_type3,
                            S_type4 = specification.S_type4,
                            S_type5 = specification.S_type5,
                            S_type6 = specification.S_type6,
                            S_type7 = specification.S_type7,
                            S_type8 = specification.S_type8,
                            S_type9 = specification.S_type9,
                            S_type10 = specification.S_type10
                        });
                        db.SaveChanges();
                    }
                    if (specification.S_Id == 0)
                    {
                        db.Add(new Products
                        {
                            P_Name = products.P_Name,
                            P_Image = FileName,
                            P_ImageSec = FileName2,
                            P_AddDate = DateTime.Today.Date,
                            Ca_Id = products.Ca_Id,
                            KeyWords = products.KeyWords,
                            HsnCode = products.HsnCode,
                            S_Id = id,
                            P_Description = products.P_Description,
                            Avalbility_Id = products.Avalbility_Id,
                            Active_Id = products.Active_Id
                        });
                    }
                    else
                    {
                        db.Add(new Products
                        {
                            P_Name = products.P_Name,
                            P_Image = FileName,
                            HsnCode = products.HsnCode,
                            P_ImageSec = FileName2,
                            KeyWords = products.KeyWords,
                            P_AddDate = DateTime.Today.Date,
                            Ca_Id = products.Ca_Id,
                            S_Id = specification.S_Id,
                            P_Description = products.P_Description,
                            Avalbility_Id = products.Avalbility_Id,
                            Active_Id = products.Active_Id
                        });
                    }
                    db.SaveChanges();
                    TempData["Message"] = $"{products.P_Name} added Successfully";
                }
                catch (Exception ex)
                {
                    return View(ex.InnerException.Message);
                }
            }
            return RedirectToAction("AddProduct");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult ProductList()
        {
            try
            {
                var Products = db.Product.Include(ca => ca.Category)
                                         .Include(sp => sp.Specification)
                                         .Include(ac => ac.Active)
                                         .Include(ac => ac.PriceMaster)
                                         .Include(av => av.Avalbility)
                                         .OrderBy(av => av.P_Name)
                                         .ToList();
                return View(Products);
            }
            catch (Exception ex)
            {

                return View(ex.InnerException.Message);
            }
        }
        [Route("PriceList/{PriceId}")]
        public IActionResult PriceList(int PriceId)
        {
            if (PriceId == 0)
            {
                var data = db.PriceMaster
                .Include(PT => PT.Products)
                .OrderBy(av => av.Products.P_Name)
                .ToList();
                return View(data);
            }
            else
            {

                var data = db.PriceMaster
                    .Where(PID => PID.PriceId == PriceId)
                    .Include(PT => PT.Products)
                .OrderBy(av => av.Products.P_Name)
                    .ToList();
                return View(data);
            }
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AddPrice(int ProductId)
        {
            var Verfiya = db.PriceMaster.Where(PI => PI.ProductId == ProductId);
            if (Verfiya.Any())
            {
                return RedirectToAction("PriceList");
            }
            TempData["ProductId"] = ProductId;
            return View();
        }
        [HttpPost]
        public IActionResult AddPrice(PriceMaster priceMaster)
        {
            if (priceMaster == null)
            {
                return Redirect("/PriceList/0");
            }
            db.Add(new PriceMaster
            {
                ProductId = priceMaster.ProductId,
                DiscountPrice = priceMaster.DiscountPrice,
                FinalPrice = priceMaster.FinalPrice,
                StartDate = DateTime.Now.Date,

            });
            db.SaveChanges();
            var PriceId = db.PriceMaster.Where(pid => pid.ProductId == priceMaster.ProductId).FirstOrDefault();
            var UpdateProduct = db.Product.Single(PID => PID.P_Id == priceMaster.ProductId);
            UpdateProduct.PriceID = PriceId.PriceId;
            db.SaveChanges();
            return Redirect("/PriceList/0");
        }
        [Route("Edit/Price/{PriceId}")]
        public IActionResult EditPrice(int? PriceId)
        {
            if (PriceId == null)
            {
                TempData["Error"] = "Select price to edit";
                return Redirect("/PriceList/0");
            }
            var Data = db.PriceMaster
                .Include(PTB => PTB.Products)
                .Where(PRID => PRID.PriceId == PriceId).FirstOrDefault();
            ViewBag.ReturnUrl = "/PriceList/"+PriceId+"";
            if (Data == null)
            {
                TempData["Error"] = "Select price to edit";
                return Redirect("/PriceList/0");
            }
            return View(Data);
        }
        [Route("Edit/Price/{PriceId}")]
        [HttpPost]
        public IActionResult EditPrice(int? PriceId, PriceMaster model, string ReturnUrl)
        {
            if (PriceId == null)
            {
                TempData["Error"] = "Select price to edit";
                return Redirect(ReturnUrl);
            }
            try
            {
                var Updateprice = db.PriceMaster.Single(ID => ID.PriceId == PriceId);
                Updateprice.DiscountPrice = model.DiscountPrice;
                Updateprice.FinalPrice = model.FinalPrice;
                db.SaveChanges();
                TempData["Message"] = "Price Updated";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException.Message;
                return Redirect(ReturnUrl);
            }
            return Redirect(ReturnUrl);
        }
        [Authorize(Roles = "Admin")]
        [Route("Product/{ProductID}/Edit")]
        public IActionResult EditProduct(int? ProductId)
        {
            if (ProductId == null)
            {
                TempData["Error"] = "Select product to edit";
                return RedirectToAction("ProductList");
            }
            var Data = db.Product.Where(PID => PID.P_Id == ProductId).FirstOrDefault();
            if (Data == null)
            {
                TempData["Error"] = "Select product to edit";
                return RedirectToAction("ProductList");
            }
            else
            {
                List<Avalbility> avalbilities = new List<Avalbility>();
                List<Active> actives = new List<Active>();
                List<Category> categories = new List<Category>();
                categories = (from c in db.Category select c).ToList();
                categories.Insert(0, new Category { Ca_ID = 0, Ca_Name = "- Select Name -" });
                actives = (from a in db.Active select a).ToList();
                actives.Insert(0, new Active { Ac_Id = 0, Ac_Name = "- Select -" });
                avalbilities = (from av in db.Avalbility select av).ToList();
                avalbilities.Insert(0, new Avalbility { Av_Id = 0, Av_Name = "- Select -" });
                ViewBag.Avalbility = avalbilities;
                ViewBag.Categories = categories;
                ViewBag.Active = actives;
                return View(Data);
            }
        }
        [HttpPost]
        [Route("Product/{ProductID}/Edit")]
        public IActionResult EditProduct(int? ProductID, Products products)
        {
            if (ProductID == null)
            {
                TempData["Error"] = "Select product to edit";
                return RedirectToAction("ProductList");
            }
            var UpdateProduct = db.Product.Single(PID => PID.P_Id == ProductID);
            UpdateProduct.P_Name = products.P_Name;
            UpdateProduct.Ca_Id = products.Ca_Id;
            UpdateProduct.KeyWords = products.KeyWords;
            UpdateProduct.Active_Id = products.Active_Id;
            UpdateProduct.Avalbility_Id = products.Avalbility_Id;
            UpdateProduct.P_Description = products.P_Description;
            db.SaveChanges();
            TempData["Message"] = "Product Updated";
            return RedirectToAction("ProductList");
        }
        [Authorize(Roles = "Admin")]
        [Route("Product/{id}/{ActiveId}/{StockId}")]
        public IActionResult ActiveProduct(int id, int ActiveId,string? Message,int StockId)
        {
            try
            {

                var products = db.Product.Find(id);
                var Update = db.Product.Single(pid => pid.P_Id == id);
                if (ActiveId == 1)
                {
                    Update.Active_Id = 2;
                }
                else if (ActiveId == 2)
                {
                    Update.Active_Id = 1;
                }
                else if (StockId == 1)
                {
                    Update.Avalbility_Id = 2;
                    Update.Message = Message;
                }
                else if (StockId == 2)
                {
                    Update.Avalbility_Id = 1;
                    Update.Message = null;
                }
                db.SaveChanges();
                return RedirectToAction("Productlist");
            }
            catch (Exception ex)
            {
                return View(ex.InnerException.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        public IActionResult OrderList(int Order)
        {
            var data = db.Order.ToList().Where(ID =>ID.OrderStatus == Order);
            return View(data);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult OrderIteamList(int customerid, int subcartid)
        {
            try
            {

                var data = db.Cart.Where(id => id.UserId == customerid & id.SubItemId == subcartid)
                        .Include(p => p.Products)
                        .ToList();
                return View(data);
            }
            catch (Exception ex)
            {
                return View(ex.InnerException.Message);

            }
        }
        [Authorize(Roles = "Admin")]
        public IActionResult OrderStatus(int orderstatus, int OrderId)
        {
            try
            {
                var OrderModel = db.Order.Single(oid => oid.OrderId == OrderId);
                OrderModel.OrderId = OrderId;
                if (orderstatus == 1)
                {
                    OrderModel.OrderStatus = 2;
                }
                else if (orderstatus == 2)
                {
                    OrderModel.OrderStatus = 3;
                    OrderModel.PackedDate = DateTime.Now.Date;
                }
                else if (orderstatus == 3)
                {
                    OrderModel.OrderStatus = 4;
                    OrderModel.ShippedDate = DateTime.Now.Date;
                }
                else if (orderstatus == 4)
                {
                    OrderModel.OrderStatus = 5;
                }
                else if (orderstatus == 5)
                {
                    OrderModel.OrderStatus = 6;
                    OrderModel.DeliveryDate = DateTime.Now.Date;
                }
                db.SaveChanges();
                return RedirectToAction("OrderList");

            }
            catch (Exception ex)
            {
                return View(ex.InnerException.Message);
            }

        }
        [Authorize(Roles = "Admin")]
        public IActionResult UserList()
        {
            var data = db.Users
                .Include(ct => ct.CityMaster)
                .Include(st => st.StateMaster)
                .ToList();
            return View(data);
        }
        [HttpPost]
        public IActionResult UserList(string PhoneNumber, string UserName)
        {
            var data = db.Users
                .Where(pn => pn.Phone_No == PhoneNumber || pn.First_Name.Contains(UserName))
                .Include(ct => ct.CityMaster)
                .Include(st => st.StateMaster)
                .ToList();
            return View(data);
        }
    }
}
