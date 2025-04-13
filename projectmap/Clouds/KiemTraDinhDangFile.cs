using projectmap.ViewModel;

namespace projectmap.Clouds
{
    public class KiemTraDinhDangFile
    {
        private readonly HashSet<string> VideoMimeTypes = new HashSet<string> { "video/mp4", "video/quicktime", "video/x-msvideo" };
        private readonly HashSet<string> AudioMimeTypes = new HashSet<string> { "audio/mpeg", "audio/wav" };
        private readonly HashSet<string> DocumentMimeTypes = new HashSet<string>
        {
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };
        private readonly HashSet<string> ImageMimeTypes = new HashSet<string>
        {
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/bmp",
            "image/webp"
        };

        private readonly HashSet<string> VideoExtensions = new HashSet<string> { ".mp4", ".mov", ".avi", ".mkv" };
        private readonly HashSet<string> AudioExtensions = new HashSet<string> { ".mp3", ".wav", ".aac" };
        private readonly HashSet<string> DocumentExtensions = new HashSet<string> { ".pdf", ".doc", ".docx" };
        private readonly HashSet<string> ImageExtensions = new HashSet<string>
        {
            ".jpeg", ".jpg", ".png", ".gif", ".bmp", ".webp"
        };
        public string GetFileType(IFormFile file)
        {
            var contentType = file.ContentType;
            var extension = Path.GetExtension(file.FileName)?.ToLower();

            if (VideoExtensions.Contains(extension) || VideoMimeTypes.Contains(contentType))
                return Status.VIDEO;

            if (ImageExtensions.Contains(extension) || ImageMimeTypes.Contains(contentType))
                return Status.IMAGE;

            if (AudioExtensions.Contains(extension) || AudioMimeTypes.Contains(contentType))
                return Status.AUDIO;

            if (DocumentExtensions.Contains(extension) || DocumentMimeTypes.Contains(contentType))
                return Status.DOCUMENT;

            return Status.UNKNOWN;
        }

        public string getFileTypeString(string file)
        {
            var extension = Path.GetExtension(file)?.ToLower();

            if (VideoExtensions.Contains(extension))
                return Status.VIDEO;

            if (ImageExtensions.Contains(extension))
                return Status.IMAGE;

            if (AudioExtensions.Contains(extension))
                return Status.AUDIO;

            if (DocumentExtensions.Contains(extension))
                return Status.DOCUMENT;

            return Status.UNKNOWN;
        }
    }
}
