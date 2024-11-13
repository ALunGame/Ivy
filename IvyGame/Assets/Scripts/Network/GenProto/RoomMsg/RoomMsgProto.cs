// <auto-generated>
//   This file was generated by a tool; you should avoid making direct changes.
//   Consider using 'partial classes' to extend these types
//   Input: RoomMsgProto.proto
// </auto-generated>

#region Designer generated code
#pragma warning disable CS0612, CS0618, CS1591, CS3021, IDE0079, IDE1006, RCS1036, RCS1057, RCS1085, RCS1192
namespace Proto
{

    [global::ProtoBuf.ProtoContract(Name = @"join_gamer_info")]
    public partial class JoinGamerInfo : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"id", IsRequired = true)]
        public int Id { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"name", IsRequired = true)]
        public string Name { get; set; }

        [global::ProtoBuf.ProtoMember(3, IsRequired = true)]
        public int fightMusicId { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class CreateRoomC2s : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
        public int gameMode { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"gamer", IsRequired = true)]
        public JoinGamerInfo Gamer { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class CreateRoomS2c : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"ret_code", IsRequired = true)]
        public int RetCode { get; set; }

        [global::ProtoBuf.ProtoMember(2, IsRequired = true)]
        public int gameMode { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class JoinRoomC2s : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"gamer", IsRequired = true)]
        public JoinGamerInfo Gamer { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class JoinRoomS2c : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"ret_code", IsRequired = true)]
        public int RetCode { get; set; }

        [global::ProtoBuf.ProtoMember(2, IsRequired = true)]
        public int gameMode { get; set; }

        [global::ProtoBuf.ProtoMember(3, IsRequired = true)]
        public string roomMastergamerUid { get; set; }

        [global::ProtoBuf.ProtoMember(4, IsRequired = true)]
        public string selfgamerUid { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class LeaveRoomS2c : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
        public string gamerUid { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class RoomMembersChangeS2c : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"gamers")]
        public global::System.Collections.Generic.List<GamerInfo> Gamers { get; } = new global::System.Collections.Generic.List<GamerInfo>();

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class StartGameC2s : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
        public int gameCfgId { get; set; }

        [global::ProtoBuf.ProtoMember(2, IsRequired = true)]
        public int gameMode { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class StartGameS2c : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"ret_code", IsRequired = true)]
        public int RetCode { get; set; }

        [global::ProtoBuf.ProtoMember(2, IsRequired = true)]
        public int gameCfgId { get; set; }

        [global::ProtoBuf.ProtoMember(3, IsRequired = true)]
        public int gameMode { get; set; }

        [global::ProtoBuf.ProtoMember(4, Name = @"gamers")]
        public global::System.Collections.Generic.List<GamerInfo> Gamers { get; } = new global::System.Collections.Generic.List<GamerInfo>();

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class EnterMapS2c : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"ret_code", IsRequired = true)]
        public int RetCode { get; set; }

        [global::ProtoBuf.ProtoMember(2, IsRequired = true)]
        public int mapId { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class GamerInputC2s : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
        public string gamerUid { get; set; }

        [global::ProtoBuf.ProtoMember(2, IsRequired = true)]
        public int commandType { get; set; }

        [global::ProtoBuf.ProtoMember(3, Name = @"rotation", IsRequired = true)]
        public float Rotation { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class GamerInputS2c : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
        public string gamerUid { get; set; }

        [global::ProtoBuf.ProtoMember(2, IsRequired = true)]
        public int moveClickType { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class GamerSkillInputC2s : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
        public string gamerUid { get; set; }

        [global::ProtoBuf.ProtoMember(2, IsRequired = true)]
        public int skillId { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class GamerSkillInputS2c : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"ret_code", IsRequired = true)]
        public int RetCode { get; set; }

        [global::ProtoBuf.ProtoMember(2, IsRequired = true)]
        public string gamerUid { get; set; }

        [global::ProtoBuf.ProtoMember(3, Name = @"pos", IsRequired = true)]
        public NetVector2 Pos { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class GamerBaseState : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
        public string gamerUid { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"pos", IsRequired = true)]
        public NetVector2 Pos { get; set; }

        [global::ProtoBuf.ProtoMember(3, Name = @"rotation", IsRequired = true)]
        public int Rotation { get; set; }

        [global::ProtoBuf.ProtoMember(4, IsRequired = true)]
        public int commandTick { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ServerStateS2c : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
        public int serverTick { get; set; }

        [global::ProtoBuf.ProtoMember(2, IsRequired = true)]
        public int commandTick { get; set; }

        [global::ProtoBuf.ProtoMember(3, IsRequired = true)]
        public int gameTime { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public global::System.Collections.Generic.List<GamerBaseState> gamerStates { get; } = new global::System.Collections.Generic.List<GamerBaseState>();

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ChangeGridCampS2c : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"gridPosList")]
        public global::System.Collections.Generic.List<NetVector2> gridPosLists { get; } = new global::System.Collections.Generic.List<NetVector2>();

        [global::ProtoBuf.ProtoMember(2, Name = @"camp", IsRequired = true)]
        public int Camp { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class GamerPathChangeS2c : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
        public string gamerUid { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"operate", IsRequired = true)]
        public int Operate { get; set; }

        [global::ProtoBuf.ProtoMember(3, Name = @"pos", IsRequired = true)]
        public NetVector2 Pos { get; set; }

    }

    [global::ProtoBuf.ProtoContract(Name = @"gamer_die_info")]
    public partial class GamerDieInfo : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
        public string gamerUid { get; set; }

        [global::ProtoBuf.ProtoMember(2, IsRequired = true)]
        public int rebornTime { get; set; }

        [global::ProtoBuf.ProtoMember(3, IsRequired = true)]
        public string killergamerUid { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class GamerDieS2c : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<GamerDieInfo> dieGamerInfos { get; } = new global::System.Collections.Generic.List<GamerDieInfo>();

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class GamerRebornS2c : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
        public string gamerUid { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"pos", IsRequired = true)]
        public NetVector2 Pos { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class GameEndS2c : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"gamers")]
        public global::System.Collections.Generic.List<GamerInfo> Gamers { get; } = new global::System.Collections.Generic.List<GamerInfo>();

    }

}

#pragma warning restore CS0612, CS0618, CS1591, CS3021, IDE0079, IDE1006, RCS1036, RCS1057, RCS1085, RCS1192
#endregion
