using System.Collections.Generic;
using UnityEngine;

namespace RQ.Physics
{
    //[Obsolete("Obsolete... maybe.  Must test this vs. Unity's functions or another library.  Would like to avoid Unity's functions since those can't be unit tested.")]
    /// <summary>
    /// Most of these probably aren't needed, we can use the Unity built-in functions instead for Matrix and Transform math.
    /// This stuff is standard.
    /// Will need to refactor.
    /// </summary>
    public static class Transformations
    {
        //--------------------------- WorldTransform -----------------------------
        //
        //  given a std::vector of 2D vectors, a position, orientation and scale,
        //  this function transforms the 2D vectors into the object's world space
        //------------------------------------------------------------------------
        public static IEnumerable<Vector2> WorldTransform(List<Vector2> points,
                                                    Vector2 pos,
                                                    Vector2 forward,
                                                    Vector2 side,
                                                    Vector2 scale)
        {
            //copy the original vertices into the buffer about to be transformed
            Vector2[] TranVector2Ds = new Vector2[points.Count];
            points.CopyTo(TranVector2Ds);

            //create a transformation matrix
            C2DMatrix matTransform = new C2DMatrix();
            matTransform.Identity();
            //Matrix4x4 matTransform = Matrix4x4.identity;

            //scale
            if ((scale.x != 1.0) || (scale.y != 1.0))
            {
                matTransform.Scale(scale.x, scale.y);
            }

            //rotate
            matTransform.Rotate(forward, side);

            //and translate
            matTransform.Translate(pos.x, pos.y);

            //now transform the object's vertices
            matTransform.TransformVector2Ds(TranVector2Ds);

            return TranVector2Ds;
        }

        //--------------------------- WorldTransform -----------------------------
        //
        //  given a std::vector of 2D vectors, a position and  orientation
        //  this function transforms the 2D vectors into the object's world space
        //------------------------------------------------------------------------
        public static IEnumerable<Vector2> WorldTransform(List<Vector2> points,
                                         Vector2 pos,
                                         Vector2 forward,
                                         Vector2 side)
        {
            //copy the original vertices into the buffer about to be transformed
            Vector2[] TranVector2Ds = new Vector2[points.Count];
            points.CopyTo(TranVector2Ds);

            //create a transformation matrix
            C2DMatrix matTransform = new C2DMatrix();
            matTransform.Identity();

            //rotate
            matTransform.Rotate(forward, side);

            //and translate
            matTransform.Translate(pos.x, pos.y);

            //now transform the object's vertices
            matTransform.TransformVector2Ds(TranVector2Ds);

            return TranVector2Ds;
        }

        //--------------------- PointToWorldSpace --------------------------------
        //
        //  Transforms a point from the agent's local space into world space
        //------------------------------------------------------------------------
        public static Vector2 PointToWorldSpace(Vector2 point,
                                            Vector2 AgentHeading,
                                            Vector2 AgentSide,
                                            Vector2 AgentPosition)
        {
            //make a copy of the point
            Vector2 TransPoint = new Vector2(point.x, point.y);

            //create a transformation matrix
            C2DMatrix matTransform = new C2DMatrix();
            matTransform.Identity();

            //rotate
            matTransform.Rotate(AgentHeading, AgentSide);

            //and translate
            matTransform.Translate(AgentPosition.x, AgentPosition.y);

            //now transform the vertices
            TransPoint = matTransform.TransformVector2Ds(TransPoint);

            return TransPoint;
        }

        //--------------------- VectorToWorldSpace --------------------------------
        //
        //  Transforms a vector from the agent's local space into world space
        //------------------------------------------------------------------------
        public static Vector2 VectorToWorldSpace(Vector2 vec,
                                             Vector2 AgentHeading,
                                             Vector2 AgentSide)
        {
            //make a copy of the point
            Vector2 TransVec = new Vector2(vec.x, vec.y);

            //create a transformation matrix
            C2DMatrix matTransform = new C2DMatrix();
            matTransform.Identity();

            //rotate
            matTransform.Rotate(AgentHeading, AgentSide);

            //now transform the vertices
            TransVec = matTransform.TransformVector2Ds(TransVec);

            return TransVec;
        }


        //--------------------- PointToLocalSpace --------------------------------
        //
        //------------------------------------------------------------------------
        public static Vector2 PointToLocalSpace(Vector2 point,
                                     Vector2 AgentHeading,
                                     Vector2 AgentSide,
                                      Vector2 AgentPosition)
        {

            //make a copy of the point
            Vector2 TransPoint = new Vector2(point.x, point.y);

            //create a transformation matrix
            C2DMatrix matTransform = new C2DMatrix();
            matTransform.Identity();

            float Tx = -Vector2.Dot(AgentPosition, AgentHeading);
            float Ty = -Vector2.Dot(AgentPosition, AgentSide);

            //create the transformation matrix
            matTransform._11(AgentHeading.x); matTransform._12(AgentSide.x);
            matTransform._21(AgentHeading.y); matTransform._22(AgentSide.y);
            matTransform._31(Tx); matTransform._32(Ty);

            //now transform the vertices
            TransPoint = matTransform.TransformVector2Ds(TransPoint);

            return TransPoint;
        }

        //--------------------- VectorToLocalSpace --------------------------------
        //
        //------------------------------------------------------------------------
        public static Vector2 VectorToLocalSpace(Vector2 vec,
                                     Vector2 AgentHeading,
                                     Vector2 AgentSide)
        {

            //make a copy of the point
            Vector2 TransPoint = new Vector2(vec.x, vec.y);

            //create a transformation matrix
            C2DMatrix matTransform = new C2DMatrix();
            matTransform.Identity();

            //create the transformation matrix
            matTransform._11(AgentHeading.x); matTransform._12(AgentSide.x);
            matTransform._21(AgentHeading.y); matTransform._22(AgentSide.y);

            //now transform the vertices
            matTransform.TransformVector2Ds(TransPoint);

            return TransPoint;
        }

        //-------------------------- Vec2DRotateAroundOrigin --------------------------
        //
        //  rotates a vector ang rads around the origin
        //-----------------------------------------------------------------------------
        public static Vector2 Vec2DRotateAroundOrigin(Vector2 v, float ang)
        {
            //create a transformation matrix
            C2DMatrix mat = new C2DMatrix();
            mat.Identity();

            //rotate
            mat.Rotate(ang);

            //now transform the object's vertices
            v = mat.TransformVector2Ds(v);

            return v;
        }

        //------------------------ CreateWhiskers ------------------------------------
        //
        //  given an origin, a facing direction, a 'field of view' describing the 
        //  limit of the outer whiskers, a whisker length and the number of whiskers
        //  this method returns a vector containing the end positions of a series
        //  of whiskers radiating away from the origin and with equal distance between
        //  them. (like the spokes of a wheel clipped to a specific segment size)
        //----------------------------------------------------------------------------
        //public static List<Vector2D> CreateWhiskers(uint NumWhiskers,
        //                                            float WhiskerLength,
        //                                            float fov,
        //                                            Vector2D facing,
        //                                            Vector2D origin)
        //{
        //    //this is the magnitude of the angle separating each whisker
        //    float SectorSize = fov / (float)(NumWhiskers - 1);

        //    List<Vector2D> whiskers = new List<Vector2D>();
        //    Vector2D temp;
        //    float angle = -fov * 0.5f;

        //    for (uint w = 0; w < NumWhiskers; ++w)
        //    {
        //        //create the whisker extending outwards at this angle
        //        temp = facing;
        //        Vec2DRotateAroundOrigin(temp, angle);
        //        whiskers.Add(origin + WhiskerLength * temp);

        //        angle += SectorSize;
        //    }

        //    return whiskers;
        //}
    }
}
