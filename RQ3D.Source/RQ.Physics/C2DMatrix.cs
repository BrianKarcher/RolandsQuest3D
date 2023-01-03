using System.Collections.Generic;
using UnityEngine;

namespace RQ.Physics
{
    public struct C2DMatrix
    {
        public struct Matrix
        {

            public float _11, _12, _13;
            public float _21, _22, _23;
            public float _31, _32, _33;

            //Matrix()
            //{
            //    _11 = 0.0f; _12 = 0.0f; _13 = 0.0f;
            //    _21 = 0.0f; _22 = 0.0f; _23 = 0.0f;
            //    _31 = 0.0f; _32 = 0.0f; _33 = 0.0f;
            //}

        };

        Matrix _matrix;

        //multiplies m_Matrix with mIn
        public void MatrixMultiply(Matrix mIn)
        {
            C2DMatrix.Matrix mat_temp;

            //first row
            mat_temp._11 = (_matrix._11 * mIn._11) + (_matrix._12 * mIn._21) + (_matrix._13 * mIn._31);
            mat_temp._12 = (_matrix._11 * mIn._12) + (_matrix._12 * mIn._22) + (_matrix._13 * mIn._32);
            mat_temp._13 = (_matrix._11 * mIn._13) + (_matrix._12 * mIn._23) + (_matrix._13 * mIn._33);

            //second
            mat_temp._21 = (_matrix._21 * mIn._11) + (_matrix._22 * mIn._21) + (_matrix._23 * mIn._31);
            mat_temp._22 = (_matrix._21 * mIn._12) + (_matrix._22 * mIn._22) + (_matrix._23 * mIn._32);
            mat_temp._23 = (_matrix._21 * mIn._13) + (_matrix._22 * mIn._23) + (_matrix._23 * mIn._33);

            //third
            mat_temp._31 = (_matrix._31 * mIn._11) + (_matrix._32 * mIn._21) + (_matrix._33 * mIn._31);
            mat_temp._32 = (_matrix._31 * mIn._12) + (_matrix._32 * mIn._22) + (_matrix._33 * mIn._32);
            mat_temp._33 = (_matrix._31 * mIn._13) + (_matrix._32 * mIn._23) + (_matrix._33 * mIn._33);

            _matrix = mat_temp;
        }

        //public C2DMatrix()
        //{
        //    //initialize the matrix to an identity matrix
        //    Identity();
        //}

        //create an identity matrix
        public void Identity()
        {
            _matrix._11 = 1; _matrix._12 = 0; _matrix._13 = 0;

            _matrix._21 = 0; _matrix._22 = 1; _matrix._23 = 0;

            _matrix._31 = 0; _matrix._32 = 0; _matrix._33 = 1;
        }

        //create a transformation matrix
        public void Translate(float x, float y)
        {
            Matrix mat;

            mat._11 = 1; mat._12 = 0; mat._13 = 0;

            mat._21 = 0; mat._22 = 1; mat._23 = 0;

            mat._31 = x; mat._32 = y; mat._33 = 1;

            //and multiply
            MatrixMultiply(mat);
        }

        //create a scale matrix
        public void Scale(float xScale, float yScale)
        {
            C2DMatrix.Matrix mat;

            mat._11 = xScale; mat._12 = 0; mat._13 = 0;

            mat._21 = 0; mat._22 = yScale; mat._23 = 0;

            mat._31 = 0; mat._32 = 0; mat._33 = 1;

            //and multiply
            MatrixMultiply(mat);
        }

        //create a rotation matrix
        public void Rotate(float rotation)
        {
            C2DMatrix.Matrix mat;

            float Sin = Mathf.Sin(rotation);
            float Cos = Mathf.Cos(rotation);

            mat._11 = Cos; mat._12 = Sin; mat._13 = 0;

            mat._21 = -Sin; mat._22 = Cos; mat._23 = 0;

            mat._31 = 0; mat._32 = 0; mat._33 = 1;

            //and multiply
            MatrixMultiply(mat);
        }

        //create a rotation matrix from a fwd and side 2D vector
        public void Rotate(Vector2 fwd, Vector2 side)
        {
            C2DMatrix.Matrix mat;

            mat._11 = fwd.x; mat._12 = fwd.y; mat._13 = 0;

            mat._21 = side.x; mat._22 = side.y; mat._23 = 0;

            mat._31 = 0; mat._32 = 0; mat._33 = 1;

            //and multiply
            MatrixMultiply(mat);
        }

        //applys a transformation matrix to a std::vector of points
        public void TransformVector2Ds(IEnumerable<Vector2> vPoints)
        {
            var enumerator = vPoints.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var vector = enumerator.Current;
                float tempX = (_matrix._11 * vector.x) + (_matrix._21 * vector.y) + (_matrix._31);

                float tempY = (_matrix._12 * vector.x) + (_matrix._22 * vector.y) + (_matrix._32);

                vector.x = tempX;

                vector.y = tempY;
            }
        }

        //applys a transformation matrix to a point
        public Vector2 TransformVector2Ds(Vector2 vPoint)
        {
            float tempX = (_matrix._11 * vPoint.x) + (_matrix._21 * vPoint.y) + (_matrix._31);

            float tempY = (_matrix._12 * vPoint.x) + (_matrix._22 * vPoint.y) + (_matrix._32);

            vPoint.x = tempX;

            vPoint.y = tempY;

            return vPoint;
        }

        //accessors to the matrix elements
        public void _11(float val)
        {
            _matrix._11 = val;
        }
        public void _12(float val)
        {
            _matrix._12 = val;
        }
        public void _13(float val)
        {
            _matrix._13 = val;
        }

        public void _21(float val)
        {
            _matrix._21 = val;
        }
        public void _22(float val)
        {
            _matrix._22 = val;
        }
        public void _23(float val)
        {
            _matrix._23 = val;
        }

        public void _31(float val)
        {
            _matrix._31 = val;
        }
        public void _32(float val)
        {
            _matrix._32 = val;
        }
        public void _33(float val)
        {
            _matrix._33 = val;
        }
    }
}
