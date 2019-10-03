﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InExRecordApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InExRecordApp.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly DataContext dataContext;

        public ExpenseController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [HttpGet]
        public IActionResult Store()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Store(Expense expense)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    expense.UserId = Convert.ToInt32(HttpContext.Session.GetInt32("userId"));
                    dataContext.Expenses.Add(expense);
                    dataContext.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        redirecturl = Url.Action("Show", "Expense"),
                        message = "Data Submited!"
                    });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = e.Message });
                }

            }
            return View();
        }

        [HttpGet]
        public IActionResult Show()
        {
            List<Expense> expenses = dataContext.Expenses.Where(i => i.IsApproved == false).ToList();
            return View(expenses);
        }

        [HttpPost]
        public IActionResult Approve(int[] ids)
        {
            try
            {
                var entity = dataContext.Expenses.Where(i => ids.Contains(i.Id));

                foreach (var e in entity)
                {
                    e.IsApproved = true;
                    dataContext.Expenses.Update(e);
                }
                dataContext.SaveChanges();

                return Json(new { success = true, redirecturl = Url.Action("Show", "Expense"), message = "Data approved!" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
           
        }
    }
}