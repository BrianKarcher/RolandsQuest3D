﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RQ.Physics.Helpers
{
    public class DrawBoxCast
    {
        //Draws just the box at where it is currently hitting.
        public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float hitInfoDistance, Color color, float duration = 1f)
        {
            origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
            DrawBox(origin, halfExtents, orientation, color, duration);
        }

        //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
        public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color, float duration = 1f)
        {
            direction.Normalize();
            Box bottomBox = new Box(origin, halfExtents, orientation);
            Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);

            Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, color, duration);
            Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color, duration);
            Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color, duration);
            Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, color, duration);
            Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, color, duration);
            Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color, duration);
            Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color, duration);
            Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, color, duration);

            DrawBox(bottomBox, color, duration);
            DrawBox(topBox, color, duration);
        }

        public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color, float duration = 1f)
        {
            DrawBox(new Box(origin, halfExtents, orientation), color, duration);
        }
        public static void DrawBox(Box box, Color color, float duration)
        {
            Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color, duration);
            Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color, duration);
            Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color, duration);
            Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color, duration);

            Debug.DrawLine(box.backTopLeft, box.backTopRight, color, duration);
            Debug.DrawLine(box.backTopRight, box.backBottomRight, color, duration);
            Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color, duration);
            Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color, duration);

            Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color, duration);
            Debug.DrawLine(box.frontTopRight, box.backTopRight, color, duration);
            Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color, duration);
            Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color, duration);
        }

        public struct Box
        {
            public Vector3 localFrontTopLeft { get; private set; }
            public Vector3 localFrontTopRight { get; private set; }
            public Vector3 localFrontBottomLeft { get; private set; }
            public Vector3 localFrontBottomRight { get; private set; }
            public Vector3 localBackTopLeft { get { return -localFrontBottomRight; } }
            public Vector3 localBackTopRight { get { return -localFrontBottomLeft; } }
            public Vector3 localBackBottomLeft { get { return -localFrontTopRight; } }
            public Vector3 localBackBottomRight { get { return -localFrontTopLeft; } }

            public Vector3 frontTopLeft { get { return localFrontTopLeft + origin; } }
            public Vector3 frontTopRight { get { return localFrontTopRight + origin; } }
            public Vector3 frontBottomLeft { get { return localFrontBottomLeft + origin; } }
            public Vector3 frontBottomRight { get { return localFrontBottomRight + origin; } }
            public Vector3 backTopLeft { get { return localBackTopLeft + origin; } }
            public Vector3 backTopRight { get { return localBackTopRight + origin; } }
            public Vector3 backBottomLeft { get { return localBackBottomLeft + origin; } }
            public Vector3 backBottomRight { get { return localBackBottomRight + origin; } }

            public Vector3 origin { get; private set; }

            public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
            {
                Rotate(orientation);
            }
            public Box(Vector3 origin, Vector3 halfExtents)
            {
                this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
                this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
                this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
                this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

                this.origin = origin;
            }


            public void Rotate(Quaternion orientation)
            {
                localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
                localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
                localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
                localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
            }
        }

        //This should work for all cast types
        static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance)
        {
            return origin + (direction.normalized * hitInfoDistance);
        }

        static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            Vector3 direction = point - pivot;
            return pivot + rotation * direction;
        }
    }
}
