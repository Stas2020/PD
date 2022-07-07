﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace PDiscountCard.EGAISSrv {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="wsExtAlco_WebServiceAlcoSoapBinding", Namespace="1c_ws_alco")]
    public partial class wsExtAlco_WebServiceAlco : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback stringGetItemByQRCodeOperationCompleted;
        
        private System.Threading.SendOrPostCallback stringPutBottleOpenOperationCompleted;
        
        private System.Threading.SendOrPostCallback stringPutStockInventoryOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetBeerStockAlcoCodesOperationCompleted;
        
        private System.Threading.SendOrPostCallback stringPutBeerOpenOperationCompleted;
        
        private System.Threading.SendOrPostCallback stringGetSiteNameByCodeOperationCompleted;
        
        private System.Threading.SendOrPostCallback boolCheckQRCodeDoubleDayUseOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public wsExtAlco_WebServiceAlco() {
            this.Url = global::PDiscountCard.Properties.Settings.Default.PDiscountCard_EGAISSrv_wsExtAlco_WebServiceAlco;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event stringGetItemByQRCodeCompletedEventHandler stringGetItemByQRCodeCompleted;
        
        /// <remarks/>
        public event stringPutBottleOpenCompletedEventHandler stringPutBottleOpenCompleted;
        
        /// <remarks/>
        public event stringPutStockInventoryCompletedEventHandler stringPutStockInventoryCompleted;
        
        /// <remarks/>
        public event GetBeerStockAlcoCodesCompletedEventHandler GetBeerStockAlcoCodesCompleted;
        
        /// <remarks/>
        public event stringPutBeerOpenCompletedEventHandler stringPutBeerOpenCompleted;
        
        /// <remarks/>
        public event stringGetSiteNameByCodeCompletedEventHandler stringGetSiteNameByCodeCompleted;
        
        /// <remarks/>
        public event boolCheckQRCodeDoubleDayUseCompletedEventHandler boolCheckQRCodeDoubleDayUseCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("1c_ws_alco#wsExtAlco_WebServiceAlco:stringGetItemByQRCode", RequestNamespace="1c_ws_alco", ResponseNamespace="1c_ws_alco", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public string stringGetItemByQRCode(string QRCode) {
            object[] results = this.Invoke("stringGetItemByQRCode", new object[] {
                        QRCode});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void stringGetItemByQRCodeAsync(string QRCode) {
            this.stringGetItemByQRCodeAsync(QRCode, null);
        }
        
        /// <remarks/>
        public void stringGetItemByQRCodeAsync(string QRCode, object userState) {
            if ((this.stringGetItemByQRCodeOperationCompleted == null)) {
                this.stringGetItemByQRCodeOperationCompleted = new System.Threading.SendOrPostCallback(this.OnstringGetItemByQRCodeOperationCompleted);
            }
            this.InvokeAsync("stringGetItemByQRCode", new object[] {
                        QRCode}, this.stringGetItemByQRCodeOperationCompleted, userState);
        }
        
        private void OnstringGetItemByQRCodeOperationCompleted(object arg) {
            if ((this.stringGetItemByQRCodeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.stringGetItemByQRCodeCompleted(this, new stringGetItemByQRCodeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("1c_ws_alco#wsExtAlco_WebServiceAlco:stringPutBottleOpen", RequestNamespace="1c_ws_alco", ResponseNamespace="1c_ws_alco", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public string stringPutBottleOpen(string QRCode, string SiteCode) {
            object[] results = this.Invoke("stringPutBottleOpen", new object[] {
                        QRCode,
                        SiteCode});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void stringPutBottleOpenAsync(string QRCode, string SiteCode) {
            this.stringPutBottleOpenAsync(QRCode, SiteCode, null);
        }
        
        /// <remarks/>
        public void stringPutBottleOpenAsync(string QRCode, string SiteCode, object userState) {
            if ((this.stringPutBottleOpenOperationCompleted == null)) {
                this.stringPutBottleOpenOperationCompleted = new System.Threading.SendOrPostCallback(this.OnstringPutBottleOpenOperationCompleted);
            }
            this.InvokeAsync("stringPutBottleOpen", new object[] {
                        QRCode,
                        SiteCode}, this.stringPutBottleOpenOperationCompleted, userState);
        }
        
        private void OnstringPutBottleOpenOperationCompleted(object arg) {
            if ((this.stringPutBottleOpenCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.stringPutBottleOpenCompleted(this, new stringPutBottleOpenCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("1c_ws_alco#wsExtAlco_WebServiceAlco:stringPutStockInventory", RequestNamespace="1c_ws_alco", ResponseNamespace="1c_ws_alco", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public string stringPutStockInventory([System.Xml.Serialization.XmlArrayItemAttribute("QRCode", Namespace="http://alco.coffeemania.ru", IsNullable=false)] string[] ArrQRCodes, string SiteCode, System.DateTime DateOfInventory) {
            object[] results = this.Invoke("stringPutStockInventory", new object[] {
                        ArrQRCodes,
                        SiteCode,
                        DateOfInventory});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void stringPutStockInventoryAsync(string[] ArrQRCodes, string SiteCode, System.DateTime DateOfInventory) {
            this.stringPutStockInventoryAsync(ArrQRCodes, SiteCode, DateOfInventory, null);
        }
        
        /// <remarks/>
        public void stringPutStockInventoryAsync(string[] ArrQRCodes, string SiteCode, System.DateTime DateOfInventory, object userState) {
            if ((this.stringPutStockInventoryOperationCompleted == null)) {
                this.stringPutStockInventoryOperationCompleted = new System.Threading.SendOrPostCallback(this.OnstringPutStockInventoryOperationCompleted);
            }
            this.InvokeAsync("stringPutStockInventory", new object[] {
                        ArrQRCodes,
                        SiteCode,
                        DateOfInventory}, this.stringPutStockInventoryOperationCompleted, userState);
        }
        
        private void OnstringPutStockInventoryOperationCompleted(object arg) {
            if ((this.stringPutStockInventoryCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.stringPutStockInventoryCompleted(this, new stringPutStockInventoryCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("1c_ws_alco#wsExtAlco_WebServiceAlco:GetBeerStockAlcoCodes", RequestNamespace="1c_ws_alco", ResponseNamespace="1c_ws_alco", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute("return")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute(Namespace="http://alco.coffeemania.ru", IsNullable=false)]
        public Element[] GetBeerStockAlcoCodes(string SiteCode) {
            object[] results = this.Invoke("GetBeerStockAlcoCodes", new object[] {
                        SiteCode});
            return ((Element[])(results[0]));
        }
        
        /// <remarks/>
        public void GetBeerStockAlcoCodesAsync(string SiteCode) {
            this.GetBeerStockAlcoCodesAsync(SiteCode, null);
        }
        
        /// <remarks/>
        public void GetBeerStockAlcoCodesAsync(string SiteCode, object userState) {
            if ((this.GetBeerStockAlcoCodesOperationCompleted == null)) {
                this.GetBeerStockAlcoCodesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetBeerStockAlcoCodesOperationCompleted);
            }
            this.InvokeAsync("GetBeerStockAlcoCodes", new object[] {
                        SiteCode}, this.GetBeerStockAlcoCodesOperationCompleted, userState);
        }
        
        private void OnGetBeerStockAlcoCodesOperationCompleted(object arg) {
            if ((this.GetBeerStockAlcoCodesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetBeerStockAlcoCodesCompleted(this, new GetBeerStockAlcoCodesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("1c_ws_alco#wsExtAlco_WebServiceAlco:stringPutBeerOpen", RequestNamespace="1c_ws_alco", ResponseNamespace="1c_ws_alco", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public string stringPutBeerOpen(string AlcoCode, string SiteCode) {
            object[] results = this.Invoke("stringPutBeerOpen", new object[] {
                        AlcoCode,
                        SiteCode});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void stringPutBeerOpenAsync(string AlcoCode, string SiteCode) {
            this.stringPutBeerOpenAsync(AlcoCode, SiteCode, null);
        }
        
        /// <remarks/>
        public void stringPutBeerOpenAsync(string AlcoCode, string SiteCode, object userState) {
            if ((this.stringPutBeerOpenOperationCompleted == null)) {
                this.stringPutBeerOpenOperationCompleted = new System.Threading.SendOrPostCallback(this.OnstringPutBeerOpenOperationCompleted);
            }
            this.InvokeAsync("stringPutBeerOpen", new object[] {
                        AlcoCode,
                        SiteCode}, this.stringPutBeerOpenOperationCompleted, userState);
        }
        
        private void OnstringPutBeerOpenOperationCompleted(object arg) {
            if ((this.stringPutBeerOpenCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.stringPutBeerOpenCompleted(this, new stringPutBeerOpenCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("1c_ws_alco#wsExtAlco_WebServiceAlco:stringGetSiteNameByCode", RequestNamespace="1c_ws_alco", ResponseNamespace="1c_ws_alco", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public string stringGetSiteNameByCode(string SiteCode) {
            object[] results = this.Invoke("stringGetSiteNameByCode", new object[] {
                        SiteCode});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void stringGetSiteNameByCodeAsync(string SiteCode) {
            this.stringGetSiteNameByCodeAsync(SiteCode, null);
        }
        
        /// <remarks/>
        public void stringGetSiteNameByCodeAsync(string SiteCode, object userState) {
            if ((this.stringGetSiteNameByCodeOperationCompleted == null)) {
                this.stringGetSiteNameByCodeOperationCompleted = new System.Threading.SendOrPostCallback(this.OnstringGetSiteNameByCodeOperationCompleted);
            }
            this.InvokeAsync("stringGetSiteNameByCode", new object[] {
                        SiteCode}, this.stringGetSiteNameByCodeOperationCompleted, userState);
        }
        
        private void OnstringGetSiteNameByCodeOperationCompleted(object arg) {
            if ((this.stringGetSiteNameByCodeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.stringGetSiteNameByCodeCompleted(this, new stringGetSiteNameByCodeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("1c_ws_alco#wsExtAlco_WebServiceAlco:boolCheckQRCodeDoubleDayUse", RequestNamespace="1c_ws_alco", ResponseNamespace="1c_ws_alco", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public bool boolCheckQRCodeDoubleDayUse(string QRCode, string SiteCode) {
            object[] results = this.Invoke("boolCheckQRCodeDoubleDayUse", new object[] {
                        QRCode,
                        SiteCode});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void boolCheckQRCodeDoubleDayUseAsync(string QRCode, string SiteCode) {
            this.boolCheckQRCodeDoubleDayUseAsync(QRCode, SiteCode, null);
        }
        
        /// <remarks/>
        public void boolCheckQRCodeDoubleDayUseAsync(string QRCode, string SiteCode, object userState) {
            if ((this.boolCheckQRCodeDoubleDayUseOperationCompleted == null)) {
                this.boolCheckQRCodeDoubleDayUseOperationCompleted = new System.Threading.SendOrPostCallback(this.OnboolCheckQRCodeDoubleDayUseOperationCompleted);
            }
            this.InvokeAsync("boolCheckQRCodeDoubleDayUse", new object[] {
                        QRCode,
                        SiteCode}, this.boolCheckQRCodeDoubleDayUseOperationCompleted, userState);
        }
        
        private void OnboolCheckQRCodeDoubleDayUseOperationCompleted(object arg) {
            if ((this.boolCheckQRCodeDoubleDayUseCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.boolCheckQRCodeDoubleDayUseCompleted(this, new boolCheckQRCodeDoubleDayUseCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.3761.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://alco.coffeemania.ru")]
    public partial class Element {
        
        private string alcoCodeField;
        
        private string alcoNameField;
        
        /// <remarks/>
        public string AlcoCode {
            get {
                return this.alcoCodeField;
            }
            set {
                this.alcoCodeField = value;
            }
        }
        
        /// <remarks/>
        public string AlcoName {
            get {
                return this.alcoNameField;
            }
            set {
                this.alcoNameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    public delegate void stringGetItemByQRCodeCompletedEventHandler(object sender, stringGetItemByQRCodeCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class stringGetItemByQRCodeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal stringGetItemByQRCodeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    public delegate void stringPutBottleOpenCompletedEventHandler(object sender, stringPutBottleOpenCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class stringPutBottleOpenCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal stringPutBottleOpenCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    public delegate void stringPutStockInventoryCompletedEventHandler(object sender, stringPutStockInventoryCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class stringPutStockInventoryCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal stringPutStockInventoryCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    public delegate void GetBeerStockAlcoCodesCompletedEventHandler(object sender, GetBeerStockAlcoCodesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetBeerStockAlcoCodesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetBeerStockAlcoCodesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public Element[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((Element[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    public delegate void stringPutBeerOpenCompletedEventHandler(object sender, stringPutBeerOpenCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class stringPutBeerOpenCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal stringPutBeerOpenCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    public delegate void stringGetSiteNameByCodeCompletedEventHandler(object sender, stringGetSiteNameByCodeCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class stringGetSiteNameByCodeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal stringGetSiteNameByCodeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    public delegate void boolCheckQRCodeDoubleDayUseCompletedEventHandler(object sender, boolCheckQRCodeDoubleDayUseCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3761.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class boolCheckQRCodeDoubleDayUseCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal boolCheckQRCodeDoubleDayUseCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591