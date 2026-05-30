using Microsoft.JSInterop;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Element.X
{
    public class ElementXRecordService
    {
        private readonly IJSRuntime jsRuntime;

        public ElementXRecordService(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async Task<bool> IsSupportedAsync(CancellationToken cancellationToken = default)
        {
            if (jsRuntime == null)
            {
                return false;
            }

            try
            {
                return await jsRuntime.InvokeAsync<bool>("elementXRecordIsSupported", cancellationToken);
            }
            catch
            {
                return false;
            }
        }

        public async Task<XRecordResult> StartAsync(XRecordOptions options = null, CancellationToken cancellationToken = default)
        {
            options ??= new XRecordOptions();
            if (!await IsSupportedAsync(cancellationToken))
            {
                return Unsupported();
            }

            try
            {
                var result = await jsRuntime.InvokeAsync<XRecordResult>("elementXRecordStart", cancellationToken, options);
                return result ?? Unsupported();
            }
            catch (Exception ex)
            {
                return new XRecordResult
                {
                    Supported = false,
                    Started = false,
                    State = "error",
                    Message = ex.Message
                };
            }
        }

        public async Task<XRecordResult> StopAsync(string sessionId = null, CancellationToken cancellationToken = default)
        {
            if (jsRuntime == null)
            {
                return Unsupported();
            }

            try
            {
                var result = await jsRuntime.InvokeAsync<XRecordResult>("elementXRecordStop", cancellationToken, sessionId);
                return result ?? Unsupported();
            }
            catch (Exception ex)
            {
                return new XRecordResult
                {
                    Supported = false,
                    Started = false,
                    State = "error",
                    Message = ex.Message
                };
            }
        }

        public async Task<XRecordResult> CancelAsync(string sessionId = null, CancellationToken cancellationToken = default)
        {
            if (jsRuntime == null)
            {
                return Unsupported();
            }

            try
            {
                var result = await jsRuntime.InvokeAsync<XRecordResult>("elementXRecordCancel", cancellationToken, sessionId);
                return result ?? Unsupported();
            }
            catch (Exception ex)
            {
                return new XRecordResult
                {
                    Supported = false,
                    Started = false,
                    State = "error",
                    Message = ex.Message
                };
            }
        }

        private static XRecordResult Unsupported()
        {
            return new XRecordResult
            {
                Supported = false,
                Started = false,
                State = "unsupported",
                Message = "Browser audio recording is not available."
            };
        }
    }
}
