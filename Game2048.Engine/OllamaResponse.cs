namespace Game2048.Engine
{

    public class OllamaResponse
    {
        public string model { get; set; }
        public DateTime created_at { get; set; }
        public string response { get; set; }
        public bool done { get; set; }
        public string done_reason { get; set; }
        public int[] context { get; set; }
        public long total_duration { get; set; }
        public long load_duration { get; set; }
        public long prompt_eval_count { get; set; }
        public long prompt_eval_duration { get; set; }
        public long eval_count { get; set; }
        public long eval_duration { get; set; }
    }

}
