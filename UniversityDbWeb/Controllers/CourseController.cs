using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UniversityDbCommon.DAL;
using UniversityDbCommon.Models;

namespace UniversityDbWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CourseController : Controller
    {
        private UniversityDbContext db = new UniversityDbContext();

        // GET: Course
        public ActionResult Index()
        {
            return View(db.Courses.ToList());
        }

        // GET: Course/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Course/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Course/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,Title,Credits")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Course/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Course/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CourseID,Title,Credits")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Course/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStudent(int id, int studentId)
        {
            Course course = db.Courses.Find(id);
            Student student = db.Students.Find(studentId);
            if (student != null)
            {
                Enrollment enrollment = new Enrollment
                {
                    Student = student,
                    Course = course
                };
                db.Enrollments.Add(enrollment);
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = id });
        }
        public ActionResult EditScore(Course course)
        {
            try
            {
            
                Course oldcourse = db.Courses.Find(course.CourseID);
                foreach (Enrollment enroll in course.Enrollments)
                {
                    bool flag = false;
                    JsonMessage message = new JsonMessage();
                    message.StudentID = enroll.StudentID + "";
                    message.CourseID = course.Title;
                    Enrollment oldenroll = db.Enrollments.Find(enroll.EnrollmentID);
                    if (oldenroll.Midterm != enroll.Midterm)
                    {
                        flag = true;
                        message.oldMidterm = oldenroll.Midterm + "";
                        message.Midterm = enroll.Midterm + "";
                        oldenroll.Midterm = enroll.Midterm;

                    }
                    if (oldenroll.Quiz1 != enroll.Quiz1)
                    {
                        flag = true;
                        message.oldQuiz1 = oldenroll.Quiz1 + "";
                        message.Quiz1 = enroll.Quiz1 + "";
                        oldenroll.Quiz1 = enroll.Quiz1;

                    }
                    if (oldenroll.Quiz2 != enroll.Quiz2)
                    {
                        flag = true;
                        message.oldQuiz2 = oldenroll.Quiz2 + "";
                        message.Quiz2 = enroll.Quiz2 + "";
                        oldenroll.Quiz2 = enroll.Quiz2;

                    }
                    if (oldenroll.Quiz3 != enroll.Quiz3)
                    {
                        flag = true;
                        message.oldQuiz3 = oldenroll.Quiz3 + "";
                        message.Quiz3 = enroll.Quiz3 + "";
                        oldenroll.Quiz3 = enroll.Quiz3;

                    }
                    if (oldenroll.Project != enroll.Project)
                    {
                        flag = true;
                        message.oldProject = oldenroll.Project + "";
                        message.Project = enroll.Project + "";
                        oldenroll.Project = enroll.Project;

                    }
                    if (oldenroll.Final != enroll.Final)
                    {
                        flag = true;
                        message.oldFinal = oldenroll.Final + "";
                        message.Final = enroll.Final + "";
                        oldenroll.Final = enroll.Final;

                    }
                    if (flag)
                    {
                        JavaScriptSerializer serializer = new JavaScriptSerializer();

                        String newmessage = serializer.Serialize(message);
                        db.SaveChanges();
                        CloudStorageAccount ac = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
                        CloudQueueClient qclient = ac.CreateCloudQueueClient();
                        CloudQueue que = qclient.GetQueueReference("nhatvhnqueue");
                        que.CreateIfNotExists();
                        CloudQueueMessage messagee = new CloudQueueMessage(newmessage);
                        que.AddMessage(messagee);
                        System.Diagnostics.Debug.WriteLine("AAAAAAAAAAAAAAAAAAAAAA " + messagee);
                    }

                }

                db.SaveChanges();
                return RedirectToAction("Details", new { id = course.CourseID });
            }
            catch (Exception e) { }
            return View();
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
    public class JsonMessage
    {
        public JsonMessage(string json)
        {

        }
        public String CourseID { get; set; }
        public String StudentID { get; set; }
        public String Quiz1 { get; set; }
        public String Quiz2 { get; set; }
        public String Quiz3 { get; set; }
        public String Midterm { get; set; }
        public String Project { get; set; }
        public String Final { get; set; }
        public String oldQuiz1 { get; set; }
        public String oldQuiz2 { get; set; }
        public String oldQuiz3 { get; set; }
        public String oldMidterm { get; set; }
        public String oldProject { get; set; }
        public String oldFinal { get; set; }
        public JsonMessage()
        {
            CourseID = "";
            StudentID = "";
            Quiz1 = "";
            Quiz2 = "";
            Quiz3 = "";
            Midterm = "";
            Project = "";
            Final = "";
            oldQuiz1 = "";
            oldQuiz2 = "";
            oldQuiz3 = "";
            oldMidterm = "";
            oldProject = "";
            oldFinal = "";
        }
    }
}