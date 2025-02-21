using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BNB.Morphing
{
    public class SliderWeight : MonoBehaviour
    {
        private readonly float padding = 20.0f;
        public Slider slider;
        public Text text;


        private RectTransform _rectTransform;
        private RectTransform _sliderRectTransform;
        private RectTransform _scrollRectTransform;

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _sliderRectTransform = slider.GetComponent<RectTransform>();
            ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
            if (scrollRect != null)
                _scrollRectTransform = scrollRect.GetComponent<RectTransform>();
        }

        void OnEnable()
        {
            UpdateWidth();
        }

        public void UpdateWidth()
        {
            var size = new Vector2(_scrollRectTransform.rect.size.x - padding, _rectTransform.sizeDelta.y);
            _rectTransform.sizeDelta = size;
            _sliderRectTransform.sizeDelta = size;
        }
    }
}
