using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Google.Apis.Classroom.v1;
using static Google.Apis.Classroom.v1.CoursesResource;
using static Classroom_Client.Utils;
using static Classroom_Client.ClassroomResolver;

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


        private static List<CourseWrapper> CourseQueue = new List<CourseWrapper>();
        public static void Update(bool alert = true)
        {
            CourseQueue.Clear();
            var response = ListCourseHandler.Execute();
            if(response.Courses!=null)
            {
                foreach(var course in response.Courses)
                {
                    var CurrentCourse = Courses.Where(c => c.course.Id == course.Id);
                    CourseWrapper CurrentWrapper = CurrentCourse.Count() == 0 ? null : CurrentCourse.First();
                    if(CurrentWrapper==null)
                    {
                        if(alert) Notify($"New course found: {course.Name}");
                        CurrentWrapper = new CourseWrapper(course);
                        Courses.Add(CurrentWrapper);
                        CourseQueue.Add(CurrentWrapper);
                    }
                    else
                    {
                        CurrentWrapper.Update(alert);
                    }
                }
            }
        }

        private static MainWindow _instance = null;
        public static MainWindow Instance
        {
            get => _instance;
            set
            {
                _instance?.Close();
                Resolve();
                _instance = value;
                foreach (var queued in CourseQueue) Instance.CreateCourseButton(queued);
                Application.Run(Instance);
            }
        }
    }
}