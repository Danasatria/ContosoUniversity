using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using PagedList;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using System.Data.Entity.Infrastructure;

namespace ContosoUniversity.Controllers
{
    public class EnrollmentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: Student
        public ViewResult Index(string sortOrder, string currentFilter, string searchString)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.TitleSortParm = sortOrder == "Title" ? "title_desc" : "Title";
            ViewBag.GradeSortParm = sortOrder == "Grade" ? "grade_desc" : "Grade";
            ViewBag.CurrentFilter = searchString;
            var enroll = from s in db.Enrollments
                         select s;
            if (!String.IsNullOrEmpty(searchString))
            {

               enroll = enroll.Where(s => s.Student.LastName.Contains(searchString)
                                       || s.Course.Title.Contains(searchString));  
            }
            switch (sortOrder)
            {
                case "name_desc":
                    enroll = enroll.OrderByDescending(s => s.Student.LastName);
                    break;
                case "title_desc":
                    enroll = enroll.OrderByDescending(s => s.Course.Title);
                    break;
                case "Title":
                    enroll = enroll.OrderBy(s => s.Course.Title);
                    break;
                case "Grade":
                    enroll = enroll.OrderBy(s => s.Grade);
                    break;
                case "grade_desc":
                    enroll = enroll.OrderByDescending(s => s.Grade);
                    break;
                default:
                    enroll = enroll.OrderBy(s => s.Student.LastName);
                    break;
            }

            var enrollments = db.Enrollments.Include(s => s.Course).Include(s => s.Student);
            return View(enroll.ToList());
        }

        private DateTime? tokenize_and_extract_date_end(string searchString)
        {
            throw new NotImplementedException();
        }

        // GET: Student/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enroll = db.Enrollments.Find(id);
            if (enroll == null)
            {
                return HttpNotFound();
            }
            return View(enroll);
        }

        // GET: Student/Create
        public ActionResult Create()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Title");
            ViewBag.StudentID = new SelectList(db.Students, "ID", "LastName");
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EnrollmentID,CourseID,StudentID,Grade")] Enrollment enroll)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Enrollments.Add(enroll);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Title", enroll.CourseID);
            ViewBag.StudentID = new SelectList(db.Students, "ID", "LastName", enroll.StudentID);
            return View(enroll);
        }

        // GET: Student/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enroll = db.Enrollments.Find(id);
            if (enroll == null)
            {
                return HttpNotFound();
            }

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Title", enroll.CourseID);
            ViewBag.StudentID = new SelectList(db.Students, "ID", "LastName", enroll.StudentID);
            return View(enroll);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var enrollToUpdate = db.Enrollments.Find(id);
            if (TryUpdateModel(enrollToUpdate, "",
               new string[] { "EnrollmentID", "CourseID", "StudentID", "Grade" }))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Title", enrollToUpdate.CourseID);
            ViewBag.StudentID = new SelectList(db.Students, "ID", "LastName", enrollToUpdate.StudentID);
            return View(enrollToUpdate);
        }

        // GET: Student/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Enrollment enroll = db.Enrollments.Find(id);
            if (enroll == null)
            {
                return HttpNotFound();
            }
            return View(enroll);
        }

        // POST: Student/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Enrollment enroll = db.Enrollments.Find(id);
                db.Enrollments.Remove(enroll);
                db.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
