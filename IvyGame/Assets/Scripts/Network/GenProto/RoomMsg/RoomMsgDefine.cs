﻿
namespace Proto
{
    public enum RoomMsgDefine : ushort
    {
        
        CreateRoomC2s = 200,
        JoinRoomC2s = 201,
        StartGameC2s = 202,
        GamerInputC2s = 203,
        GamerSkillInputC2s = 204,
        CreateRoomS2c = 300,
        JoinRoomS2c = 301,
        LeaveRoomS2c = 302,
        RoomMembersChangeS2c = 303,
        StartGameS2c = 304,
        EnterMapS2c = 305,
        ServerStateS2c = 306,
        GamerInputS2c = 307,
        GamerSkillInputS2c = 308,
        GamerDieS2c = 309,
        GamerRebornS2c = 310,
        GamerPathChangeS2c = 311,
        ChangeGridCampS2c = 312,
        GameEndS2c = 313,
    }
}
