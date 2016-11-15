﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using ABS_LMS.Service.Interface;
using ABS_LMS.Service.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ABS_LMS.Controllers
{
    public class OrganizationalChartController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        // GET: OrganizationalChart
        public OrganizationalChartController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        private ApplicationUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                    _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                return _userManager;
            }
        }

        private ApplicationRoleManager RoleManager
        {
            get
            {
                if (_roleManager == null)
                    _roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();

                return _roleManager;
            }
        }
        [Authorize(Roles = "Admin, Hr,User,Manager")]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetChartData(int id)
        {
            List<object> chartData = new List<object>();
            var employees = _employeeService.GetEmployees().Where(e => !e.IsArchive && e.ReportingManager==id);
            var employee2 = _employeeService.GetEmployees().Where(e => !e.IsArchive && e.EmployeeId == id).FirstOrDefault();
            foreach (var employee in employees)
            {
                // Adding new Employee object to List
                chartData.Add(new object[]
                {
                     employee.EmployeeId,
                      employee.FirstName+' '+employee.LastName,
                     employee.Designation,
                   employee.ReportingManager,
                 
                  
                });
            }
            chartData.Add(new object[]
                {
                     employee2.EmployeeId,
                     employee2.FirstName,
                     employee2.Designation,
                    id


                });
            return Json(chartData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetFullChartDataNew()
        {
            List<EmployeeChart> chartData = new List<EmployeeChart>();
            var employees = _employeeService.GetEmployees().Where(e => !e.IsArchive);
            foreach (var employee in employees)
            {
                var employeeNew=new EmployeeChart
                {
                    EmployeeId =employee.EmployeeId,
                    EmployeeName = employee.FirstName + ' ' + employee.LastName,
                    Designation = employee.Designation,
                    ReportingManager = employee.EmployeeCode=="1002"?null:employee.ReportingManager,
                    EmployeeImageUrl = GetEmployeeImageUrl(employee.EmployeeImage, employee.Gender)
                };

                           chartData.Add(employeeNew);
            }

            var jsonResult = Json(chartData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
            //return Json(chartData, JsonRequestBehavior.AllowGet);
        }

        public string GetEmployeeImageUrl(byte[] imagebBytes,string gender)
        {
            string imageUrl;
            if (imagebBytes != null && imagebBytes.Length > 0)
            {
                string imageBase64 = Convert.ToBase64String(imagebBytes);
                imageUrl = string.Format("data:image/gif;base64,{0}", imageBase64);
               

            }
            else
            {
                imageUrl = gender.ToLower()!= "female" ? "/Content/themes/LMS/images/profile-img.jpg" : "/Content/themes/LMS/images/female.jpg";
                
              
            }

     return imageUrl;
        }

        [HttpPost]
        public JsonResult GetFullChartData()
        {
            List<object> chartData = new List<object>();
            var employees = _employeeService.GetEmployees().Where(e => !e.IsArchive);
         
            foreach (var employee in employees)
            {
                // Adding new Employee object to List
                chartData.Add(new object[]
                {
                     employee.EmployeeId,
                     employee.FirstName+' '+employee.LastName,
                     employee.Designation,
                   employee.ReportingManager,


                });
            }
           
            return Json(chartData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetReportingManager()
        {

            return Json(Getlist());
        }
       
        private IEnumerable<SelectListItem> Getlist()
        {
            var employee = _employeeService.GetEmployees().ToList();
            var userByRole = RoleManager.FindByName("Manager").Users.ToList();
            var employeebyUsers = (from u in UserManager.Users.ToList()
                                   join ur in userByRole on u.Id equals ur.UserId
                                   join e in employee on u.EmployeeId equals e.EmployeeId
                                   select new
                                   {
                                       e.FirstName,
                                       e.LastName,
                                       u.EmployeeId
                                   }).ToList();
            var reportingManager = employeebyUsers.Select(m => new SelectListItem
            {
                Text = m.FirstName + " " + m.LastName,
                Value = m.EmployeeId.ToString(),
            }).ToList();
          
            return reportingManager;
        }


        public class EmployeeChart
        {
            public string EmployeeName { get; set; }
            public int EmployeeId { get; set; }
            public string Designation { get; set; }
            public int? ReportingManager { get; set; }
            public string EmployeeImageUrl { get; set; }
        }
    }
    
}