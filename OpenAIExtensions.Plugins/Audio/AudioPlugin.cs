using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextToAudio;
using System.ComponentModel;

namespace OpenAIExtensions.Plugins.Audio
{
    public class AudioPlugin
    {
        [KernelFunction, Description("Transcribes audio content to text.")]
        public async Task<string?> AudioToTextAsync(
            Kernel kernel,
           [Description("The filename of the audio file")] string audioFilename,
           [Description("The filestream of the audio file")] Stream audioStream,
            OpenAIAudioToTextExecutionSettings? executionSettings = null,
            CancellationToken ct = default)
        {
            AudioContent audioContent = new(BinaryData.FromStream(audioStream));

            var audioToTextService = kernel.GetRequiredService<IAudioToTextService>();

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

        [KernelFunction, Description("Transcribes audio content to text.")]
        public async Task<string?> AudioFileToTextAsync(
            Kernel kernel,
            [Description("The filename of the audio file")] string path,
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
                kernel,
                fileName,
                audioStreamFromFile,
                executionSettings, ct);
        }

        [KernelFunction, Description("Converts text to audio content.")]
        public async Task<AudioContent?> TextToAudioAsync(
            Kernel kernel,
            [Description("The text to convert to audio.")] string text,
            OpenAITextToAudioExecutionSettings? executionSettings = null,
            CancellationToken ct = default)
        {
            // Set execution settings (optional)
            executionSettings ??= new()
            {
                Voice = "alloy", // The voice to use when generating the audio.
                                 // Supported voices are alloy, echo, fable, onyx, nova, and shimmer.
                ResponseFormat = "mp3", // The format to audio in.
                                        // Supported formats are mp3, opus, aac, and flac.
                Speed = 1.0f // The speed of the generated audio.
                             // Select a value from 0.25 to 4.0. 1.0 is the default.
            };

            var textToAudioService = kernel.GetRequiredService<ITextToAudioService>();
            var audioContent = await textToAudioService.GetAudioContentAsync(text, executionSettings, cancellationToken: ct);

            return audioContent;

        }
    }
}