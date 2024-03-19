using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace OpenAIExtensions.Services
{
    public interface IAIAudioService
    {
        Task<string?> AudioToTextAsync(
            string fileName,
            Stream audioStream,
            OpenAIAudioToTextExecutionSettings? executionSettings = null,
            CancellationToken ct = default);

        Task<string?> AudioToTextAsync(
             string path,
             OpenAIAudioToTextExecutionSettings? executionSettings = null,
             CancellationToken ct = default);
    }

    /// <summary>
    /// https://drlee.io/transforming-audio-to-text-with-openais-speech-to-text-api-a-practical-step-by-step-guide-8139e4e65fdf
    /// </summary>
    public class AIAudioService : IAIAudioService
    {
        private readonly Kernel _kernel;
        private readonly ILogger<AIAudioService> _logger;

        public AIAudioService(
            Kernel kernel,
            ILogger<AIAudioService> logger)
        {
            _kernel = kernel;
            _logger = logger;
        }

        public async Task<string?> AudioToTextAsync(
            string audioFilename,
            Stream audioStream,
            OpenAIAudioToTextExecutionSettings? executionSettings = null,
            CancellationToken ct = default)
        {
            AudioContent audioContent = new(BinaryData.FromStream(audioStream));

            var audioToTextService = _kernel.GetRequiredService<IAudioToTextService>();

            executionSettings ??= new(audioFilename)
            {
                Prompt = null, // An optional text to guide the model's style or continue a previous audio segment.
                               // The prompt should match the audio language.
                Language = "en", // The language of the audio data as two-letter ISO-639-1 language code (e.g. 'en' or 'es').
                ResponseFormat = "text", // The format to return the transcribed text in.
                                         // Supported formats are json, text, srt, verbose_json, or vtt. Default is 'json'.
                Temperature = 0.3f, // The randomness of the generated text.
                                    // Select a value from 0.0 to 1.0. 0 is the default.
            };

            var textContent = await audioToTextService.GetTextContentAsync(
                audioContent,
                executionSettings: executionSettings,
                cancellationToken: ct);

            return textContent.Text;
        }

        public async Task<string?> AudioToTextAsync(
            string path,
            OpenAIAudioToTextExecutionSettings? executionSettings = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
            }

            var fileName = Path.GetFileName(path);

            using Stream audioStreamFromFile = File.OpenRead(path);

            return await AudioToTextAsync(
                fileName,
                audioStreamFromFile,
                executionSettings, ct);
        }

    }
}