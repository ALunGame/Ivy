using System;

namespace Gameplay.GameData
{
    public class InternalGameDataField
    {
        public InternalGameDataField(BaseGameData pGameData)
        {
            pGameData.AddGameDataField(this);
        }

        public virtual void Clear()
        {

        }
    }

    public class GameDataField<T> : InternalGameDataField where T : struct
    {
        private T _value;
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (Equals(Value, value))
                    return;
                _value = value;
                OnValueChanged();
            }
        }

        private Action<T> changeNotifys = null;

        public GameDataField(BaseGameData pGameData) : base(pGameData)
        {
        }

        private void OnValueChanged()
        {
            if (changeNotifys != null)
                changeNotifys.Invoke(Value);
        }

        public void SetValueWithoutNotify(T pValue)
        {
            if (Equals(Value, pValue))
                return;
            Value = pValue;
        }

        public void RegValueChangedEvent(Action<T> pOnValueChanged)
        {
            changeNotifys -= pOnValueChanged;
            changeNotifys += pOnValueChanged;
        }

        public void RemoveValueChangedEvent(Action<T> pOnValueChanged)
        {
            changeNotifys -= pOnValueChanged;
        }

        public override void Clear()
        {
            changeNotifys = null;
        }
    }
}
