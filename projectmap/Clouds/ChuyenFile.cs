namespace projectmap.Clouds
{
    public class ChuyenFile
    {
        public IFormFile chuyendoi(string data, string fileName)
        {
            var base64Data = data;

            // Chuyển chuỗi Base64 thành byte array
            byte[] fileBytes = Convert.FromBase64String(base64Data);

            var kiemTraBase64 = new KiemTraDinhDangBase64();
            var kiemTra = kiemTraBase64.GetFileTypeByHeader(fileBytes);

            if (kiemTra == "image/jpeg")
                fileName = fileName + ".jpg";
            if (kiemTra == "image/png")
                fileName = fileName + ".png";
            if (kiemTra == "application/pdf")
                fileName = fileName + ".pdf";
            if (kiemTra == "video/mp4")
                fileName = fileName + ".mp4";

            var file = new ChuyenDoiIFormFile(fileBytes, fileName, kiemTra);
            return file;

        }
    }
}
