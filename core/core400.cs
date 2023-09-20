using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CMS.Models;
using CMS.Models.ViewModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace CMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;

        public AdminController(RoleManager<IdentityRole> roleMgr, UserManager<AppUser> userMrg, SignInManager<AppUser> signMgr)
        {
            roleManager = roleMgr;
            userManager = userMrg;
            signInManager = signMgr;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Role()
        {
            ViewBag.Title = "All Roles";
            return View(roleManager.Roles);
        }

        public IActionResult CreateRole()
        {
            ViewBag.Title = "All Roles";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                    return RedirectToAction("Role");
                else
                    AddErrorsFromResult(result);
            }
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("Role");
                else
                    AddErrorsFromResult(result);
            }
            else
                ModelState.AddModelError("", "No role found");
            return View("Role", roleManager.Roles);
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }

        public IActionResult Index(int? id, string searchText, int? status)
        {
            BlogCategoryList list = new BlogCategoryList();
            list = GetBlogCategory(id, searchText, status);
            return View(list);
        }

        public IActionResult Add(int? id)
        {
            ViewBag.CategoryList = id != null ? GetActiveCategory().Where(x => x.Value != id.ToString()) : GetActiveCategory();

            if (id != null)
            {
                var context = new CMSContext();
                var result = context.BlogCategory.Where(x => x.Id == id).FirstOrDefault();
                ViewBag.Title = "Update Blog Category";
                return View(result);
            }

            ViewBag.Title = "Add New Blog Category";
            return View();
        }

        [HttpPost]
        public IActionResult Add(BlogCategory blogCategory, int? id)
        {
            ViewBag.CategoryList = id != null ? GetActiveCategory().Where(x => x.Value != id.ToString()) : GetActiveCategory();

            if (ModelState.IsValid)
            {
                var context = new CMSContext();
                if (id == null)
                {
                    var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@Url",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 100,
                                        Value = CommonFunction.Url(blogCategory.Url)
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Sep",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 1,
                                        Value = "-"
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@TableName",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 25,
                                        Value = DBNull.Value
                                    },
                                     new SqlParameter() {
                                        ParameterName = "@Id",
                                        SqlDbType =  System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = DBNull.Value
                                    },
                                    new SqlParameter()
                                    {
                                        ParameterName = "@TempUrl",
                                        SqlDbType = System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Output,
                                        Size = 100
                                    }};
                    context.Database.ExecuteSqlRaw("[dbo].[sp_GetURL] @Url, @Sep, @TableName, @Id, @TempUrl out", param);

                    blogCategory.ParentId = blogCategory.ParentId == 0 ? null : (int?)blogCategory.ParentId;
                    blogCategory.Url = Convert.ToString(param[4].Value);

                    context.BlogCategory.Add(blogCategory);
                    int result = context.SaveChanges();

                    TempData["result"] = result == 1 ? "Insert Successful" : "Failed";
                    if (result == 1)
                        return RedirectToAction("Add", new { id = blogCategory.Id });
                }
                else
                {
                    var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@Url",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 100,
                                        Value = CommonFunction.Url(blogCategory.Url)
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Sep",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 1,
                                        Value = "-"
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@TableName",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 25,
                                        Value = "Blog"
                                    },
                                     new SqlParameter() {
                                        ParameterName = "@Id",
                                        SqlDbType =  System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = id
                                    },
                                    new SqlParameter()
                                    {
                                        ParameterName = "@TempUrl",
                                        SqlDbType = System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Output,
                                        Size = 100
                                    }};
                    context.Database.ExecuteSqlRaw("[dbo].[sp_GetURL] @Url, @Sep, @TableName, @Id, @TempUrl out", param);

                    var category = context.BlogCategory.Where(x => x.Id == id).FirstOrDefault();
                    category.ParentId = blogCategory.ParentId == 0 ? null : (int?)blogCategory.ParentId;
                    category.Name = blogCategory.Name;
                    category.Url = Convert.ToString(param[4].Value);
                    category.MetaTitle = blogCategory.MetaTitle;
                    category.MetaKeyword = blogCategory.MetaKeyword;
                    category.MetaDescription = blogCategory.MetaDescription;
                    category.Description = blogCategory.Description;
                    category.Status = Convert.ToBoolean(blogCategory.Status);

                    int result = context.SaveChanges();
                    ModelState.Clear();
                    blogCategory.Url = Convert.ToString(param[4].Value);
                    TempData["result"] = result == 1 ? "Update Successful" : "Failed";
                }
            }
            ViewBag.Title = id == null ? "Add New Blog Category" : "Update Blog Category";
            return View(blogCategory);
        }

        List<SelectListItem> GetActiveCategory()
        {
            List<SelectListItem> activeBlogCategory = new List<SelectListItem>();
            var context = new CMSContext();
            activeBlogCategory = context.BlogCategory.Where(x => x.Status == true).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return activeBlogCategory;
        }

        public string UpdateBulkStatus(string idChecked, string statusToChange)
        {
            string result = "";
            if (statusToChange == "Status")
                result = "Select Status";
            else if (idChecked == null)
                result = "Select at least 1 item";
            else
            {
                using (var context = new CMSContext())
                {
                    var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@Id",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 50,
                                        Value = idChecked
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Status",
                                        SqlDbType =  System.Data.SqlDbType.Bit,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value =Convert.ToBoolean(Convert.ToInt32(statusToChange))
                                    },
                                    new SqlParameter()
                                    {
                                        ParameterName = "@Result",
                                        SqlDbType = System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Output,
                                        Size = 50
                                    }
                    };
                    context.Database.ExecuteSqlRaw("[dbo].[sp_UpdateBulkBlogCategoryStatus] @Id, @Status, @Result out", param);
                    result = Convert.ToString(param[2].Value);
                }
            }
            return result;
        }

        public BlogCategoryList GetBlogCategory(int? page, string searchText, int? status)
        {
            int pageSize = 3;
            int pageNo = page == null ? 1 : Convert.ToInt32(page);
            var context = new CMSContext();
            var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@PageNo",
                                        SqlDbType =  System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = pageNo
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Name",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 100,
                                        Value =  searchText ?? (object)DBNull.Value
        },
                                    new SqlParameter() {
                                        ParameterName = "@PageSize",
                                        SqlDbType =  System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = pageSize
                                    },
                                    new SqlParameter()
                                    {
                                        ParameterName = "@Status",
                                        SqlDbType = System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value=status ?? (object)DBNull.Value
                                    }
                };
            //var student3 = context.BlogCategory.FromSql("[dbo].[sp_GetBlogCategoryWithPaging] @PageNo, @Name, @PageSize, @Status", param);

            BlogCategoryList bcList = new BlogCategoryList();
            using (var cnn = context.Database.GetDbConnection())
            {
                var cmm = cnn.CreateCommand();
                cmm.CommandType = System.Data.CommandType.StoredProcedure;
                cmm.CommandText = "[dbo].[sp_GetBlogCategoryWithPaging]";
                cmm.Parameters.AddRange(param);
                cmm.Connection = cnn;
                cnn.Open();
                var reader = cmm.ExecuteReader();

                List<BlogCategory> list = new List<BlogCategory>();
                PagingInfo pagingInfo = new PagingInfo();
                //while (reader.HasRows)
                //{
                while (reader.Read())
                {
                    BlogCategory blogCategory = new BlogCategory();
                    blogCategory.Id = Convert.ToInt32(reader["Id"]);
                    blogCategory.ParentId = reader["ParentId"] == DBNull.Value ? null : (Int32?)Convert.ToInt32(reader["ParentId"]);
                    blogCategory.Name = Convert.ToString(reader["Name"]);
                    blogCategory.Url = Convert.ToString(reader["Url"]);
                    blogCategory.MetaTitle = Convert.ToString(reader["MetaTitle"]);
                    blogCategory.MetaKeyword = Convert.ToString(reader["MetaKeyword"]);
                    blogCategory.MetaDescription = Convert.ToString(reader["MetaDescription"]);
                    blogCategory.Description = Convert.ToString(reader["Description"]);
                    blogCategory.AddedOn = Convert.ToDateTime(reader["AddedOn"]);
                    blogCategory.Status = Convert.ToBoolean(reader["Status"]);
                    list.Add(blogCategory);
                }
                reader.NextResult();
                while (reader.Read())
                {
                    pagingInfo.CurrentPage = pageNo;
                    pagingInfo.TotalItems = Convert.ToInt32(reader["Total"]);
                    pagingInfo.ItemsPerPage = pageSize;

                    bcList.blogCategory = list;
                    bcList.allTotal = Convert.ToInt32(reader["AllTotalPage"]);
                    bcList.activeTotal = Convert.ToInt32(reader["ActiveTotalPage"]);
                    bcList.inactiveTotal = Convert.ToInt32(reader["InActiveTotalPage"]);
                    bcList.searchText = searchText;
                    bcList.status = status;
                    bcList.pagingInfo = pagingInfo;
                }
                //}
            }
            return bcList;
        }

        public ActionResult Index(int? id, string searchText, int? status)
        {
            BlogList list = new BlogList();
            list = GetBlog(id, searchText, status);
            return View(list);
        }

        public ActionResult Add(int? id)
        {
            ViewBag.CategoryList = GetActiveCategory();
            ViewBag.MediaDate = GetMediaDate();

            Blog blog = new Blog();
            blog.PrimaryImageUrl = "/images/addphoto.jpg";
            if (id != null)
            {
                var context = new CMSContext();
                blog = context.Blog.Where(x => x.Id == id).FirstOrDefault();
                blog.PrimaryImageUrl = blog.PrimaryImageId != null ? "/" + context.Media.Where(x => x.Id == blog.PrimaryImageId).Select(x => x.Url).FirstOrDefault() : "/images/addphoto.jpg";
                ViewBag.Title = "Update Blog";
                return View(blog);
            }

            ViewBag.Title = "Add New Blog";
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Blog blog, int? id)
        {
            ViewBag.CategoryList = GetActiveCategory();
            ViewBag.MediaDate = GetMediaDate();

            if (ModelState.IsValid)
            {
                var context = new CMSContext();
                if (id == null)
                {
                    var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@Url",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 100,
                                        Value = CommonFunction.Url(blog.Url)
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Sep",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 1,
                                        Value = "-"
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@TableName",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 25,
                                        Value = DBNull.Value
                                    },
                                     new SqlParameter() {
                                        ParameterName = "@Id",
                                        SqlDbType =  System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = DBNull.Value
                                    },
                                    new SqlParameter()
                                    {
                                        ParameterName = "@TempUrl",
                                        SqlDbType = System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Output,
                                        Size = 100
                                    }};
                    context.Database.ExecuteSqlRaw("[dbo].[sp_GetURL] @Url, @Sep, @TableName, @Id, @TempUrl out", param);

                    blog.Url = Convert.ToString(param[4].Value);
                    context.Blog.Add(blog);
                    int result = context.SaveChanges();

                    TempData["result"] = result == 1 ? "Insert Successful" : "Failed";
                    if (result == 1)
                        return RedirectToAction("Add", new { id = blog.Id });
                }
                else
                {
                    var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@Url",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 100,
                                        Value = CommonFunction.Url(blog.Url)
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Sep",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 1,
                                        Value = "-"
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@TableName",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 25,
                                        Value = "Blog"
                                    },
                                     new SqlParameter() {
                                        ParameterName = "@Id",
                                        SqlDbType =  System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = id
                                    },
                                    new SqlParameter()
                                    {
                                        ParameterName = "@TempUrl",
                                        SqlDbType = System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Output,
                                        Size = 100
                                    }};
                    context.Database.ExecuteSqlRaw("[dbo].[sp_GetURL] @Url, @Sep, @TableName, @Id, @TempUrl out", param);

                    var blogResult = context.Blog.Where(x => x.Id == id).FirstOrDefault();
                    blogResult.Name = blog.Name;
                    blogResult.Url = Convert.ToString(param[4].Value);
                    blogResult.CategoryId = blog.CategoryId;
                    blogResult.PrimaryImageId = blog.PrimaryImageId;
                    blogResult.Description = blog.Description;
                    blogResult.MetaTitle = blog.MetaTitle;
                    blogResult.MetaKeyword = blog.MetaKeyword;
                    blogResult.MetaDescription = blog.MetaDescription;
                    blogResult.Status = blog.Status;

                    int result = context.SaveChanges();
                    ModelState.Clear();
                    TempData["result"] = result == 1 ? "Update Successful" : "Failed";
                }
                blog.PrimaryImageUrl = blog.PrimaryImageUrl ?? "~/images/addphoto.jpg";
            }
            ViewBag.Title = id == null ? "Add New Blog" : "Update Blog";
            return View(blog);
        }
    }
}
