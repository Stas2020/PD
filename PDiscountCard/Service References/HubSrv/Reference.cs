﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PDiscountCard.HubSrv {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OperationRequest", Namespace="http://schemas.datacontract.org/2004/07/DeliveryHubConnector")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(PDiscountCard.HubSrv.OperationRequestFromlong))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(PDiscountCard.HubSrv.OperationRequestFromPOSInfo))]
    public partial class OperationRequest : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int DepNumField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int DepNum {
            get {
                return this.DepNumField;
            }
            set {
                if ((this.DepNumField.Equals(value) != true)) {
                    this.DepNumField = value;
                    this.RaisePropertyChanged("DepNum");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OperationRequestFromlong", Namespace="http://schemas.datacontract.org/2004/07/DeliveryHubConnector")]
    [System.SerializableAttribute()]
    public partial class OperationRequestFromlong : PDiscountCard.HubSrv.OperationRequest {
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long RequestDataField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long RequestData {
            get {
                return this.RequestDataField;
            }
            set {
                if ((this.RequestDataField.Equals(value) != true)) {
                    this.RequestDataField = value;
                    this.RaisePropertyChanged("RequestData");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OperationRequestFromPOSInfo", Namespace="http://schemas.datacontract.org/2004/07/DeliveryHubConnector")]
    [System.SerializableAttribute()]
    public partial class OperationRequestFromPOSInfo : PDiscountCard.HubSrv.OperationRequest {
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private PDiscountCard.HubSrv.POSInfo RequestDataField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public PDiscountCard.HubSrv.POSInfo RequestData {
            get {
                return this.RequestDataField;
            }
            set {
                if ((object.ReferenceEquals(this.RequestDataField, value) != true)) {
                    this.RequestDataField = value;
                    this.RaisePropertyChanged("RequestData");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="POSInfo", Namespace="http://schemas.datacontract.org/2004/07/DeliveryHubConnector")]
    [System.SerializableAttribute()]
    public partial class POSInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string IpAddressField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool MainPOSFlagField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string IpAddress {
            get {
                return this.IpAddressField;
            }
            set {
                if ((object.ReferenceEquals(this.IpAddressField, value) != true)) {
                    this.IpAddressField = value;
                    this.RaisePropertyChanged("IpAddress");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool MainPOSFlag {
            get {
                return this.MainPOSFlagField;
            }
            set {
                if ((this.MainPOSFlagField.Equals(value) != true)) {
                    this.MainPOSFlagField = value;
                    this.RaisePropertyChanged("MainPOSFlag");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OperationResponse", Namespace="http://schemas.datacontract.org/2004/07/DeliveryHubConnector")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(PDiscountCard.HubSrv.OperationResponseOfShortOrderInfo))]
    public partial class OperationResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private PDiscountCard.HubSrv.OperationError[] ErrorsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool SuccessField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public PDiscountCard.HubSrv.OperationError[] Errors {
            get {
                return this.ErrorsField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorsField, value) != true)) {
                    this.ErrorsField = value;
                    this.RaisePropertyChanged("Errors");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Success {
            get {
                return this.SuccessField;
            }
            set {
                if ((this.SuccessField.Equals(value) != true)) {
                    this.SuccessField = value;
                    this.RaisePropertyChanged("Success");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OperationResponseOfShortOrderInfo", Namespace="http://schemas.datacontract.org/2004/07/DeliveryHubConnector")]
    [System.SerializableAttribute()]
    public partial class OperationResponseOfShortOrderInfo : PDiscountCard.HubSrv.OperationResponse {
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private PDiscountCard.HubSrv.ShortOrderInfo ResponseDataField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public PDiscountCard.HubSrv.ShortOrderInfo ResponseData {
            get {
                return this.ResponseDataField;
            }
            set {
                if ((object.ReferenceEquals(this.ResponseDataField, value) != true)) {
                    this.ResponseDataField = value;
                    this.RaisePropertyChanged("ResponseData");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OperationError", Namespace="http://schemas.datacontract.org/2004/07/DeliveryHubConnector")]
    [System.SerializableAttribute()]
    public partial class OperationError : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorMessageField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorCode {
            get {
                return this.ErrorCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorCodeField, value) != true)) {
                    this.ErrorCodeField = value;
                    this.RaisePropertyChanged("ErrorCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorMessage {
            get {
                return this.ErrorMessageField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorMessageField, value) != true)) {
                    this.ErrorMessageField = value;
                    this.RaisePropertyChanged("ErrorMessage");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ShortOrderInfo", Namespace="http://schemas.datacontract.org/2004/07/DeliveryHubConnector")]
    [System.SerializableAttribute()]
    public partial class ShortOrderInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AddressField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CommentField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FullNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MobilePhoneField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Address {
            get {
                return this.AddressField;
            }
            set {
                if ((object.ReferenceEquals(this.AddressField, value) != true)) {
                    this.AddressField = value;
                    this.RaisePropertyChanged("Address");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Comment {
            get {
                return this.CommentField;
            }
            set {
                if ((object.ReferenceEquals(this.CommentField, value) != true)) {
                    this.CommentField = value;
                    this.RaisePropertyChanged("Comment");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FullName {
            get {
                return this.FullNameField;
            }
            set {
                if ((object.ReferenceEquals(this.FullNameField, value) != true)) {
                    this.FullNameField = value;
                    this.RaisePropertyChanged("FullName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string MobilePhone {
            get {
                return this.MobilePhoneField;
            }
            set {
                if ((object.ReferenceEquals(this.MobilePhoneField, value) != true)) {
                    this.MobilePhoneField = value;
                    this.RaisePropertyChanged("MobilePhone");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="HubSrv.IDeliveryHubConnectorService")]
    public interface IDeliveryHubConnectorService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDeliveryHubConnectorService/Online", ReplyAction="http://tempuri.org/IDeliveryHubConnectorService/OnlineResponse")]
        bool Online();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDeliveryHubConnectorService/Hello", ReplyAction="http://tempuri.org/IDeliveryHubConnectorService/HelloResponse")]
        bool Hello(PDiscountCard.HubSrv.OperationRequestFromPOSInfo posHelloRequest);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDeliveryHubConnectorService/GetShortOrderInfo", ReplyAction="http://tempuri.org/IDeliveryHubConnectorService/GetShortOrderInfoResponse")]
        PDiscountCard.HubSrv.OperationResponseOfShortOrderInfo GetShortOrderInfo(PDiscountCard.HubSrv.OperationRequestFromlong shortInfoReq);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDeliveryHubConnectorServiceChannel : PDiscountCard.HubSrv.IDeliveryHubConnectorService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DeliveryHubConnectorServiceClient : System.ServiceModel.ClientBase<PDiscountCard.HubSrv.IDeliveryHubConnectorService>, PDiscountCard.HubSrv.IDeliveryHubConnectorService {
        
        public DeliveryHubConnectorServiceClient() {
        }
        
        public DeliveryHubConnectorServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DeliveryHubConnectorServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DeliveryHubConnectorServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DeliveryHubConnectorServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool Online() {
            return base.Channel.Online();
        }
        
        public bool Hello(PDiscountCard.HubSrv.OperationRequestFromPOSInfo posHelloRequest) {
            return base.Channel.Hello(posHelloRequest);
        }
        
        public PDiscountCard.HubSrv.OperationResponseOfShortOrderInfo GetShortOrderInfo(PDiscountCard.HubSrv.OperationRequestFromlong shortInfoReq) {
            return base.Channel.GetShortOrderInfo(shortInfoReq);
        }
    }
}
