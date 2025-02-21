using System;
using UnityEditor;
using UnityEngine;

namespace BNB
{
    public class FacesController : MonoBehaviour
    {
        // Drop FacesController prefab to EffectBase hierarchy to enable face tracking and face mesh.

        public event Action<int> onInstantiateFace;

        [SerializeField]
        bool _enableUVDraw = false;

        [SerializeField]
        bool _enableStaticPos = false;

        [SerializeField]
        UVDraw _uvDraw;

        [SerializeField]
        StaticPos _staticPos;
        private FaceController _faceTemplate;

#if UNITY_EDITOR
        private UnityEngine.Object AddPrefab<T>(string path, bool val)
        {
            if (val) {
                var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(T));
                return prefab;
            }
            return null;
        }

        private void OnValidate()
        {
            _uvDraw = AddPrefab<UVDraw>("Assets/BanubaFaceAR/BaseAssets/Prefabs/UVDraw.prefab", _enableUVDraw) as UVDraw;

            _staticPos = AddPrefab<StaticPos>("Assets/BanubaFaceAR/BaseAssets/Prefabs/StaticPos.prefab", _enableStaticPos) as StaticPos;
        }
#endif

        private void Awake()
        {
            _faceTemplate = GetComponentInChildren<FaceController>();
            BanubaSDKManager.instance.onRecognitionResult += OnRecognitionResult;

            if (_uvDraw != null) {
                var firstFace = GetFace(0);
                if (firstFace == null) {
                    return;
                }

                CreateUVDraw(firstFace);
            }
            if (_staticPos != null) {
                Instantiate(_staticPos, transform.parent);
            }
        }

        private void OnDestroy()
        {
            BanubaSDKManager.instance.onRecognitionResult -= OnRecognitionResult;
        }

        private void OnRecognitionResult(FrameData frameData)
        {
            var error = IntPtr.Zero;
            var res = BanubaSDKBridge.bnb_frame_data_has_frx_result(frameData, out error);
            Utils.CheckError(error);

            if (!res) {
                return;
            }

            var face_count = BanubaSDKBridge.bnb_frame_data_get_face_count(frameData, out error);
            Utils.CheckError(error);
            OnFaceInstantiatedHandler(face_count);

            if (onInstantiateFace != null) {
                onInstantiateFace(face_count);
            }
        }

        private void OnFaceInstantiatedHandler(int faceCount)
        {
            int morphsCount = transform.childCount;
            if (morphsCount == faceCount) {
                return;
            }
            if (morphsCount < faceCount) {
                for (int i = morphsCount; i < faceCount; i++) {
                    CreateFace(i);
                }
            } else if (morphsCount > faceCount) {
                for (int i = morphsCount; i > faceCount; i--) {
                    Destroy(transform.GetChild(morphsCount).gameObject);
                }
            }
        }

        void CreateUVDraw(FaceController faceController)
        {
            if (_uvDraw != null) {
                var uv = Instantiate(_uvDraw, faceController.gameObject.transform);
                uv.GetComponent<UVDraw>().Initialize(faceController);
            }
        }

        private void CreateFace(int faceIndex)
        {
            FaceController face = Instantiate(_faceTemplate, transform);
            face.Initialize(faceIndex);
            face.name = "Face" + faceIndex;
            face.gameObject.SetActive(true);
            CreateUVDraw(face);
        }

        public FaceController GetFace(int index)
        {
            if (index >= transform.childCount) {
                return null;
            }
            return transform.GetChild(index).GetComponent<FaceController>();
        }
    }

}