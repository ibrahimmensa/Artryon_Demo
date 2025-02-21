using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;

namespace BNB
{
    public class HandSkeleton : MonoBehaviour
    {
        [SerializeField]
        GameObject lms;
        private Mesh _mesh;
        private Mesh _lmsMesh;
        private Material _mat;
        private Material _lmsMat;

        private readonly int[] _additional_points = { 2, 5, 9, 13, 17 };
        private readonly int[] _not_connectable = { 4, 8, 12, 16, 20 };

        private List<int> _indices = new List<int>();
        private List<int> _lmsIndices = new List<int>();


        public BanubaSDKBridge.bnb_hand_gesture_t Gesture { private set; get; }


        // Start is called before the first frame update
        void Start()
        {
            var mesh = new Mesh();
            var lmsMesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            lms.GetComponent<MeshFilter>().mesh = lmsMesh;
            _mesh = mesh;
            _lmsMesh = lmsMesh;


            _mat = GetComponent<MeshRenderer>().material;
            _lmsMat = lms.GetComponent<MeshRenderer>().material;

            BanubaSDKManager.instance.onRecognitionResult += OnRecognitionResult;

            var featuresId = BanubaSDKBridge.bnb_recognizer_get_features_id();

            IntPtr error = IntPtr.Zero;
            BanubaSDKBridge.bnb_recognizer_insert_feature(BanubaSDKManager.instance.Recognizer, featuresId.hand_gestures, out error);
            Utils.CheckError(error);
        }

        private void OnDestroy()
        {
            BanubaSDKManager.instance.onRecognitionResult -= OnRecognitionResult;
            var featuresId = BanubaSDKBridge.bnb_recognizer_get_features_id();
            var error = IntPtr.Zero;
            BanubaSDKBridge.bnb_recognizer_remove_feature(BanubaSDKManager.instance.Recognizer, featuresId.hand_gestures, out error);
            Utils.CheckError(error);
        }

        private void OnRecognitionResult(FrameData frameData)
        {
            var error = IntPtr.Zero;
            var hand = BanubaSDKBridge.bnb_frame_data_get_hand(frameData, Screen.width, Screen.height, BanubaSDKManager.FitMode, Application.isMobilePlatform, out error);
            Utils.CheckError(error);

            gameObject.SetActive(hand.vertices_count > 0);
            if (!gameObject.activeInHierarchy) {
                return;
            }

            Gesture = hand.gesture;

            var vert_array = new float[hand.vertices_count];
            Marshal.Copy(hand.vertices, vert_array, 0, vert_array.Length);

            var vertices = new Vector3[vert_array.Length / 2];
            for (int j = 0, i = 0; j < vertices.Length; ++j, i += 2) {
                vertices[j] = new Vector3(
                    vert_array[i + 0],
                    vert_array[i + 1],
                    -1000f
                );
            }

            UpdateIndices(vertices.Length);

            _mesh.vertices = vertices;
            _mesh.SetIndices(_indices, MeshTopology.Lines, 0);

            _lmsMesh.vertices = vertices;
            _lmsMesh.SetIndices(_lmsIndices, MeshTopology.Points, 0);

            var mv = Utils.ArrayToMatrix4x4(hand.transform);
            _mat.SetMatrix("_MV", mv);
            _lmsMat.SetMatrix("_MV", mv);


            _mat.SetColor("_Color", Color.green);
            _lmsMat.SetColor("_Color", Color.red);
        }

        private void UpdateIndices(int count)
        {
            if (_indices.Count > 0) {
                return;
            }
            for (var i = 0; i < count; ++i) {
                if (_not_connectable.Contains(i)) {
                    continue;
                }
                _indices.Add(i);
                _indices.Add(i + 1);
            }
            for (var i = 0; i < _additional_points.Length - 1; ++i) {
                _indices.Add(_additional_points[i]);
                _indices.Add(_additional_points[i + 1]);
            }
            _indices.Add(_additional_points[_additional_points.Length - 1]);
            _indices.Add(_indices[0]);


            for (int i = 0; i < count; ++i) {
                _lmsIndices.Add(i);
            }
        }
    }
}
