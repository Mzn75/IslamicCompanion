using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IslamicCompanion.Models
{
    public class QuranVerse
    {
        public string SurahName { get; set; }
        public string SurahNameArabic { get; set; }
        public int SurahNumber { get; set; }
        public int AyahNumber { get; set; }
        public string ArabicText { get; set; }
        public string Translation { get; set; }
    }

    // JSON Mapping for Quran.com API
    public class QuranComResponse
    {
        [JsonPropertyName("verse")]
        public QuranComVerse Verse { get; set; }
    }

    public class QuranComVerse
    {
        [JsonPropertyName("verse_key")]
        public string VerseKey { get; set; }

        [JsonPropertyName("text_uthmani")]
        public string TextUthmani { get; set; }

        [JsonPropertyName("translations")]
        public List<QuranTranslation> Translations { get; set; }
    }

    public class QuranChapterResponse
    {
        [JsonPropertyName("chapter")]
        public QuranChapter Chapter { get; set; }
    }

    public class QuranChapter
    {
        [JsonPropertyName("name_simple")]
        public string NameSimple { get; set; }

        [JsonPropertyName("verses_count")]
        public int VersesCount { get; set; }
    }

    public class QuranTranslation
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
