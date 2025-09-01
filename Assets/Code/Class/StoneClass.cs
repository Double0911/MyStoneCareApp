using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Class
{
    public class StoneClass
    {
        [Serializable]
        public class PetUIButton
        {
            [SerializeField] public RectTransform petUIRectTransform;
            [SerializeField] public GameObject patButton;
            [SerializeField] public GameObject waterButton;
            [SerializeField] public GameObject feedButton;
            [SerializeField] public GameObject patButtonGroup;
            [SerializeField] public GameObject waterButtonGroup;
            [SerializeField] public GameObject feedButtonGroup;
        }
    }
}
