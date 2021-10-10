using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.ViewModels;
using PagedList;

namespace ContosoUniversity.Controllers
{
    public class HomeController : Controller
    {
        private SchoolContext db = new SchoolContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About(string sortOrder, string searchString, string currentFilter, int? page)
        {
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.IntSortParm = sortOrder == "count" ? "count_desc" : "count";

            IQueryable<EnrollmentDateGroup> data = from s in db.Students
                                                   group s by s.EnrollmentDate into dateGroup
                                                   select new EnrollmentDateGroup()
                                                   {
                                                       EnrollmentDate = dateGroup.Key,
                                                       StudentCount = dateGroup.Count()
                                                   };

            if (!String.IsNullOrEmpty(searchString))
            {
                try
                {
                    var date = DateTime.Parse(searchString);
                    data = data.Where(s => s.EnrollmentDate == date);
                }
                catch (FormatException)
                {
                    var count = int.Parse(searchString);
                    data = data.Where(s => s.StudentCount == count);
                }


            }

            switch (sortOrder)
            {
                case "count_desc":
                    data = data.OrderByDescending(s => s.StudentCount);
                    break;
                case "count":
                    data = data.OrderBy(s => s.StudentCount);
                    break;
                case "date_desc":
                    data = data.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    data = data.OrderBy(s => s.EnrollmentDate);
                    break;
            }

            return View(data.ToList());
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}