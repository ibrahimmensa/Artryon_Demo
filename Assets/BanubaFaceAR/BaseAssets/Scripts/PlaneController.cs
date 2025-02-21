using System;
using UnityEngine;
using UnityEngine.UI;

namespace BNB
{
    public class PlaneController : MonoBehaviour
    {
        private readonly int _BNBCameraScale = Shader.PropertyToID("_BNBCameraScale");
        private readonly int _BNBCameraStencil = Shader.PropertyToID("_BNBCameraStencil");


        private bool _getRotated;
        private Resolution _resolution;
        private RawImage _image;
        private RectTransform _rectTransform;

        public int cameraAngle { get; private set; }
        private bool _camVerticalFlip { get; set; }

        private RawImage _stencilMask;

        private void Awake()
        {
            var shader = Shader.Find("BNB/CameraStencilMask");
            var mat = new Material(shader) {
                hideFlags = HideFlags.DontSave
            };

            var stencilObj = new GameObject("CameraStencil");
            var image = stencilObj.AddComponent<RawImage>();
            image.material = mat;
            image.material.SetInt(_BNBCameraStencil, 10);

            var obj = Instantiate(stencilObj, transform);
            _stencilMask = obj.GetComponent<RawImage>();
            Destroy(stencilObj);
        }

        private void Start()
        {
            if (BanubaSDKManager.instance == null) {
                return;
            }
            _image = GetComponent<RawImage>();
            _rectTransform = GetComponent<RectTransform>();

            _resolution.width = Screen.width;
            _resolution.height = Screen.height;

            CameraDevice.instance.onCameraTexture += OnCameraTexture;
            OnCameraTexture(CameraDevice.instance.CameraTexture, CameraDevice.instance.cameraTextureData);
        }

        private void Update()
        {
            if (_resolution.width != Screen.width || _resolution.height != Screen.height) {
                Debug.Log("Resolution changed: " + Screen.width + "x" + Screen.height);
                _resolution.width = Screen.width;
                _resolution.height = Screen.height;
                OnCameraTexture(CameraDevice.instance.CameraTexture, CameraDevice.instance.cameraTextureData);
            }
        }

        private void OnDestroy()
        {
            CameraDevice.instance.onCameraTexture -= OnCameraTexture;
        }

        private void OnCameraTexture(Texture2D tex, CameraDevice.CameraTextureData cameraTextureData)
        {
            if (tex == null) {
                return;
            }
            _image.texture = tex;
            cameraAngle = cameraTextureData.angle;
            _camVerticalFlip = false;
            transform.rotation = Quaternion.AngleAxis(cameraAngle, Vector3.back);

            UpdatePlaneRect();
        }

        private void UpdatePlaneRect()
        {
            var w = _image.texture.width;
            var h = _image.texture.height;
            _getRotated = cameraAngle == 90 || cameraAngle == 270;

            if (_getRotated) {
                _image.uvRect = _camVerticalFlip
                                    ? new Rect(0, 1, 1, -1)
                                    : new Rect(0, 0, 1, 1);

                // rotate rect to correct fit on screen
                w = _image.texture.height;
                h = _image.texture.width;
            } else {
                _image.uvRect = new Rect(0, 0, 1, 1);
            }

            var src = new BanubaSDKBridge.bnb_pixel_rect_t {
                x = 0,
                y = 0,
                w = w,
                h = h
            };
            var dst = new BanubaSDKBridge.bnb_pixel_rect_t {
                x = 0,
                y = 0,
                w = Screen.width,
                h = Screen.height
            };

            var error = IntPtr.Zero;
            BanubaSDKBridge.bnb_fit_rects_aspect_ratio(ref src, ref dst, BanubaSDKManager.FitMode, out error);
            Utils.CheckError(error);

            // compensate height, due to all fitting modes. For full screen appearance
            var scaley = (float) h / src.h;
            var scalex = (float) w / src.w;

            var width = dst.w * scalex;
            var height = dst.h * scaley;

            var camScaleX = Screen.width / width;
            var camScaleY = Screen.height / height;

            Shader.SetGlobalMatrix(_BNBCameraScale, Matrix4x4.Scale(new Vector3(camScaleX, camScaleY, 1f)));

            // unrotate rect
            if (_getRotated) {
                (width, height) = (height, width);
            }

            _rectTransform.sizeDelta = new Vector2(width, height);

            _stencilMask.rectTransform.localPosition = Vector3.zero;
            _stencilMask.rectTransform.sizeDelta = new Vector2(width, height);
        }
    }
}
