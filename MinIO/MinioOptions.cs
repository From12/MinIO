namespace MinIOStorage
{
    public class MinioOptions
    {
        public const string SectionName = "MinIO";

        public string Endpoint { get; set; } = "localhost:9000";

        public bool UseSSL { get; set; } = false;

        public string AccessKey { get; set; } = string.Empty;

        public string SecretKey { get; set; } = string.Empty;

        public string DefaultBucket { get; set; } = "files";

        public int PresignedUrlExpiryMinutes { get; set; } = 60;

        public bool CreateBucketIfNotExists { get; set; } = true;
    }
}
