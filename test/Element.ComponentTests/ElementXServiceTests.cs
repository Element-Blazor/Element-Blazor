using Element.X;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Element.ComponentTests
{
    public class ElementXServiceTests
    {
        [Fact]
        public async Task StreamServiceSplitsAndCollectsOfflineResponse()
        {
            var service = new ElementXStreamService();

            var chunks = await service.StreamAsync(new XStreamRequest
            {
                RequestId = "stream-1",
                ResponseText = "Hello streaming service",
                ChunkSize = 5,
                ChunkDelay = TimeSpan.Zero
            }).ToListAsync();

            Assert.True(chunks.First().IsStart);
            Assert.True(chunks.Last().IsEnd);
            Assert.Equal("Hello streaming service", string.Concat(chunks.Select(x => x.Content)));
            Assert.Equal("Hello streaming service", await service.CollectAsync(chunks.ToAsyncEnumerable()));
        }

        [Fact]
        public async Task StreamServiceParsesServerSentEvents()
        {
            var service = new ElementXStreamService();
            var lines = new[] { "data: hello", "data: world", string.Empty, "data: done" };

            var chunks = await service.ParseServerSentEventsAsync(lines.ToAsyncEnumerable()).ToListAsync();

            Assert.Equal(2, chunks.Count);
            Assert.Equal("hello\nworld", chunks[0].Content);
            Assert.Equal("done", chunks[1].Content);
        }

        [Fact]
        public async Task RequestServiceSendsAndCancelsRequests()
        {
            var streamService = new ElementXStreamService();
            var requestService = new ElementXRequestService(streamService);

            var result = await requestService.SendAsync(new XRequest
            {
                RequestId = "request-1",
                ConversationId = "roadmap",
                Prompt = "Build matrix",
                Transport = (request, token) => YieldChunksAsync(new[] { "Build", " matrix" }, token)
            });

            Assert.False(result.Canceled);
            Assert.Equal("request-1", result.RequestId);
            Assert.Equal("Build matrix", result.UserMessage.Content);
            Assert.Equal("Build matrix", result.AssistantMessage.Content);
            Assert.False(requestService.HasActiveRequest("request-1"));

            var cancelResult = await requestService.SendAsync(new XRequest
            {
                RequestId = "cancel-1",
                Prompt = "Stop me",
                Transport = (request, token) => YieldAndCancelAsync(requestService, request, token)
            });

            Assert.True(cancelResult.Canceled);
            Assert.Equal("partial", cancelResult.AssistantMessage.Content);
            Assert.False(requestService.HasActiveRequest("cancel-1"));
        }

        [Fact]
        public async Task RecordServiceFallsBackWhenBrowserSupportIsMissing()
        {
            var service = new ElementXRecordService(new ThrowingJsRuntime());

            var supported = await service.IsSupportedAsync();
            var result = await service.StartAsync();

            Assert.False(supported);
            Assert.False(result.Supported);
            Assert.Equal("unsupported", result.State);
        }

        private static async IAsyncEnumerable<XStreamChunk> YieldChunksAsync(
            IEnumerable<string> values,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var value in values)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return new XStreamChunk { Content = value };
                await Task.Yield();
            }
        }

        private static async IAsyncEnumerable<XStreamChunk> YieldAndCancelAsync(
            ElementXRequestService requestService,
            XRequest request,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            yield return new XStreamChunk { Content = "partial" };
            await Task.Yield();
            requestService.Cancel(request.RequestId);
            cancellationToken.ThrowIfCancellationRequested();
            yield return new XStreamChunk { Content = " never" };
        }

        private sealed class ThrowingJsRuntime : IJSRuntime
        {
            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object[] args)
            {
                throw new JSException("missing runtime");
            }

            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object[] args)
            {
                throw new JSException("missing runtime");
            }
        }
    }

    internal static class AsyncEnumerableTestExtensions
    {
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
        {
            var result = new List<T>();
            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                result.Add(item);
            }

            return result;
        }

        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(
            this IEnumerable<T> source,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var item in source)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
                await Task.Yield();
            }
        }
    }
}
