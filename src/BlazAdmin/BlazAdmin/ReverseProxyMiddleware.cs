using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazAdmin
{
    public class ReverseProxyMiddleware
    {
        private static readonly HttpClient _httpClient;
        private readonly RequestDelegate _nextMiddleware;
        private static HttpClientHandler httpClientHandler;

        static ReverseProxyMiddleware()
        {
            httpClientHandler = new HttpClientHandler();
            httpClientHandler.AllowAutoRedirect = false;
            httpClientHandler.CookieContainer = new System.Net.CookieContainer();
            _httpClient = new HttpClient(httpClientHandler);
        }
        public ReverseProxyMiddleware(RequestDelegate nextMiddleware)
        {
            _nextMiddleware = nextMiddleware;
        }
        private HttpRequestMessage CreateTargetMessage(HttpContext context, Uri targetUri)
        {
            var requestMessage = new HttpRequestMessage();
            CopyFromOriginalRequestContentAndHeaders(context, requestMessage);

            requestMessage.RequestUri = targetUri;
            requestMessage.Method = GetMethod(context.Request.Method);

            return requestMessage;
        }

        private void CopyFromOriginalRequestContentAndHeaders(HttpContext context, HttpRequestMessage requestMessage)
        {
            var requestMethod = context.Request.Method;

            if (!HttpMethods.IsGet(requestMethod) &&
              !HttpMethods.IsHead(requestMethod) &&
              !HttpMethods.IsDelete(requestMethod) &&
              !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(context.Request.Body);
                requestMessage.Content = streamContent;
            }

            foreach (var header in context.Request.Headers)
            {
                requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }
        private void CopyFromTargetResponseHeaders(HttpContext context, HttpResponseMessage responseMessage)
        {
            foreach (var header in responseMessage.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
            var cookies = httpClientHandler.CookieContainer.GetCookies(new Uri("http://localhost:5050"));
            //var protector = context.RequestServices.GetDataProtector(string.Empty);
            //context.Response.Headers.TryGetValue("Set-Cookie", out var cookieString);
            //if (cookieString != string.Empty)
            //{
            //}
            foreach (Cookie cookie in cookies)
            {
                //protector.Unprotect(Convert.FromBase64String(cookie.Value));
                context.Response.Cookies.Append(cookie.Name, cookie.Value);
            }
            context.Response.Headers.Remove("transfer-encoding");
        }
        private static HttpMethod GetMethod(string method)
        {
            if (HttpMethods.IsDelete(method)) return HttpMethod.Delete;
            if (HttpMethods.IsGet(method)) return HttpMethod.Get;
            if (HttpMethods.IsHead(method)) return HttpMethod.Head;
            if (HttpMethods.IsOptions(method)) return HttpMethod.Options;
            if (HttpMethods.IsPost(method)) return HttpMethod.Post;
            if (HttpMethods.IsPut(method)) return HttpMethod.Put;
            if (HttpMethods.IsTrace(method)) return HttpMethod.Trace;
            return new HttpMethod(method);
        }
        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/proxy", StringComparison.CurrentCultureIgnoreCase, out var targetPath))
            {
                await _nextMiddleware(context);
                return;
            }

            var targetUri = "http://localhost:5050" + targetPath + context.Request.QueryString;
            var targetRequestMessage = CreateTargetMessage(context, new Uri(targetUri));

            using (var responseMessage = await _httpClient.SendAsync(targetRequestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
            {
                context.Response.StatusCode = (int)responseMessage.StatusCode;
                CopyFromTargetResponseHeaders(context, responseMessage);
                await responseMessage.Content.CopyToAsync(context.Response.Body);
            }
        }
    }
}
