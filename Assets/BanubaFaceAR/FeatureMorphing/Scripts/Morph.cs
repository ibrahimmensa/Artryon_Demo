using UnityEditor;
using UnityEngine;

namespace BNB.Morphing
{
    public class Morph : MonoBehaviour
    {
        [Header("Children references")]
        [SerializeField]
        private MorphDrawIterations _MDI;

        [Header("UV Morph References")]
        [SerializeField]
        private RenderToTexture _UVMorph;

        public RenderToTexture uvMorph
        { private
            set {
                _UVMorph = value;
            }
            get {
                return _UVMorph;
            }
        }

        private FaceController _face;

        public void Initialize(FaceController face, IMorphDraw morphShape)
        {
            _face = face;
            _face.onFaceEnabled += OnFaceEnable;
            if (morphShape.Type() == IMorphDraw.MorphType.UV_MORPH) {
                var uvComponent = face.GetComponentInChildren<UVDraw>();
                if (uvComponent == null) {
                    throw new System.Exception("This Morphing require UVDraw Feature in FacesController Component");
                }
                _UVMorph = uvComponent.UVTexture;
            }
            _MDI.Initialize(_face, this, morphShape);
        }

        private void OnFaceEnable(bool enabled)
        {
            gameObject.SetActive(enabled);
        }
    }
}
