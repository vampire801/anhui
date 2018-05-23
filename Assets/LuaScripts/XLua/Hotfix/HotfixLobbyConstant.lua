-- added by le @ 2018-04-27
-- xlua热修复，修改lobbyContent包括版本号
-- 注意：
-- 1、现在的做法热修复模块一定要提供Register、Unregister两个接口，因为现在热修复模块要支持动态加载和卸载
-- 2、注册使用xlua.hotfix或者util.hotfix_ex
-- 3、注销一律使用xlua.hotfix

local util = require "XLua.Common.util"
local LobbyContants = CS.MahjongLobby_AH.LobbyContants
xlua.private_accessible(LobbyContants)

local function Register()
	LobbyContants.version_v='v1.2.11'
	--xlua.hotfix(LobbyContants, "_g_get_SeverVersion", get_SeverVersion_x)
end

local function Unregister()
	--xlua.hotfix(SeverVersion, "_g_get_SeverVersion", nil)
end

return {
	Register = Register,
	Unregister = Unregister,
}