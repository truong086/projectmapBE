using projectmap.ViewModel;

namespace projectmap.Common
{
    public class MapperData
    {
        public static TModel GanData<TModel, TData>(TModel model, TData dataDTO)
        {
            // Kiểm tra nếu model hoặc data null
            if (model == null || dataDTO == null)
                throw new Exception(Status.DATANULL);

            // Lặp qua các thuộc tính của model
            var modelProperties = model.GetType().GetProperties();
            var dataProperties = dataDTO.GetType().GetProperties();

            var excludedTypes = new[] { typeof(Microsoft.AspNetCore.Http.FormFile), typeof(IFormFile) };  // Danh sách kiểu dữ liệu loại trừ không được Map
            foreach (var property in modelProperties)
            {
                // Tìm thuộc tính tương ứng trong data
                var checkDataModel = dataProperties.FirstOrDefault(x => x.Name == property.Name);

                if (checkDataModel != null && property.CanWrite
                    && !excludedTypes.Contains(checkDataModel.PropertyType) // Loại trừ kiểu dữ liệu không được Map
                    && !excludedTypes.Contains(property.PropertyType) // Loại trừ kiểu dữ liệu không được Map
                    /*&& checkDataModel.PropertyType != typeof(Microsoft.AspNetCore.Http.FormFile)*/)
                {
                    // Lấy giá trị từ data và gán vào model
                    var value = checkDataModel.GetValue(dataDTO);
                    property.SetValue(model, value);
                }
            }

            return model;
        }
    }
}
