using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace OpenAIExtensions.Services
{
    public interface IAIAudioService
    {
        Task<AudioTranscription?> TranscribeAsync(string fileName, Stream audioStream);

        Task<AudioTranscription?> TranscribeAsync(string path);

        Task<AudioTranslation?> TranslateAsync(string path);

        Task<AudioTranslation?> TranslateAsync(string fileName, Stream audioStream);
    }

    /// <summary>
    /// https://drlee.io/transforming-audio-to-text-with-openais-speech-to-text-api-a-practical-step-by-step-guide-8139e4e65fdf
    /// </summary>
    public class AIAudioService : IAIAudioService
    {
        private readonly OpenAIClient _client;

        private readonly ILogger<AIAudioService> _logger;

        private readonly string _deploymentName = "whisper-001";

        public AIAudioService(
            IAIBroker aIBroker,
            ILogger<AIAudioService> logger,
            string? deploymentName = null)
        {
            _logger = logger;
            _client = aIBroker.GetClient();

            if (!string.IsNullOrEmpty(deploymentName))
            {
                _deploymentName = deploymentName;
            }
        }

        public async Task<AudioTranscription?> TranscribeAsync(string fileName, Stream audioStream)
        {
            var transcriptionOptions = new AudioTranscriptionOptions()
            {
                DeploymentName = _deploymentName,
                AudioData = BinaryData.FromStream(audioStream),
                ResponseFormat = AudioTranscriptionFormat.Verbose,
                Filename = fileName
            };

            Response<AudioTranscription> transcriptionResponse
                = await _client.GetAudioTranscriptionAsync(transcriptionOptions);

            var transcription = transcriptionResponse.Value;
            return transcription;
        }

        public async Task<AudioTranscription?> TranscribeAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
            }

            var fileName = Path.GetFileName(path);

            using Stream audioStreamFromFile = File.OpenRead(path);

            return await TranscribeAsync(fileName, audioStreamFromFile);
        }

        public async Task<AudioTranslation?> TranslateAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
            }

            var fileName = Path.GetFileName(path);

            using Stream audioStreamFromFile = File.OpenRead(path);

            return await TranslateAsync(fileName, audioStreamFromFile);
        }

        public async Task<AudioTranslation?> TranslateAsync(string fileName, Stream audioStream)
        {
            var translationOptions = new AudioTranslationOptions()
            {
                DeploymentName = _deploymentName,
                AudioData = BinaryData.FromStream(audioStream),
                ResponseFormat = AudioTranslationFormat.Verbose,
                Filename = fileName,
            };

            Response<AudioTranslation> translationResponse = await _client.GetAudioTranslationAsync(translationOptions);

            return translationResponse?.Value;
        }
    }
}