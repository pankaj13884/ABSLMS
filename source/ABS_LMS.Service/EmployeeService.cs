﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using ABS_LMS.Data;
using ABS_LMS.Repository;
using ABS_LMS.Service.Interface;
using ABS_LMS.Service.Model;
using Department = ABS_LMS.Service.Model.Department;

namespace ABS_LMS.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly UnitOfWork _unitOfWork;

        public EmployeeService()
        {
            _unitOfWork = new UnitOfWork(new ABSLMSEntities(ConfigHelper.ConnectionString));

        }

        public void AddEmployee(Model.Employee employee)
        {
            _unitOfWork.Employee.Add(new Data.Employee
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                AadharId = employee.AadharId,
                Address1 = employee.Address1,
                Address2 = employee.Address2,
                Address3 = employee.Address3,
                Address4 = employee.Address4,
                City = employee.City,
                PostCode = employee.PostCode,
                Country = employee.Country,
                State = employee.State,
                ConfirmationDateUTC = employee.ConfirmationDateUTC,
                DepartmentId = employee.DepartmentId,
                DesignationId = employee.DesignationId,
                DOB = employee.DOB,
                Gender = employee.Gender,
                MobileNo = employee.MobileNo,
                EmailId = employee.EmailId,
                CompanyEmailId = employee.CompanyEmailId,
                PANCard = employee.PANCard,
                LeavingDateUTC = employee.LeavingDateUTC,
                DOJ = employee.DOJ,
                EmployeeImage = employee.EmployeeImage,
                ReportingManager = employee.ReportingManager,
                ClientId = employee.ClientId
               
            });
            _unitOfWork.Complete();
        }

        public void DeleteEmployee(int employeeId)
        {
            var employeedetails = _unitOfWork.Employee.Get(employeeId);
            employeedetails.IsArchive = true;
            _unitOfWork.Complete();
        }

        public List<Model.Department> GetDepartments()
        {
            var department = _unitOfWork.Department.GetAll();
            return department.Select(d => new Department
            {
                DepartmentId = d.DepartmentId,
                DeparmentName = d.DeparmentName
            }).ToList();
        }

        public List<Model.Client> GetClients()
        {
            var client = _unitOfWork.Client.GetAll();
            return client.Select(c => new Model.Client
            {
                ClientId = c.ClientId,
                ClientName = c.Name
            }).ToList();
        }

        public List<Model.Designation> GetDesignations()
        {
            var designation = _unitOfWork.Designation.GetAll();
            return designation.Select(d => new Model.Designation
            {
                DesignationId = d.DesignationId,
                DesignationName = d.Name
            }).ToList();
        }

        public Model.Employee GetEmployee(int employeeId)
        {
            var employee = _unitOfWork.Employee.Get(employeeId);
            return new Model.Employee
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                AadharId = employee.AadharId,
                Address1 = employee.Address1,
                Address2 = employee.Address2,
                Address3 = employee.Address3,
                Address4 = employee.Address4,
                PostCode = employee.PostCode,
                City = employee.City,
                State = employee.State,
                Country = employee.Country,
                ConfirmationDateUTC = employee.ConfirmationDateUTC,
                DepartmentId = employee.DepartmentId,
                DesignationId = employee.DesignationId,
                DOB = employee.DOB,
                Gender = employee.Gender,
                MobileNo = employee.MobileNo,
                EmailId = employee.EmailId,
                CompanyEmailId = employee.CompanyEmailId,
                PANCard = employee.PANCard,
                LeavingDateUTC = employee.LeavingDateUTC,
                DOJ = employee.DOJ,
                EmployeeImage = employee.EmployeeImage,
                ClientId = employee.ClientId,
                ReportingManager = employee.ReportingManager,
                IsArchive = employee.IsArchive
            };
        }

        public List<Model.Employee> GetEmployees()
        {
            var employees = _unitOfWork.Employee.GetAll();
            return employees.Select(e => new Model.Employee
            {
                EmployeeId = e.EmployeeId,
                EmployeeCode = e.EmployeeCode,
                FirstName = e.FirstName,
                LastName = e.LastName,
                AadharId = e.AadharId,
                DepartmentId = e.DepartmentId,
                DesignationId = e.DesignationId,
                Designation = e.Designation.Name,
                ReportingManager = e.ReportingManager,
                EmployeeImage = e.EmployeeImage,
                DOB = e.DOB,
                MobileNo = e.MobileNo,
                CompanyEmailId = e.CompanyEmailId,
                Gender = e.Gender,
                ClientId = e.ClientId,
                Client = e.Client.Name,
                DOJ = e.DOJ,
                LeavingDateUTC = e.LeavingDateUTC,
                IsArchive = e.IsArchive
            }).OrderBy(o => o.EmployeeCode).ToList();
        }

        public void UpdateEmployee(int employeeId, Model.Employee employee)
        {
            var employeedetails = _unitOfWork.Employee.Get(employeeId);
            employeedetails.EmployeeCode = employee.EmployeeCode;
            employeedetails.FirstName = employee.FirstName;
            employeedetails.LastName = employee.LastName;
            employeedetails.AadharId = employee.AadharId;
            employeedetails.Address1 = employee.Address1;
            employeedetails.Address2 = employee.Address2;
            employeedetails.Address3 = employee.Address3;
            employeedetails.Address4 = employee.Address4;
            employeedetails.City = employee.City;
            employeedetails.PostCode = employee.PostCode;
            employeedetails.Country = employee.Country;
            employeedetails.State = employee.State;
            employeedetails.ConfirmationDateUTC = employee.ConfirmationDateUTC;
            employeedetails.DepartmentId = employee.DepartmentId;
            employeedetails.DesignationId = employee.DesignationId;
            employeedetails.DOB = employee.DOB;
            employeedetails.Gender = employee.Gender;
            employeedetails.MobileNo = employee.MobileNo;
            employeedetails.EmailId = employee.EmailId;
            employeedetails.CompanyEmailId = employee.CompanyEmailId;
            employeedetails.PANCard = employee.PANCard;
            employeedetails.LeavingDateUTC = employee.LeavingDateUTC;
            employeedetails.DOJ = employee.DOJ;
            employeedetails.EmployeeImage = employee.EmployeeImage;
            employeedetails.ReportingManager = employee.ReportingManager;
            employeedetails.ClientId = employee.ClientId;
            _unitOfWork.Complete();
        }

     
    }
}
