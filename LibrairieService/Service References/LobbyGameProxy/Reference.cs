﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.34209
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LibrairieService.LobbyGameProxy {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="RoomInfo", Namespace="http://schemas.datacontract.org/2004/07/LibrairieService.Models")]
    [System.SerializableAttribute()]
    public partial class RoomInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MasterNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid TokenField;
        
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
        public string MasterName {
            get {
                return this.MasterNameField;
            }
            set {
                if ((object.ReferenceEquals(this.MasterNameField, value) != true)) {
                    this.MasterNameField = value;
                    this.RaisePropertyChanged("MasterName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid Token {
            get {
                return this.TokenField;
            }
            set {
                if ((this.TokenField.Equals(value) != true)) {
                    this.TokenField = value;
                    this.RaisePropertyChanged("Token");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="UserMessage", Namespace="http://schemas.datacontract.org/2004/07/LibrairieService.Models")]
    [System.SerializableAttribute()]
    public partial class UserMessage : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ContentField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime DateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UsernameField;
        
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
        public string Content {
            get {
                return this.ContentField;
            }
            set {
                if ((object.ReferenceEquals(this.ContentField, value) != true)) {
                    this.ContentField = value;
                    this.RaisePropertyChanged("Content");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime Date {
            get {
                return this.DateField;
            }
            set {
                if ((this.DateField.Equals(value) != true)) {
                    this.DateField = value;
                    this.RaisePropertyChanged("Date");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Username {
            get {
                return this.UsernameField;
            }
            set {
                if ((object.ReferenceEquals(this.UsernameField, value) != true)) {
                    this.UsernameField = value;
                    this.RaisePropertyChanged("Username");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="PlayerConfigEntry", Namespace="http://schemas.datacontract.org/2004/07/LibrairieService.Models")]
    [System.SerializableAttribute()]
    public partial class PlayerConfigEntry : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int OrderField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int TeamField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid UserTokenField;
        
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
        public int Order {
            get {
                return this.OrderField;
            }
            set {
                if ((this.OrderField.Equals(value) != true)) {
                    this.OrderField = value;
                    this.RaisePropertyChanged("Order");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Team {
            get {
                return this.TeamField;
            }
            set {
                if ((this.TeamField.Equals(value) != true)) {
                    this.TeamField = value;
                    this.RaisePropertyChanged("Team");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid UserToken {
            get {
                return this.UserTokenField;
            }
            set {
                if ((this.UserTokenField.Equals(value) != true)) {
                    this.UserTokenField = value;
                    this.RaisePropertyChanged("UserToken");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="LobbyGameProxy.ILobbyService")]
    public interface ILobbyService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/CreateRoom", ReplyAction="http://tempuri.org/ILobbyService/CreateRoomResponse")]
        System.Nullable<System.Guid> CreateRoom(string name, System.Guid roomMaster);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/CreateRoom", ReplyAction="http://tempuri.org/ILobbyService/CreateRoomResponse")]
        System.Threading.Tasks.Task<System.Nullable<System.Guid>> CreateRoomAsync(string name, System.Guid roomMaster);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/JoinRoom", ReplyAction="http://tempuri.org/ILobbyService/JoinRoomResponse")]
        bool JoinRoom(System.Guid player, System.Guid room);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/JoinRoom", ReplyAction="http://tempuri.org/ILobbyService/JoinRoomResponse")]
        System.Threading.Tasks.Task<bool> JoinRoomAsync(System.Guid player, System.Guid room);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/JoinRoomGameEnd", ReplyAction="http://tempuri.org/ILobbyService/JoinRoomGameEndResponse")]
        bool JoinRoomGameEnd(System.Guid player, System.Guid gameEndToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/JoinRoomGameEnd", ReplyAction="http://tempuri.org/ILobbyService/JoinRoomGameEndResponse")]
        System.Threading.Tasks.Task<bool> JoinRoomGameEndAsync(System.Guid player, System.Guid gameEndToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/LeaveRoom", ReplyAction="http://tempuri.org/ILobbyService/LeaveRoomResponse")]
        bool LeaveRoom(System.Guid player, System.Guid room);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/LeaveRoom", ReplyAction="http://tempuri.org/ILobbyService/LeaveRoomResponse")]
        System.Threading.Tasks.Task<bool> LeaveRoomAsync(System.Guid player, System.Guid room);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/SetReady", ReplyAction="http://tempuri.org/ILobbyService/SetReadyResponse")]
        bool SetReady(System.Guid player, bool ready);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/SetReady", ReplyAction="http://tempuri.org/ILobbyService/SetReadyResponse")]
        System.Threading.Tasks.Task<bool> SetReadyAsync(System.Guid player, bool ready);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/SendRoomMessage", ReplyAction="http://tempuri.org/ILobbyService/SendRoomMessageResponse")]
        bool SendRoomMessage(System.Guid player, System.Guid room, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/SendRoomMessage", ReplyAction="http://tempuri.org/ILobbyService/SendRoomMessageResponse")]
        System.Threading.Tasks.Task<bool> SendRoomMessageAsync(System.Guid player, System.Guid room, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/SetConfig", ReplyAction="http://tempuri.org/ILobbyService/SetConfigResponse")]
        bool SetConfig(System.Guid player, System.Guid room);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/SetConfig", ReplyAction="http://tempuri.org/ILobbyService/SetConfigResponse")]
        System.Threading.Tasks.Task<bool> SetConfigAsync(System.Guid player, System.Guid room);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/SetName", ReplyAction="http://tempuri.org/ILobbyService/SetNameResponse")]
        bool SetName(System.Guid player, System.Guid room, string newName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/SetName", ReplyAction="http://tempuri.org/ILobbyService/SetNameResponse")]
        System.Threading.Tasks.Task<bool> SetNameAsync(System.Guid player, System.Guid room, string newName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/SetMaster", ReplyAction="http://tempuri.org/ILobbyService/SetMasterResponse")]
        bool SetMaster(System.Guid player, System.Guid room, string newMaster);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/SetMaster", ReplyAction="http://tempuri.org/ILobbyService/SetMasterResponse")]
        System.Threading.Tasks.Task<bool> SetMasterAsync(System.Guid player, System.Guid room, string newMaster);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetOpenRoomsInfo", ReplyAction="http://tempuri.org/ILobbyService/GetOpenRoomsInfoResponse")]
        LibrairieService.LobbyGameProxy.RoomInfo[] GetOpenRoomsInfo();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetOpenRoomsInfo", ReplyAction="http://tempuri.org/ILobbyService/GetOpenRoomsInfoResponse")]
        System.Threading.Tasks.Task<LibrairieService.LobbyGameProxy.RoomInfo[]> GetOpenRoomsInfoAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetLoggedUsers", ReplyAction="http://tempuri.org/ILobbyService/GetLoggedUsersResponse")]
        System.Collections.Generic.Dictionary<string, byte> GetLoggedUsers();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetLoggedUsers", ReplyAction="http://tempuri.org/ILobbyService/GetLoggedUsersResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, byte>> GetLoggedUsersAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetRoomPlayers", ReplyAction="http://tempuri.org/ILobbyService/GetRoomPlayersResponse")]
        string[] GetRoomPlayers(System.Guid room);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetRoomPlayers", ReplyAction="http://tempuri.org/ILobbyService/GetRoomPlayersResponse")]
        System.Threading.Tasks.Task<string[]> GetRoomPlayersAsync(System.Guid room);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetRoomMessages", ReplyAction="http://tempuri.org/ILobbyService/GetRoomMessagesResponse")]
        LibrairieService.LobbyGameProxy.UserMessage[] GetRoomMessages(System.Guid room, int limit);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetRoomMessages", ReplyAction="http://tempuri.org/ILobbyService/GetRoomMessagesResponse")]
        System.Threading.Tasks.Task<LibrairieService.LobbyGameProxy.UserMessage[]> GetRoomMessagesAsync(System.Guid room, int limit);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetMasterName", ReplyAction="http://tempuri.org/ILobbyService/GetMasterNameResponse")]
        string GetMasterName(System.Guid room);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetMasterName", ReplyAction="http://tempuri.org/ILobbyService/GetMasterNameResponse")]
        System.Threading.Tasks.Task<string> GetMasterNameAsync(System.Guid room);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/SetTeam", ReplyAction="http://tempuri.org/ILobbyService/SetTeamResponse")]
        bool SetTeam(System.Guid player, System.Guid room, int team);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/SetTeam", ReplyAction="http://tempuri.org/ILobbyService/SetTeamResponse")]
        System.Threading.Tasks.Task<bool> SetTeamAsync(System.Guid player, System.Guid room, int team);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetCurrentTeamIndex", ReplyAction="http://tempuri.org/ILobbyService/GetCurrentTeamIndexResponse")]
        int GetCurrentTeamIndex(System.Guid player);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetCurrentTeamIndex", ReplyAction="http://tempuri.org/ILobbyService/GetCurrentTeamIndexResponse")]
        System.Threading.Tasks.Task<int> GetCurrentTeamIndexAsync(System.Guid player);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/SetRoomName", ReplyAction="http://tempuri.org/ILobbyService/SetRoomNameResponse")]
        bool SetRoomName(System.Guid room, string newName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/SetRoomName", ReplyAction="http://tempuri.org/ILobbyService/SetRoomNameResponse")]
        System.Threading.Tasks.Task<bool> SetRoomNameAsync(System.Guid room, string newName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetCurrentGameToken", ReplyAction="http://tempuri.org/ILobbyService/GetCurrentGameTokenResponse")]
        System.Nullable<System.Guid> GetCurrentGameToken(System.Guid room);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetCurrentGameToken", ReplyAction="http://tempuri.org/ILobbyService/GetCurrentGameTokenResponse")]
        System.Threading.Tasks.Task<System.Nullable<System.Guid>> GetCurrentGameTokenAsync(System.Guid room);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetNewMessageFrom", ReplyAction="http://tempuri.org/ILobbyService/GetNewMessageFromResponse")]
        LibrairieService.LobbyGameProxy.UserMessage[] GetNewMessageFrom(System.Guid player, string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetNewMessageFrom", ReplyAction="http://tempuri.org/ILobbyService/GetNewMessageFromResponse")]
        System.Threading.Tasks.Task<LibrairieService.LobbyGameProxy.UserMessage[]> GetNewMessageFromAsync(System.Guid player, string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/DoTheBigBastardThatWaveHisFlagHasNewMessageFromS" +
            "omeoneIfSoTellMeLad", ReplyAction="http://tempuri.org/ILobbyService/DoTheBigBastardThatWaveHisFlagHasNewMessageFromS" +
            "omeoneIfSoTellMeLadResponse")]
        string[] DoTheBigBastardThatWaveHisFlagHasNewMessageFromSomeoneIfSoTellMeLad(System.Guid player);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/DoTheBigBastardThatWaveHisFlagHasNewMessageFromS" +
            "omeoneIfSoTellMeLad", ReplyAction="http://tempuri.org/ILobbyService/DoTheBigBastardThatWaveHisFlagHasNewMessageFromS" +
            "omeoneIfSoTellMeLadResponse")]
        System.Threading.Tasks.Task<string[]> DoTheBigBastardThatWaveHisFlagHasNewMessageFromSomeoneIfSoTellMeLadAsync(System.Guid player);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/SendPrivateMessage", ReplyAction="http://tempuri.org/ILobbyService/SendPrivateMessageResponse")]
        bool SendPrivateMessage(System.Guid from, string to, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/SendPrivateMessage", ReplyAction="http://tempuri.org/ILobbyService/SendPrivateMessageResponse")]
        System.Threading.Tasks.Task<bool> SendPrivateMessageAsync(System.Guid from, string to, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetPlayerConfig", ReplyAction="http://tempuri.org/ILobbyService/GetPlayerConfigResponse")]
        LibrairieService.LobbyGameProxy.PlayerConfigEntry[] GetPlayerConfig(System.Guid roomToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILobbyService/GetPlayerConfig", ReplyAction="http://tempuri.org/ILobbyService/GetPlayerConfigResponse")]
        System.Threading.Tasks.Task<LibrairieService.LobbyGameProxy.PlayerConfigEntry[]> GetPlayerConfigAsync(System.Guid roomToken);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ILobbyServiceChannel : LibrairieService.LobbyGameProxy.ILobbyService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LobbyServiceClient : System.ServiceModel.ClientBase<LibrairieService.LobbyGameProxy.ILobbyService>, LibrairieService.LobbyGameProxy.ILobbyService {
        
        public LobbyServiceClient() {
        }
        
        public LobbyServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LobbyServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LobbyServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LobbyServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Nullable<System.Guid> CreateRoom(string name, System.Guid roomMaster) {
            return base.Channel.CreateRoom(name, roomMaster);
        }
        
        public System.Threading.Tasks.Task<System.Nullable<System.Guid>> CreateRoomAsync(string name, System.Guid roomMaster) {
            return base.Channel.CreateRoomAsync(name, roomMaster);
        }
        
        public bool JoinRoom(System.Guid player, System.Guid room) {
            return base.Channel.JoinRoom(player, room);
        }
        
        public System.Threading.Tasks.Task<bool> JoinRoomAsync(System.Guid player, System.Guid room) {
            return base.Channel.JoinRoomAsync(player, room);
        }
        
        public bool JoinRoomGameEnd(System.Guid player, System.Guid gameEndToken) {
            return base.Channel.JoinRoomGameEnd(player, gameEndToken);
        }
        
        public System.Threading.Tasks.Task<bool> JoinRoomGameEndAsync(System.Guid player, System.Guid gameEndToken) {
            return base.Channel.JoinRoomGameEndAsync(player, gameEndToken);
        }
        
        public bool LeaveRoom(System.Guid player, System.Guid room) {
            return base.Channel.LeaveRoom(player, room);
        }
        
        public System.Threading.Tasks.Task<bool> LeaveRoomAsync(System.Guid player, System.Guid room) {
            return base.Channel.LeaveRoomAsync(player, room);
        }
        
        public bool SetReady(System.Guid player, bool ready) {
            return base.Channel.SetReady(player, ready);
        }
        
        public System.Threading.Tasks.Task<bool> SetReadyAsync(System.Guid player, bool ready) {
            return base.Channel.SetReadyAsync(player, ready);
        }
        
        public bool SendRoomMessage(System.Guid player, System.Guid room, string message) {
            return base.Channel.SendRoomMessage(player, room, message);
        }
        
        public System.Threading.Tasks.Task<bool> SendRoomMessageAsync(System.Guid player, System.Guid room, string message) {
            return base.Channel.SendRoomMessageAsync(player, room, message);
        }
        
        public bool SetConfig(System.Guid player, System.Guid room) {
            return base.Channel.SetConfig(player, room);
        }
        
        public System.Threading.Tasks.Task<bool> SetConfigAsync(System.Guid player, System.Guid room) {
            return base.Channel.SetConfigAsync(player, room);
        }
        
        public bool SetName(System.Guid player, System.Guid room, string newName) {
            return base.Channel.SetName(player, room, newName);
        }
        
        public System.Threading.Tasks.Task<bool> SetNameAsync(System.Guid player, System.Guid room, string newName) {
            return base.Channel.SetNameAsync(player, room, newName);
        }
        
        public bool SetMaster(System.Guid player, System.Guid room, string newMaster) {
            return base.Channel.SetMaster(player, room, newMaster);
        }
        
        public System.Threading.Tasks.Task<bool> SetMasterAsync(System.Guid player, System.Guid room, string newMaster) {
            return base.Channel.SetMasterAsync(player, room, newMaster);
        }
        
        public LibrairieService.LobbyGameProxy.RoomInfo[] GetOpenRoomsInfo() {
            return base.Channel.GetOpenRoomsInfo();
        }
        
        public System.Threading.Tasks.Task<LibrairieService.LobbyGameProxy.RoomInfo[]> GetOpenRoomsInfoAsync() {
            return base.Channel.GetOpenRoomsInfoAsync();
        }
        
        public System.Collections.Generic.Dictionary<string, byte> GetLoggedUsers() {
            return base.Channel.GetLoggedUsers();
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, byte>> GetLoggedUsersAsync() {
            return base.Channel.GetLoggedUsersAsync();
        }
        
        public string[] GetRoomPlayers(System.Guid room) {
            return base.Channel.GetRoomPlayers(room);
        }
        
        public System.Threading.Tasks.Task<string[]> GetRoomPlayersAsync(System.Guid room) {
            return base.Channel.GetRoomPlayersAsync(room);
        }
        
        public LibrairieService.LobbyGameProxy.UserMessage[] GetRoomMessages(System.Guid room, int limit) {
            return base.Channel.GetRoomMessages(room, limit);
        }
        
        public System.Threading.Tasks.Task<LibrairieService.LobbyGameProxy.UserMessage[]> GetRoomMessagesAsync(System.Guid room, int limit) {
            return base.Channel.GetRoomMessagesAsync(room, limit);
        }
        
        public string GetMasterName(System.Guid room) {
            return base.Channel.GetMasterName(room);
        }
        
        public System.Threading.Tasks.Task<string> GetMasterNameAsync(System.Guid room) {
            return base.Channel.GetMasterNameAsync(room);
        }
        
        public bool SetTeam(System.Guid player, System.Guid room, int team) {
            return base.Channel.SetTeam(player, room, team);
        }
        
        public System.Threading.Tasks.Task<bool> SetTeamAsync(System.Guid player, System.Guid room, int team) {
            return base.Channel.SetTeamAsync(player, room, team);
        }
        
        public int GetCurrentTeamIndex(System.Guid player) {
            return base.Channel.GetCurrentTeamIndex(player);
        }
        
        public System.Threading.Tasks.Task<int> GetCurrentTeamIndexAsync(System.Guid player) {
            return base.Channel.GetCurrentTeamIndexAsync(player);
        }
        
        public bool SetRoomName(System.Guid room, string newName) {
            return base.Channel.SetRoomName(room, newName);
        }
        
        public System.Threading.Tasks.Task<bool> SetRoomNameAsync(System.Guid room, string newName) {
            return base.Channel.SetRoomNameAsync(room, newName);
        }
        
        public System.Nullable<System.Guid> GetCurrentGameToken(System.Guid room) {
            return base.Channel.GetCurrentGameToken(room);
        }
        
        public System.Threading.Tasks.Task<System.Nullable<System.Guid>> GetCurrentGameTokenAsync(System.Guid room) {
            return base.Channel.GetCurrentGameTokenAsync(room);
        }
        
        public LibrairieService.LobbyGameProxy.UserMessage[] GetNewMessageFrom(System.Guid player, string name) {
            return base.Channel.GetNewMessageFrom(player, name);
        }
        
        public System.Threading.Tasks.Task<LibrairieService.LobbyGameProxy.UserMessage[]> GetNewMessageFromAsync(System.Guid player, string name) {
            return base.Channel.GetNewMessageFromAsync(player, name);
        }
        
        public string[] DoTheBigBastardThatWaveHisFlagHasNewMessageFromSomeoneIfSoTellMeLad(System.Guid player) {
            return base.Channel.DoTheBigBastardThatWaveHisFlagHasNewMessageFromSomeoneIfSoTellMeLad(player);
        }
        
        public System.Threading.Tasks.Task<string[]> DoTheBigBastardThatWaveHisFlagHasNewMessageFromSomeoneIfSoTellMeLadAsync(System.Guid player) {
            return base.Channel.DoTheBigBastardThatWaveHisFlagHasNewMessageFromSomeoneIfSoTellMeLadAsync(player);
        }
        
        public bool SendPrivateMessage(System.Guid from, string to, string message) {
            return base.Channel.SendPrivateMessage(from, to, message);
        }
        
        public System.Threading.Tasks.Task<bool> SendPrivateMessageAsync(System.Guid from, string to, string message) {
            return base.Channel.SendPrivateMessageAsync(from, to, message);
        }
        
        public LibrairieService.LobbyGameProxy.PlayerConfigEntry[] GetPlayerConfig(System.Guid roomToken) {
            return base.Channel.GetPlayerConfig(roomToken);
        }
        
        public System.Threading.Tasks.Task<LibrairieService.LobbyGameProxy.PlayerConfigEntry[]> GetPlayerConfigAsync(System.Guid roomToken) {
            return base.Channel.GetPlayerConfigAsync(roomToken);
        }
    }
}
