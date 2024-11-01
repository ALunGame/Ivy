using IAEngine;
using UnityEngine;

namespace Gameplay.GameMap.Actor
{
    public class ActorModel_Display
    {
        #region Const

        public const string DefaultName = "Default";
        public const string StateRootName = "State";
        public const string DisplayGoName = "Display";
        public const string CameraFollowGoName = "Camera_Follow";

        #endregion

        private ActorModel actor;

        //状态节点
        public GameObject StateRootGo { get; private set; }
        //状态节点
        public GameObject StateGo { get; private set; }
        //表现节点
        public GameObject DisplayGo { get; private set; }
        //相机跟随节点
        public GameObject CameraFollowGo { get; private set; }
        //点击区域
        public PolygonCollider2D ClickCollider { get; private set; }

        public ActorModel_Display(ActorModel pActor)
        {
            actor = pActor;
            StateRootGo = pActor.ActorGo.transform.Find(StateRootName).gameObject;
        }

        public void ChangeState(string pStateName)
        {
            if (actor.ActorGo == null)
            {
                StateGo = null;
                DisplayGo = null;
                CameraFollowGo = null;
                return;
            }

            //没有状态节点，直接默认
            Transform stateRoot = StateRootGo.transform;
            if (stateRoot == null)
            {
                StateGo = actor.ActorGo.gameObject;
                DisplayGo = actor.ActorGo.gameObject;
                CameraFollowGo = actor.ActorGo.gameObject;
                return;
            }

            //隐藏旧的
            if (stateRoot.Find(pStateName, out Transform oldTrans))
            {
                oldTrans.gameObject.SetActive(false);
            }

            //没有这个状态走第一个默认
            if (!stateRoot.Find(pStateName, out Transform checkTrans))
            {
                pStateName = stateRoot.GetChild(0).name;
            }

            //赋值新的
            if (stateRoot.Find(pStateName, out Transform newStateTrans))
            {
                StateGo = newStateTrans.gameObject;

                //表现节点
                if (StateGo.transform.Find(DisplayGoName, out Transform newDisplayTrans))
                    DisplayGo = newDisplayTrans.gameObject;
                else
                    DisplayGo = StateGo;

                //跟随节点
                if (StateGo.transform.Find(CameraFollowGoName, out Transform newCMFollowTrans))
                    CameraFollowGo = newCMFollowTrans.gameObject;
                else
                    CameraFollowGo = StateGo;

                //点击节点
                if (DisplayGo.Find("ClickBox", out Transform clickTrans))
                {
                    ClickCollider = clickTrans.GetComponent<PolygonCollider2D>();
                }
                StateGo.SetActive(true);
            }
            else
            {
                Debug.LogError($"设置状态节点出错>>>{actor.ActorGo.name},{pStateName}");
            }
        }

        public Vector3 GetCameraFollowPos()
        {
            return CameraFollowGo.transform.position;
        }
    }
}
