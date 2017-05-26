using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCDatabaseFirst.Models;
using System.Net;
using PagedList;

namespace MVCDatabaseFirst.Controllers
{
    public class EmployeeController : Controller
    {
        EmployeeContext ctx = new EmployeeContext();
        // GET: Employee
        //[HttpGet]
        //public ActionResult Index()
        //{
        //    return View(ctx.Employees.ToList());
        //}
        [HttpGet]
        public ActionResult Index(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No, string SelectedFirstName, string SelectedLastName )
        {
            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingFirstName = String.IsNullOrEmpty(Sorting_Order) ? "FirstNameDESC" : "";
            ViewBag.SortingLastName = Sorting_Order == "LastName" ? "LastNameDESC" : "LastName";
            ViewBag.SortingMiddleName = Sorting_Order == "MiddleName" ? "MiddleNameDESC" : "MiddleName";
            ViewBag.SortingAddress = Sorting_Order == "Address" ? "AddressDESC" : "Address";
            ViewBag.SortingSalary = Sorting_Order == "Salary" ? "SalaryDESC" : "Salary";
            ViewBag.SortingEmail = Sorting_Order == "Email" ? "EmailDESC" : "Email";

            if(Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;

            var rawData = (from r in ctx.Employees
                           select r).ToList();

            var employee = from emp in ctx.Employees select emp;

            
            if(!String.IsNullOrEmpty(SelectedFirstName))
            {
                employee = employee.Where(p => p.FirstName.Trim().Equals(SelectedFirstName.Trim()));
            }

            var UniqueFirstNames = employee.Select(s => new { firstname = s.FirstName }).Distinct();

            ViewBag.UniqueFirstNames = UniqueFirstNames.Select(s => new SelectListItem { Value = s.firstname, Text = s.firstname }).ToList();

            if (!String.IsNullOrEmpty(SelectedLastName))
            {
                employee = employee.Where(p => p.LastName.Trim().Equals(SelectedLastName.Trim()));
            }

            var UniqueLastNames = employee.Select(s => new { lastname = s.LastName }).Distinct();

            ViewBag.UniqueLastNames = UniqueLastNames.Select(s => new SelectListItem { Value = s.lastname, Text = s.lastname }).ToList();

            if (!string.IsNullOrEmpty(Search_Data))
            { 
                employee = employee.Where(p => p.FirstName.ToLower().Contains(Search_Data.ToLower())
                || p.LastName.ToLower().Contains(Search_Data.ToLower())
                );
            }

            switch (Sorting_Order)
            {
                case "FirstNameDESC":
                    employee = employee.OrderByDescending(o => o.FirstName);
                    break;
                case "LastNameDESC":
                    employee = employee.OrderByDescending(o => o.LastName);
                    break;
                case "LastName":
                    employee = employee.OrderBy(o => o.LastName);
                    break;
                case "MiddleNameDESC":
                    employee = employee.OrderByDescending(o => o.MiddleName);
                    break;
                case "MiddleName":
                    employee = employee.OrderBy(o => o.MiddleName);
                    break;
                case "AddressDESC":
                    employee = employee.OrderByDescending(o => o.Address);
                    break;
                case "Address":
                    employee = employee.OrderBy(o => o.Address);
                    break;
                case "SalaryDESC":
                    employee = employee.OrderByDescending(o => o.Salary);
                    break;
                case "Salary":
                    employee = employee.OrderBy(o => o.Salary);
                    break;
                case "EmailDESC":
                    employee = employee.OrderByDescending(o => o.Email);
                    break;
                case "Email":
                    employee = employee.OrderBy(o => o.Email);
                    break;
                default:
                    employee = employee.OrderBy(o => o.FirstName);
                        break;
            }
            int Size_Of_Page = 4; //Record per page
            int No_Of_Page = (Page_No ?? 1); //if page != null use Page else assign 1
            return View(employee.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ctx.Employees.Add(employee);
                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(employee);
            }
            catch
            {
                return View();
            }

        }
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Employee employee = ctx.Employees.Find(id);
            if (employee == null)
                return HttpNotFound();
            return View(employee);

        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Employee employee = ctx.Employees.Find(id);
            if (employee == null)
                return HttpNotFound();
            //Return the employee
            return View(employee);
        }
        [HttpPost]
        public ActionResult Edit(Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ctx.Entry(employee).State = System.Data.Entity.EntityState.Modified;
                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(employee);
                    
            }
            catch
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Employee employee = ctx.Employees.Find(id);
            if (employee == null)
                return HttpNotFound();
            return View(employee);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Employee employee = new Employee(); 
                if (ModelState.IsValid)
                {
                    employee = ctx.Employees.Find(id);
                    if (employee == null)
                        return HttpNotFound();
                    ctx.Employees.Remove(employee);
                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(employee);
            }
            catch
            {
                return View();
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (ctx != null)
            {
                ctx.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}