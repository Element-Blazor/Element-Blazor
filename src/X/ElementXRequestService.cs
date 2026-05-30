using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Element.X
{
    public class ElementXRequestService
    {
        private readonly ElementXStreamService streamService;
        private readonly ConcurrentDictionary<string, CancellationTokenSource> activeRequests = new();

        public ElementXRequestService(ElementXStreamService streamService)
        {
            this.streamService = streamService ?? throw new ArgumentNullException(nameof(streamService));
        }

        public bool HasActiveRequest(string requestId)
        {
            return !string.IsNullOrWhiteSpace(requestId) && activeRequests.ContainsKey(requestId);
        }

        public void Cancel(string requestId)
        {
            if (string.IsNullOrWhiteSpace(requestId))
            {
                return;
            }

            if (activeRequests.TryGetValue(requestId, out var source))
            {
                source.Cancel();
            }
        }

        public async Task<XRequestResult> SendAsync(XRequest request, CancellationToken cancellationToken = default)
        {
            request ??= new XRequest();
            PrepareRequest(request);
            var userMessage = CreateUserMessage(request);
            var assistantMessage = CreateAssistantMessage(request);
            var chunks = new List<XStreamChunk>();
            var builder = new StringBuilder();

            try
            {
                await foreach (var chunk in StreamAsync(request, cancellationToken))
                {
                    chunks.Add(chunk);
                    if (!string.IsNullOrEmpty(chunk.Content))
                    {
                        builder.Append(chunk.Content);
                    }
                }

                assistantMessage.Content = builder.ToString();
                assistantMessage.Loading = false;
                assistantMessage.Typing = false;
                return new XRequestResult
                {
                    RequestId = request.RequestId,
                    ConversationId = request.ConversationId,
                    UserMessage = request.IncludeUserMessage ? userMessage : null,
                    AssistantMessage = assistantMessage,
                    Chunks = chunks
                };
            }
            catch (OperationCanceledException)
            {
                assistantMessage.Content = builder.ToString();
                assistantMessage.Loading = false;
                assistantMessage.Typing = false;
                return new XRequestResult
                {
                    RequestId = request.RequestId,
                    ConversationId = request.ConversationId,
                    UserMessage = request.IncludeUserMessage ? userMessage : null,
                    AssistantMessage = assistantMessage,
                    Chunks = chunks,
                    Canceled = true
                };
            }
            catch (Exception ex)
            {
                assistantMessage.Content = builder.ToString();
                assistantMessage.Loading = false;
                assistantMessage.Typing = false;
                return new XRequestResult
                {
                    RequestId = request.RequestId,
                    ConversationId = request.ConversationId,
                    UserMessage = request.IncludeUserMessage ? userMessage : null,
                    AssistantMessage = assistantMessage,
                    Chunks = chunks,
                    Error = ex.Message
                };
            }
        }

        public async IAsyncEnumerable<XStreamChunk> StreamAsync(
            XRequest request,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            request ??= new XRequest();
            PrepareRequest(request);

            using var requestSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            activeRequests[request.RequestId] = requestSource;

            try
            {
                var stream = request.Transport != null
                    ? request.Transport(request, requestSource.Token)
                    : streamService.StreamAsync(new XStreamRequest
                    {
                        RequestId = request.RequestId,
                        ConversationId = request.ConversationId,
                        Prompt = request.Prompt,
                        Messages = request.Messages,
                        Attachments = request.Attachments
                    }, requestSource.Token);

                await foreach (var chunk in stream.WithCancellation(requestSource.Token))
                {
                    yield return chunk;
                }
            }
            finally
            {
                activeRequests.TryRemove(request.RequestId, out _);
            }
        }

        public XMessageItem CreateUserMessage(XRequest request)
        {
            request ??= new XRequest();
            PrepareRequest(request);
            return new XMessageItem
            {
                Id = $"{request.RequestId}-user",
                Role = XMessageRole.User,
                Header = request.UserName,
                Content = request.Prompt,
                Footer = "sent",
                AvatarText = string.IsNullOrWhiteSpace(request.UserName) ? "U" : request.UserName.Substring(0, 1),
                Attachments = request.Attachments?.ToList()
            };
        }

        public XMessageItem CreateAssistantMessage(XRequest request)
        {
            request ??= new XRequest();
            PrepareRequest(request);
            return new XMessageItem
            {
                Id = $"{request.RequestId}-assistant",
                Role = XMessageRole.Assistant,
                Header = request.AssistantName,
                Content = string.Empty,
                Footer = "streaming",
                AvatarIcon = "cpu",
                Loading = true,
                Typing = true
            };
        }

        private static void PrepareRequest(XRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RequestId))
            {
                request.RequestId = Guid.NewGuid().ToString("N");
            }

            if (string.IsNullOrWhiteSpace(request.ConversationId))
            {
                request.ConversationId = "default";
            }
        }
    }
}
