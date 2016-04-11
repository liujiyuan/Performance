// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading;

namespace StarterMvc
{
    /// <summary>
    /// This controller will relay all the requests to the TextContentController via async Http calls
    /// </summary>
    public class TextContentRelayController : Controller
    {

        public static bool UseSingletonClient = true;
        private static HttpClient singletonTestClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } }); //Ignore server cert error
        private List<IDisposable> _objsToDispose = new List<IDisposable>(); //Objects to be desposed when the request finishes
        bool disposed = false;

        [HttpGet]
        public async Task<IActionResult> GetContent(int size, int timeoutMs)
        {
            var cancelTokeSource = new CancellationTokenSource();
            cancelTokeSource.CancelAfter(timeoutMs);
            this.HttpContext.RequestAborted.Register(CancelCurrentRequest, cancelTokeSource);
            string nextUrl = this.Request.Scheme + "://" + this.Request.Host.ToString() + String.Format("/TextContent/GetContent?size={0}", size);
            try
            {
                var testClient = GetHttpClient();
                if (!UseSingletonClient)
                    _objsToDispose.Add(testClient);
                var response = await testClient.GetAsync(nextUrl, cancelTokeSource.Token);
                _objsToDispose.Add(response);
                if (response.IsSuccessStatusCode)
                {
                    var tmpStream = await response.Content.ReadAsStreamAsync();
                    _objsToDispose.Add(tmpStream);
                    return new FileStreamResult(tmpStream, "text/plain");
                }
                else
                {
                    return new StatusCodeResult((int)response.StatusCode);
                }

            }
            catch (TaskCanceledException)
            {
                //Ignore the task cancel exception
            }
            catch (HttpRequestException)
            {
                //Ignore the Http request exception
            }

            cancelTokeSource.Dispose();
            //Since we don't care about business logic, let's always return success even if the request is cancelled
            //So client expects and verifies the same status code
            return new StatusCodeResult(200);
        }

        /// <summary>
        /// AddContent: Avoid sending in big form content. Since we read the form parts, each part would land in LOH in each chained request if >85KB
        /// </summary>
        /// <param name="content"></param>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddContent(string content, int timeoutMs)
        {
            var cancelTokeSource = new CancellationTokenSource();

            HttpContent newContent = await GetNewRelayPostRequestContent();

            string nextUrl = this.Request.Scheme + "://" + this.Request.Host.ToString() + "/TextContent/AddContent" + this.Request.QueryString;

            cancelTokeSource.CancelAfter(timeoutMs);
            this.HttpContext.RequestAborted.Register(CancelCurrentRequest, cancelTokeSource);
            try
            {
                var testClient = GetHttpClient();
                if (!UseSingletonClient)
                    _objsToDispose.Add(testClient);
                var response = await testClient.PostAsync(nextUrl, newContent, cancelTokeSource.Token);
                _objsToDispose.Add(response);
                if (response.IsSuccessStatusCode)
                {
                    return new StatusCodeResult(201);
                }
                else
                {
                    return new StatusCodeResult((int)response.StatusCode);
                }
            }
            catch (TaskCanceledException)
            {
                //Ignore the task cancel exception
            }
            catch (HttpRequestException)
            {
                //Ignore the Http request exception
            }

            cancelTokeSource.Dispose();
            //Since we don't care about business logic, let's always return success even if the request is cancelled
            //So client expects and verifies the same status code
            return new StatusCodeResult(201);
        }

        [HttpGet]
        public async Task<IActionResult> GetContentChained(int size, int timeoutMs, int numReqChained)
        {
            var cancelTokeSource = new CancellationTokenSource();
            cancelTokeSource.CancelAfter(timeoutMs);
            this.HttpContext.RequestAborted.Register(CancelCurrentRequest, cancelTokeSource);
            string nextUrl = this.Request.Scheme + "://" + this.Request.Host.ToString();
            if (numReqChained > 0)
            {
                nextUrl = nextUrl + String.Format("/TextContentRelay/GetContentChained?size={0}&timeoutMs={1}&numReqChained={2}", size, timeoutMs, --numReqChained);
            }
            else
            {
                nextUrl = nextUrl + String.Format("/TextContent/GetContent?size={0}", size);
            }

            try
            {
                var testClient = GetHttpClient();
                if (!UseSingletonClient)
                    _objsToDispose.Add(testClient);
                var response = await testClient.GetAsync(nextUrl, cancelTokeSource.Token);
                _objsToDispose.Add(response);
                if (response.IsSuccessStatusCode)
                {
                    var tmpStream = await response.Content.ReadAsStreamAsync();
                    _objsToDispose.Add(tmpStream);
                    return new FileStreamResult(await response.Content.ReadAsStreamAsync(), "text/plain");
                }
                else
                {
                    return new StatusCodeResult((int)response.StatusCode);
                }
            }
            catch (TaskCanceledException)
            {
                //Ignore the task cancel exception
            }
            catch (HttpRequestException)
            {
                //Ignore the Http request exception
            }

            cancelTokeSource.Dispose();
            //Since we don't care about business logic, let's always return success even if the request is cancelled
            //So client expects and verifies the same status code
            return new StatusCodeResult(200);
        }

        /// <summary>
        /// AddContentChained: Avoid sending in big form content. 
        /// Since we read the form parts when constructing new request, each part would land in LOH in each chained request if >85KB
        /// </summary>
        /// <param name="content"></param>
        /// <param name="timeoutMs"></param>
        /// <param name="numReqChained"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddContentChained(string content, int timeoutMs, int numReqChained)
        {
            var cancelTokeSource = new CancellationTokenSource();
            cancelTokeSource.CancelAfter(timeoutMs);
            this.HttpContext.RequestAborted.Register(CancelCurrentRequest, cancelTokeSource);
            string nextUrl = this.Request.Scheme + "://" + this.Request.Host.ToString();
            if (numReqChained > 0)
            {
                if (this.Request.QueryString.Value.ToLower().Contains("content="))
                {
                    nextUrl = nextUrl + String.Format("/TextContentRelay/AddContentChained?content={0}&timeoutMs={1}&numReqChained={2}", content, timeoutMs, --numReqChained);
                }
                else
                {
                    nextUrl = nextUrl + String.Format("/TextContentRelay/AddContentChained?timeoutMs={0}&numReqChained={1}", timeoutMs, --numReqChained);
                }
                
            }
            else
            {
                nextUrl = nextUrl + "/TextContent/AddContent" + this.Request.QueryString;
            }
            var newContent = await GetNewRelayPostRequestContent();
            cancelTokeSource.CancelAfter(timeoutMs);

            try
            {
                var testClient = GetHttpClient();
                if (!UseSingletonClient)
                    _objsToDispose.Add(testClient);
                var response = await testClient.PostAsync(nextUrl, newContent, cancelTokeSource.Token);
                _objsToDispose.Add(response);
                if (response.IsSuccessStatusCode)
                {
                    return new StatusCodeResult(201);
                }
                else
                {
                    return new StatusCodeResult((int)response.StatusCode);
                }
            }
            catch (TaskCanceledException)
            {
                //Ignore the task cancel exception
            }
            catch (HttpRequestException)
            {
                //Ignore the Http request exception
            }
            cancelTokeSource.Dispose();
            //Since we don't care about business logic, let's always return success even if the request is cancelled
            //So client expects and verifies the same status code
            return new StatusCodeResult(201);
        }

        #region DISPOSE
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                foreach (var tmpObj in _objsToDispose)
                {
                    tmpObj.Dispose();
                }
                _objsToDispose.Clear();
            }
            disposed = true;
            base.Dispose(disposing);
        }
        #endregion DISPOSE

        #region HELPERS
        private async Task<HttpContent> GetNewRelayPostRequestContent()
        {
            HttpContent newContent = null;
            if (this.Request.HasFormContentType)
            {
                var formCollection = await this.Request.ReadFormAsync();
                if (this.Request.ContentType.ToLower().Contains("multipart"))
                {
                    string strBoundary = "";
                    //Get the multipart boundary.
                    //For test purpose for now so assume it's in the format of "multipart/form-data; boundary=xxx" or "multipart/form-data; boundary=\"xxx\""
                    string[] subString = this.Request.ContentType.Split(new string[] { "boundary=", ";", "/", " ", "\"" }, StringSplitOptions.RemoveEmptyEntries);
                    if (subString.Length >= 3)
                    {
                        strBoundary = subString[2];
                    }
                    var multiPartFormContent = new MultipartFormDataContent(strBoundary);
                    foreach (string key in formCollection.Keys)
                    {
                        multiPartFormContent.Add(new StringContent(formCollection[key]), key);
                    }
                    newContent = multiPartFormContent;
                }
                else
                {
                    newContent = new FormUrlEncodedContent(formCollection.Select(item => new KeyValuePair<string, string> (item.Key, item.Value.ToString())));
                }
            }
            else
            {
                newContent = new StreamContent(this.Request.Body);
            }
            return newContent;
        }

        private void CancelCurrentRequest(object state)
        {
            try
            {
                CancellationTokenSource cancelTokenSource = state as CancellationTokenSource;
                if (cancelTokenSource != null)
                {
                    if (!cancelTokenSource.IsCancellationRequested)
                        cancelTokenSource.Cancel();
                }
            }
            catch (Exception)
            {
                //Suppress any error on cancellation
            }
        }
        private HttpClient GetHttpClient()
        {
            HttpClient testClient = null;
            if (UseSingletonClient)
                testClient = singletonTestClient;
            else
                testClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = delegate { return true; } });
            return testClient;
        }
        #endregion HELPERS
    }
}
