using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Element.X
{
    public class ElementXStreamService
    {
        public async IAsyncEnumerable<XStreamChunk> StreamAsync(
            XStreamRequest request,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            request ??= new XStreamRequest();
            var content = ResolveResponseText(request);
            var chunkSize = request.ChunkSize <= 0 ? 6 : request.ChunkSize;

            yield return new XStreamChunk
            {
                Id = request.RequestId,
                IsStart = true
            };

            for (var index = 0; index < content.Length; index += chunkSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (request.ChunkDelay > TimeSpan.Zero)
                {
                    await Task.Delay(request.ChunkDelay, cancellationToken);
                }

                var length = Math.Min(chunkSize, content.Length - index);
                yield return new XStreamChunk
                {
                    Id = request.RequestId,
                    Content = content.Substring(index, length)
                };
            }

            yield return new XStreamChunk
            {
                Id = request.RequestId,
                IsEnd = true
            };
        }

        public async IAsyncEnumerable<XStreamChunk> ParseServerSentEventsAsync(
            IAsyncEnumerable<string> lines,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (lines == null)
            {
                yield break;
            }

            var data = new StringBuilder();
            await foreach (var line in lines.WithCancellation(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (string.IsNullOrEmpty(line))
                {
                    if (data.Length > 0)
                    {
                        yield return new XStreamChunk { Content = data.ToString() };
                        data.Clear();
                    }
                    continue;
                }

                if (line.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    if (data.Length > 0)
                    {
                        data.Append('\n');
                    }
                    data.Append(line.Substring(5).TrimStart());
                }
            }

            if (data.Length > 0)
            {
                yield return new XStreamChunk { Content = data.ToString() };
            }
        }

        public Task<string> CollectAsync(
            IAsyncEnumerable<XStreamChunk> chunks,
            CancellationToken cancellationToken = default)
        {
            return CollectInternalAsync(chunks, cancellationToken);
        }

        private static async Task<string> CollectInternalAsync(
            IAsyncEnumerable<XStreamChunk> chunks,
            CancellationToken cancellationToken)
        {
            var builder = new StringBuilder();
            if (chunks == null)
            {
                return string.Empty;
            }

            await foreach (var chunk in chunks.WithCancellation(cancellationToken))
            {
                if (!string.IsNullOrEmpty(chunk?.Content))
                {
                    builder.Append(chunk.Content);
                }
            }

            return builder.ToString();
        }

        private static string ResolveResponseText(XStreamRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.ResponseText))
            {
                return request.ResponseText;
            }

            var prompt = string.IsNullOrWhiteSpace(request.Prompt) ? "当前请求" : request.Prompt.Trim();
            return $"已收到：{prompt}。这是 ElementXStreamService 的离线流式响应，可替换 Transport 接入真实后端。";
        }
    }
}
