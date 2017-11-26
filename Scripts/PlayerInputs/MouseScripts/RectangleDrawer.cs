using UnityEngine;

namespace Assets.Scripts.PlayerInputs.MouseScripts
{
    public class RectangleDrawer
    {
        #region Singleton

        private static readonly RectangleDrawer I = new RectangleDrawer();
        static RectangleDrawer() { }
        private RectangleDrawer() { }
        public static RectangleDrawer Instance { get { return I; } }

        #endregion

        private Texture2D _whiteTexture;
        public  Texture2D WhiteTexture
        {
            get
            {
                if ( _whiteTexture != null )
                    return _whiteTexture;

                _whiteTexture = new Texture2D( 1, 1 );
                _whiteTexture.SetPixel( 0, 0, Color.white );
                _whiteTexture.Apply();

                return _whiteTexture;
            }
        }

        public void DrawScreenRect( Rect rect, Color color )
        {
            GUI.color = color;
            GUI.DrawTexture( rect, WhiteTexture );
            GUI.color = Color.white;
        }

        public void DrawScreenRectBorder( Rect rect, float thickness, Color color )
        {
            // Top
            DrawScreenRect( new Rect( rect.xMin, rect.yMin, rect.width, thickness ), color );
            // Left
            DrawScreenRect(new Rect( rect.xMin, rect.yMin, thickness, rect.height ), color );
            // Right
            DrawScreenRect(new Rect( rect.xMax - thickness, rect.yMin, thickness, rect.height ), color );
            // Bottom
            DrawScreenRect(new Rect( rect.xMin, rect.yMax - thickness, rect.width, thickness ), color );
        }

        public Rect GetScreenRect( Vector3 screenPosition1, Vector3 screenPosition2 )
        {
            // Move origin from bottom left to top left
            screenPosition1.y = Screen.height - screenPosition1.y;
            screenPosition2.y = Screen.height - screenPosition2.y;
            // Calculate corners
            var topLeft = Vector3.Min(screenPosition1, screenPosition2);
            var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
            // Create Rect
            return Rect.MinMaxRect( topLeft.x, topLeft.y, bottomRight.x, bottomRight.y );
        }

        public Bounds GetViewportBounds( Camera camera, Vector3 screenPosition1, Vector3 screenPosition2 )
        {
            var v1 = Camera.main.ScreenToViewportPoint( screenPosition1 );
            var v2 = Camera.main.ScreenToViewportPoint( screenPosition2 );
            var min = Vector3.Min(v1, v2);
            var max = Vector3.Max(v1, v2);
            min.z = camera.nearClipPlane;
            max.z = camera.farClipPlane;

            var bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }
    }
}