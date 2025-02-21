using UnityEngine;

namespace BNB.Morphing
{
    public class MorphDraw : IMorphDraw
    {
        private readonly int _UVMorphTex = Shader.PropertyToID("_UVMorphTex");
        private readonly int _StaticPosTex = Shader.PropertyToID("_StaticPosTex");
        private readonly int _MorphWeight = Shader.PropertyToID("_MorphWeight");

        private RenderToTexture _uvMorph;
        private Texture _staticPos;

        [SerializeField]
        float _weight = 1f;

        public override void Create(Morph morph, int iteration)
        {
            base.Create(morph, iteration);

            _uvMorph = morph.uvMorph;
            _staticPos = StaticPos.instance.Texture.texture;

            _meshMaterial.SetFloat(_MorphWeight, _weight);
            _meshMaterial.SetInt(_DrawID, _iteration);
            _meshMaterial.SetTexture(_StaticPosTex, _staticPos);
        }

        public override MorphType Type()
        {
            return MorphType.UV_MORPH;
        }

        protected override void UpdateMorph(FrameData data)
        {
            _meshMaterial.SetTexture(_UVMorphTex, _uvMorph.texture);
        }
    }
}
