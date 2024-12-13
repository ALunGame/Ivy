using System;

namespace IAUI
{
    public class InternalUserData
    {
        private event Action onUserDataChange;

        public void Init()
        {
            OnInit();
        }

        public void Clear()
        {
            onUserDataChange = null;
            OnClear();
        }

        public void RegUserDataChange(Action pCallBack)
        {
            onUserDataChange += pCallBack;
        }

        public void RemoveUserDataChange(Action pCallBack)
        {
            onUserDataChange -= pCallBack;
        }

        /// <summary>
        /// 更新用户数据
        /// 1，执行监听事件
        /// </summary>
        public void UpdateUserData()
        {
            onUserDataChange?.Invoke();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void OnInit() { }

        /// <summary>
        /// 清理
        /// </summary>
        protected virtual void OnClear() { }
    }

    public class BaseUserData<T> : InternalUserData where T : new()
    {
        private static readonly T instance = new T();
        public static T Instance
        {
            get
            {
                return instance;
            }
        }

        private event Action onUserDataChange;

        protected BaseUserData() { }

    }
}
