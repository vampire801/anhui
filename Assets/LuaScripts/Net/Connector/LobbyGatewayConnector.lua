--[[
-- added by wsh @ 2017-01-09
-- 大厅网关网络连接器
--]]

local LobbyGatewayConnector = BaseClass("LobbyGatewayConnector", Singleton)
local SendMsgDefine = require "Net.Config.SendMsgDefine"
local NetUtil = require "Net.Util.NetUtil"

local ConnStatus = {
	Init = 0,
	Connecting = 1,
	WaitLogin = 2,
	Done = 3,
}

local function __init(self)
	self.socket = nil
	self.globalSeq = 0
end

local function OnReceivePackage(self, receive_bytes)
	local receive_msg = NetUtil.DeserializeMessage(receive_bytes)
	Logger.Log(tostring(receive_msg))
end

local function Connect(self, host_ip, host_port, on_connect, on_close)
	if not self.socket then
		self.socket = CS.Networks.HjTcpNetwork()
		self.socket.ReceivePkgHandle = Bind(self, OnReceivePackage)
	end
	self.socket.OnConnect = on_connect
	self.socket.OnClosed = on_close
	self.socket:SetHostPort(host_ip, host_port)
	self.socket:Connect()
	Logger.Log("Connect to "..host_ip..", port : "..host_port)
	return self.socket
end

local function SendMessage(self, msg_id, msg_obj, show_mask, need_resend)
	show_mask = show_mask == nil and true or show_mask
	need_resend = need_resend == nil and true or need_resend
	
	local request_seq = 0
	local send_msg = SendMsgDefine.New(msg_id, msg_obj, request_seq)
	local msg_bytes = NetUtil.SerializeMessage(send_msg, self.globalSeq)
	Logger.Log(tostring(send_msg))	
	self.socket:SendMessage(msg_id, msg_bytes)
	self.globalSeq = self.globalSeq + 1
end

local function Update(self)
	if self.socket then
		self.socket:UpdateNetwork()
	end
end

local function Disconnect(self)
	if self.socket then
		self.socket:Disconnect()
	end
end

local function Dispose(self)
	if self.socket then
		self.socket:Dispose()
	end
	self.socket = nil
end

LobbyGatewayConnector.__init = __init
LobbyGatewayConnector.Connect = Connect
LobbyGatewayConnector.SendMessage = SendMessage
LobbyGatewayConnector.Update = Update
LobbyGatewayConnector.Disconnect = Disconnect
LobbyGatewayConnector.Dispose = Dispose

return LobbyGatewayConnector
