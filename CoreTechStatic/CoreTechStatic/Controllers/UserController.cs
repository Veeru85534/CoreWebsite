using CoreTechStatic.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using System.Web;
using CoreTechStatic.Models.DateBase;
using System.Runtime.CompilerServices;

namespace CoreTechStatic.Controllers
{
    public class UserController : Controller
    {
        private readonly DataConn db;
        public UserController(DataConn db)
        {
            this.db = db;
        }
        public Object StateDropDownList()
        {
            return (db.StateMaster.Select(x => new
            {
                ID = x.StateId,
                Name = x.StateName

            }).ToList().OrderBy(sn => sn.Name));
        }
        public Object GetCityDropDownList(int stateid)
        {
            if(stateid == 0)
            {
            return (db.CityMaster.Select(x => new
            {
                ID = x.CityId,
                Name = x.CityName,
                StateId = x.StateId

            }).OrderBy(nm => nm.Name).ToList());
            }
            else
            {
                return (db.CityMaster.Select(x => new
                {
                    ID = x.CityId,
                    Name = x.CityName,
                    StateId = x.StateId

                }).OrderBy(nm => nm.Name).Where(id => id.StateId == stateid).ToList());
            }

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult login(string ReturnUrl)
        {
            if (User.Identity.Name != null)
            {
                var data = db.Users
                    .Include(st => st.StateMaster)
                    .Include(ct => ct.CityMaster)
                    .ToList()
                    .Where(uid => uid.User_Id == int.Parse(User.Identity.Name));
                return View(data);
            }
            if (ReturnUrl == null)
            {
                TempData["ReturnUrl"] = "/User/Login";
            }
            else
            {
                TempData["ReturnUrl"] = ReturnUrl;
            }
            return View();
        }
        [HttpPost]
        public IActionResult Login(string PhoneNumber, string password, string ReturnUrl)
        {
            var loginVerifya = db.Users.Where(nm => nm.Phone_No == PhoneNumber && nm.Password == password);
            if (loginVerifya.Any())
            {
                var ForCredintials = db.Users.Where(nm => nm.Phone_No == PhoneNumber && nm.Password == password).FirstOrDefault();
                var claims = new List<Claim>
                    {
                    new Claim(ClaimTypes.Name, ForCredintials.User_Id.ToString()),
                    new Claim(ClaimTypes.Role, ForCredintials.Role.Trim())
                    };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                          new ClaimsPrincipal(identity),
                          new AuthenticationProperties
                          {
                              AllowRefresh = true,
                              ExpiresUtc = DateTime.UtcNow.AddDays(1),
                              IsPersistent = true
                          });
                //TempData["UserId"] = PhoneNumber;
                TempData["Message"] = $"Welcome back {ForCredintials.First_Name.Trim()}.";
                //return RedirectToAction("DashBoard", "Admin");
                return Redirect(ReturnUrl);
            }
            else
            {
                TempData["Error"] = "Incorrect Name and Password";
            }
            return View();
        }
        [Authorize]
        public IActionResult Logout()
        {
            foreach (string cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Error"] = "Thank you! You are successfully loged out.";
            return RedirectToAction("Login");
        }
        [Route("SignUp")]
        public IActionResult SignUp()
        {
            return View();
        }
        [Route("SignUp")]
        [HttpPost]
        public IActionResult SignUp(User user)
        {
            try
            {
                var VerfiyaExists = db.Users.Where(pn => pn.Phone_No == user.Phone_No).ToList();
                if (VerfiyaExists.Any())
                {
                    TempData["Error"] = "You are already registered. Please log in.";
                    return RedirectToAction("SignUp");
                }
                else
                {
                    var citynmae = db.CityMaster.Where(id => id.CityId == user.City).FirstOrDefault();
                    var statenmae = db.StateMaster.Where(id => id.StateId == user.State).FirstOrDefault();
                    db.Add(new User
                    {
                        User_Name = user.First_Name + " " + user.Last_Name,
                        First_Name = user.First_Name,
                        Last_Name = user.Last_Name,
                        Password = user.Password,
                        Email_Id = user.Email_Id,
                        Role = "User",
                        Phone_No = user.Phone_No,
                        Created_Date = DateTime.Now.Date,
                        Address = user.Address,
                        ShippingAddress = user.Address + " " + citynmae.CityName + "-" + user.Pin_Code + " " + statenmae.StateName,
                        City = user.City,
                        State = user.State,
                        Country = user.Country,
                        Pin_Code = user.Pin_Code
                    });
                    db.SaveChanges();
                    TempData["Message"] = $"Welcome {user.First_Name} for Core Technologies. Please sign in to continue";
                    return RedirectToAction("Login");
                }


            }
            catch (Exception ex)
            {
                return NotFound(ex.InnerException.Message);
                throw;
            }
            return View();
        }
        public IActionResult ForgottenPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgottenPassword(string Email)
        {
            try
            {
                var verifya = db.Users.Where(email => email.Email_Id == Email);
                if (verifya.Any())
                {
                    var sendmail = db.Users.Where(email => email.Email_Id == Email).FirstOrDefault();
                    var UserUpdateModel = db.Users.Single(uid => uid.User_Id == sendmail.User_Id);
                    UserUpdateModel.Otp = 123;
                    db.SaveChanges();
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse("coretech750@gmail.com"));
                    email.To.Add(MailboxAddress.Parse(sendmail.Email_Id.Trim()));
                    email.Subject = "Your Password";
                    var bodyBulider = new BodyBuilder();
                    bodyBulider.HtmlBody = @$"
<table cellspacing=""0"" border=""0"" cellpadding=""0"" width=""100%"" bgcolor=""#f2f3f8""
        <tr>
            <td>
                <table style=""background-color: #f2f3f8; max-width:670px;  margin:0 auto;"" width=""100%"" border=""0""
                       align=""center"" cellpadding=""0"" cellspacing=""0"">
                    <tr>
                        <td style=""height:80px;"">&nbsp;</td>
                    </tr>
                    <tr>
                        <td style=""text-align:center;"">
                            <a href=""https://coretechnolgies.bsite.net/"" title=""logo"" target=""_blank"">
                                <img width=""250"" src=""https://coretechnolgies.bsite.net/Images/AppImages/AppLogo.png"" title=""logo"" alt=""logo"">
                            </a>
                        </td>
                    </tr>
                    <tr>
                        <td style=""height:20px;"">&nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <table width=""95%"" border=""0"" align=""center"" cellpadding=""0"" cellspacing=""0""
                                   style=""max-width:670px;background:#fff; border-radius:3px; text-align:center;-webkit-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);-moz-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);box-shadow:0 6px 18px 0 rgba(0,0,0,.06);"">
                                <tr>
                                    <td style=""height:40px;"">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td style=""padding:0 35px;"">
                                        <h1 style=""color:#1e1e2d; font-weight:500; margin:0;font-size:32px;font-family:'Rubik',sans-serif;"">
                                            You have
                                            requested to reset your password
                                        </h1>
                                        <span style=""display:inline-block; vertical-align:middle; margin:29px 0 26px; border-bottom:1px solid #cecece; width:100px;""></span>
                                        <p style=""color:#455056; font-size:15px;line-height:24px; margin:0;"">
                                            We cannot simply send you your old password. A unique link to reset your
                                            password has been generated for you. To reset your password, click the
                                            following link and follow the instructions.
                                        </p>
                                        <a href=""https://coretechnolgies.bsite.net/ResetPassword/{sendmail.User_Id}/Reset""
                                           style=""background:#4683B2;text-decoration:none !important; font-weight:500; margin-top:35px; color:#fff;text-transform:uppercase; font-size:14px;padding:10px 24px;display:inline-block;border-radius:50px;"">
                                            Reset
                                            Password
                                        </a>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""height:40px;"">&nbsp;</td>
                                </tr>
                            </table>
                        </td>
                    <tr>
                        <td style=""height:20px;"">&nbsp;</td>
                    </tr>
                    <tr>
                        <td style=""text-align:center;"">
                            <p style=""font-size:14px; color:#4683B2; line-height:18px; margin:0 0 0;"">&copy; <strong>www.CoreTechonlogies.com</strong></p>
                        </td>
                    </tr>
                    <tr>
                        <td style=""height:80px;"">&nbsp;</td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
";
                    //email.Body = new TextPart(TextFormat.Plain) {  ContentDescription = "Passwoed recovery message", Text = $"Your Password is :" + sendmail.Password + " " };                    
                    email.Body = bodyBulider.ToMessageBody();



                    // send email
                    using var smtp = new SmtpClient();
                    smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    smtp.Authenticate("coretech750", "bjvgfxarnxjillqh");
                    smtp.Send(email);
                    smtp.Disconnect(true);
                    TempData["Message"] = "Password Reset Link is Sent to your email Id. Please check your Email";
                    return RedirectToAction("ForgottenPassword");
                }
                else
                {
                    TempData["Error"] = "Email ID does't exists Check Email";
                    return RedirectToAction("ForgottenPassword");

                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = (ex.InnerException.Message);
                return View();
            }
        }
        [Route("ResetPassword/{UserId}/Reset")]
        public IActionResult ResetPassword(int? UserId)
        {
                if (UserId == null)
                {
                    return RedirectToAction("UserAccessDenied");
                }
            var verfiya = db.Users.Where(uid => uid.User_Id == UserId && uid.Otp == 123);
            if (verfiya.Any())
            {
                var data = db.Users.Where(uid => uid.User_Id == UserId).FirstOrDefault();
                if (data == null)
                {
                    return RedirectToAction("UserAccessDenied");
                }
                return View(data);
            }
            else
            {
                    return RedirectToAction("SessionExpired");
            }
        }
        [HttpPost]
        [Route("ResetPassword/{UserId}/Reset")]
        public IActionResult ResetPassword(int? UserId, string Password)
        {
            if (UserId == null)
            {
                TempData["Error"] = "Invalid Link. Contact Us";
                return RedirectToAction("Login");
            }
            try
            {
                var UpdatePassword = db.Users.Single(uid => uid.User_Id == UserId);
                UpdatePassword.Otp = 00;
                UpdatePassword.Password = Password;
                db.SaveChanges();
                TempData["Message"] = "Password updated Login";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                return View(ex.InnerException.Message);
            }

        }
        public IActionResult ChangeShippingAddress(User user)
        {
            try
            {
                int userId = int.Parse(User.Identity.Name);
                var citynmae = db.CityMaster.Where(id => id.CityId == user.City).FirstOrDefault();
                var statenmae = db.StateMaster.Where(id => id.StateId == user.State).FirstOrDefault();
                var UpdateModel = db.Users.Single(ID => ID.User_Id == userId);
                UpdateModel.ShippingAddress = user.Address + " " + citynmae.CityName + "-" + user.Pin_Code + " " + statenmae.StateName;
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }
        [HttpPost]
        public IActionResult ChangeShippingAddress(int? Id)
        {
            return View();
        }
        [Route("AccessDenied")]
        public IActionResult UserAccessDenied()
        {
            return View();
        }
        [Route("SessionExpired")]
        public IActionResult SessionExpired()
        {
            return View();
        }

    }
}
