using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;

namespace Leayal.Net
{
    public class ExtendedWebClient : IDisposable
    {
        private System.Threading.SynchronizationContext syncContext;
        private BaseWebClient innerWebClient;
        private short retried;
        private string downloadfileLocalPath;
        public ExtendedWebClient()
        {
            this.syncContext = System.Threading.SynchronizationContext.Current;
            this.innerWebClient = new BaseWebClient();
            this.innerWebClient.DownloadStringCompleted += InnerWebClient_DownloadStringCompleted;
            this.innerWebClient.DownloadDataCompleted += InnerWebClient_DownloadDataCompleted;
            this.innerWebClient.DownloadFileCompleted += InnerWebClient_DownloadFileCompleted;
            this.innerWebClient.DownloadProgressChanged += InnerWebClient_DownloadProgressChanged;
            this.retried = 0;
            this.Retry = 4;
            this.LastURL = null;
            this.downloadfileLocalPath = null;
            this.IsBusy = false;
        }

        private void InnerWebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.UserState is DownloadAsyncWrapper)
            { }
            else
            {
                this.DownloadProgressChanged?.Invoke(sender, e);
                //this.OnDownloadProgressChanged(e);
            }
        }

        #region "Properties"
        private short Retry { get; set; }
        public bool IsBusy { get; private set; }
        public Uri LastURL { get; private set; }
        public WebHeaderCollection Headers { get { return this.innerWebClient.Headers; } set { this.innerWebClient.Headers = value; } }
        public IWebProxy Proxy { get { return this.innerWebClient.Proxy; } set { this.innerWebClient.Proxy = value; } }
        public System.Net.Cache.RequestCachePolicy CachePolicy { get { return this.innerWebClient.CachePolicy; } set { this.innerWebClient.CachePolicy = value; } }
        public int TimeOut { get { return this.innerWebClient.TimeOut; } set { this.innerWebClient.TimeOut = value; } }
        public string UserAgent { get { return this.innerWebClient.UserAgent; } set { this.innerWebClient.UserAgent = value; } }
        public WebHeaderCollection ResponseHeaders { get { return this.innerWebClient.ResponseHeaders; } }
        #endregion

        #region "Open"
        public WebRequest CreateRequest(Uri url, string _method, WebHeaderCollection _headers, IWebProxy _proxy, int _timeout, System.Net.Cache.RequestCachePolicy _cachePolicy)
        {
            if (this.IsHTTP(url))
            {
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                HttpWebRequestHeaders placeholder = new HttpWebRequestHeaders();

                foreach (string key in _headers.AllKeys)
                    switch (key)
                    {
                        case "Accept":
                            placeholder[HttpRequestHeader.Accept] = _headers[HttpRequestHeader.Accept];
                            _headers.Remove(HttpRequestHeader.Accept);
                            break;
                        case "ContentType":
                            placeholder[HttpRequestHeader.ContentType] = _headers[HttpRequestHeader.ContentType];
                            _headers.Remove(HttpRequestHeader.ContentType);
                            break;
                        case "Expect":
                            placeholder[HttpRequestHeader.Expect] = _headers[HttpRequestHeader.Expect];
                            _headers.Remove(HttpRequestHeader.Expect);
                            break;
                        case "Referer":
                            placeholder[HttpRequestHeader.Referer] = _headers[HttpRequestHeader.Referer];
                            _headers.Remove(HttpRequestHeader.Referer);
                            break;
                        case "TransferEncoding":
                            placeholder[HttpRequestHeader.TransferEncoding] = _headers[HttpRequestHeader.TransferEncoding];
                            _headers.Remove(HttpRequestHeader.TransferEncoding);
                            break;
                        case "UserAgent":
                            placeholder[HttpRequestHeader.UserAgent] = _headers[HttpRequestHeader.UserAgent];
                            _headers.Remove(HttpRequestHeader.UserAgent);
                            break;
                        case "ContentLength":
                            placeholder[HttpRequestHeader.ContentLength] = _headers[HttpRequestHeader.ContentLength];
                            _headers.Remove(HttpRequestHeader.ContentLength);
                            break;
                    }
                request.Headers = _headers;
                request.Proxy = _proxy;
                request.CachePolicy = _cachePolicy;
                request.Timeout = _timeout;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.SendChunked = false;
                if (!string.IsNullOrWhiteSpace(_method))
                    request.Method = _method.ToUpper();
                if (!string.IsNullOrEmpty(placeholder[HttpRequestHeader.Accept]))
                    request.Accept = placeholder[HttpRequestHeader.Accept];
                if (!string.IsNullOrEmpty(placeholder[HttpRequestHeader.ContentType]))
                    request.ContentType = placeholder[HttpRequestHeader.ContentType];
                if (!string.IsNullOrEmpty(placeholder[HttpRequestHeader.Expect]))
                    request.Expect = placeholder[HttpRequestHeader.Expect];
                if (!string.IsNullOrEmpty(placeholder[HttpRequestHeader.Referer]))
                    request.Referer = placeholder[HttpRequestHeader.Referer];
                if (!string.IsNullOrEmpty(placeholder[HttpRequestHeader.TransferEncoding]))
                    request.TransferEncoding = placeholder[HttpRequestHeader.TransferEncoding];
                if (!string.IsNullOrEmpty(placeholder[HttpRequestHeader.UserAgent]))
                    request.UserAgent = placeholder[HttpRequestHeader.UserAgent];
                else
                    request.UserAgent = this.UserAgent;
                if (!string.IsNullOrEmpty(placeholder[HttpRequestHeader.ContentLength]))
                    request.ContentLength = long.Parse(placeholder[HttpRequestHeader.ContentLength]);

                //request.Headers = _headers;
                return request;
            }
            else
            {
                WebRequest request = WebRequest.Create(url);
                request.Proxy = _proxy;
                request.CachePolicy = _cachePolicy;
                request.Timeout = _timeout;
                request.Headers = _headers;
                return request;
            }
        }

        public WebRequest CreateRequest(Uri url, WebHeaderCollection _headers, IWebProxy _proxy, int _timeout, System.Net.Cache.RequestCachePolicy _cachePolicy)
        {
            return CreateRequest(url, string.Empty, _headers, _proxy, _timeout, _cachePolicy);
        }

        public WebRequest CreateRequest(Uri url, WebHeaderCollection _headers, IWebProxy _proxy, int _timeout)
        {
            return this.CreateRequest(url, _headers, _proxy, _timeout, this.CachePolicy);
        }

        public WebRequest CreateRequest(Uri url, WebHeaderCollection _headers, IWebProxy _proxy)
        {
            return this.CreateRequest(url, _headers, _proxy, this.TimeOut, this.CachePolicy);
        }

        public WebRequest CreateRequest(Uri url, WebHeaderCollection _headers, int _timeout)
        {
            return this.CreateRequest(url, _headers, null, _timeout, this.CachePolicy);
        }

        public WebRequest CreateRequest(Uri url, IWebProxy _proxy, int _timeout)
        {
            return this.CreateRequest(url, this.Headers, _proxy, _timeout, this.CachePolicy);
        }

        public WebRequest CreateRequest(Uri url, IWebProxy _proxy)
        {
            return this.CreateRequest(url, this.Headers, _proxy, this.TimeOut, this.CachePolicy);
        }

        public WebRequest CreateRequest(Uri url, WebHeaderCollection _headers)
        {
            return this.CreateRequest(url, _headers, null, this.TimeOut, this.CachePolicy);
        }

        public WebRequest CreateRequest(Uri url, int _timeout)
        {
            return this.CreateRequest(url, this.Headers, null, _timeout, this.CachePolicy);
        }

        public WebRequest CreateRequest(Uri url, string _method, int _timeout)
        {
            return this.CreateRequest(url, _method, this.Headers, null, _timeout, this.CachePolicy);
        }

        public WebRequest CreateRequest(Uri url, string _method)
        {
            return this.CreateRequest(url, _method, this.TimeOut);
        }

        public WebRequest CreateRequest(Uri url)
        {
            return this.CreateRequest(url, this.TimeOut);
        }

        public WebRequest CreateRequest(string url, string _method, int _timeout)
        {
            return this.CreateRequest(new Uri(url), _method, this.Headers, null, _timeout, this.CachePolicy);
        }

        public WebRequest CreateRequest(string url, string _method)
        {
            return this.CreateRequest(new Uri(url), _method, this.TimeOut);
        }

        public WebRequest CreateRequest(string url)
        {
            return this.CreateRequest(new Uri(url));
        }

        public WebRequest CreateRequest(string url, int _timeout)
        {
            return this.CreateRequest(new Uri(url), _timeout);
        }

        public WebRequest CreateRequest(string url, IWebProxy _proxy)
        {
            return this.CreateRequest(new Uri(url), _proxy);
        }

        public WebRequest CreateRequest(string url, IWebProxy _proxy, int _timeout)
        {
            return this.CreateRequest(new Uri(url), _proxy, _timeout);
        }

        public WebRequest CreateRequest(string url, WebHeaderCollection _headers)
        {
            return this.CreateRequest(new Uri(url), _headers);
        }

        public WebRequest CreateRequest(string url, WebHeaderCollection _headers, int _timeout)
        {
            return this.CreateRequest(new Uri(url), _headers, null, _timeout, this.CachePolicy);
        }

        private WebResponse Open(WebRequest request)
        {
            this.LastURL = request.RequestUri;
            return request.GetResponse();
        }

        public WebResponse Open(Uri url, WebHeaderCollection _headers, IWebProxy _proxy, int _timeout, System.Net.Cache.RequestCachePolicy _cachePolicy)
        {
            return this.Open(this.CreateRequest(url, _headers, _proxy, _timeout, _cachePolicy));
        }

        public WebResponse Open(Uri url, WebHeaderCollection _headers, IWebProxy _proxy, int _timeout)
        {
            return this.Open(url, _headers, _proxy, _timeout, this.CachePolicy);
        }

        public WebResponse Open(Uri url, WebHeaderCollection _headers, IWebProxy _proxy)
        {
            return this.Open(url, _headers, _proxy, this.TimeOut, this.CachePolicy);
        }

        public WebResponse Open(Uri url, WebHeaderCollection _headers, int _timeout)
        {
            return this.Open(url, _headers, null, _timeout, this.CachePolicy);
        }

        public WebResponse Open(Uri url, IWebProxy _proxy, int _timeout)
        {
            return this.Open(url, this.Headers, _proxy, _timeout, this.CachePolicy);
        }

        public WebResponse Open(Uri url, IWebProxy _proxy)
        {
            return this.Open(url, this.Headers, _proxy, this.TimeOut, this.CachePolicy);
        }

        public WebResponse Open(Uri url, WebHeaderCollection _headers)
        {
            return this.Open(url, _headers, null, this.TimeOut, this.CachePolicy);
        }

        public WebResponse Open(Uri url, int _timeout)
        {
            return this.Open(url, this.Headers, null, _timeout, this.CachePolicy);
        }

        public WebResponse Open(Uri url)
        {
            return this.Open(url, this.Headers, null, this.TimeOut, this.CachePolicy);
        }

        public WebResponse Open(string url)
        {
            return this.Open(new Uri(url));
        }

        public WebResponse Open(string url, int _timeout)
        {
            return this.Open(new Uri(url), _timeout);
        }

        public WebResponse Open(string url, IWebProxy _proxy)
        {
            return this.Open(new Uri(url), _proxy);
        }

        public WebResponse Open(string url, IWebProxy _proxy, int _timeout)
        {
            return this.Open(new Uri(url), _proxy, _timeout);
        }

        public WebResponse Open(string url, WebHeaderCollection _headers)
        {
            return this.Open(new Uri(url), _headers);
        }

        public WebResponse Open(string url, WebHeaderCollection _headers, int _timeout)
        {
            return this.Open(new Uri(url), _headers, null, _timeout, this.CachePolicy);
        }
        #endregion

        #region "OpenRead"
        public Stream OpenRead(Uri url)
        {
            this.LastURL = url;
            return this.innerWebClient.OpenRead(url);
        }

        public Stream OpenRead(string url)
        {
            return this.OpenRead(new Uri(url));
        }
        #endregion

        #region "DownloadString"
        public string DownloadString(string address)
        {
            return this.DownloadString(new Uri(address));
        }

        public string DownloadString(Uri address)
        {
            string result = string.Empty;
            if (!this.IsBusy)
            {
                OnWorkStarted();
                this.retried = 0;
                this.LastURL = address;
                for (short i = 0; i < Retry; i++)
                {
                    try
                    {
                        result = this.innerWebClient.DownloadString(address);
                        break;
                    }
                    catch (WebException ex)
                    {
                        if (IsHTTP(address))
                        {
                            if (ex.Response is HttpWebResponse && !WorthRetry(ex.Response as HttpWebResponse))
                                throw ex;
                        }
                        else
                            throw ex;
                    }
                }
                OnWorkFinished();
            }
            return result;
        }

        public void DownloadStringAsync(Uri address)
        {
            this.DownloadStringAsync(address, null);
        }

        public void DownloadStringAsync(Uri address, object userToken)
        {
            this.retried = 0;
            this.LastURL = address;
            this.IsBusy = true;
            this.innerWebClient.DownloadStringAsync(address, userToken);
        }

        private void InnerWebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (e.Error is WebException)
                {
                    WebException ex = e.Error as WebException;
                    if (IsHTTP(this.LastURL))
                    {
                        if (ex.Response is HttpWebResponse && WorthRetry(ex.Response as HttpWebResponse))
                            this.innerWebClient.DownloadStringAsync(this.LastURL, e.UserState);
                        else
                            this.OnDownloadStringFinished(new DownloadStringFinishedEventArgs(ex, e.Cancelled, e.Result, e.UserState));
                    }
                    else
                        this.OnDownloadStringFinished(new DownloadStringFinishedEventArgs(e.Error, e.Cancelled, e.Result, e.UserState));
                }
                else if (e.Error.InnerException is WebException)
                {
                    WebException ex = e.Error.InnerException as WebException;
                    if (IsHTTP(this.LastURL))
                    {
                        if (ex.Response is HttpWebResponse && WorthRetry(ex.Response as HttpWebResponse))
                            this.innerWebClient.DownloadStringAsync(this.LastURL);
                        else
                            this.OnDownloadStringFinished(new DownloadStringFinishedEventArgs(ex, e.Cancelled, e.Result, e.UserState));
                    }
                    else
                        this.OnDownloadStringFinished(new DownloadStringFinishedEventArgs(e.Error.InnerException, e.Cancelled, e.Result, e.UserState));
                }
            }
            else if (e.Cancelled)
            { this.OnDownloadStringFinished(new DownloadStringFinishedEventArgs(e.Error, e.Cancelled, e.Result, e.UserState)); }
            else
            {
                this.OnDownloadStringFinished(new DownloadStringFinishedEventArgs(e.Error, e.Cancelled, e.Result, e.UserState));
            }
        }
        #endregion

        #region "DownloadData"
        public byte[] DownloadData(string address)
        {
            return this.DownloadData(new Uri(address));
        }

        public byte[] DownloadData(Uri address)
        {
            byte[] result = null;
            if (!this.IsBusy)
            {
                OnWorkStarted();
                this.retried = 0;
                this.LastURL = address;
                for (short i = 0; i < Retry; i++)
                {
                    try
                    {
                        result = this.innerWebClient.DownloadData(address);
                        break;
                    }
                    catch (WebException ex)
                    {
                        if (IsHTTP(address))
                        {
                            if (ex.Response is HttpWebResponse && !WorthRetry(ex.Response as HttpWebResponse))
                                throw ex;
                        }
                        else
                            throw ex;
                    }
                }
                OnWorkFinished();
            }
            return result;
        }

        public void DownloadDataAsync(Uri address)
        {
            this.DownloadDataAsync(address, null);
        }

        public void DownloadDataAsync(Uri address, object userToken)
        {
            if (!this.IsBusy)
            {
                OnWorkStarted();
                this.retried = 0;
                this.LastURL = address;
                this.IsBusy = true;
                this.innerWebClient.DownloadDataAsync(address, userToken);
            }
        }

        private void InnerWebClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (e.Error is WebException)
                {
                    WebException ex = e.Error as WebException;
                    if (IsHTTP(this.LastURL))
                    {
                        if (ex.Response is HttpWebResponse && WorthRetry(ex.Response as HttpWebResponse))
                            this.innerWebClient.DownloadDataAsync(this.LastURL, e.UserState);
                        else
                            this.OnDownloadDataFinished(new DownloadDataFinishedEventArgs(ex, e.Cancelled, e.Result, e.UserState));
                    }
                    else
                        this.OnDownloadDataFinished(new DownloadDataFinishedEventArgs(e.Error, e.Cancelled, e.Result, e.UserState));
                }
                else if (e.Error.InnerException is WebException)
                {
                    WebException ex = e.Error.InnerException as WebException;
                    if (IsHTTP(this.LastURL))
                    {
                        if (ex.Response is HttpWebResponse && WorthRetry(ex.Response as HttpWebResponse))
                            this.innerWebClient.DownloadStringAsync(this.LastURL);
                        else
                            this.OnDownloadDataFinished(new DownloadDataFinishedEventArgs(ex, e.Cancelled, e.Result, e.UserState));
                    }
                    else
                        this.OnDownloadDataFinished(new DownloadDataFinishedEventArgs(e.Error.InnerException, e.Cancelled, e.Result, e.UserState));
                }
            }
            else if (e.Cancelled)
            { this.OnDownloadDataFinished(new DownloadDataFinishedEventArgs(e.Error, e.Cancelled, e.Result, e.UserState)); }
            else
            { this.OnDownloadDataFinished(new DownloadDataFinishedEventArgs(e.Error, e.Cancelled, e.Result, e.UserState)); }
        }
        #endregion

        #region "DownloadFile"
        public void DownloadFile(string address, string localpath)
        {
            this.DownloadFile(new Uri(address), localpath);
        }

        public bool DownloadFile(Uri address, string localpath)
        {
            bool result = false;
            if (!this.IsBusy)
            {
                OnWorkStarted();
                this.retried = 0;
                this.LastURL = address;
                FileInfo asd = new FileInfo(localpath + ".dtmp");
                for (short i = 0; i < Retry; i++)
                    try
                    {
                        if (asd.Exists)
                            asd.Open(FileMode.Open, FileAccess.ReadWrite).Close();
                        else
                            Microsoft.VisualBasic.FileIO.FileSystem.CreateDirectory(asd.DirectoryName);
                        this.innerWebClient.DownloadFile(address, asd.FullName);
                        if (IsHTTP(address))
                            if (!string.IsNullOrEmpty(this.innerWebClient.ResponseHeaders[HttpResponseHeader.ContentLength]))
                                if (long.Parse(this.innerWebClient.ResponseHeaders[HttpResponseHeader.ContentLength]) != asd.Length)
                                    throw new WebException($"Session '{address.OriginalString}' aborted.", WebExceptionStatus.RequestCanceled);
                        File.Delete(localpath);
                        asd.MoveTo(localpath);
                        //File.Move(asd.FullName, localpath);
                        result = true;
                        break;
                    }
                    catch (WebException ex)
                    {
                        if (IsHTTP(address))
                        {
                            if (ex.Response is HttpWebResponse)
                                if (!WorthRetry(ex.Response as HttpWebResponse))
                                    throw ex;
                        }
                        else
                            throw ex;
                    }
                OnWorkFinished();
            }
            return result;
        }

        public void DownloadFileAsync(Uri address, string localPath)
        {
            this.DownloadFileAsync(address, localPath, null);
        }

        public void DownloadFileAsync(Uri address, string localPath, object userToken)
        {
            if (!this.IsBusy)
                DownloadFileAsyncEx(address, localPath, userToken);
        }

        private void DownloadFileAsyncEx(Uri address, string localPath, object userToken)
        {
            OnWorkStarted();
            this.retried = 0;
            this.downloadfileLocalPath = localPath;
            this.LastURL = address;
            this.IsBusy = true;
            if (File.Exists(localPath))
                File.Open(localPath, FileMode.Open).Close();
            else
                Directory.CreateDirectory(Path.GetDirectoryName(localPath));
            this.innerWebClient.DownloadFileAsync(address, localPath + ".dtmp", userToken);
        }

        public void DownloadFileListAsync(Dictionary<Uri, string> fileList, object userToken)
        {
            if (fileList.Count > 0)
            {
                DownloadInfoCollection list = new DownloadInfoCollection();
                foreach (var fileNode in fileList)
                    list.Add(new DownloadInfo(fileNode.Key, fileNode.Value));
                this.DownloadFileListAsync(list, userToken);
            }
        }

        public void DownloadFileListAsync(List<DownloadInfo> filelist, object userToken)
        {
            this.DownloadFileListAsync(new DownloadInfoCollection(filelist), userToken);
        }

        public void DownloadFileListAsync(DownloadInfo[] filelist, object userToken)
        {
            this.DownloadFileListAsync(new DownloadInfoCollection(filelist), userToken);
        }

        public void DownloadFileListAsync(DownloadInfoCollection filelist, object userToken)
        {
            if (!this.IsBusy)
            {
                OnWorkStarted();
                if (!filelist.IsEmpty)
                {
                    DownloadInfo item = filelist.TakeFirst();
                    this.retried = 0;
                    this.downloadfileLocalPath = item.Filename;
                    this.LastURL = item.URL;
                    this.IsBusy = true;

                    if (File.Exists(item.Filename))
                        File.Open(item.Filename, FileMode.Open).Close();
                    else
                        Directory.CreateDirectory(Path.GetDirectoryName(item.Filename));
                    this.innerWebClient.DownloadFileAsync(item.URL, item.Filename + ".dtmp", new DownloadAsyncWrapper(filelist, userToken));
                }
            }
        }

        private void InnerWebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            object token = null;
            DownloadAsyncWrapper info = null;
            if (e.UserState != null)
            {
                if (e.UserState is DownloadAsyncWrapper)
                {
                    info = e.UserState as DownloadAsyncWrapper;
                    token = info.userToken;
                }
                else
                    token = e.UserState;
            }
            if (e.Error != null)
            {
                if (e.Error is WebException)
                {
                    WebException ex = e.Error as WebException;
                    if (IsHTTP(this.LastURL))
                    {
                        if (ex.Response is HttpWebResponse && WorthRetry(ex.Response as HttpWebResponse))
                            this.innerWebClient.DownloadFileAsync(this.LastURL, this.downloadfileLocalPath + ".dtmp", e.UserState);
                        else
                            this.SeekActionDerpian(info, e, token);
                    }
                    else
                        this.SeekActionDerpian(info, e, token);
                }
                else if (e.Error.InnerException is WebException)
                {

                    WebException ex = e.Error.InnerException as WebException;
                    if (IsHTTP(this.LastURL))
                    {
                        if (ex.Response is HttpWebResponse && WorthRetry(ex.Response as HttpWebResponse))
                            this.innerWebClient.DownloadStringAsync(this.LastURL);
                        else
                            this.SeekActionDerpian(info, e, token);
                    }
                    else
                        this.SeekActionDerpian(info, e, token);
                }
                else
                {
                    this.SeekActionDerpian(info, e, token);
                }
            }
            else if (e.Cancelled)
            { this.OnDownloadFileCompleted(new AsyncCompletedEventArgs(e.Error, e.Cancelled, token)); }
            else
            {
                try
                {
                    File.Delete(this.downloadfileLocalPath);
                    File.Move(this.downloadfileLocalPath + ".dtmp", this.downloadfileLocalPath);
                }
                catch { }
                this.SeekActionDerpian(info, e, token);
            }
        }

        private void SeekActionDerpian(DownloadAsyncWrapper info, AsyncCompletedEventArgs e, object token)
        {
            if ((info != null) && (!info.filelist.IsEmpty))
            {
                DownloadInfo item = info.filelist.TakeFirst();
                this.OnDownloadFileProgressChanged(new DownloadFileProgressChangedEventArgs(info.filelist.CurrentIndex, info.filelist.Count));
                this.DownloadFileAsyncEx(item.URL, item.Filename, info);
            }
            else
            { this.OnDownloadFileCompleted(new AsyncCompletedEventArgs(e.Error, e.Cancelled, token)); }
        }
        #endregion

        #region "Public Methods"
        public void Dispose()
        {
            this.innerWebClient.Dispose();
        }

        public void CancelAsync()
        {
            this.innerWebClient.CancelAsync();
        }

        public void ClearProgressEvents()
        {
            DownloadProgressChangedEventHandler myDe = this.DownloadProgressChanged;
            while (myDe != null)
            {
                this.DownloadProgressChanged -= myDe;
                myDe = this.DownloadProgressChanged;
            }
            /*
            FieldInfo f1 = typeof(ExtendedWebClient).GetField("DownloadProgressChanged", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = f1.GetValue(this);
            PropertyInfo pi = this.GetType().GetProperty("Events",
                BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList list = (EventHandlerList)pi.GetValue(this, null);
            list.RemoveHandler(obj, list[obj]);
            */
        }
        #endregion

        #region "Private Methods"
        private bool IsHTTP(Uri url)
        {
            if (url == null)
                return false;
            else
            {
                if ((url.Scheme == Uri.UriSchemeHttp) || (url.Scheme == Uri.UriSchemeHttps))
                    return true;
                else
                    return false;
            }
        }

        private bool WorthRetry(HttpWebResponse rep)
        {
            if (retried > this.Retry)
                return false;
            else
            {
                retried++;
                switch (rep.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return false;
                    case HttpStatusCode.NotAcceptable:
                        return false;
                    case HttpStatusCode.Unauthorized:
                        return false;
                    case HttpStatusCode.ServiceUnavailable:
                        return false;
                    case HttpStatusCode.NotImplemented:
                        return false;
                    case HttpStatusCode.PaymentRequired:
                        return false;
                    case HttpStatusCode.PreconditionFailed:
                        return false;
                    case HttpStatusCode.ProxyAuthenticationRequired:
                        return false;
                    default:
                        return true;
                }
            }
        }
        #endregion

        #region "Events"
        internal event EventHandler WorkFinished;
        protected void OnWorkFinished()
        {
            this.IsBusy = false;
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.WorkFinished?.Invoke(this, EventArgs.Empty); }), null);
        }
        internal event EventHandler WorkStarted;
        protected void OnWorkStarted()
        {
            this.IsBusy = true;
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.WorkStarted?.Invoke(this, EventArgs.Empty); }), null);
        }

        public event DownloadProgressChangedEventHandler DownloadProgressChanged;
        protected void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            if (DownloadProgressChanged != null)
                this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.DownloadProgressChanged.Invoke(this, e); }), null);
        }

        public event AsyncCompletedEventHandler DownloadFileCompleted;
        protected void OnDownloadFileCompleted(AsyncCompletedEventArgs e)
        {
            OnWorkFinished();
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.DownloadFileCompleted?.Invoke(this, e); }), null);
        }

        public delegate void DownloadDataFinishedEventHandler(object sender, DownloadDataFinishedEventArgs e);
        public event DownloadDataFinishedEventHandler DownloadDataCompleted;
        protected void OnDownloadDataFinished(DownloadDataFinishedEventArgs e)
        {
            OnWorkFinished();
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.DownloadDataCompleted?.Invoke(this, e); }), null);
        }
        
        public delegate void DownloadStringFinishedEventHandler(object sender, DownloadStringFinishedEventArgs e);
        public event DownloadStringFinishedEventHandler DownloadStringCompleted;
        protected void OnDownloadStringFinished(DownloadStringFinishedEventArgs e)
        {
            OnWorkFinished();
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.DownloadStringCompleted?.Invoke(this, e); }), null);
        }
        
        public delegate void DownloadFileProgressChangedEventHandler(object sender, DownloadFileProgressChangedEventArgs e);
        public event DownloadFileProgressChangedEventHandler DownloadFileProgressChanged;
        protected void OnDownloadFileProgressChanged(DownloadFileProgressChangedEventArgs e)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.DownloadFileProgressChanged?.Invoke(this, e); }), null);
        }

        #endregion

        #region "Private Class"
        private class HttpWebRequestHeaders
        {
            Dictionary<HttpRequestHeader, string> innerDictionary;
            public HttpWebRequestHeaders()
            {
                this.innerDictionary = new Dictionary<HttpRequestHeader, string>();
            }

            public string this[HttpRequestHeader key]
            {
                get
                {
                    if (!this.ContainsKey(key))
                        return null;
                    else
                    {
                        if (string.IsNullOrEmpty(this.innerDictionary[key]))
                            return null;
                        else
                            return this.innerDictionary[key];
                    }
                }
                set
                {
                    if (!this.ContainsKey(key))
                        this.Add(key, value);
                    else
                        this.innerDictionary[key] = value;
                }
            }

            public int Count { get { return this.innerDictionary.Count; } }

            public ICollection<HttpRequestHeader> Keys { get { return this.innerDictionary.Keys; } }

            public ICollection<string> Values { get { return this.innerDictionary.Values; } }

            public void Add(HttpRequestHeader key, string value) { this.innerDictionary.Add(key, value); }

            public void Clear() { this.innerDictionary.Clear(); }

            public bool ContainsKey(HttpRequestHeader key) { return this.innerDictionary.ContainsKey(key); }

            public IEnumerator<KeyValuePair<HttpRequestHeader, string>> GetEnumerator() { return this.GetEnumerator(); }

            public bool Remove(HttpRequestHeader key) { return this.innerDictionary.Remove(key); }

            public bool TryGetValue(HttpRequestHeader key, out string value) { return this.innerDictionary.TryGetValue(key, out value); }
        }
        private class DownloadAsyncWrapper
        {
            public object userToken { get; }
            public DownloadInfoCollection filelist;
            public DownloadAsyncWrapper(DownloadInfoCollection list, object token)
            {
                this.userToken = token;
                this.filelist = list;
            }
            public DownloadAsyncWrapper(DownloadInfoCollection list) : this(list, null) { }
        }
        public class DownloadInfo
        {
            public Uri URL
            { get; private set; }
            public string Filename
            { get; private set; }

            public DownloadInfo(Uri uUrl, string sFilename)
            {
                this.URL = uUrl;
                this.Filename = sFilename;
            }
        }

        public class DownloadInfoCollection
        {
            private ConcurrentQueue<DownloadInfo> myList;
            public bool IsEmpty
            { get { return this.myList.IsEmpty; } }
            public int Count
            { get; private set; }
            public int CurrentIndex
            { get; private set; }
            public DownloadInfoCollection(IEnumerable<DownloadInfo> list)
            {
                this.myList = new ConcurrentQueue<DownloadInfo>(list);
                this.Count = 0;
                this.CurrentIndex = 0;
            }
            public DownloadInfoCollection()
            {
                this.myList = new ConcurrentQueue<DownloadInfo>();
                this.Count = 0;
                this.CurrentIndex = 0;
            }

            public void Add(string sUrl, string sFilename)
            {
                this.Add(new DownloadInfo(new Uri(sUrl), sFilename));
            }

            public void Add(Uri uUrl, string sFilename)
            {
                this.Add(new DownloadInfo(uUrl, sFilename));
            }

            public void Add(DownloadInfo item)
            {
                this.Count++;
                this.myList.Enqueue(item);
            }

            public DownloadInfo TakeFirst()
            {
                if (this.IsEmpty)
                    return null;
                else
                {
                    DownloadInfo result;
                    if (this.myList.TryDequeue(out result))
                    {
                        this.CurrentIndex++;
                        return result;
                    }
                    else
                        return null;
                }
            }

            public void Dispose()
            {
                this.myList = null;
            }
        }
        #endregion
    }

    public class BaseWebClient : WebClient
    {
        public BaseWebClient(CookieContainer cookies = null, bool autoRedirect = true) : base()
        {
            this.CookieContainer = cookies ?? new CookieContainer();
            this.AutoRedirect = autoRedirect;
            this.UserAgent = "Mozilla/4.0";
            this.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            this.Proxy = null;
            this.TimeOut = 5000;
            this._response = null;
        }
        public BaseWebClient(int iTimeOut, CookieContainer cookies = null, bool autoRedirect = true) : this(cookies, autoRedirect)
        {
            this.TimeOut = iTimeOut;
        }
        public string UserAgent { get; set; }
        public int TimeOut { get; set; }
        public Uri CurrentURL { get; private set; }
        private WebResponse _response;

        /// Initializes a new instance of the BetterWebClient class.  <pa...

        /// Gets or sets a value indicating whether to automatically redi...
        public bool AutoRedirect { get; set; }

        /// Gets or sets the cookie container. This contains all the cook...
        public CookieContainer CookieContainer { get; set; }

        /// Gets the cookies header (Set-Cookie) of the last request.
        public string Cookies
        {
            get { return GetHeaderValue("Set-Cookie"); }
        }

        /// Gets the location header for the last request.
        public string Location
        {
            get { return GetHeaderValue("Location"); }
        }

        /// Gets the status code. When no request is present, <see cref="...
        public HttpStatusCode StatusCode
        {
            get
            {
                var result = HttpStatusCode.Gone;
                if (_response != null && this.IsHTTP())
                {
                    try
                    {
                        var rep = _response as HttpWebResponse;
                        result = rep.StatusCode;
                    }
                    catch
                    { result = HttpStatusCode.Gone; }
                }
                return result;
            }
        }

        /// Gets or sets the setup that is called before the request is d...
        public Action<HttpWebRequest> Setup { get; set; }

        /// Gets the header value.
        public string GetHeaderValue(string headerName)
        {
            if (_response == null)
                return null;
            else
            {
                string result = null;
                result = _response.Headers?[headerName];
                return result;
            }
        }

        public string GetHeaderValue(HttpResponseHeader headerenum)
        {
            if (_response == null)
                return null;
            else
            {
                string result = null;
                result = _response.Headers?[headerenum];
                return result;
            }
        }

        /// Returns a <see cref="T:System.Net.WebRequest" /> object for t...
        protected override WebRequest GetWebRequest(Uri address)
        {
            this.CurrentURL = address;
            var request = base.GetWebRequest(address);
            if (this.IsHTTP())
            {
                HttpWebRequest httpRequest = request as HttpWebRequest;
                if (request != null)
                {
                    httpRequest.AllowAutoRedirect = AutoRedirect;
                    httpRequest.CookieContainer = CookieContainer;
                    httpRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    httpRequest.Timeout = this.TimeOut;
                    httpRequest.SendChunked = false;
                    if ((this.Headers != null) && (this.Headers.HasKeys()))
                        httpRequest.Headers = this.Headers;
                    if (!string.IsNullOrEmpty(UserAgent))
                        httpRequest.UserAgent = this.UserAgent;
                    Setup?.Invoke(httpRequest);
                }
            }
            else
            {
                WebRequest Request = request as WebRequest;
                Request.Timeout = this.TimeOut;
                if ((this.Headers != null) && (this.Headers.HasKeys()))
                    Request.Headers = this.Headers;
            }
            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            this._response = base.GetWebResponse(request);
            if ((this._response.Headers != null) && (this._response.Headers.HasKeys()))
            {
                this.ResponseHeaders.Clear();
                foreach (string s in this._response.Headers.AllKeys)
                    this.ResponseHeaders[s] = this._response.Headers[s];
            }
            return this._response;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            this._response = base.GetWebResponse(request, result);
            if ((this._response.Headers != null) && (this._response.Headers.HasKeys()))
            {
                this.ResponseHeaders.Clear();
                this.ResponseHeaders.Add(this._response.Headers);
            }
            return this._response;
        }

        private bool IsHTTP()
        {
            if (this.CurrentURL == null)
                return false;
            else
            {
                if ((this.CurrentURL.Scheme == Uri.UriSchemeHttp) || (this.CurrentURL.Scheme == Uri.UriSchemeHttps))
                    return true;
                else
                    return false;
            }
        }
    }

    #region "EventArgs"
    public class DownloadStringFinishedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        public string Result { get; }
        public DownloadStringFinishedEventArgs(Exception ex, bool cancel, string taskresult, object userToken) : base(ex, cancel, userToken)
        {
            this.Result = taskresult;
        }
    }

    public class DownloadDataFinishedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        public byte[] Result { get; }
        public DownloadDataFinishedEventArgs(Exception ex, bool cancel, byte[] taskresult, object userToken) : base(ex, cancel, userToken)
        {
            this.Result = taskresult;
        }
    }

    public class DownloadFileProgressChangedEventArgs : System.EventArgs
    {
        public int CurrentFileIndex { get; }
        public int TotalFileCount { get; }
        public int Percent { get; }
        public DownloadFileProgressChangedEventArgs(int current, int total)
        {
            this.CurrentFileIndex = current;
            this.TotalFileCount = total;
            this.Percent = (int)((CurrentFileIndex * 100) / TotalFileCount);
        }
    }
    #endregion

    public class DownloadInfo
    {
        public Uri URL
        { get; private set; }
        public string Filename
        { get; private set; }

        public DownloadInfo(Uri uUrl, string sFilename)
        {
            this.URL = uUrl;
            this.Filename = sFilename;
        }
    }

    public class DownloadInfoCollection
    {
        private ConcurrentQueue<DownloadInfo> myList;
        public bool IsEmpty
        { get { return this.myList.IsEmpty; } }
        public int Count
        { get; private set; }
        public int CurrentIndex
        { get; private set; }
        public DownloadInfoCollection(IEnumerable<DownloadInfo> list)
        {
            this.myList = new ConcurrentQueue<DownloadInfo>(list);
            this.Count = 0;
            this.CurrentIndex = 0;
        }
        public DownloadInfoCollection()
        {
            this.myList = new ConcurrentQueue<DownloadInfo>();
            this.Count = 0;
            this.CurrentIndex = 0;
        }

        public void Add(string sUrl, string sFilename)
        {
            this.Add(new DownloadInfo(new Uri(sUrl), sFilename));
        }

        public void Add(Uri uUrl, string sFilename)
        {
            this.Add(new DownloadInfo(uUrl, sFilename));
        }

        public void Add(DownloadInfo item)
        {
            this.Count++;
            this.myList.Enqueue(item);
        }

        public DownloadInfo TakeFirst()
        {
            if (this.IsEmpty)
                return null;
            else
            {
                DownloadInfo result;
                if (this.myList.TryDequeue(out result))
                {
                    this.CurrentIndex++;
                    return result;
                }
                else
                    return null;
            }
        }

        public void Dispose()
        {
            this.myList = null;
        }
    }
}
