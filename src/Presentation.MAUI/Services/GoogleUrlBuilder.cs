using Microsoft.Extensions.Options;
using Presentation.MAUI.Interfaces;

namespace Presentation.MAUI.Services
{
    /// <summary>
    /// Holds the base URL configuration used for building search URLs.
    /// </summary>
    public class GoogleUrlBuilder : IUrlBuilder
    {
        /// <summary>
        /// Gets the URL configuration options injected from the application's settings.
        /// </summary>
        public UrlBuilder Url { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleUrlBuilder"/> class
        /// with the specified URL configuration options.
        /// </summary>
        /// <param name="opts">
        /// The options wrapper containing a <see cref="UrlBuilder"/> instance
        /// with the configured <see cref="UrlBuilder.BaseUrl"/>.
        /// </param>
        public GoogleUrlBuilder(IOptions<UrlBuilder> opts) => Url = opts.Value;

        /// <summary>
        /// Builds a full Google search URL by appending the given query string
        /// (with spaces replaced by '+') to the configured base URL.
        /// </summary>
        /// <param name="query">The search terms to include in the URL.</param>
        /// <returns>
        /// A complete Google search URL combining <see cref="UrlBuilder.BaseUrl"/>
        /// and the encoded query string, or an empty string if <paramref name="query"/>
        /// is null, empty, or whitespace, or if the base URL is not set.
        /// </returns>
        public string BuildSearchUrl(string query)
        {
            if (string.IsNullOrWhiteSpace(query) & !string.IsNullOrEmpty(Url.BaseUrl))
            {
                return Url.BaseUrl;
            }

            var terms = query
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var encodedQuery = string.Join("+", terms);
            return Url.BaseUrl + encodedQuery;
        }
    }

    public class UrlBuilder
    {
        public string BaseUrl { get; set; } = string.Empty;
    }
}