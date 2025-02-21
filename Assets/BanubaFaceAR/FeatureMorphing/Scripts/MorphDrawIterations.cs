using UnityEngine;

namespace BNB.Morphing
{
    public class MorphDrawIterations : MonoBehaviour
    {
        private FaceController _face;
        private IMorphDraw _morphShape;

        // Cached components
        private Transform _transform;
        private Transform _faceTransform;

        private void Awake()
        {
            _transform = transform;
        }

        private void OnEnable()
        {
            BanubaSDKManager.instance.onRecognitionResult += UpdateTransform;
        }

        private void OnDisable()
        {
            BanubaSDKManager.instance.onRecognitionResult -= UpdateTransform;
        }

        public void Initialize(FaceController face, Morph parent, IMorphDraw morphVariant)
        {
            _face = face;
            _faceTransform = _face.gameObject.transform;
            _morphShape = morphVariant;

            for (var i = 0; i < 9; ++i) {
                var morph = Instantiate(_morphShape, gameObject.transform);
                if (morph != null) {
                    morph.Create(parent, i);
                }
            }
        }

        private void UpdateTransform(FrameData data)
        {
            _transform.localScale = _faceTransform.localScale;
            _transform.position = _faceTransform.position;
            _transform.rotation = _faceTransform.rotation;
        }
    }
}
