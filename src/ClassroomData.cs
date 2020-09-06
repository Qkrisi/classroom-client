using System.Linq;
using System.Collections.Generic;
using Google.Apis.Classroom.v1;
using static Google.Apis.Classroom.v1.CoursesResource;
using static Classroom_Client.Utils;

namespace Classroom_Client
{
    public static class ClassroomData
    {
        private static ClassroomService _service = null;
        public static ClassroomService Service
        {
            get => _service;
            set
            {
                _service = value;
                WorkResourceHanlder = new CourseWorkResource(Service);
                AnnouncementResourceHandler = new AnnouncementsResource(Service);
                ListCourseHandler = Service.Courses.List();
            }
        }

        public static CourseWorkResource WorkResourceHanlder = null;
        public static AnnouncementsResource AnnouncementResourceHandler = null;
        public static CourseWorkResource.StudentSubmissionsResource SubmissionResourceHandler => WorkResourceHanlder.StudentSubmissions;

        public static List<CourseWrapper> Courses = new List<CourseWrapper>();
        public static ListRequest ListCourseHandler = null;

        public static void Update(bool alert = true)
        {
            var response = ListCourseHandler.Execute();
            if(response.Courses!=null)
            {
                foreach(var course in response.Courses)
                {
                    var CurrentCourse = Courses.Where(c => c.course.Id == course.Id);
                    CourseWrapper CurrentWrapper = CurrentCourse.Count() == 0 ? null : CurrentCourse.First();
                    if(CurrentWrapper==null)
                    {
                        if(alert) Notify($"New course found: {course.Id}");
                        CurrentWrapper = new CourseWrapper(course);
                        Courses.Add(CurrentWrapper);
                    }
                    else
                    {
                        CurrentWrapper.Update(alert);
                    }
                }
            }
        }
    }
}