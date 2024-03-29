﻿
namespace Proto
{
    public enum RoomMsgDefine : ushort
    {
        
        CreateRoomC2s = 200,
        JoinRoomC2s = 201,
        StartGameC2s = 202,
        PlayerMoveC2s = 203,
        CreateRoomS2c = 300,
        JoinRoomS2c = 301,
        LeaveRoomS2c = 302,
        StartGameS2c = 303,
        EnterMapS2c = 304,
        PlayerMoveS2c = 305,
        PlayerDieS2c = 306,
        PlayerRebornS2c = 307,
        PlayerPathChangeS2c = 308,
        CampAreaChangeS2c = 309,
        GameEndS2c = 310,
        RoomMembersChangeS2c = 311,
    }
}
