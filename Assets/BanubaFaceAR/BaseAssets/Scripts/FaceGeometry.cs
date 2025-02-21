using System;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;


namespace BNB
{
    public class FaceGeometry
    {
        private Mesh _mesh;

        private int[] _triangles;
        private int[] _wireIndicies;
        private Vector2[] _uv;
        private float[] _verts;
        private Vector3[] _vertices;

        public int index { set; get; }

        public FaceGeometry(Mesh mesh)
        {
            _mesh = mesh;
        }


        public void Update(FrameData frameData)
        {
            var error = IntPtr.Zero;
            var res = BanubaSDKBridge.bnb_frame_data_has_frx_result(frameData, out error);
            Utils.CheckError(error);
            if (!res) {
                return;
            }

            var face = BanubaSDKBridge.bnb_frame_data_get_face(frameData, index, out error);
            Utils.CheckError(error);

            if (face.rectangle.hasFaceRectangle == 0) {
                return;
            }

            UpdateIndicies(frameData);

            if (_verts == null || _verts.Length != face.vertices_count) {
                _verts = new float[face.vertices_count];
                _vertices = new Vector3[_verts.Length / 3];
            }

            Marshal.Copy(face.vertices, _verts, 0, _verts.Length);

            for (int j = 0, i = 0; j < _vertices.Length; ++j, i += 3) {
                _vertices[j].x = _verts[i + 0];
                _vertices[j].y = _verts[i + 1];
                _vertices[j].z = _verts[i + 2];
            }

            var faceMesh = _mesh;
            faceMesh.vertices = _vertices;
            faceMesh.uv = _uv;
            faceMesh.triangles = _triangles;
        }


        private void UpdateIndicies(FrameData frameData)
        {
            var error = IntPtr.Zero;

            if (_uv == null) {
                var uv_size = BanubaSDKBridge.bnb_frame_data_get_tex_coords_size(frameData, out error);
                Utils.CheckError(error);
                var uv_ptr = BanubaSDKBridge.bnb_frame_data_get_tex_coords(frameData, out error);
                Utils.CheckError(error);
                if (uv_ptr != IntPtr.Zero) {
                    var uv_array = new float[uv_size];
                    Marshal.Copy(uv_ptr, uv_array, 0, uv_array.Length);

                    _uv = new Vector2[uv_size / 2];
                    for (int j = 0, i = 0; j < _uv.Length; ++j, i += 2) {
                        _uv[j] = new Vector2(uv_array[i], uv_array[i + 1]);
                    }
                }
            }

            if (_triangles == null) {
                var triangles_size = BanubaSDKBridge.bnb_frame_data_get_triangles_size(frameData, out error);
                Utils.CheckError(error);
                var triangles_ptr = BanubaSDKBridge.bnb_frame_data_get_triangles(frameData, out error);
                Utils.CheckError(error);
                if (triangles_ptr != IntPtr.Zero) {
                    _triangles = new int[triangles_size];
                    Marshal.Copy(triangles_ptr, _triangles, 0, _triangles.Length);
                }
            }

            if (_wireIndicies == null) {
                var indicies_size = BanubaSDKBridge.bnb_frame_data_get_wire_indicies_size(frameData, out error);
                Utils.CheckError(error);
                var indicies_ptr = BanubaSDKBridge.bnb_frame_data_get_wire_indicies(frameData, out error);
                Utils.CheckError(error);
                if (indicies_ptr != IntPtr.Zero) {
                    _wireIndicies = new int[indicies_size];
                    Marshal.Copy(indicies_ptr, _wireIndicies, 0, _wireIndicies.Length);
                }
            }
        }
    }


}
