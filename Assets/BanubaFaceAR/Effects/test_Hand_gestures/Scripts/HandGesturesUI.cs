using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace BNB
{
    [RequireComponent(typeof(Text))]
    public class HandGesturesUI : MonoBehaviour
    {
        [SerializeField]
        HandSkeleton _handSkeleton;

        BanubaSDKBridge.bnb_hand_gesture_t _lastGeture = BanubaSDKBridge.bnb_hand_gesture_t.BNB_NONE;
        Text _text;

        void Start()
        {
            _text = GetComponent<Text>();
        }

        private void Update()
        {
            if (_lastGeture != _handSkeleton.Gesture) {
                _lastGeture = _handSkeleton.Gesture;
                _text.text = _lastGeture.ToString().Substring(4);
            }
        }
    }
}
