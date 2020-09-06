using Google.Apis.Auth.OAuth2;
using Google.Apis.Classroom.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using static Google.Apis.Classroom.v1.ClassroomService;
using static Classroom_Client.Settings;

namespace Classroom_Client
{
    public static class ClassroomResolver
    {
        static readonly string[] Scopes =
        {
            Scope.ClassroomCoursesReadonly,
            Scope.ClassroomAnnouncementsReadonly,
            Scope.ClassroomCourseworkMe,
            Scope.ClassroomStudentSubmissionsMeReadonly,
            Scope.ClassroomTopicsReadonly
        };

        static readonly string ApplicationName = "Classroom Client";

        public static void Resolve()
        {
            UserCredential credential;

            using (var stream =
                new FileStream(settings.AuthPath, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
            ClassroomData.Service = new ClassroomService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            ClassroomData.Update(false);
        }
    }
}