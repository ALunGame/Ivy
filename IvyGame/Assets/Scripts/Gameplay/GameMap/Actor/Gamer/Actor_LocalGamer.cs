using Game;
using Game.Network;
using Game.Network.Client;
using Gameplay.GameData;
using IAEngine;
using IAFramework;
using Proto;
using ProtoBuf;
using UnityEngine;

namespace Gameplay.GameMap.Actor
{
    public class Actor_LocalGamer : Actor_Gamer<LocalGamerData>
    {
        private float preMoveTotalTime = NetworkLogicTimer.FixedDelta * 3;
        private Vector2 currTargetPos;
        private Vector3 currMoveTargetPos;
        private float currMoveTime;

        public Actor_LocalGamer(string pUid, int pId, ActorType pType, GameObject pActorGo) : base(pUid, pId, pType, pActorGo)
        {
        }

        protected override void OnInit()
        {
            base.OnInit();
            NetworkEvent.RegisterEvent($"Actor_{Uid}", (ushort)RoomMsgDefine.GamerInputS2c, OnRecGamerInputS2c);
        }

        protected override void OnClear()
        {
            NetworkEvent.RemoveEvent($"Actor_{Uid}");
        }

        private void OnRecGamerInputS2c(IExtensible pMsg)
        {
            GamerInputS2c msg = (GamerInputS2c)pMsg;
            MoveClickType clickType = (MoveClickType)msg.moveClickType;

            if (clickType == MoveClickType.Perfect)
            {
                Transform effectRTTrans;
                if (!Wheel_RL.Find("WheelDriveEffect", out effectRTTrans))
                {
                    effectRTTrans = GameEnv.Asset.CreateGo("WheelDriveEffect").transform;
                    effectRTTrans.name = "WheelDriveEffect";
                    effectRTTrans.SetParent(Wheel_RL);
                    effectRTTrans.Reset();
                }
                effectRTTrans.gameObject.SetActive(true);
                ParticleSystem effectRTParticle = effectRTTrans.Find("Perfect").GetComponent<ParticleSystem>();
                effectRTParticle.Play();

                Transform effectRRTrans;
                if (!Wheel_RR.Find("WheelDriveEffect", out effectRRTrans))
                {
                    effectRRTrans = GameEnv.Asset.CreateGo("WheelDriveEffect").transform;
                    effectRRTrans.name = "WheelDriveEffect";
                    effectRRTrans.SetParent(Wheel_RR);
                    effectRRTrans.Reset();
                }
                    
                effectRRTrans.gameObject.SetActive(true);
                ParticleSystem effectRRParticle = effectRRTrans.Find("Perfect").GetComponent<ParticleSystem>();
                effectRRParticle.Play();
            }
        }
    }
}
