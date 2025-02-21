using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNB
{

    public class CameraTextureSmoothing : MonoBehaviour
    {
        private readonly int _camTex = Shader.PropertyToID("_CameraTex");
        private readonly int _blurTex = Shader.PropertyToID("_BluredTex");

        [SerializeField]
        Texture2D _cameraTex;
        [SerializeField]
        Shader _cameraShader;
        Material _camMaterial;

        [SerializeField]
        Shader _smoothingShader;
        Material _smoothingMaterial;

        Blur _blur;
        RenderToTexture _rrtComponent;

        public RenderTexture texture
        {
            get {
                return _rrtComponent.texture;
            }
        }


        // Start is called before the first frame update
        void Start()
        {
            _blur = gameObject.AddComponent<Blur>();
            _rrtComponent = GetComponent<RenderToTexture>();
            _camMaterial = new Material(_cameraShader) {
                hideFlags = HideFlags.DontSave
            };

            _smoothingMaterial = new Material(_smoothingShader) {
                hideFlags = HideFlags.DontSave
            };

            CameraDevice.instance.onCameraTexture += OnCameraTexture;
            OnCameraTexture(CameraDevice.instance.CameraTexture, CameraDevice.instance.cameraTextureData);
        }

        private void OnCameraTexture(Texture2D tex, CameraDevice.CameraTextureData cameraTextureData)
        {
            if (tex == null) {
                return;
            }
            _cameraTex = tex;
            var w = _cameraTex.width;
            var h = _cameraTex.height;

            _rrtComponent.Resize(w, h);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_rrtComponent.texture == null) {
                return;
            }

            _camMaterial.SetTexture(_camTex, _cameraTex);
            RenderTexture cameraBuffer = RenderTexture.GetTemporary(_rrtComponent.texture.width, _rrtComponent.texture.height, 0, _rrtComponent.texture.format);
            Graphics.Blit(source, cameraBuffer, _camMaterial);
            RenderTexture smoothBuffer = RenderTexture.GetTemporary(_rrtComponent.texture.width, _rrtComponent.texture.height, 0, _rrtComponent.texture.format);

            _blur.BlurTexture(cameraBuffer, smoothBuffer);

            _smoothingMaterial.SetTexture(_camTex, _cameraTex);
            //_smoothingMaterial.SetTexture(_blurTex, smoothBuffer);
            Graphics.Blit(smoothBuffer, destination, _smoothingMaterial);

            RenderTexture.ReleaseTemporary(smoothBuffer);
            RenderTexture.ReleaseTemporary(cameraBuffer);
        }
    }
}
