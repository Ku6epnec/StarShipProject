using System.Linq;
using UnityEngine.Networking;
using UnityEngine;

namespace UI
{
    public class ObjectLabel : MonoBehaviour
    {
        public void DrawLabel()
        {
            var camera = FindObjectOfType<Camera>();
            if (camera == null)
                return;

            var style = new GUIStyle();
            style.normal.background = Texture2D.redTexture;
            style.normal.textColor = Color.blue;

            var position = camera.WorldToScreenPoint(transform.position);

            var collider = GetComponent<Collider>();
            if (collider != null && camera.Visible(collider))
            {
                GUI.Label(new Rect(new Vector2(position.x, Screen.height - position.y), new Vector2(10, name.Length * 10.5f)), name, style);
                Debug.Log("Увидел коллайдер: " + collider + name);
            }
        }
    }
}
