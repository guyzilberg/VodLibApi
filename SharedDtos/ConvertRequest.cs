using System;

namespace SharedDtos
{
    public class ConvertRequest
    {
        public string UserID { get; set; }
        public string MediaID { get; set; }
        public string MediaSafeName { get; set; }
        public int? ConvertRequestID { get; set; }
        public string FilePath { get; set; }
    }
}
