using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityDbCommon.Models
{
    public class Student
    {
        [DisplayName("Roll Number")]
        public int ID { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [DisplayName("First Name")]
        public string FirstMidName { get; set; }
        [DisplayName("Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }
        [DisplayName("Profile Image")]
        public string ProfileImageUrl { get; set; }
        [DisplayName("Profile Image")]
        public string ProfileThumbnailUrl { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }

        public string FullName
        {
            get
            {
                return LastName + ", " + FirstMidName;
            }
        }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
