﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SanXuatCheckList.Controllers
{
    public class ChecklistApproveController : BaseController
    {
         
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Info(int Id)
        {
            return View(Id);
        }
    }
}