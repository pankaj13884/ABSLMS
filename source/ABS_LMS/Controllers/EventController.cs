﻿using System.Linq;
using System.Web.Mvc;
using ABS_LMS.Models;
using ABS_LMS.Service.Interface;
using PagedList;
using ABS_LMS.Service.Model;

namespace ABS_LMS.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET: Event
        [Authorize(Roles = "Admin")]
        public ActionResult Index(int pagenumber = 1, int pagesize = 10)
        {
            var leaveDetails = _eventService.GetEvents();
            var model = new EventViewModel
            {
                Events = leaveDetails.ToPagedList(pagenumber, pagesize)
            };
            return View(model);
        }

        public ActionResult Create()
        {
            var model = TempData["Event"] as Event;
            return model != null ? View(model) : View();
        }


        public ActionResult Add(Event model)
        {
            if (model.EventId == 0)
                _eventService.AddEvent(model, HttpContext.User.Identity.Name);
            else
                _eventService.UpdateEvent(model.EventId, model, HttpContext.User.Identity.Name);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var model = _eventService.GetEvents().FirstOrDefault(e => e.EventId == id);
            TempData["Event"] = model;
            return RedirectToAction("Create", model);
        }

        public ActionResult Delete(int id)
        {
            var result = 1;
            _eventService.DeleteEvent(id);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}