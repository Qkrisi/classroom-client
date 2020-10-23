using System.IO;
using System.Web;
using System.Collections.Generic;
using Google.Apis.Drive.v3;
using static System.IO.File;
using File = Google.Apis.Drive.v3.Data.File;

namespace Classroom_Client
{
    public static class DriveHandler
    {
        public static DriveService driveService = null;

        public static File[] UpdloadFile(params string[] path)
        {
            List<File> files = new List<File>();
            foreach (string FilePath in path)
            {
                var info = new FileInfo(FilePath);
                string MimeType = MimeMapping.GetMimeMapping(info.Name);
                var request = new FilesResource.CreateMediaUpload(driveService, new File()
                {
                    Name = info.Name,
                    Description = "A file uploaded from Classroom Client",
                    MimeType = MimeType,
                }, new MemoryStream(ReadAllBytes(FilePath)), MimeType);
                request.Upload();
                files.Add(request.ResponseBody);
            }
            return files.ToArray();
        }
    }
}