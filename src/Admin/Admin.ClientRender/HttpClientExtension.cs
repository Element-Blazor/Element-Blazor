using Element;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Element.Admin.ClientRender
{
    public static class HttpClientExtension
    {
        /// <summary>
        /// 发送一个 Post 请求
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="url">请求地址</param>
        /// <param name="requestContent"></param>
        /// <returns></returns>
        public static async Task<string> PostAsync(this HttpClient httpClient, string url, object requestContent)
        {
            var response = await httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json"));
            return await GetContentAsync(response);

        }

        private static async Task<string> GetContentAsync(HttpResponseMessage httpResponse)
        {
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new ElementException("服务器端发生内部错误");
            }
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var content = await httpResponse.Content.ReadAsStringAsync();
                return content;
            }
            return string.Empty;
        }

        /// <summary>
        /// 发送一个 Get 请求
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="url">请求地址</param>
        /// <param name="requestContent"></param>
        /// <returns></returns>
        public static async Task<string> GetAsync(this HttpClient httpClient, string url)
        {
            var response = await httpClient.GetAsync(url);
            return await GetContentAsync(response);
        }

        /// <summary>
        /// 发送一个 Delete 请求
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="url">请求地址</param>
        /// <param name="requestContent"></param>
        /// <returns></returns>
        public static async Task<string> DeleteAsync(this HttpClient httpClient, string url)
        {
            var response = await httpClient.DeleteAsync(url);
            return await GetContentAsync(response);
        }
    }
}
