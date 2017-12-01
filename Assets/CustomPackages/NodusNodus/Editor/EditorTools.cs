using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace CW
{
    public class EditorTools
    {
        static Material _CapMaterial2D = null;
        static Material CapMaterial2D
        {
            get
            {
                if (_CapMaterial2D == null) { _CapMaterial2D = new Material(Shader.Find("Sprites/Default")); }
                return _CapMaterial2D;
            }
        }
        #region Cap methods
        public static void ImageCapBase(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, Texture2D aTex)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            aPosition = Handles.matrix.MultiplyPoint(aPosition);
            Vector3 right = Camera.current.transform.right * aSize;
            Vector3 top = Camera.current.transform.up * aSize;
            CapMaterial2D.mainTexture = aTex;
            CapMaterial2D.SetPass(0);

            GL.Begin(GL.QUADS);
            GL.Color(Handles.color);
            GL.TexCoord2(1, 1);
            GL.Vertex(aPosition + right + top);

            GL.TexCoord2(1, 0);
            GL.Vertex(aPosition + right - top);

            GL.TexCoord2(0, 0);
            GL.Vertex(aPosition - right - top);

            GL.TexCoord2(0, 1);
            GL.Vertex(aPosition - right + top);

            GL.End();
        }
        #endregion

        public static float HandleScale(Vector3 aPos)
        {
            float dist = SceneView.lastActiveSceneView.camera.orthographic ? SceneView.lastActiveSceneView.camera.orthographicSize / 0.8f : GetCameraDist(aPos);
            return Mathf.Min(0.2f, (dist / 5.0f) * 0.5f);
        }
        public static float GetCameraDist(Vector3 aPt)
        {
            return Vector3.Distance(SceneView.lastActiveSceneView.camera.transform.position, aPt);
        }
        public static Vector3 GetMousePos(Vector2 aMousePos, Transform aTransform)
        {
            Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(new Vector3(aMousePos.x, aMousePos.y, 0));
            Plane plane = new Plane(aTransform.TransformDirection(new Vector3(0, 0, -1)), aTransform.position);
            float dist = 0;
            Vector3 result = new Vector3(0, 0, 0);

            ray = HandleUtility.GUIPointToWorldRay(aMousePos);
            if (plane.Raycast(ray, out dist))
            {
                result = ray.GetPoint(dist);
            }
            return result;
        }
    }
}