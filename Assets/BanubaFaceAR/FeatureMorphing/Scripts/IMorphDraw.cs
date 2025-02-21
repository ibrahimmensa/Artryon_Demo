using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNB.Morphing
{
    public abstract class IMorphDraw : MonoBehaviour
    {
        public enum MorphType : int
        {
            UV_MORPH = 0,
            CUSTOM_MORPH,
            SIZE
        }
        // Cached shader properties
        protected readonly int _DrawID = Shader.PropertyToID("_DrawID");

        protected int _iteration;
        protected Material _meshMaterial;

        private void OnEnable()
        {
            BanubaSDKManager.instance.onRecognitionResult += UpdateMorph;
        }

        private void OnDisable()
        {
            BanubaSDKManager.instance.onRecognitionResult -= UpdateMorph;
        }

        public virtual void Create(Morph morph, int iteration)
        {
            _iteration = iteration;
            var rendererComponent = GetComponent<Renderer>();
            rendererComponent.sortingOrder = 9 - _iteration;
            _meshMaterial = rendererComponent.material;

            _meshMaterial.SetInt(_DrawID, _iteration);
        }

        public abstract MorphType Type();
        protected abstract void UpdateMorph(FrameData data);
    }
}
