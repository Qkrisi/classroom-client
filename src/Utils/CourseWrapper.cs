using System.Linq;
using System.Collections.Generic;
using Google.Apis.Classroom.v1.Data;
using static Google.Apis.Classroom.v1.CoursesResource;
using static Classroom_Client.ClassroomData;
using static Classroom_Client.Utils;
using static Classroom_Client.Settings;

namespace Classroom_Client
{
    public class CourseWrapper
    {
        public readonly Course course;
        public readonly List<CourseWorkWrapper> Assignments = new List<CourseWorkWrapper>();
        public readonly List<Announcement> Announcements = new List<Announcement>();

        private readonly RequestBatch Requests = null;

        public void Update(bool alert = true)
        {
            var WorkResponse = Requests.CourseWorkRequest.Execute();
            if (WorkResponse.CourseWork != null)
            {
                foreach (var courseWork in WorkResponse.CourseWork)
                {
                    if (!Assignments.Any(a => a.Work.Id == courseWork.Id))
                    {
                        Assignments.Add(new CourseWorkWrapper(courseWork));
                        if (alert) Notify($"New assigment in {course.Name}: {courseWork.Title}");
                    }
                    else
                    {
                        Assignments.Where(a => a.Work.Id == courseWork.Id).First().Update(alert);
                    }
                }
            }
            var AnnouncementResponse = Requests.AnnouncementRequest.Execute();
            if (AnnouncementResponse.Announcements != null)
            {
                foreach (var announcement in AnnouncementResponse.Announcements)
                {
                    if (!Announcements.Any(a => a.Id == announcement.Id))
                    {
                        Announcements.Add(announcement);
                        if (alert) Notify($"New announcement in {course.Name}!");
                    }
                }
            }
        }

        public CourseWrapper(Course _course)
        {
            course = _course;
            Requests = new RequestBatch(course.Id);
            Update(settings.DefaultNotificatons);
        }
    }

    public class RequestBatch
    {
        public readonly CourseWorkResource.ListRequest CourseWorkRequest;
        public readonly AnnouncementsResource.ListRequest AnnouncementRequest;

        public RequestBatch(string ID)
        {
            CourseWorkRequest = WorkResourceHanlder.List(ID);
            AnnouncementRequest = AnnouncementResourceHandler.List(ID);
        }
    }
}