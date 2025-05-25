using Microsoft.Extensions.Options;
using Presentation.MAUI.Interfaces;
using Presentation.MAUI.Models;
using Presentation.MAUI.Resources.Localization;


namespace Presentation.MAUI.Services
{
    public class OutputFileNameProvider : IOutputFileNameProvider
    {
        private readonly Dictionary<string, string> _patterns;

        public OutputFileNameProvider(IOptions<FileNameSettings> opts)
        {
            _patterns = opts.Value.Patterns;
        }

        public string GetFileName(string key, params object?[]? args)
        {
            if (!_patterns.TryGetValue(key, out var pattern))
                throw new KeyNotFoundException($"{ExceptionMessage.MissingPatternOutputFileName} '{key}'.");

            // If args is null, or contains nulls, replace with empty strings:
            var safeArgs = (args ?? Array.Empty<object?>())
                           .Select(a => a ?? string.Empty)
                           .ToArray();

            // If the pattern has no placeholders, this will just return it verbatim.
            return string.Format(pattern, safeArgs);
        }
    }
}
