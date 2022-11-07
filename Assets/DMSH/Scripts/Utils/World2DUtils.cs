using DMSH.Scripts.Objects.Projectiles;

using UnityEngine;

namespace Scripts.Utils
{
    public static class World2DUtils
    {
        public static void MoveRigidbodyInsideScreen(this Rigidbody2D src, Vector2 velocity, Camera camera)
        {
            src.velocity = velocity;

            if (IsOutOfGameView(src.position, camera, out var insidePosition))
            {
                src.position = insidePosition;
            }
        }

        /// <summary>Is current position out of the game screen?</summary>
        /// <param name="currentPosition">object position in world</param>
        /// <param name="camera">game camera</param>
        /// <param name="positionInWorld">updated position in game screen</param>
        /// <param name="collidePositionOffset">move positionInWorld to center by this value offset</param>
        /// <returns>true - if game object out of the game screen</returns>
        public static bool IsOutOfGameView(Vector2 currentPosition, Camera camera, out Vector2 positionInWorld, float collidePositionOffset = 0)
        {
            var cameraPixelRect = camera.pixelRect;
            var bottomLeft = camera.ScreenToWorldPoint(new Vector3(cameraPixelRect.x, cameraPixelRect.y, 0));
            var topRight = camera.ScreenToWorldPoint(new Vector3(cameraPixelRect.x + cameraPixelRect.width, cameraPixelRect.y + cameraPixelRect.height));

            var cameraRect = new Rect(
                bottomLeft.x,
                bottomLeft.y,
                topRight.x - bottomLeft.x,
                topRight.y - bottomLeft.y);

            var clampedPosition = new Vector2(
                Mathf.Clamp(currentPosition.x, cameraRect.xMin, cameraRect.xMax),
                Mathf.Clamp(currentPosition.y, cameraRect.yMin, cameraRect.yMax)
            );

            if (clampedPosition != currentPosition)
            {
                if (collidePositionOffset > 0)
                {
                    if (clampedPosition.x == cameraRect.xMin)
                    {
                        clampedPosition.x += collidePositionOffset;
                    }
                    else if (clampedPosition.x == cameraRect.xMax)
                    {
                        clampedPosition.x -= collidePositionOffset;
                    }

                    if (clampedPosition.y == cameraRect.yMin)
                    {
                        clampedPosition.y += collidePositionOffset;
                    }
                    else if (clampedPosition.y == cameraRect.yMax)
                    {
                        clampedPosition.y -= collidePositionOffset;
                    }
                }

                positionInWorld = clampedPosition;
                return true;
            }

            positionInWorld = default;
            return false;
        }

        public static WallsFlags GetCollidedWall(Vector2 currentPosition, Camera camera)
        {
            var cameraPixelRect = camera.pixelRect;
            var bottomLeft = camera.ScreenToWorldPoint(new Vector3(cameraPixelRect.x, cameraPixelRect.y, 0));
            var topRight = camera.ScreenToWorldPoint(new Vector3(cameraPixelRect.x + cameraPixelRect.width, cameraPixelRect.y + cameraPixelRect.height));

            var cameraRect = new Rect(
                bottomLeft.x,
                bottomLeft.y,
                topRight.x - bottomLeft.x,
                topRight.y - bottomLeft.y);

            var result = WallsFlags.Unset;

            if (currentPosition.x < cameraRect.xMin)
            {
                // left
                result |= WallsFlags.Left;
                result &= ~WallsFlags.Unset;
            }
            else if (currentPosition.x > cameraRect.xMax)
            {
                // right
                result |= WallsFlags.Right;
                result &= ~WallsFlags.Unset;
            }

            if (currentPosition.y > cameraRect.yMax)
            {
                // top
                result |= WallsFlags.Top;
                result &= ~WallsFlags.Unset;
            }
            else if (currentPosition.y < cameraRect.yMin)
            {
                // down
                result |= WallsFlags.Down;
                result &= ~WallsFlags.Unset;
            }

            return result;
        }

        public static Vector2 MoveToScreen(Vector2 sourcePosition, float valueInPixels)
        {
            return sourcePosition;
        }
    }
}