using Gameplay.Map;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameMap.Actor
{
    public class ActorModel
    {
        public string Uid { get; private set; }

        public int Id { get; private set; }

        public ActorType Type { get; private set; }

        public bool IsActive { get; private set; }

        /// <summary>
        /// 状态名
        /// </summary>
        public string StateName { get; private set; }

        public ActorModel_Display ActorDisplay { get; private set; }

        public GameObject ActorGo { get; private set; }

        public ActorModel(string pUid, int pId, ActorType pType, GameObject pActorGo)
        {
            Uid = pUid;
            Id = pId;
            Type = pType;
            ActorGo = pActorGo;

            ActorDisplay = new ActorModel_Display(this);
            ChangeState("Default");

            IsActive = true;

            SetEditorDisplayName();
        }

        public virtual void Init() { }

        public virtual void Clear() { }

        public virtual void UpdateLogic(float pTimeDelta, float pGameTime)
        {
        }

        public void ChangeState(string pStateName)
        {
            ActorDisplay.ChangeState(pStateName);
            StateName = pStateName;
        }

        #region 移动函数

        public virtual void OnMoveStart(List<Vector3> pMovePoslist, Vector3 pTargetPos)
        {

        }

        public virtual void OnMoveUpdate()
        {

        }

        public virtual void OnMoveFinish()
        {

        }

        public virtual void OnMoveFail()
        {

        }

        #endregion

        #region Getter

        public Vector3 GetPos()
        {
            return ActorGo.transform.position;
        }

        public Vector2 GetPosXZ()
        {
            return new Vector2();
        }

        public Vector3 GetLocalPos()
        {
            return ActorGo.transform.localPosition;
        }

        public Quaternion GetRotation()
        {
            return ActorDisplay.DisplayGo.transform.localRotation;
        }


        #endregion

        #region Setter

        public void SetActive(bool pIsActive)
        {
            if (pIsActive == IsActive)
            {
                return;
            }
            IsActive = pIsActive;
            ActorGo.SetActive(pIsActive);
        }

        public void SetPos(Vector3 pPos)
        {
            ActorGo.transform.position = pPos;
            OnPosChange();
        }
        public void SetPos(Vector2 pPos)
        {
            ActorGo.transform.position = new Vector3(pPos.x, 0, pPos.y);
            OnPosChange();
        }
        protected virtual void OnPosChange() { }

        public void SetLocalPos(Vector3 pPos)
        {
            ActorGo.transform.localPosition = pPos;
            OnLocalPosChange();
        }
        protected virtual void OnLocalPosChange() { }

        public void SetRotation(Quaternion pRotate)
        {
            ActorDisplay.DisplayGo.transform.localRotation = pRotate;
            OnRotationChange();
        }
        protected virtual void OnRotationChange() { }

        public void SetDisplayActive(bool pIsActive)
        {
            ActorDisplay.DisplayGo.SetActive(pIsActive);
            OnDisplayActiveChange();
        }
        protected virtual void OnDisplayActiveChange() { }

        public void SetEditorDisplayName()
        {
#if UNITY_EDITOR
            ActorCfg actorCfg = IAConfig.Config.ActorCfg[Id];
            ActorGo.name = $"{Uid}_{Id}_{actorCfg.name}";
#endif
        }

        #endregion

    }
}
