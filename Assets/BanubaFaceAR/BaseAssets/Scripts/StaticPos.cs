using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BNB
{
    public class StaticPos : MonoBehaviour
    {
        public static StaticPos instance = null;
        public RenderToTexture Texture { get; private set; }

        private void Awake()
        {
            instance = this;
            Texture = GetComponentInChildren<RenderToTexture>();
        }
    }
}
