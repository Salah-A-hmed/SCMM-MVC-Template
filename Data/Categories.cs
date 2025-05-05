using System.ComponentModel.DataAnnotations;

namespace SCMM.Data
{
    public enum Categories
    {
        Academic ,
        Administrative ,
        Technical ,
        Financial ,
        [Display(Name = "Course Registration")]
        CourseRegistration ,
        Facilities,
    }
}
