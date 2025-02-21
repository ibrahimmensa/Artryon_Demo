using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.Collections;

namespace BNB.Morphing
{
    public class CustomMorphDraw : IMorphDraw
    {
        // Cached shader properties
        private readonly int _tex = Shader.PropertyToID("_MorphTex");
        private readonly int _morphsWights = Shader.PropertyToID("_morphsWights");

        private FaceGeometry _faceMesh;

        private void Awake()
        {
            _faceMesh = new FaceGeometry(GetComponent<MeshFilter>().mesh);
        }

        public override void Create(Morph morph, int iteration)
        {
            base.Create(morph, iteration);

            var updater = morph.GetComponent<WeightsUpdater>();
            if (updater == null) {
                Debug.LogError("CustomMorphDraw require WeightsUpdater on Morph Entity");
                return;
            }
            updater.onWeightsUpdate += UpdateWeights;
            _meshMaterial.SetTexture(_tex, MorphsTexture.texture);
        }

        private void UpdateWeights(float[] weights)
        {
            _meshMaterial.SetFloatArray(_morphsWights, weights);
        }

        protected override void UpdateMorph(FrameData data)
        {
            _faceMesh.Update(data);
        }

        public override MorphType Type()
        {
            return MorphType.CUSTOM_MORPH;
        }
    }
}
