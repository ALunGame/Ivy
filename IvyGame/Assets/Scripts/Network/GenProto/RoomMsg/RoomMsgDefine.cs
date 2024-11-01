﻿
namespace Proto
{
    public enum RoomMsgDefine : ushort
    {
        
        CreateRoomC2s = 200,
        JoinRoomC2s = 201,
        StartGameC2s = 202,
        GamerInputC2s = 203,
        CreateRoomS2c = 300,
        JoinRoomS2c = 301,
        LeaveRoomS2c = 302,
        RoomMembersChangeS2c = 303,
        StartGameS2c = 304,
        EnterMapS2c = 305,
        ServerStateS2c = 306,
        GamerInputS2c = 307,
        GamerDieS2c = 308,
        GamerRebornS2c = 309,
        GamerPathChangeS2c = 310,
        ChangeGridCampS2c = 311,
        GameEndS2c = 312,
    }
}
