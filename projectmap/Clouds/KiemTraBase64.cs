using System.Text.RegularExpressions;

namespace projectmap.Clouds
{
    public class KiemTraBase64
    {
        public bool kiemtra(string data)
        {
            if (string.IsNullOrEmpty(data))
                return false;

            // Kiểm tra nếu dữ liệu có độ dài là bội số của 4 (theo chuẩn Base64)
            if (data.Length % 4 != 0)
                return false;

            // Kiểm tra nếu dữ liệu chỉ chứa các ký tự hợp lệ của Base64
            string base64Pattern = @"^[a-zA-Z0-9\+/]*={0,2}$";
            if (!Regex.IsMatch(data, base64Pattern))
                return false;

            try
            {
                // Thử giải mã dữ liệu Base64
                Convert.FromBase64String(data);
                return true; // Nếu không có lỗi chuỗi Base64 là hợp lệ

            }
            catch (FormatException)
            {
                // Nếu có lỗi, dữ liệu không phải Base64 hợp lệ
                return false;
            }
        }
    }
}
