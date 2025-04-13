using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using projectmap.ViewModel;

namespace projectmap.Clouds
{
    public class uploadCloud
    {
        public static Cloudinary _cloudinary;
        public static string publicId, Link;

        public static void CloudInaryAccount(string path, string id, Cloud _cloud)
        {
            const long MaxLeng = 10 * 1024 * 1024;
            FileInfo fileCheck = new FileInfo(path);

            if (fileCheck.Length > MaxLeng)
                throw new Exception(Status.MAXLENG);

            var checkFile = new KiemTraDinhDangFile();
            var funcCheck = checkFile.getFileTypeString(path);

            if (funcCheck == Status.IMAGE)
            {
                Account account = new Account(_cloud.Cloudinary_Name, _cloud.Api_Key, _cloud.Serec_Key);
                _cloudinary = new Cloudinary(account);
                UpdateImageCloud(path, id);
            }
            else if (funcCheck == Status.VIDEO)
            {
                Account account = new Account(_cloud.Cloudinary_Name, _cloud.Api_Key, _cloud.Serec_Key);
                _cloudinary = new Cloudinary(account);
                UpdateVideoStringCloudIFrom(path, id);
            }
            else if (funcCheck == Status.AUDIO)
            {
                Account account = new Account(_cloud.Cloudinary_Name, _cloud.Api_Key, _cloud.Serec_Key);
                _cloudinary = new Cloudinary(account);
                UpdateAudioStringCloudIFrom(path, id);
            }
            else if (funcCheck == Status.DOCUMENT)
            {
                Account account = new Account(_cloud.Cloudinary_Name, _cloud.Api_Key, _cloud.Serec_Key);
                _cloudinary = new Cloudinary(account);
                UpdateDocumentStringCloudIFrom(path, id);
            }
        }

        public static void CloudInaryIFromAccount(IFormFile path, string id, Cloud _cloud)
        {
            const long MaxLeng = 10 * 1024 * 1024;
            if (path.Length > MaxLeng)
                throw new Exception(Status.MAXLENG);

            var checkFile = new KiemTraDinhDangFile();
            var funcCheck = checkFile.GetFileType(path);
            if (funcCheck == Status.IMAGE)
            {
                Account account = new Account(_cloud.Cloudinary_Name, _cloud.Api_Key, _cloud.Serec_Key);
                _cloudinary = new Cloudinary(account);
                UpdateImageCloudIFrom(path, id);
            }
            else if (funcCheck == Status.VIDEO)
            {
                Account account = new Account(_cloud.Cloudinary_Name, _cloud.Api_Key, _cloud.Serec_Key);
                _cloudinary = new Cloudinary(account);
                UpdateVideoCloudIFrom(path, id);
            }
            else if (funcCheck == Status.AUDIO)
            {
                Account account = new Account(_cloud.Cloudinary_Name, _cloud.Api_Key, _cloud.Serec_Key);
                _cloudinary = new Cloudinary(account);
                UpdateAudioCloudIFrom(path, id);
            }
            else if (funcCheck == Status.DOCUMENT)
            {
                Account account = new Account(_cloud.Cloudinary_Name, _cloud.Api_Key, _cloud.Serec_Key);
                _cloudinary = new Cloudinary(account);
                UpdateDocumentCloudIFrom(path, id);
            }
        }
        public static void UpdateImageCloud(string path, string id)
        {
            var uploadPath = new ImageUploadParams()
            {
                File = new FileDescription(path),
                Folder = id
            };

            var uploads = _cloudinary.Upload(uploadPath);
            publicId = uploads.PublicId.ToString();
            Link = uploads.Uri.ToString();

            //return (publicid, link); // Trả ra 2 giá trị bằng return
        }

        public static void UpdateImageCloudIFrom(IFormFile path, string id)
        {
            var uploadPath = new ImageUploadParams()
            {
                File = new FileDescription(path.FileName, path.OpenReadStream()),
                Folder = id
            };

            var uploads = _cloudinary.Upload(uploadPath);
            publicId = uploads.PublicId.ToString();
            Link = uploads.Uri.ToString();
        }
        // UP VIDEO
        public static void UpdateVideoCloudIFrom(IFormFile path, string id)
        {
            var uploadPath = new VideoUploadParams()
            {
                File = new FileDescription(path.FileName, path.OpenReadStream()),
                Folder = id,
                UseFilename = true,
                UniqueFilename = false
            };

            var uploads = _cloudinary.Upload(uploadPath);
            publicId = uploads.PublicId.ToString();
            Link = uploads.Uri.ToString();
        }

        // UP VIDEO STRING
        public static void UpdateVideoStringCloudIFrom(string path, string id)
        {
            var uploadPath = new VideoUploadParams()
            {
                File = new FileDescription(path),
                Folder = id,
                UseFilename = true,
                UniqueFilename = false
            };

            var uploads = _cloudinary.Upload(uploadPath);
            publicId = uploads.PublicId.ToString();
            Link = uploads.Uri.ToString();
        }
        // UP AUDIO
        public static void UpdateAudioCloudIFrom(IFormFile path, string id)
        {
            var uploadPath = new RawUploadParams()
            {
                File = new FileDescription(path.FileName, path.OpenReadStream()),
                Folder = id,
                UseFilename = true,
                UniqueFilename = false
            };

            var uploads = _cloudinary.Upload(uploadPath);
            publicId = uploads.PublicId.ToString();
            Link = uploads.Uri.ToString();
        }

        // UP AUDIO STRING
        public static void UpdateAudioStringCloudIFrom(string path, string id)
        {
            var uploadPath = new RawUploadParams()
            {
                File = new FileDescription(path),
                Folder = id,
                UseFilename = true,
                UniqueFilename = false
            };

            var uploads = _cloudinary.Upload(uploadPath);
            publicId = uploads.PublicId.ToString();
            Link = uploads.Uri.ToString();
        }
        // UP DOCUMENT
        public static void UpdateDocumentCloudIFrom(IFormFile path, string id)
        {
            var uploadPath = new RawUploadParams()
            {
                File = new FileDescription(path.FileName, path.OpenReadStream()),
                Folder = id,
                UseFilename = true,
                UniqueFilename = false
            };

            var uploads = _cloudinary.Upload(uploadPath);
            publicId = uploads.PublicId.ToString();
            Link = uploads.Uri.ToString();
        }

        // UP DOCUMENT STRING
        public static void UpdateDocumentStringCloudIFrom(string path, string id)
        {
            var uploadPath = new RawUploadParams()
            {
                File = new FileDescription(path),
                Folder = id,
                UseFilename = true,
                UniqueFilename = false
            };

            var uploads = _cloudinary.Upload(uploadPath);
            publicId = uploads.PublicId.ToString();
            Link = uploads.Uri.ToString();
        }
        // XÓA TOÀN BỘ ẢNH TRONG FOLDER TRONG CLOUD
        public static async void DeleteImageAllOnFoderCloud(string id, Cloud _cloud) // "id" ở đây là tên Folder trên Cloud
        {
            Account account = new Account(_cloud.Cloudinary_Name, _cloud.Api_Key, _cloud.Serec_Key);
            _cloudinary = new Cloudinary(account);
            // Liệt kê tất cả tài nguyên trong thư mục
            var listResource = await _cloudinary.ListResourcesAsync(new ListResourcesParams
            {
                Type = "upload",
                MaxResults = 500 // Giới hạn số tài nguyên trả về
            });

            // Lọc tài nguyên theo thư mục
            var SearchData = listResource.Resources.Where(x => x.PublicId.StartsWith(id)).ToList();

            if (SearchData.Count > 0)
            {
                var selectId = SearchData.Select(x => x.PublicId).ToList();

                var deleteDatas = new DelResParams
                {
                    PublicIds = selectId,
                    Invalidate = true
                };

                await _cloudinary.DeleteResourcesAsync(deleteDatas);


            }

            //var deleteCloudImage = new DeletionParams(publicIdImage);
            //await _cloudinary.DestroyAsync(deleteCloudImage);
        }

        public static async void DeleteImageListFolder(List<string> id, Cloud _cloud)
        {
            try
            {
                foreach (var item in id)
                {
                    // Liệt kê tất cả tài nguyên
                    var listResoure = new ListResourcesParams
                    {
                        Type = "upload", // Tìm ảnh đã tải lên
                        MaxResults = 500 // Giới hạn số tài nguyên trả về mỗi lần
                    };

                    Account account = new Account(_cloud.Cloudinary_Name, _cloud.Api_Key, _cloud.Serec_Key);
                    _cloudinary = new Cloudinary(account);
                    var clouds = await _cloudinary.ListResourcesAsync(listResoure);

                    if (clouds.Resources.Length > 0)
                    {
                        // Lọc các tài nguyên theo PublicId chứa thư mục (folder)
                        var publicIds = clouds.Resources
                            .Where(x => x.PublicId.StartsWith(item)) // Kiểm tra xem PublicId có bắt đầu bằng tên thư mục không
                            .Select(x => x.PublicId)
                            .ToList();

                        // Xóa các ảnh theo public_id
                        var Deleteparams = new DelResParams
                        {
                            PublicIds = publicIds,
                            Invalidate = true // Xóa khỏi cache CDN
                        };

                        await _cloudinary.DeleteResourcesAsync(Deleteparams);


                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Xóa 1 ảnh trong Cloud
        public static async void DeleteImageItemCloud(string publicIdImage)
        {
            var deleteCloudImage = new DeletionParams(publicIdImage);
            await _cloudinary.DestroyAsync(deleteCloudImage);
        }

        // Xóa toàn bộ ảnh trong Folder và xóa folder
        public static async void DeleteAllImageAndFolder(string folder, Cloud _cloud)
        {

            try
            {
                Account account = new Account(_cloud.Cloudinary_Name, _cloud.Api_Key, _cloud.Serec_Key);
                _cloudinary = new Cloudinary(account);


                // Lấy danh sách tất cả tài nguyên
                var listResourcesParams = new ListResourcesParams()
                {
                    Type = "upload",  // Chỉ lấy tài nguyên upload
                    MaxResults = 500  // Số lượng tối đa mỗi lần
                };

                var resources = _cloudinary.ListResources(listResourcesParams);

                // Lọc tài nguyên trong folder cần xóa
                var publicIds = new List<string>();
                foreach (var resource in resources.Resources)
                {
                    if (resource.PublicId.StartsWith(folder + "/")) // Kiểm tra xem tài nguyên thuộc folder hay không
                    {
                        publicIds.Add(resource.PublicId);
                    }
                }

                // Xóa tất cả tài nguyên trong folder
                if (publicIds.Count > 0)
                {
                    var deletionParams = new DelResParams()
                    {
                        PublicIds = publicIds,
                        All = false,
                        Invalidate = true // Làm mới CDN cache
                    };

                    var deletionResult = _cloudinary.DeleteResources(deletionParams);
                    Console.WriteLine($"Đã xóa {deletionResult.Deleted.Count} tài nguyên trong folder '{folder}'.");
                }

                // Sau khi xóa toàn bộ tài nguyên, folder sẽ tự động bị loại bỏ
                Console.WriteLine($"Folder '{folder}' sẽ tự động bị xóa sau khi trống.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error {ex.Message}");
            }
        }
    }
}
