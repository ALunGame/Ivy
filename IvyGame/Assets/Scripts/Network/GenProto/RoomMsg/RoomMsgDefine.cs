
namespace Proto
{
    public enum RoomMsgDefine : ushort
    {
        
        JoinRoomC2s = 200,
        StartGameC2s = 201,
        PlayerMoveC2s = 203,
        JoinRoomS2c = 300,
        LeaveRoomS2c = 301,
        StartGameS2c = 302,
        EnterMapS2c = 303,
        PlayerMoveS2c = 304,
        PlayerDieS2c = 305,
        PlayerRebornS2c = 306,
        PlayerPathChangeS2c = 307,
        CampAreaChangeS2c = 308,
    }
}
