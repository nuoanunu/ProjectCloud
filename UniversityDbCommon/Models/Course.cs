using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityDbCommon.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("Course ID")]
        public int CourseID { get; set; }
        [DisplayName("Title")]
        public string Title { get; set; }
        [DisplayName("Credits")]
        public int Credits { get; set; }

        public virtual List<Enrollment> Enrollments { get; set; }
    }
}
