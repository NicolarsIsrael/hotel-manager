using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HotelManager.Utility;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using System.Net;

namespace HotelMnager.Web.ApiManager
{
    public interface IRequestManager
    {
        Task<TOut> Send<T, TOut>(string path, T data, HttpMethod verb);
        Task<TOut> Send<T, TOut>(string path, T data, HttpMethod verb, Func<string, TOut> deserialize);
        Task<Stream> Send<T>(string path, T data, HttpMethod verb);
        Task<TOut> SendAsync<T, TOut>(string path, Stream data, T metadata, string fileName, HttpMethod verb);
    }

    public class RequestManager : IRequestManager
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestManager(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            string token = httpContextAccessor.HttpContext.Session.GetString(AppConstant.AccessToken)?.ToString();
            if (!String.IsNullOrEmpty(token))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<TOut> Send<T, TOut>(string path, T data, HttpMethod verb)
        {
            return await Send<T, TOut>(path, data, verb, s =>
            {
                TOut result = JsonConvert.DeserializeObject<TOut>(s);
                return result;
            });
        }
        public async Task<TOut> Send<T, TOut>(string path, T data, HttpMethod verb, Func<string, TOut> deserialize)
        {
            HttpRequestMessage request = new HttpRequestMessage(verb, path);

            if (verb == HttpMethod.Post || verb == HttpMethod.Put) //put, patch
            {
                var json = JsonConvert.SerializeObject(data);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            else
            {
                if (data != null)
                {
                    throw new ArgumentException("Only Post, Put, or Patch can have a body");
                }
            }

            var token = GetToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var ex = new WebException($"API Failure for {response.RequestMessage.RequestUri} returning status code {response.StatusCode}");
                ex.Data.Add("Content", response.Content);
                ex.Data.Add("API Route", $"GET {request.RequestUri}");
                ex.Data.Add("API Status", (int)response.StatusCode);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException(ex.Message, ex);
                }

                throw ex;
            }
            var result = deserialize(responseContent);
            return result;
        }
        public async Task<Stream> Send<T>(string path, T data, HttpMethod verb)
        {
            HttpRequestMessage request = new HttpRequestMessage(verb, path);

            if (verb == HttpMethod.Post || verb == HttpMethod.Put)
            {
                var json = JsonConvert.SerializeObject(data);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            }
            else
            {
                if (data != null)
                {
                    throw new ArgumentException("Only Post, Put, or Patch can have a body");
                }
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var ex = new WebException($"API Failure for {response.RequestMessage.RequestUri} returning status code {response.StatusCode}");
                ex.Data.Add("Content", response.Content);
                ex.Data.Add("API Route", $"GET {request.RequestUri}");
                ex.Data.Add("API Status", (int)response.StatusCode);

                throw ex;
            }

            var result = await response.Content.ReadAsStreamAsync();

            return result;
        }

        public async Task<TOut> SendAsync<T, TOut>(string path, Stream data, T metadata, string fileName, HttpMethod verb)
        {
            HttpRequestMessage request = new HttpRequestMessage(verb, path);


            if (verb == HttpMethod.Post || verb == HttpMethod.Put)
            {

                if (metadata != null)
                {
                    var json = JsonConvert.SerializeObject(metadata);

                    request.Content = new MultipartFormDataContent()
                    {
                        {new StreamContent(data),"File", fileName},
                        {new StringContent(fileName),"Filename"},
                        {new StringContent(json, Encoding.UTF8, "application/json"), "Data"}
                    };
                }
                else
                {
                    request.Content = new MultipartFormDataContent()
                    {
                        {new StreamContent(data),"File", fileName},
                        {new StringContent(fileName),"Filename"}
                    };
                }
            }
            else
            {
                if (data != null)
                {
                    throw new ArgumentException("Only Post, Put, or Patch can have a body");
                }
            }

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var ex = new WebException($"API Failure for {response.RequestMessage.RequestUri} returning status code {response.StatusCode}");
                ex.Data.Add("Content", response.Content);
                ex.Data.Add("API Route", $"GET {request.RequestUri}");
                ex.Data.Add("API Status", (int)response.StatusCode);
                throw ex;
            }

            var resultStr = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<TOut>(resultStr);
            return result;
        }

        private string GetToken()
        {
            return _httpContextAccessor.HttpContext.Session.GetString(AppConstant.AccessToken)?.ToString();
        }
    }
}
