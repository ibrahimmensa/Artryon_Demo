using System;
using System.IO;
using UnityEngine;

namespace BNB.Morphing
{
    // TODO: remove
    public class MorphsTexture
    {
        public static Texture2D texture
        {
            get {
                return Instance()._tex;
            }
        }

        private static MorphsTexture _inst = null;
        private Texture2D _tex;
        private MorphsTexture()
        {
            _tex = new Texture2D(3308, 37, TextureFormat.RGFloat, false);
#if UNITY_ANDROID && !UNITY_EDITOR
            var path = Path.Combine(Application.persistentDataPath, "BanubaFaceAR/unity/morphs_f32x2x3308x37.bin");
#else
            var path = Path.Combine(Application.streamingAssetsPath, "BanubaFaceAR/unity/morphs_f32x2x3308x37.bin");
#endif
            byte[] bytes = File.ReadAllBytes(path);

            _tex.LoadRawTextureData(bytes);
            _tex.Apply();
        }

        private static MorphsTexture Instance()
        {
            if (_inst == null) {
                _inst = new MorphsTexture();
            }
            return _inst;
        }
    }


}
