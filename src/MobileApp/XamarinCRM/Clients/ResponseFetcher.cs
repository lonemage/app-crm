﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XamarinCRM.Extensions;
using XamarinCRM.Serialization;

namespace XamarinCRM.Clients
{
    internal class ResponseFetcher<T>
    {
        readonly string _BaseUri;
        readonly string _AppKey;

        public ResponseFetcher(string baseUri, string appKey = null)
        {
            _BaseUri = baseUri;
            _AppKey = appKey;
        }

        internal async Task<T> GetResponseAsync(string requestUri)
        {
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(_AppKey))
                {
                    client.DefaultRequestHeaders.Add("X-ZUMO-APPLICATION", _AppKey); // X-ZUMO-APPLICATION is the HTTP header that Azure Mobile Services accepts for the AppKey
                }
                client.BaseAddress = new Uri(_BaseUri);
                var response = await client.GetAsync(requestUri);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    var newEx = new Exception((await response.Content.ReadAsStringAsync()), ex);
                    newEx.WriteFormattedMessageToDebugConsole(this);
                    throw newEx;
                }

                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(content);
            }
        }
    }
}

