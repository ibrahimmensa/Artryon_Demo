using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace BNB.Morphing
{
    public class CustomMorphUI : MonoBehaviour
    {
        [SerializeField]
        MorphingController controller;

        [SerializeField]
        GameObject weightsSetter;

        [SerializeField]
        private Transform _effectsList;

        [SerializeField]
        RectTransform _buttonRect;

        private RectTransform _rt;
        private List<WeightsUpdater> morphs = new List<WeightsUpdater>();
        private List<SliderWeight> sliders = new List<SliderWeight>();

        ScreenOrientation _currentOrientation = ScreenOrientation.LandscapeRight;
        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
        }
        void Start()
        {
            var weights = controller.transform.GetComponentsInChildren<WeightsUpdater>();
            foreach (var morph in weights) {
                morphs.Add(morph);
            }
            for (int i = 0; i < (int) WeightsUpdater.ID.SIZE; ++i) {
                var slider = GameObject.Instantiate(weightsSetter, _effectsList).GetComponent<SliderWeight>();
                var pos = slider.transform.position;

                var id = (WeightsUpdater.ID) i;
                slider.text.text = id.ToString();
                slider.slider.onValueChanged.AddListener((value) =>
                                                         {
                                                             foreach (var morph in morphs) {
                                                                 morph.setWeight(id, value);
                                                             }
                                                         });
                sliders.Add(slider);
            }
        }

        private void Update()
        {
            if (_currentOrientation != Screen.orientation) {
                var isPortrait = Application.isMobilePlatform && (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown);
                _rt.anchorMin = isPortrait ? new Vector2(0.6f, 0) : new Vector2(0.8f, 0);
                _currentOrientation = Screen.orientation;
            }
        }

        public void SetEnabled()
        {
            gameObject.SetActive(!gameObject.activeInHierarchy);
        }

        void OnRectTransformDimensionsChange()
        {
            foreach (var slider in sliders) {
                slider.UpdateWidth();
            }
            if (_rt) {
                _buttonRect.anchoredPosition = new Vector2(-(_rt.rect.size.x) * 0.5f, -50.0f);
            }
        }
    }

}
