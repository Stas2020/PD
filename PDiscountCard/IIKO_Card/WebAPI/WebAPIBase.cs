using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Reflection;


// !!! Стандрантый класс базового АПИ для всего айковского семейства
// !!!!!! Прим: отсюда удале обход XML кодирования Employee и NomenclatureFilter

namespace PDiscountCard.IIKO_Card
{
    //////public delegate void AuthorizedStateEventHandler(object sender, AuthorizedStateChangeEventArgs e);
    public abstract class WebAPIBase
    {
        protected int maxURILength = 2000;

        //////public event AuthorizedStateEventHandler AuthorizedStateUpdate;

        int connErrorWaitMS = 3500;
        int connErrorTryCount = 5;

        public class XWWWFormUrlencodedContainer
        {
            public object obj;
            public XWWWFormUrlencodedContainer(object _obj)
            {
                obj = _obj;
            }
            public HttpContent GetHttpContent()
            {
                List<string> propsList = new List<string>();
                PropertyInfo[] props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in props)
                {
                    object propObj = obj.GetType().GetProperty(prop.Name, BindingFlags.Public | BindingFlags.Instance).GetValue(obj, null);
                    if (propObj == null)
                        continue;
                    if (propObj is List<string>)
                        propsList.Add(string.Join("&", (propObj as List<string>).Select(_objEntity => $"{prop.Name}={_objEntity}")));
                    else
                        propsList.Add($"{prop.Name}={propObj.ToString()}");
                }
                return new StringContent(string.Join("&", propsList), Encoding.UTF8, "application/x-www-form-urlencoded");
            }
        }

        public class XmlObjectContainer
        {
            static Dictionary<XmlSerialirezTypeRoot, XmlSerializer> XmlSerializersCash = new Dictionary<XmlSerialirezTypeRoot, XmlSerializer>();

            struct XmlSerialirezTypeRoot
            {
                public Type type;
                public string rootName;
                public static bool operator ==(XmlSerialirezTypeRoot c1, XmlSerialirezTypeRoot c2)
                {
                    return (c1.type.Equals(c2.type) && c1.rootName.Equals(c2.rootName));
                }
                public static bool operator !=(XmlSerialirezTypeRoot c1, XmlSerialirezTypeRoot c2)
                {
                    return !(c1.type.Equals(c2.type) && c1.rootName.Equals(c2.rootName));
                }
                public override bool Equals(object obj)
                {
                    if (!(obj is XmlSerialirezTypeRoot xmlTypeStruct))
                        return false;
                    return (this.type.Equals(xmlTypeStruct.type) && this.rootName.Equals(xmlTypeStruct.rootName));
                }
                public override int GetHashCode()
                {
                    return this.type.GetHashCode() ^ this.rootName.GetHashCode();
                }
            }
            public static XmlSerializer GetXMLSerializer(Type _type, string _rootName)
            {
                XmlSerialirezTypeRoot key = new XmlSerialirezTypeRoot() { type = _type, rootName = _rootName };
                if (XmlSerializersCash.ContainsKey(key))
                    return XmlSerializersCash[key];

                XmlSerializer newSerializer;
                if (_rootName != "")
                {
                    XmlRootAttribute xRoot = new XmlRootAttribute() { ElementName = _rootName, IsNullable = true };
                    newSerializer = new XmlSerializer(_type, xRoot);
                }
                else
                    newSerializer = new XmlSerializer(_type);
                XmlSerializersCash.Add(key, newSerializer);
                return newSerializer;
            }


            public object obj;
            public string rootName;
            public Type type;
            public string nameSpace;
            public XmlObjectContainer(object _obj, string _rootName, Type _type, string _nameSpace = null)
            {
                obj = _obj;
                rootName = _rootName;
                type = _type;
                nameSpace = _nameSpace;
            }
            public HttpContent GetHttpContent()
            {
                string xmlString = null;

                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);


                if (obj != null)
                {
                    StringBuilder sb = new StringBuilder();
                    using (XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true }))
                    {
                        GetXMLSerializer(obj.GetType(), rootName).Serialize(writer, obj, namespaces);
                    }
                    xmlString = sb.ToString();
                }

                for (int i = 1; i <= 6; i++)
                    xmlString = xmlString.Replace($" p{i}:nil=\"true\" xmlns:p{i}=\"http://www.w3.org/2001/XMLSchema-instance\" ", "");

                return new StringContent(xmlString, Encoding.UTF8, "application/xml");
            }
        }
        //////protected virtual void OnAuthorizedStateUpdate(AuthorizedStateChangeEventArgs e)
        //////{
        //////    AuthorizedStateUpdate(this, e);
        //////}

        //protected CookieCollection cookies = new CookieCollection();
        //HttpClientHandler handler = new HttpClientHandler();

        protected CookieContainer cookieContainer = new CookieContainer();
        protected HttpClientHandler handler;
        protected HttpClient client = new HttpClient();
        public bool authorized { get; protected set; }

        protected string login;
        protected string pass;
        protected string token = null;

        protected string urlAPI;
        protected string cookieDomain;

        //protected CookieCollection cookies = null;

        public WebAPIBase()
        {
            handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            client = new HttpClient(handler);

            //handler.CookieContainer = cookies;
            //client = new HttpClient(handler);
        }
        public abstract bool Login(out string _errorMessage);
        public abstract bool Logout(out string _errorMessage);

        protected void AuthorizedStateCheck()
        {
            //AuthorizedStateUpdate(this, new AuthorizedStateChangeEventArgs(authorized));
        }

        HttpResponseMessage sendRequest(string _path, HttpMethod _method = null, Object _content = null)
        {
            if (_method == null || _method == HttpMethod.Get)
            {
                try
                {
                    return client.GetAsync(_path).Result;
                }
                catch (Exception ex)
                {
                    ;
                    Utils.ToLog($"Connection ERROR while GET in start (pos.11). HResult:{ex.HResult}  Path:{_path}  Message: {ex.Message}");

                    for (int i = 0; i < connErrorTryCount; i++)
                    {
                        System.Threading.Thread.Sleep(connErrorWaitMS);
                        try
                        {
                            return client.GetAsync(_path).Result;
                        }
                        catch (Exception ex2)
                        {
                            Utils.ToLog($"Connection ERROR while GET in cycle try #{i + 1} (pos.22). HResult:{ex2.HResult}  Path:{_path}  Message: {ex2.Message}");
                        }
                    }
                }
                System.Threading.Thread.Sleep(connErrorWaitMS);
                return client.GetAsync(_path).Result;
            }
            {

                HttpContent content = (_content is HttpContent)
                ? (_content as HttpContent)
                : ((_content is XmlObjectContainer)
                    ? (_content as XmlObjectContainer).GetHttpContent()
                    : ((_content is XWWWFormUrlencodedContainer)
                        ? (_content as XWWWFormUrlencodedContainer).GetHttpContent()
                        : new StringContent(JsonConvert.SerializeObject(_content, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), Encoding.UTF8, "application/json")));


                try
                {
                    if (_method == HttpMethod.Post)
                        return client.PostAsync(_path, content).Result;
                    else if (_method == HttpMethod.Put)
                        return client.PutAsync(_path, content).Result;
                    else if (_method == HttpMethod.Delete)
                        return client.DeleteAsync(_path).Result;
                    else
                        return client.GetAsync(_path).Result;
                }
                catch (Exception ex)
                {
                    ;
                    Utils.ToLog($"Connection ERROR while POST/PUT in start (pos.33). HResult:{ex.HResult}  Path:{_path}  Message: {ex.Message}");

                    for (int i = 0; i < connErrorTryCount; i++)
                    {
                        System.Threading.Thread.Sleep(connErrorWaitMS);
                        try
                        {
                            if (_method == HttpMethod.Post)
                                return client.PostAsync(_path, content).Result;
                            else if (_method == HttpMethod.Put)
                                return client.PutAsync(_path, content).Result;
                            else if (_method == HttpMethod.Delete)
                                return client.DeleteAsync(_path).Result;
                            else
                                return client.GetAsync(_path).Result;
                        }
                        catch (Exception ex2)
                        {
                            Utils.ToLog($"Connection ERROR while POST/PUT in cycle try #{i + 1} (pos.44). HResult:{ex2.HResult}  Path:{_path}  Message: {ex2.Message}");
                        }
                    }
                }
                Utils.ToLog($"Connection FAIL, return NULL (pos.55). Path:{_path}");
                return null;
            }
        }

        public string GetString(string _shortRequest, out string _errorMessage, HttpMethod _method = null, Object _content = null)
        {
            string path = urlAPI + _shortRequest;
            string result = null;
            _errorMessage = null;
            HttpResponseMessage response = sendRequest(path, _method, _content);

            if (response != null)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (Login(out string _errMsg))
                        response = sendRequest(path, _method, _content);
                    else
                    {
                        // ToDo - спорно
                        authorized = false;
                        token = null;
                        client.DefaultRequestHeaders.Authorization = null;
                    }
                    //AuthorizedStateUpdate(this, new AuthorizedStateChangeEventArgs(authorized));
                }
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        using (StreamReader stream = new StreamReader(response.Content.ReadAsStreamAsync().Result))
                            result = stream.ReadToEnd();
                    }
                    catch (Exception ex)
                    {
                        _errorMessage = ex.Message;
                    }
                }
                else
                {
                    if (/*response.StatusCode == HttpStatusCode.BadRequest && */path.IndexOf("iiko.biz") != -1 || path.ToLower().IndexOf("giftcard/api") != -1)// && path.IndexOf("create_or_update") != -1)
                    {
                        using (StreamReader stream = new StreamReader(response.Content.ReadAsStreamAsync().Result))
                            _errorMessage = stream.ReadToEnd();
                        //"{\"code\":null,\"description\":null,\"httpStatusCode\":400,\"isIntegrationError\":null,\"message\":\"Customer is required\",\"uiMessage\":null}"
                    }
                    else
                        _errorMessage = $"{(int)response.StatusCode} {response.ReasonPhrase}   Метод:{_shortRequest}";//{path}";
                                                                                                                      //_statusCode = response.StatusCode;
                }
            }
            else
            {
                _errorMessage = $"Request fail, PATH:{path}";
                return null;
            }
            //Object content = _content;
            return result;
        }

        //////public class AuthorizedStateChangeEventArgs : EventArgs
        //////{
        //////    public bool authorizedState;
        //////    public AuthorizedStateChangeEventArgs(bool _authorizedState)
        //////    {
        //////        authorizedState = _authorizedState;
        //////    }
        //////}

    }
}
