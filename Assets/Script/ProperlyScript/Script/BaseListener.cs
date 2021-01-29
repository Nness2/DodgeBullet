using UnityEngine.Events;


namespace DodgeBullet
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BaseListener<T, E, UE> : MonoBehaviour, IListener<T> where E : BaseEvent<T> where UE : UnityEvent<T>
    {
        [SerializeField] private E _event;
        [SerializeField] private UE _responce;


        private void OnEnable()
        {
            if (_event != null)
            {
                _event.Register(this);
            }
        }

        private void OnDisable()
        {
            if (_event != null)
            {
                _event.Unregister(this);
            }
        }

        public void Raise(T value)
        {
            if(_responce != null)
            {
                _responce.Invoke(value);
            }
        }

    }

}