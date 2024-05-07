using Microsoft.Extensions.Logging;
using Polly;

namespace Acerto.Shared.Utilities
{
    public sealed class PollytContextHandler<TSdk> : DelegatingHandler
    {
        private readonly ILogger<TSdk> _logger;

        public PollytContextHandler(ILogger<TSdk> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var pollyContext = new Context
            {
                { "Logger", _logger }
            };

            request.SetPolicyExecutionContext(pollyContext);

            var response = await base.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            return response;
        }
    }
}
