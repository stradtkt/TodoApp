using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    public class HomeController : Controller
    {
        private TodoContext _tContext;
        public HomeController(TodoContext context)
        {
            _tContext = context;
        }
        private User ActiveUser 
        {
            get 
            {
                return _tContext.users.Where(u => u.user_id == HttpContext.Session.GetInt32("user_id")).FirstOrDefault();
            }
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            ViewBag.user = ActiveUser;
            return View();
        }
        [HttpGet("register")]
        public IActionResult Register()
        {
            ViewBag.user = ActiveUser;
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            ViewBag.user = ActiveUser;
            return View();
        }

        [HttpPost("registeruser")]
        public IActionResult RegisterUser(RegisterUser newuser)
        {
            User CheckEmail = _tContext.users
                .Where(u => u.email == newuser.email)
                .SingleOrDefault();

            if(CheckEmail != null)
            {
                ViewBag.errors = "That email already exists";
                return RedirectToAction("Register");
            }
            if(ModelState.IsValid)
            {
                PasswordHasher<RegisterUser> Hasher = new PasswordHasher<RegisterUser>();
                User newUser = new User
                {
                    user_id = newuser.user_id,
                    first_name = newuser.first_name,
                    last_name = newuser.last_name,
                    email = newuser.email,
                    password = Hasher.HashPassword(newuser, newuser.password)
                  };
                _tContext.Add(newUser);
                _tContext.SaveChanges();
                ViewBag.success = "Successfully registered";
                return RedirectToAction("Login");
            }
            else
            {
                return View("Register");
            }
        }

        [HttpPost("loginuser")]
        public IActionResult LoginUser(LoginUser loginUser) 
        {
            User CheckEmail = _tContext.users
                .SingleOrDefault(u => u.email == loginUser.email);
            if(CheckEmail != null)
            {
                var Hasher = new PasswordHasher<User>();
                if(0 != Hasher.VerifyHashedPassword(CheckEmail, CheckEmail.password, loginUser.password))
                {
                    HttpContext.Session.SetInt32("user_id", CheckEmail.user_id);
                    HttpContext.Session.SetString("first_name", CheckEmail.first_name);
                    return RedirectToAction("TodoApp");
                }
                else
                {
                    ViewBag.errors = "Incorrect Password";
                    return View("Register");
                }
            }
            else
            {
                ViewBag.errors = "Email not registered";
                return View("Register");
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet("TodoApp")]
        public IActionResult TodoApp() 
        {
            List<Todo> todos = _tContext.todos.Include(u => u.User).ToList();
            ViewBag.todos = todos;
            return View();
        }

        [HttpGet("DeleteTodo/{todo_id}")]
        public IActionResult DeleteTodo(int todo_id)
        {
            Todo todo = _tContext.todos.Where(t => t.todo_id == todo_id).SingleOrDefault();
            _tContext.todos.Remove(todo);
            _tContext.SaveChanges();
            return RedirectToAction("TodoApp");
        }

        [HttpPost("AddTodo")]
        public IActionResult AddTodo(Todo item)
        {
            if(ModelState.IsValid)
            {
                if(item.due_by < DateTime.Now)
                {
                    ModelState.AddModelError("due_by", "You cannot have a todo that is in the past!");
                    ViewBag.errors = "You cannot have a todo that is in the past!";
                    return RedirectToAction("TodoApp");
                }
                else
                {
                    Todo newTodo = new Todo
                    {
                        title = item.title,
                        desc = item.desc,
                        due_by = item.due_by,
                        user_id = ActiveUser.user_id
                    };
                    _tContext.todos.Add(newTodo);
                    _tContext.SaveChanges();
                    return RedirectToAction("TodoApp");
                }
            }
            return View("TodoApp");
        }
        [HttpGet("EditTodo/{todo_id}")]
        public IActionResult EditTodo(int todo_id)
        {
            Todo todo = _tContext.todos.Where(t => t.todo_id == todo_id).SingleOrDefault();
            ViewBag.todo = todo;
            return View();
        }
        [Route("{todo_id}/ProcessEditTodo")]
        public IActionResult ProcessEditTodo(int todo_id, string title, string desc, DateTime due_by)
        {
            Todo todo = _tContext.todos.Where(t => t.todo_id == todo_id).SingleOrDefault();
            todo.title = title;
            todo.desc = desc;
            todo.due_by = due_by;
            _tContext.SaveChanges();
            return RedirectToAction("TodoApp");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
