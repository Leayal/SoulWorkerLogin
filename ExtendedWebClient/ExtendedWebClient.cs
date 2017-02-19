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
            this.innerWebClient.DownloadProgressChanged += DownloadProgressChanged;
            this.retried = 0;
            this.Retry = 4;
            this.LastURL = null;
            this.downloadfileLocalPath = null;
            this.IsBusy = false;
        }

        #region "Properties"
        private short Retry { get; set; }
        public bool IsBusy { get; private set; }
        public Uri LastURL { get; private set; }
        public WebHeaderCollection Headers { get { return this.innerWebClient.Headers; } set { this.innerWebClient.Headers = value; } }
        public WebHeaderCollection ResponseHeaders { get { return this.innerWebClient.ResponseHeaders; }}
        public IWebProxy Proxy { get { return this.innerWebClient.Proxy; } set { this.innerWebClient.Proxy = value; } }
        public System.Net.Cache.RequestCachePolicy CachePolicy { get { return this.innerWebClient.CachePolicy; } set { this.innerWebClient.CachePolicy = value; } }
        public int TimeOut { get { return this.innerWebClient.TimeOut; } set { this.innerWebClient.TimeOut = value; } }
        public string UserAgent { get { return this.innerWebClient.UserAgent; } set { this.innerWebClient.UserAgent = value; } }
        #endregion

        #region "Streams"
        public Stream OpenRead(string address)
        {
            return this.innerWebClient.OpenRead(address);
        }
        public Stream OpenRead(Uri address)
        {
            return this.innerWebClient.OpenRead(address);
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
                    }
                    catch (WebException ex)
                    {
                        if (IsHTTP(address))
                        {
                            if (!WorthRetry(ex.Response as HttpWebResponse))
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
                        if (WorthRetry(ex.Response as HttpWebResponse))
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
                        if (WorthRetry(ex.Response as HttpWebResponse))
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
                    }
                    catch (WebException ex)
                    {
                        if (IsHTTP(address))
                        {
                            if (!WorthRetry(ex.Response as HttpWebResponse))
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
                        if (WorthRetry(ex.Response as HttpWebResponse))
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
                        if (WorthRetry(ex.Response as HttpWebResponse))
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

        public void DownloadFile(Uri address, string localpath)
        {
            if (!this.IsBusy)
            {
                OnWorkStarted();
                this.retried = 0;
                this.LastURL = address;
                string asd = localpath + ".dtmp";
                for (short i = 0; i < Retry; i++)
                {
                    try
                    {
                        if (File.Exists(localpath))
                            File.Open(localpath, FileMode.Open).Close();
                        else
                            Directory.CreateDirectory(Path.GetDirectoryName(localpath));
                        this.innerWebClient.DownloadFile(address, asd);
                        File.Delete(localpath);
                        File.Move(localpath, asd);
                    }
                    catch (WebException ex)
                    {
                        if (IsHTTP(address))
                        {
                            if (!WorthRetry(ex.Response as HttpWebResponse))
                                throw ex;
                        }
                        else
                            throw ex;
                    }
                }
                OnWorkFinished();
            }
        }

        public void DownloadFileAsync(Uri address, string localPath)
        {
            this.DownloadFileAsync(address, localPath, null);
        }

        public void DownloadFileAsync(Uri address, string localPath, object userToken)
        {
            if (!this.IsBusy)
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
        }

        public void DownloadFileListAsync(Dictionary<Uri, string> fileList, ProgressChangedEventHandler progress_callback, AsyncCompletedEventHandler callback, object userToken)
        {
            if (fileList.Count > 0)
            {
                DownloadInfoCollection list = new DownloadInfoCollection();
                foreach (var fileNode in fileList)
                    list.Add(new DownloadInfo(fileNode.Key, fileNode.Value));
                this.DownloadFileListAsync(list, userToken);
            }
        }

        public void DownloadFileListAsync(List<DownloadInfo> filelist, string localPath)
        {
            this.DownloadFileListAsync(new DownloadInfoCollection(filelist), null);
        }

        public void DownloadFileListAsync(DownloadInfo[] filelist, string localPath)
        {
            this.DownloadFileListAsync(new DownloadInfoCollection(filelist), null);
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
                        if (WorthRetry(ex.Response as HttpWebResponse))
                            this.innerWebClient.DownloadFileAsync(this.LastURL, this.downloadfileLocalPath + ".dtmp", e.UserState);
                        else
                            this.OnDownloadFileCompleted(new AsyncCompletedEventArgs(ex, e.Cancelled, token));
                    }
                    else
                        this.OnDownloadFileCompleted(new AsyncCompletedEventArgs(e.Error, e.Cancelled, token));
                }
                else if (e.Error.InnerException is WebException)
                {

                    WebException ex = e.Error.InnerException as WebException;
                    if (IsHTTP(this.LastURL))
                    {
                        if (WorthRetry(ex.Response as HttpWebResponse))
                            this.innerWebClient.DownloadStringAsync(this.LastURL);
                        else
                            this.OnDownloadFileCompleted(new AsyncCompletedEventArgs(ex, e.Cancelled, token));
                    }
                    else
                        this.OnDownloadFileCompleted(new AsyncCompletedEventArgs(e.Error.InnerException, e.Cancelled, token));
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

                    if ((info != null) && (!info.filelist.IsEmpty))
                    {
                        DownloadInfo item = info.filelist.TakeFirst();
                        this.OnDownloadFileProgressChanged(new DownloadFileProgressChangedEventArgs(info.filelist.CurrentIndex, info.filelist.Count));
                        this.DownloadFileAsync(item.URL, item.Filename, info);
                    }
                    else
                    { this.OnDownloadFileCompleted(new AsyncCompletedEventArgs(e.Error, e.Cancelled, token)); }
                }
                catch (Exception ex)
                { this.OnDownloadFileCompleted(new AsyncCompletedEventArgs(ex, e.Cancelled, token)); }
            }
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
                if ((url.Scheme == "http") || (url.Scheme == "https"))
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
        public class DownloadDataFinishedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
        {
            public byte[] Result { get; }
            public DownloadDataFinishedEventArgs(Exception ex, bool cancel, byte[] taskresult, object userToken) : base(ex, cancel, userToken)
            {
                this.Result = taskresult;
            }
        }

        public delegate void DownloadStringFinishedEventHandler(object sender, DownloadStringFinishedEventArgs e);
        public event DownloadStringFinishedEventHandler DownloadStringCompleted;
        protected void OnDownloadStringFinished(DownloadStringFinishedEventArgs e)
        {
            OnWorkFinished();
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.DownloadStringCompleted?.Invoke(this, e); }), null);
        }
        public class DownloadStringFinishedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
        {
            public string Result { get; }
            public DownloadStringFinishedEventArgs(Exception ex, bool cancel, string taskresult, object userToken) : base(ex, cancel, userToken)
            {
                this.Result = taskresult;
            }
        }

        public delegate void DownloadFileProgressChangedEventHandler(object sender, DownloadFileProgressChangedEventArgs e);
        public event DownloadFileProgressChangedEventHandler DownloadFileProgressChanged;
        protected void OnDownloadFileProgressChanged(DownloadFileProgressChangedEventArgs e)
        {
            this.syncContext.Post(new System.Threading.SendOrPostCallback(delegate { this.DownloadFileProgressChanged?.Invoke(this, e); }), null);
        }
        #endregion

        #region "Private Class"
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
            return this._response;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            this._response = base.GetWebResponse(request, result);
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
