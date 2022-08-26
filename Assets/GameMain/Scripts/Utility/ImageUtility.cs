using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    class ImageUtility
    {
        static private Material _uiGreyMat = null;
        static private Material uiGreyMat
        {
            get
            {
                if (!_uiGreyMat)
                {
                    var shader = Shader.Find("UI/UIGrey");
                    if (shader)
                    {
                        _uiGreyMat = new Material(shader);
                        _uiGreyMat.hideFlags = HideFlags.HideAndDontSave;
                        //_uiGreyMat.setpo
                    }
                }
                return _uiGreyMat;
            }
        }
        public static void SetChildUIDrey(GameObject uiObj, bool isGrey = true)
        {
            var graphics = uiObj.GetComponentsInChildren<Graphic>();
            int length = graphics.Length;
            for (int i = 0; i < length; i++)
            {
                graphics[i].material = isGrey ? uiGreyMat : null;
            }
        }

        public static void SetUIDrey(GameObject uiObj, bool isGrey = true)
        {
            var graphics = uiObj.GetComponent<Graphic>();
            graphics.material = isGrey ? uiGreyMat : null;

        }
    }
}
