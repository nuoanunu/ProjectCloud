using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityDbCommon.Models
{
    public enum Grade
    {
        A, B, C, D, F
    }

    public class Enrollment
    {
        [DisplayName("Enrollment ID")]
        public int EnrollmentID { get; set; }
        [DisplayName("Course ID")]
        public int CourseID { get; set; }
        [DisplayName("Roll Number")]
        public int StudentID { get; set; }
        [DisplayName("Quiz 1")]
        public double Quiz1 { get; set; }
        [DisplayName("Quiz 2")]
        public double Quiz2 { get; set; }
        [DisplayName("Quiz 3")]
        public double Quiz3 { get; set; }
        [DisplayName("Midterm")]
        public double Midterm { get; set; }
        [DisplayName("Project")]
        public double Project { get; set; }
        [DisplayName("Final")]
        public double Final { get; set; }

        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }
}
