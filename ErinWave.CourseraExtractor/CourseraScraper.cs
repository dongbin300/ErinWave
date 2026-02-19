using System;

namespace ErinWave.CourseraExtractor
{
    public class ScriptInfo
    {
        public string Content { get; set; } = string.Empty;
        public ScriptType Type { get; set; }
        public bool HasSrc { get; set; }
        public string Src { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
    }

    public class ScriptExtractionResult
    {
        public string Url { get; set; } = string.Empty;
        public List<ScriptInfo> Scripts { get; set; } = new List<ScriptInfo>();
        public int TotalScripts { get; set; }
        public DateTime ExtractionTime { get; set; }
    }

    public enum ScriptType
    {
        KoreanSubtitle,
        DownloadSection,
        DownloadLink,
        JavaScript,
        Module,
        JSON,
        LD_JSON,
        ImportMap,
        Custom,
        Unknown
    }
}