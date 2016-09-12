﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ABS_LMS.Helper;
using ABS_LMS.Models;
using ABS_LMS.Service.Interface;
using ABS_LMS.Service.Model;
using ABS_LMS.Models.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


namespace ABS_LMS.Controllers
{
    public class LeaveController : Controller
    {
        private readonly IEmployeeLeaveService _employeeLeaveService;
        private readonly IEmployeeService _employeeService;
        private readonly IHolidayService _holidayService;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

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
        public LeaveController(IEmployeeLeaveService employeeLeaveService, IEmployeeService employeeService, IHolidayService holidayService)
        {
            _employeeLeaveService = employeeLeaveService;
            _employeeService = employeeService;
            _holidayService = holidayService;
        }

        // GET: Leave
        public ActionResult Index(int id)
        {
            return Authorization.HasAccess(Convert.ToString(id), () =>
            {
                var leaveDetails = _employeeLeaveService.GetEmployeeLeaveDetails(id);

                var model = leaveDetails.Select(employeeLeave => new EmployeeLeaveViewModel
                {

                    EmployeeLeaveDetails = employeeLeave,


                }).ToList();

                return View(model);
            });
        }
        // GET: Leave

        public ActionResult LeavePendingForApproval(int id)
        {

            return Authorization.HasAccess(Convert.ToString(id), () =>
            {
                var leaveDetails = _employeeLeaveService.GetLeaveDetailsByApprovedId(id);

                var model = leaveDetails.Select(employeeLeave => new EmployeeLeaveViewModel
                {

                    EmployeeLeaveDetails = employeeLeave,

                }).ToList();
                return View(model);
            });
        }


        [Authorize(Roles = "HR")]
        public ActionResult ApprovedLeaveDetails(int id)
        {

            return Authorization.HasAccess(Convert.ToString(id), () =>
            {
                var leaveDetails = _employeeLeaveService.GetApprovedLeaves();

                var model = leaveDetails.Select(employeeLeave => new EmployeeLeaveViewModel
                {
                    EmployeeLeaveDetails = employeeLeave,
                }).ToList();
                return View(model);
            });
        }
        public ActionResult Create(int id)
        {

            return Authorization.HasAccess(Convert.ToString(id), () =>
           {
               var leavetype = _employeeLeaveService.GetLeaves().Select(l => new SelectListItem
               {
                   Text = l.Name,
                   Value = l.LeaveTypeId.ToString()

               });

               var enumValues = Enum.GetValues(typeof(LeaveStatus)) as LeaveStatus[];
               if (enumValues == null)
                   return null;

               var leaveStatus = enumValues.Select(enumValue => new SelectListItem
               {

                   Value = ((int)enumValue).ToString(),
                   Text = _employeeLeaveService.GetEnumsNameById(Convert.ToInt32(enumValue))
               }).ToList();
               var manager = (from e in _employeeService.GetEmployees()
                              join m in _employeeService.GetEmployees() on e.ReportingManager equals m.EmployeeId
                              where e.EmployeeId.Equals(id)
                              select new
                              {
                                  managerId = e.ReportingManager,
                                  managerName = m.FirstName + " " + m.LastName,
                              }).ToList();
               var model = new EmployeeLeaveViewModel
               {
                   EmployeeLeaveDetails = new EmployeeLeave { EmployeeId = id },
                   LeaveType = leavetype,
                   LeaveStatusEnums = leaveStatus
                   //    LeaveSummaries = _employeeLeaveService.GetLeaveSummary(id)
               };

               var firstOrDefault = manager.FirstOrDefault();
               if (firstOrDefault != null)
               {
                   model.EmployeeLeaveDetails.ApprovedPersonName = firstOrDefault?.managerName ?? "";
                   model.EmployeeLeaveDetails.ApprovedBy = firstOrDefault?.managerId;
               }
               else
               {
                   var hr = GetHr();
                   model.EmployeeLeaveDetails.ApprovedPersonName = hr.FirstOrDefault().FirstName + " " +
                                                                   hr.FirstOrDefault().LastName;
                   model.EmployeeLeaveDetails.ApprovedBy = hr.FirstOrDefault().EmployeeId;
               }
               return View(model);
           });
        }

        // POST: Employee/Create
        [HttpPost]
        public ActionResult Create(int id, EmployeeLeaveViewModel employeeleave)
        {
            try
            {
                var newleave = employeeleave.EmployeeLeaveDetails;
                _employeeLeaveService.AddEmployeeLeaveDetails(newleave);
                var employee = _employeeService.GetEmployee(newleave.EmployeeId);
                var leaveDetails = employeeleave.EmployeeLeaveDetails;
                //Send mail

                if (!employeeleave.IsSave)
                {
                    //To Employee
                    SendMailToEmployee(employee, leaveDetails);
                    //To Manager
                    SendMailToManager(employee, leaveDetails);
                    //To Hr
                    SendMailToHr(employee, leaveDetails);
                }
                return RedirectToAction("Index/" + id + "");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Edit(int id, int leaveId)
        {
            return Authorization.HasAccess(Convert.ToString(id), () =>
            {
                var leavedetails = _employeeLeaveService.GetLeavedetails(leaveId);
                var leavetype = _employeeLeaveService.GetLeaves().Select(l => new SelectListItem
                {
                    Text = l.Name,
                    Value = l.LeaveTypeId.ToString()
                });

                var enumValues = Enum.GetValues(typeof(LeaveStatus)) as LeaveStatus[];
                if (enumValues == null)
                    return null;

                var leaveStatus = enumValues.Select(enumValue => new SelectListItem
                {
                    Value = ((int)enumValue).ToString(),
                    Text = _employeeLeaveService.GetEnumsNameById(Convert.ToInt32(enumValue))
                }).ToList().Take(2);

                var model = new EmployeeLeaveViewModel
                {
                    EmployeeLeaveDetails = leavedetails,
                    LeaveType = leavetype,
                    LeaveStatusEnums = leaveStatus,
                };

                return View(model);
            });
        }
        [HttpPost]
        public ActionResult Edit(int leaveId, EmployeeLeaveViewModel employeeleave)
        {
            try
            {
                var leaveDetails = employeeleave.EmployeeLeaveDetails;
                _employeeLeaveService.UpdateEmployeeLeaveDetails(leaveId, leaveDetails);
                var employee = _employeeService.GetEmployee(leaveDetails.EmployeeId);
                var leaveTemplate = string.Format("Leave applied from {0} to {1} ",
                  employeeleave.EmployeeLeaveDetails.LeaveStartDate.ToShortDateString(),
                  employeeleave.EmployeeLeaveDetails.LeaveEndDate.ToShortDateString());

                //To Employee
                SendMailToEmployee(employee, leaveDetails);
                //To Manager
                SendMailToManager(employee, leaveDetails);
                //To Hr
                SendMailToHr(employee, leaveDetails);

                return RedirectToAction("Index", new { id = employeeleave.EmployeeLeaveDetails.EmployeeId });
            }
            catch
            {
                return View();
            }
        }

        public ActionResult NotFound()
        {
            return View();
        }
        public ActionResult LeaveStatus(string status, int historyid)
        {
            int result = _employeeLeaveService.UpdateLeaveStatus(status, historyid);
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult EmployeeReportCsvDownload(int id)
        {
            var leaveDetails = _employeeLeaveService.GetEmployeeLeaveDetails(id);

            return File(leaveDetails.ToDataTable().ToCsvStream(), "text/csv",
                string.Format("Employees-{0:yyyy-MM-dd-hh-mm-ss}.csv", DateTime.Now));
        }
        //public string GetEnumsNameById(int enumId)
        //{
        //    LeaveStatus enumDisplayStatus = (LeaveStatus)enumId;
        //    string stringValue = enumDisplayStatus.ToString();
        //    return stringValue;
        //}


        //[HttpPost]
        public ActionResult GetActualLeaveDaysCount(DateTime? startDate, DateTime? endDate)
        {
            var result = _holidayService.GetActualLeaveDaysCount(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
            return Json(new { Value = result }, JsonRequestBehavior.AllowGet);
        }

        private List<Employee> GetHr()
        {
            var userByHr = RoleManager.FindByName("HR").Users.ToList();
            var employee = _employeeService.GetEmployees();
            var hr = (from u in UserManager.Users.ToList()
                      join ur in userByHr on u.Id equals ur.UserId
                      join e in employee on u.EmployeeId equals e.EmployeeId
                      select new
                      {
                          e.FirstName,
                          e.LastName,
                          u.EmployeeId,
                          e.CompanyEmailId
                      }).ToList();
            return hr.Select(item => new Employee()
            {
                FirstName = item.FirstName,
                LastName = item.LastName,
                EmployeeId = item.EmployeeId,
                CompanyEmailId = item.CompanyEmailId
            }).ToList();
        }

        private List<Employee> GetManager()
        {
            var userByHr = RoleManager.FindByName("Manager").Users.ToList();
            var employee = _employeeService.GetEmployees();
            var hr = (from u in UserManager.Users.ToList()
                      join ur in userByHr on u.Id equals ur.UserId
                      join e in employee on u.EmployeeId equals e.EmployeeId
                      select new
                      {
                          e.FirstName,
                          e.LastName,
                          u.EmployeeId,
                          e.CompanyEmailId
                      }).ToList();
            return hr.Select(item => new Employee()
            {
                FirstName = item.FirstName,
                LastName = item.LastName,
                EmployeeId = item.EmployeeId,
                CompanyEmailId = item.CompanyEmailId
            }).ToList();
        }

        private void SendMailToEmployee(Employee employee, EmployeeLeave employeeLeave)
        {
            var employeeName = employee.FirstName + " " + employee.LastName;
            var employeetemplate = Template.CreateLeaveTemplate(employeeName, CompanyContainer.CompanyName, employeeLeave.LeaveTypeName, employeeLeave.NoOfDays.ToString(),
                                                employeeLeave.LeaveStartDate.ToString(CultureInfo.InvariantCulture),
                                                employeeLeave.LeaveEndDate.ToString(CultureInfo.InvariantCulture), employeeLeave.Reason, employeeLeave.LeaveStatus.ToString());

            SmtpHelper.Send(employee.CompanyEmailId, Subject.Leave, employeetemplate);
        }
        private void SendMailToManager(Employee employee, EmployeeLeave employeeLeave)
        {
            var manager = GetManager().FirstOrDefault(e => (e.FirstName + e.LastName).ToLower() ==
                                                        employeeLeave.ApprovedPersonName.Replace(
                                                            " ", "").ToLower());
            var employeeName = employee.FirstName + " " + employee.LastName;
            var managerName = manager?.FirstName + " " + manager?.LastName;
            var employeetemplate = Template.CreateLeaveTemplate(managerName, employeeName, employeeLeave.LeaveTypeName, employeeLeave.NoOfDays.ToString(),
                                                employeeLeave.LeaveStartDate.ToString(CultureInfo.InvariantCulture),
                                                employeeLeave.LeaveEndDate.ToString(CultureInfo.InvariantCulture), employeeLeave.Reason, employeeLeave.LeaveStatus.ToString());

            SmtpHelper.Send(manager?.CompanyEmailId, Subject.Leave, employeetemplate);
        }
        private void SendMailToHr(Employee employee, EmployeeLeave employeeLeave)
        {
            var employeeName = employee.FirstName + " " + employee.LastName;
            var hr = GetHr();
            if (hr.Any())
            {
                var hrName = hr.FirstOrDefault()?.FirstName + " " + hr.FirstOrDefault()?.LastName;
                var employeetemplate = Template.CreateLeaveTemplate(hrName, employeeName, employeeLeave.LeaveTypeName, employeeLeave.NoOfDays.ToString(),
                                              employeeLeave.LeaveStartDate.ToString(CultureInfo.InvariantCulture),
                                              employeeLeave.LeaveEndDate.ToString(CultureInfo.InvariantCulture), employeeLeave.Reason, employeeLeave.LeaveStatus.ToString());

                SmtpHelper.Send(hr.FirstOrDefault()?.CompanyEmailId, Subject.Leave, employeetemplate);
            }
        }
    }
}