using System.Linq;
using System.Collections.Generic;
using Google.Apis.Classroom.v1.Data;
using static Classroom_Client.ClassroomData;
using static Classroom_Client.Utils;
using static Classroom_Client.Settings;

namespace Classroom_Client
{
    public class CourseWorkWrapper
    {
        public readonly CourseWork Work;
        public readonly List<SubmissionWrapper> Submissions = new List<SubmissionWrapper>();

        public void Update(bool alert = true)
        {
            foreach (var submission in SubmissionResourceHandler.List(Work.CourseId, Work.Id).Execute().StudentSubmissions)
            {
                if (!Submissions.Any(s => s.Submission.Id == submission.Id))
                {
                    Submissions.Add(new SubmissionWrapper(submission));
                    if(alert) Notify($"New submission in {Work.Title}!");
                }
            }
        }

        public CourseWorkWrapper(CourseWork work)
        {
            Work = work;
            Update(settings.DefaultNotificatons);
        }
    }
    public class SubmissionWrapper
    {
        public readonly StudentSubmission Submission;

        public void TurnIn() => SubmissionResourceHandler.TurnIn(new TurnInStudentSubmissionRequest(), Submission.CourseId, Submission.CourseWorkId, Submission.Id).Execute();
        public void Reclaim() => SubmissionResourceHandler.Reclaim(new ReclaimStudentSubmissionRequest(), Submission.CourseId, Submission.CourseWorkId, Submission.Id).Execute();

        public SubmissionWrapper(StudentSubmission submission)
        {
            Submission = submission;
        }
    }
}