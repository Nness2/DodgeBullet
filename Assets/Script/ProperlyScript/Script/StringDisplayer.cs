using TMPro;

namespace DodgeBullet
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    public class StringDisplayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private StringVariable _variable;

        private void Start()
        {
            if (_variable != null)
            {
                UpdateDisplayer(_variable.Value);
            }
        }

        public void UpdateDisplayer(string value)
        {
            _text.SetText(value);
        }
    }
}