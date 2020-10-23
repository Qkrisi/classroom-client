using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections;
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

        private static CourseWrapper _CurrentCourse = null;
        public static CourseWrapper CurrentCourse
        {
            get => _CurrentCourse;
            set
            {
                _CurrentCourse = value;
                Instance.OpenCourse(CurrentCourse.course.Id);
            }
        }


        private static Queue<CourseWrapper> CourseQueue = new Queue<CourseWrapper>();

        private static bool _Done = false;
        public static void Update(bool alert = true)
        {
            CourseQueue.Clear();
            var response = ListCourseHandler.Execute();
            var courses = response.Courses;
            if(courses!=null)
            {
                double percent = 100f/courses.Count;
                foreach(var course in courses)
                {
                    loader.Percentage += percent;
                    var CurrentCourse = Courses.Where(c => c.course.Id == course.Id);
                    CourseWrapper CurrentWrapper = CurrentCourse.Count() == 0 ? null : CurrentCourse.First();
                    if(CurrentWrapper==null)
                    {
                        if(alert) Notify($"New course found: {course.Name}");
                        CurrentWrapper = new CourseWrapper(course);
                        Courses.Add(CurrentWrapper);
                        CourseQueue.Enqueue(CurrentWrapper);
                    }
                    else
                    {
                        CurrentWrapper.Update(alert);
                    }
                }
            }
            loader.Close();
        }

        private static Loader loader = null;

        private static MainWindow _instance = null;
        public static MainWindow Instance
        {
            get => _instance;
            set
            {
                _instance?.Close();
                _instance = value;
                loader = new Loader(0);
                StartCoroutine(Fetch());
                Application.Run(loader);
                if (!_Done) throw new ApplicationException("Closed app too early");
                while(CourseQueue.Count > 0) Instance.CreateCourseButton(CourseQueue.Dequeue());
                Application.Run(Instance);
            }
        }

        private static IEnumerator Fetch()
        {
            _Done = false;
            yield return new WaitForSeconds(.1f);
            Resolve();
            _Done = true;
        }
    }
}