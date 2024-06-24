using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Response
{
    public class Experience
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("context_popup")]
        public string ContextPopup { get; set; }
    }

    public class ResponseItem
    {
        [JsonProperty("popup")]
        public string Popup { get; set; }

        [JsonProperty("explanation")]
        public string Explanation { get; set; }
    }

    public class Question
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("question")]
        public string QuestionText { get; set; }

        [JsonProperty("options")]
        public List<string> Options { get; set; }

        [JsonProperty("correct_answer")]
        public string CorrectAnswer { get; set; }

        [JsonProperty("response")]
        public ResponseItem Response { get; set; }
    }

    public class LearningPoint
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class QuestionsResponse
    {
        [JsonProperty("experience")]
        public Experience Experience { get; set; }

        [JsonProperty("questions")]
        public List<Question> Questions { get; set; }

        [JsonProperty("learning_points")]
        public List<LearningPoint> LearningPoints { get; set; }
    }
}