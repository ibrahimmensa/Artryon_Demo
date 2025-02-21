using System;
using UnityEngine;
using UnityEngine.Video;

namespace BNB
{
    public class FaceMeshController : MonoBehaviour
    {
        private readonly int _EnableMakeup = Shader.PropertyToID("_EnableMakeup");
        private readonly int _TextureMvp = Shader.PropertyToID("_TextureMVP");
        private readonly int _TextureRotate = Shader.PropertyToID("_TextureRotate");
        private readonly int _TextureYFlip = Shader.PropertyToID("_TextureYFlip");

        protected Material _meshMaterial;
        private CameraDevice.CameraTextureData _cameraTextureData;

        private FaceGeometry _faceMesh;
        private Renderer _rendererComponent;
        private VideoPlayer _videoPlayer;

        public int FaceIndex
        {
            get {
                return _faceMesh.index;
            }
            set {
                _faceMesh.index = value;
            }
        }

        [SerializeField]
        private FaceMeshController _face;

        private void Awake()
        {
            _faceMesh = new FaceGeometry(GetComponent<MeshFilter>().mesh);
            _rendererComponent = GetComponent<Renderer>();
            _videoPlayer = GetComponent<VideoPlayer>();

            BanubaSDKManager.instance.onRecognitionResult += OnRecognitionResult;
            CameraDevice.instance.onCameraTexture += OnCameraTexture;
            _meshMaterial = GetComponent<Renderer>().material;
            if (_videoPlayer) {
                _videoPlayer.sendFrameReadyEvents = true;
                _videoPlayer.frameReady += OnVideoTexture;
                _meshMaterial.SetInt(_EnableMakeup, 0);
            }
            OnCameraTexture(CameraDevice.instance.CameraTexture, CameraDevice.instance.cameraTextureData);
        }

        private void OnDestroy()
        {
            BanubaSDKManager.instance.onRecognitionResult -= OnRecognitionResult;
            CameraDevice.instance.onCameraTexture -= OnCameraTexture;
        }

        private void OnCameraTexture(Texture2D tex, CameraDevice.CameraTextureData cameraTextureData)
        {
            if (tex == null) {
                return;
            }
            _cameraTextureData = cameraTextureData;
            _rendererComponent.material.mainTexture = tex;
        }

        private void OnVideoTexture(VideoPlayer source, long frameIdx)
        {
            _meshMaterial.SetInt(_EnableMakeup, 1);
            _videoPlayer.sendFrameReadyEvents = false;
            _videoPlayer.frameReady -= OnVideoTexture;
        }

        private void OnRecognitionResult(FrameData frameData)
        {
            var error = IntPtr.Zero;

            var res = BanubaSDKBridge.bnb_frame_data_has_frx_result(frameData, out error);
            Utils.CheckError(error);
            if (!res) {
                return;
            }

            _faceMesh.Update(frameData);

            var width = _meshMaterial.mainTexture.width;
            var height = _meshMaterial.mainTexture.height;
            if (_cameraTextureData.isRotated90) {
                var tmp = width;
                width = height;
                height = tmp;
            }

            // get MVP transform for camera texture oriented in GL basis
            var mvp = BanubaSDKBridge.bnb_frame_data_get_face_transform(
                frameData, FaceIndex, width, height, BanubaSDKManager.FitMode, out error
            );
            Utils.CheckError(error);
            var mv = Utils.ArrayToMatrix4x4(mvp.mv);
            var p = Utils.ArrayToMatrix4x4(mvp.p);

            _meshMaterial.SetMatrix(_TextureMvp, p * mv);
            _meshMaterial.SetInt(_TextureRotate, _cameraTextureData.isRotated90 ? 1 : 0);
            _meshMaterial.SetInt(_TextureYFlip, _cameraTextureData.isVerticallyFlipped ? 1 : 0);
        }
    }
}
