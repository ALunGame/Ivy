namespace Gameplay.UserData
{
    internal class UserDataCenter
    {
        public RoomUserData Room { get; private set; }
        public GameUserData Game {  get; private set; }

        public void Init()
        {
            Room = new RoomUserData();
            Game = new GameUserData();
        }

        public void Clear()
        {
            Room = null;
            Game = null;
        }
    }
}
