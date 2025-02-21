using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNB.Morphing
{
    public class WeightsUpdaterAttribute : PropertyAttribute
    {
    }
    public class WeightsUpdater : MonoBehaviour
    {
        public event Action<float[]> onWeightsUpdate;

        public enum ID : int
        {
            EYEBROWS_SPACING = 0,     //- Adjusting the space between the eyebrows [-1;1]
            EYEBROWS_HEIGHT,          //- Raising/lowering the eyebrows [-1;1]
            EYEBROWS_BEND,            //- Adjusting the bend of the eyebrows [-1;1]
            EYES_ENLARGEMENT,         //- Enlarging the eyes [0;1]
            EYES_ROUNDING,            //- Adjusting the roundness of the eyes [0;1]
            EYES_HEIGHT,              //- Raising/lowering the eyes [-1;1]
            EYES_SPACING,             //- Adjusting the space between the eyes [-1;1]
            EYES_SQUINT,              //- Making the person squint by adjusting the eyelids [-1;1]
            LOWER_EYELID_POS,         //- Raising/lowering the lower eyelid [-1;1]
            LOWER_EYELID_SIZE,        //- Enlarging/shrinking the lower eyelid [-1;1]
            NOSE_LENGHT,              //- Adjusting the nose length [-1;1]
            NOSE_WIDTH,               //- Adjusting the nose width [-1;1]
            NOSE_TIP_WIDTH,           //- Adjusting the nose tip width [0;1]
            LIPS_HEIGHT,              //- Raising/lowering the lips [-1;1]
            LIPS_SIZE,                //- Adjusting the width and vertical size of the lips [-1;1]
            LIPS_THICKNESS,           //- Adjusting the thickness of the lips [-1;1]
            MOUTH_SIZE,               //- Adjusting the size of the mouth [-1;1]
            SMILE,                    //- Making a person smile [0;1]
            LIPS_SHAPE,               //- Adjusting the shape of the lips [-1;1]
            FACE_NARROWING,           //- Narrowing the face [0;1]
            FACE_V_SHAPE,             //- Shrinking the chin and narrowing the cheeks [0;1]
            CHEEKBONES_NARROWING,     //- Narrowing the cheekbones [-1;1]
            CHEEKS_NARROWING,         //- Narrowing the cheeks [0;1]
            JAW_NARROWING,            //- Narrowing the jaw [0;1]
            CHIN_SHORTENING,          //- Decreasing the length of the chin [0;1]
            CHIN_NARROWING,           //- Narrowing the chin [0;1]
            SUNKEN_CHEEKS,            //- Sinking the cheeks and emphasizing the cheekbones [0;1]
            CHEEKS_AND_JAW_NARROWING, //- Narrowing the cheeks and the jaw [0;1]
            JAW_WIDE_THIN,            //- Jaw - wide / thin [0;1]
            NOSE_DOWN_UP,             //- Nose - down / up + Expression SmileClosed [0;1]
            EYES_DOWN,                //- Eyes Down [0;1]
            EYELID_UPPER,             //- Eyelid upper [0;1]
            EYELID_LOWER,             //- Eyelid lower [0;1]
            FACE_CHIN,                //- Face Chin [0;1]
            FOREHEAD,                 //- Forehead [0;1]
            NOSE_SELLION,             //- Nose sellion [0;1]
            LIPS_SHARP,               //- Lips Sharp [0;1]
            SIZE
        }


        [WeightsUpdaterAttribute()]
        [SerializeField]
        float[] weights = new float[(int) ID.SIZE];

        void OnValidate()
        {
            if (weights.Length != (int) ID.SIZE) {
                Debug.LogWarning("weights has constant size");
                Array.Resize(ref weights, (int) ID.SIZE);
            }

            onWeightsUpdate?.Invoke(weights);
        }

        public void setWeight(ID id, float val)
        {
            if (id == ID.SIZE) {
                return;
            }
            weights[(int) id] = val;
            onWeightsUpdate?.Invoke(weights);
        }
    }
}
