using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BNB
{
    public class StaticFaceMesh : MonoBehaviour
    {
        private Vector3[] _staticPosVert;
        private Vector2[] _staticUvVert;
        private int[] _triangles;

        private Mesh _meshComponent;

        public Texture2D BakedMesh { get; private set; } = null;

        void Awake()
        {
            _meshComponent = GetComponent<MeshFilter>().mesh;

            var staticVertArray = new float[BanubaSDKBridge.get_extended_static_pos_vertices_size()];
            _triangles = new int[BanubaSDKBridge.get_extended_static_pos_indices_size()];

            {
                IntPtr address_v = new Pinner<float[]>(staticVertArray);
                IntPtr address_i = new Pinner<int[]>(_triangles);
                BanubaSDKBridge.get_extended_static_pos_data(address_v, address_i);
            }

            _staticPosVert = new Vector3[staticVertArray.Length / 5];
            _staticUvVert = new Vector2[staticVertArray.Length / 5];


            for (int j = 0, i = 0; i < staticVertArray.Length; ++j, i += 5) {
                _staticPosVert[j] = new Vector3(staticVertArray[i], staticVertArray[i + 1], staticVertArray[i + 2]);
                _staticUvVert[j] = new Vector2(staticVertArray[i + 3], staticVertArray[i + 4]);
            }

            _meshComponent.vertices = _staticPosVert;
            _meshComponent.uv = _staticUvVert;
            _meshComponent.triangles = _triangles;
        }
    }
}
