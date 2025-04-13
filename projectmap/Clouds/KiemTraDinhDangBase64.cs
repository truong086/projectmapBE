namespace projectmap.Clouds
{
    public class KiemTraDinhDangBase64
    {
        public string GetFileTypeByHeader(byte[] fileBytes)
        {
            if (fileBytes.Length < 4)
                return "UNKNOWN";

            // Kiểm tra định dạng JPEG (FF D8 FF)
            if (fileBytes[0] == 0xFF && fileBytes[1] == 0xD8 && fileBytes[2] == 0xFF)
                return "image/jpeg";

            // Kiểm tra định dạng PNG (89 50 4E 47)
            if (fileBytes[0] == 0x89 && fileBytes[1] == 0x50 && fileBytes[2] == 0x4E && fileBytes[3] == 0x47)
                return "image/png";

            // Kiểm tra định dạng PDF (25 50 44 46)
            if (fileBytes[0] == 0x25 && fileBytes[1] == 0x50 && fileBytes[2] == 0x44 && fileBytes[3] == 0x46)
                return "application/pdf";

            // Kiểm tra định dạng MP4 (00 00 00 18 66 74 79 70 69 73 6F 6D)
            if (fileBytes.Length > 10 && fileBytes[0] == 0x00 && fileBytes[1] == 0x00 && fileBytes[2] == 0x00 && fileBytes[3] == 0x18)
            {
                if (fileBytes[4] == 0x66 && fileBytes[5] == 0x74 && fileBytes[6] == 0x79 && fileBytes[7] == 0x70)
                    return "video/mp4";
            }

            return "UNKNOWN";
        }
    }
}
