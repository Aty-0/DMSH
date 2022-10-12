using UnityEngine;

namespace Scripts.Utils
{
    public static class World2DUtils
    {
        public static void MoveRigidbodyInsideScreen(this Rigidbody2D src, Vector2 velocity, Camera camera, float removeFromRight)
        {
            var bottomLeft = camera.ScreenToWorldPoint(Vector3.zero);
            var topRight = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth - removeFromRight, camera.pixelHeight));

            var cameraRect = new Rect(
                bottomLeft.x,
                bottomLeft.y,
                topRight.x - bottomLeft.x,
                topRight.y - bottomLeft.y);

            src.velocity = velocity;

            var srcPosition = src.position;
            var clampedPosition = new Vector2(
                Mathf.Clamp(srcPosition.x, cameraRect.xMin, cameraRect.xMax),
                Mathf.Clamp(srcPosition.y, cameraRect.yMin, cameraRect.yMax)
            );

            if (clampedPosition != srcPosition)
            {
                src.position = clampedPosition;
            }
        }
    }
}