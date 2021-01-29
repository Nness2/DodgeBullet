namespace DodgeBullet
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public interface IListener<T>
    {
        void Raise(T _value);
    }
}