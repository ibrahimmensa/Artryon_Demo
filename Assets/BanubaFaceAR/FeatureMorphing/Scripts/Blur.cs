using UnityEngine;

namespace BNB.Morphing
{
    public class Blur : MonoBehaviour
    {
        RenderToTexture _rrtComponent;
        BNB.Blur _blur;

        private void Start()
        {
            _blur = gameObject.AddComponent<BNB.Blur>();
            _rrtComponent = GetComponent<RenderToTexture>();
        }


        private void OnPostRender()
        {
            if (_rrtComponent.texture == null) {
                return;
            }

            _blur.BlurTexture(_rrtComponent.texture);
        }
    }
}
