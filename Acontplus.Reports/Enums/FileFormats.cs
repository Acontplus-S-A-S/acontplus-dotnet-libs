using System.ComponentModel;

namespace Acontplus.Reports.Enums
{
    public static class FileFormats
    {
        //https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types
        public enum FileContentType
        {
            [Description("application/pdf")]
            PDF,
            [Description("application/vnd.ms-excel")]
            EXCEL,
            [Description("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
            EXCELOPENXML,
            [Description("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
            WORDOPENXML,
            [Description("text/html")]
            HTML5,
            [Description("image/jpeg")]
            IMAGE
        }
        public enum FileExtension
        {
            [Description(".pdf")]
            PDF,
            [Description(".xls")]
            EXCEL,
            [Description(".xlsx")]
            EXCELOPENXML,
            [Description(".docx")]
            WORDOPENXML,
            [Description(".html")]
            HTML5,
            [Description(".jpg")]
            IMAGE
        }
    }
}
