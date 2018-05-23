--[[
-- added by wsh @ 2017-01-10
-- 网络模块工具类
--]]

local NetUtil = {}
local unpack = unpack or table.unpack
local MsgIDMap = require "Net.Config.MsgIDMap"
local ReceiveSinglePackage = require "Net.Config.ReceiveSinglePackage"
local ReceiveMsgDefine = require "Net.Config.ReceiveMsgDefine"

local function SerializeMessage(msg_obj, global_seq)
	local output = msg_obj.MsgProto:SerializeToString()
	print("send bytes:", string.byte(output, 1, #output))
	return output
end

local function DeserializeMessage(data, start, length)
	assert(data ~= nil and type(data) == "string")
	start = start or 1
	length = length or string.len(data)
	print("receive ", string.len(data), "bytes:", string.byte(data, start, length))
	
	local receive_msg = ReceiveMsgDefine.New(0, {})
	local packages = receive_msg.Packages
	
	local index = start
	local message_version = string.unpack("=I1", data, index)
	if message_version ~= 0x4D then
		Logger.LogError("message_version error : "..message_version)
	end
	index = index + 2
	
	local msg_id = string.unpack("=I2", data, index)
	index = index + 4
		
	local msg_length = string.unpack("=I2", data, index)
	index = index + 2
		
	local msg_obj = (MsgIDMap[msg_id])()
	if msg_obj == nil then
		Logger.LogError("No proto type match msg id : "..msg_id)
		return
	end

	local pb_data = string.sub(data, index, index + msg_length - 1)
	msg_obj:ParseFromString(pb_data)
		
	local one_package = ReceiveSinglePackage.New(msg_id, msg_obj)
	table.insert(packages, one_package)
	return receive_msg
end

NetUtil.SerializeMessage = SerializeMessage
NetUtil.DeserializeMessage = DeserializeMessage

return ConstClass("NetUtil", NetUtil)