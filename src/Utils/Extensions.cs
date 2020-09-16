using Google.Apis.Classroom.v1.Data;
using static Classroom_Client.ClassroomData;

namespace Classroom_Client
{
    internal static partial class Utils
    {
        public static void AddAttachments(this StudentSubmission submission, params Attachment[] attachments)
        {
            var request = new ModifyAttachmentsRequest();
            foreach (var attachment in attachments) request.AddAttachments.Add(attachment);
            SubmissionResourceHandler.ModifyAttachments(request, submission.CourseId, submission.CourseWorkId, submission.Id).Execute();
        }
    }
}