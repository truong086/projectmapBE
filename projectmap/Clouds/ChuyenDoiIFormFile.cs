namespace projectmap.Clouds
{
    public class ChuyenDoiIFormFile : IFormFile
    {
        private readonly Stream _stream;

        public ChuyenDoiIFormFile(byte[] fileBytes, string fileName, string contentType)
        {
            _stream = new MemoryStream(fileBytes);
            FileName = fileName;
            ContentType = contentType;
        }

        public string ContentType { get; set; }
        public string FileName { get; set; }
        public long Length => _stream.Length;
        public string Name => "file";

        public string ContentDisposition => throw new NotImplementedException();

        public IHeaderDictionary Headers => throw new NotImplementedException();

        public Stream OpenReadStream() => _stream;

        public void CopyTo(Stream target)
        {
            _stream.CopyTo(target);
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            return _stream.CopyToAsync(target, cancellationToken);
        }
    }
}
