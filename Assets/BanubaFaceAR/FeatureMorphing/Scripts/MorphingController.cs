using System;
using UnityEngine;

namespace BNB.Morphing
{

    public class MorphsArrayAttribute : PropertyAttribute
    {
    }
    public class MorphingController : MonoBehaviour
    {
        [Header("References")]
        [MorphsArrayAttribute()]
        [SerializeField]
        private Morph[] _morphPrefabs;

        private FacesController _facesController;
        private IMorphDraw _morphShape;

        void OnValidate()
        {
            if (_morphPrefabs.Length != (int) IMorphDraw.MorphType.SIZE) {
                Debug.LogWarning("_morphPrefabs has constant size");
                Array.Resize(ref _morphPrefabs, (int) IMorphDraw.MorphType.SIZE);
            }
        }

        public void Initialize(FacesController facesController, IMorphDraw morphShape)
        {
            _morphShape = morphShape;
            _facesController = facesController;
            _facesController.onInstantiateFace += OnFaceInstantiatedHandler;

            CreateMorph(0);
        }

        private void CreateMorph(int faceIndex)
        {
            Morph newMorph = Instantiate(_morphPrefabs[(int) _morphShape.Type()], transform);
            newMorph.name = "Morph" + faceIndex;
            FaceController face = _facesController.GetFace(faceIndex);
            if (face != null) {
                newMorph.Initialize(face, _morphShape);
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
                    CreateMorph(i);
                }
            } else if (morphsCount > faceCount) {
                for (int i = morphsCount; i > faceCount; i--) {
                    Destroy(transform.GetChild(morphsCount).gameObject);
                }
            }
        }
    }
}
