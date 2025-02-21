using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BNB
{
    public class Blur : MonoBehaviour
    {
        public float _blurSpread = 1f;
        public int _iterations = 4;

        private Shader _blurShader;
        private Material _blurMaterial;


        private void Awake()
        {
            _blurShader = Shader.Find("Hidden/BlurEffectConeTap");
            _blurMaterial = new Material(_blurShader) {
                hideFlags = HideFlags.DontSave
            };
            // Disable if the shader can't run on the users graphics card
            if (!_blurShader || !_blurMaterial.shader.isSupported) {
                Debug.LogWarning("Blur can't run on the users graphics card");
                // enabled = false;
            }
        }

        private void OnDisable()
        {
            if (_blurMaterial) {
                DestroyImmediate(_blurMaterial);
            }
        }

        // Performs one blur iteration.
        protected void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
        {
            float off = 0.5f + iteration * _blurSpread;
            Graphics.BlitMultiTap(source, dest, _blurMaterial, new Vector2(-off, -off), new Vector2(-off, off), new Vector2(off, off), new Vector2(off, -off));
        }

        // Downsamples the texture to a quarter resolution.
        protected void DownSample4x(RenderTexture source, RenderTexture dest)
        {
            float off = 1.0f;
            Graphics.BlitMultiTap(source, dest, _blurMaterial, new Vector2(-off, -off), new Vector2(-off, off), new Vector2(off, off), new Vector2(off, -off));
        }

        // Must call only in render events like (OnRenderImage, OnPostRender and etc.)
        public void BlurTexture(RenderTexture texture, RenderTexture destination = null)
        {
            if (texture == null) {
                return;
            }
            var source = texture;
            int rtW = Math.Max(source.width / 4, 1);
            int rtH = Math.Max(source.height / 4, 1);
            var d = source.depth;
            var format = source.format;
            RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, d, format);

            // Copy source to the 4x4 smaller texture.
            DownSample4x(source, buffer);

            // Blur the small texture
            for (int i = 0; i < _iterations; i++) {
                RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, d, format);
                FourTapCone(buffer, buffer2, i);
                RenderTexture.ReleaseTemporary(buffer);
                buffer = buffer2;
            }

            Graphics.Blit(buffer, destination != null ? destination : source);

            RenderTexture.ReleaseTemporary(buffer);
        }
    }
}
