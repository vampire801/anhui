
local util = require "XLua.Common.util"
local MahjongCommonMethod = CS.anhui.MahjongCommonMethod
xlua.private_accessible(MahjongCommonMethod)

function Hotfix_ShowParamOfOpenRoom(item,discription,parma,isOnlyCoin,iPlayingMethod)
	CS.Logger.LogError("XLua>>>>>Hotfix_ShowParamOfOpenRoom".."开始热更")
	local str_0=string.split (item._dicMethodConfig[iPlayingMethod].sum,'_')
	local rowNum=2
	local prarmPrice=item:ReadInt32toInt4(parma[1],16)
	local isAA=item:ReadColumnValue(parma,rowNum,39)
	if iPlayingMethod~=20015 then
		if CS.Int32.Parse(item._dicMethodConfig[iPlayingMethod].type)==1 then
		table.insert(str_0[prarmPrice].."圈 ")	discription.Append()
		elseif CS.Int32.Parse(_dicMethodConfig[iPlayingMethod].type) == 2 then
			discription.Append(str_0[prarmPrice].."局 ")
		end	
	else
		
	end
	CS.Logger.LogError(discription)
    return discription
end

local function caseParamPrice1(discription)
	
end

local function Register()
	xlua.hotfix(MahjongCommonMethod, "ShowParamOfOpenRoom", Hotfix_ShowParamOfOpenRoom)
end

local function Unregister()
	xlua.hotfix(MahjongCommonMethod, "ShowParamOfOpenRoom", nil)
end

function string.split(input, delimiter)
    input = tostring(input)
    delimiter = tostring(delimiter)
    if (delimiter=='') then return false end
    local pos,arr = 0, {}
    for st,sp in function() return string.find(input, delimiter, pos, true) end do
        table.insert(arr, string.sub(input, pos, st - 1))
        pos = sp + 1
    end
    table.insert(arr, string.sub(input, pos))
    return arr
end

return {
	Register = Register,
	Unregister = Unregister,
}