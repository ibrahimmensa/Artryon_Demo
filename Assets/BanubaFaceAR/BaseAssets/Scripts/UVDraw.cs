using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNB
{
    public class UVDraw : MonoBehaviour
    {
        [SerializeField]
        UVDrawFace _uvDrawFace;
        [SerializeField]
        RenderToTexture _uvCamera;

        public RenderToTexture UVTexture
        {
            get {
                return _uvCamera;
            }
        }

        public void Initialize(FaceController faceController)
        {
            faceController.onFaceEnabled += (bool enabled) =>
            {
                gameObject.SetActive(enabled);
            };
            _uvDrawFace.Initialize(faceController.GetComponentInChildren<MeshFilter>());
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
