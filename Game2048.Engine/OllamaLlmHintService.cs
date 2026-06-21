using Newtonsoft.Json;
using System.Text;

namespace Game2048.Engine
{
    public class OllamaLlmHintService
    {
        public const string Prompt =
            "You are a deterministic decision engine for the game 2048.\r\n\r\nYou MUST follow instructions exactly.\r\n\r\nYou MUST output ONLY valid JSON.\r\nYou are not allowed to output anything except JSON.\r\nNo explanations outside JSON.\r\nNo markdown.\r\nNo extra text.\r\nNo comments.\r\nNo prefix or suffix.\r\n\r\nIf output is not valid JSON, it is considered invalid.\r\n\r\n---\r\n\r\nINPUT:\r\nYou will receive a JSON object containing a 4x4 2048 board.\r\nEmpty cells are represented by null.\r\n\r\n---\r\n\r\nTASK:\r\nChoose the single best next move in 2048.\r\n\r\nLegal moves:\r\nUP, DOWN, LEFT, RIGHT\r\n\r\n---\r\n\r\nSTRATEGY PRIORITIES (in order):\r\n1. Keep highest tile in a stable corner.\r\n2. Maintain monotonic tile ordering.\r\n3. Maximize empty cells.\r\n4. Maximize future merge potential.\r\n5. Avoid breaking board structure.\r\n6. Prefer long-term survival over immediate score.\r\n7. Consider random tile spawn after move.\r\n\r\n---\r\n\r\nOUTPUT RULES (VERY STRICT):\r\nYou MUST output exactly this JSON schema:\r\n\r\n{\r\n  \"MOVE\": \"UP|DOWN|LEFT|RIGHT\",\r\n  \"REASON\": \"max 20 words explanation\"\r\n}\r\n\r\n---\r\n\r\nSTRICT CONSTRAINTS:\r\n- Output must be valid JSON (parsable with no errors)\r\n- No trailing commas\r\n- No extra fields allowed\r\n- MOVE must be exactly one of: UP, DOWN, LEFT, RIGHT\r\n- REASON must be 20 words or fewer\r\n- No line breaks outside JSON values\r\n- No additional text before or after JSON\r\n- No markdown formatting\r\n\r\n---\r\n\r\nEXAMPLES (format only, do not copy reasoning style):\r\n{\"MOVE\":\"LEFT\",\"REASON\":\"Preserves corner and maintains structure for future merges\"}\r\n{\"MOVE\":\"DOWN\",\"REASON\":\"Maximizes empty space and keeps highest tile stable\"}\r\n\r\n---\r\n\r\nBOARD:\r\n{0}";

        private readonly HttpClient client;
        private readonly string modelName;

        public OllamaLlmHintService()
        {
            this.client = new HttpClient();
            this.client.BaseAddress = new Uri("http://127.0.0.1:11434");
            this.client.Timeout = TimeSpan.FromMinutes(2);
            this.modelName = "llama2:latest"; //"qwen3:8b";            
        }

        public async Task<(string, string)> GetHintAsync(int?[,] board)
        {
            if (!await this.IsOllamaRunning())
            {
                return (string.Empty, "Ollama is not running");
            }

            var jsonBoard = JsonConvert.SerializeObject(board);
            var prompt = Prompt.Replace("{0}", jsonBoard);
            var requestBody = new
            {
                model = this.modelName,
                prompt = prompt,
                stream = false,
                format = "json",
            };

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PostAsync("/api/generate", jsonContent);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<OllamaResponse>(responseContent);
            var suggestedMove = JsonConvert.DeserializeObject<SuggestedMove>(result?.response);
            return (suggestedMove?.MOVE ?? string.Empty, suggestedMove?.REASON ?? string.Empty);
        }

        public async Task<bool> IsOllamaRunning()
        {
            try
            {
                var response = await this.client.GetAsync("http://localhost:11434");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
