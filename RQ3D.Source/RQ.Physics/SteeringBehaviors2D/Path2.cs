using System.Collections.Generic;
using RQ.Physics.Enums;
using UnityEngine;

namespace RQ.AI
{
    public class Path2
    {
        //std::list<Vector2D>            m_WayPoints;
        //private LinkedList<Vector2D> _wayPoints;
        //private Vector2[] _wayPoints;
        /// <summary>
        /// Changed from array to list because a List can be Cleared and reused without allocating new memory
        /// </summary>
        private List<Vector2> _wayPoints;

        //points to the current waypoint
        //private LinkedListNode<Vector2D> _curWaypoint;
        private int _curWaypoint;


        //flag to indicate if the path should be looped
        //(The last waypoint connected to the first)
        //private bool _looped;

        private PathWalkingDirection _pathWalkingDirection;

        public PathWalkingDirection PathWalkingDirection { get { return _pathWalkingDirection; } set { _pathWalkingDirection = value; } }

        private PathType _pathType;

        public PathType PathType { get { return _pathType; } set { _pathType = value; } }
        
        private bool _isFinished;

        public bool IsFinished { get { return _isFinished; } set { _isFinished = value; } }

        public bool DecelerateToFinalWaypoint { get; set; }

        public Path2()
        {
            _pathWalkingDirection = PathWalkingDirection.Forwards;
            _pathType = PathType.Looped;
            DecelerateToFinalWaypoint = true;
            //_looped = false;
        }

        //constructor for creating a path with initial random waypoints. MinX/Y
        //& MaxX/Y define the bounding box of the path.
        //public Path(int NumWaypoints,
        //     float MinX,
        //     float MinY,
        //     float MaxX,
        //     float MaxY,
        //     PathType pathType)
        //{
        //    _pathType = pathType;
        //    CreateRandomPath(NumWaypoints, MinX, MinY, MaxX, MaxY);
        //    //_wayPoints.
        //    _curWaypoint = 0; // _wayPoints.First;
        //}


        //returns the current waypoint
        public bool CurrentWaypoint(out Vector2 wayPoint)
        {
            if (_wayPoints == null || _wayPoints.Count - 1 < _curWaypoint)
            {
                wayPoint = Vector2.zero;
                return false;
            }
            //return _curWaypoint.Value;
            wayPoint = _wayPoints[_curWaypoint];
            return true;
        }

        public bool HasWaypoints()
        {
            return _wayPoints != null;
        }

        //returns true if the end of the list has been reached
        //public bool IsFinished()
        //{
        //    // Loops never finish
        //    if (_looped)
        //        return false;
        //    //int finishedWaypoint = _direction == PathDirection.Forwards ? _wayPoints.Count - 1 : 0;
        //    //bool isFinished = _curWaypoint == finishedWaypoint;
        //    //return isFinished;
        //    return _isFinished;
        //}

        public bool IsGoingToLastWaypoint()
        {
            if (_pathType != PathType.Once)
                return false;

            if (_pathWalkingDirection == PathWalkingDirection.Forwards)
                return _curWaypoint == _wayPoints.Count - 1;
            if (_pathWalkingDirection == PathWalkingDirection.Backwards)
                return _curWaypoint == 0;

            return false;
        }

        //moves the iterator on to the next waypoint in the list
        public void SetNextWaypoint()
        {
            if (_wayPoints.Count == 0)
            {
                return;
            }
            int lastWaypoint = _wayPoints.Count - 1;
            //if (_looped)
            //{
            // Hit the end, what do we do?
            if (_pathWalkingDirection == PathWalkingDirection.Forwards && _curWaypoint == lastWaypoint)
                {
                    if (_pathType == PathType.Looped)
                    {
                        _curWaypoint = 0;
                        return;
                    }
                    else if (_pathType == PathType.Pingpong)
                    {
                        _pathWalkingDirection = PathWalkingDirection.Backwards;
                    }
                    else
                    {
                        _isFinished = true;
                        return;
                        //IsFinished = true;
                        //_direction = PathDirection.Backwards;
                    }
                    //return;
                }
            // Hit the beginning, what do we do?
            if (_pathWalkingDirection == PathWalkingDirection.Backwards && _curWaypoint == 0)
                {
                    if (_pathType == PathType.Looped)
                    {
                        _curWaypoint = lastWaypoint;
                        return;
                    }
                    else if (_pathType == PathType.Pingpong)
                    {
                        _pathWalkingDirection = PathWalkingDirection.Forwards;
                    }
                    else
                    {
                        _isFinished = true;
                        return;
                        //_direction = PathDirection.Forwards;
                    }
                }
            //}
            //if ( && _looped)
            //{
            //     // _wayPoints.First;
            //}
            //else if (_curWaypoint != lastWaypoint)
            //{
            //    //_curWaypoint = _curWaypoint.Next;
            //    _curWaypoint++;
            //}

            _curWaypoint = _pathWalkingDirection == PathWalkingDirection.Forwards ? _curWaypoint + 1 : _curWaypoint - 1;
        }

        //creates a random path which is bound by rectangle described by
        //the min/max values
        //public List<Vector2D> CreateRandomPath(int NumWaypoints,
        //                                     float MinX,
        //                                     float MinY,
        //                                     float MaxX,
        //                                     float MaxY)
        //{
        //    _wayPoints.Clear();

        //    float midX = (MaxX + MinX) / 2.0f;
        //    float midY = (MaxY + MinY) / 2.0f;

        //    float smaller = Mathf.Min(midX, midY);

        //    float spacing = (Mathf.PI * 2) / (float)NumWaypoints;

        //    for (int i = 0; i < NumWaypoints; ++i)
        //    {
        //        float RadialDist = UnityEngine.Random.Range(smaller * 0.2f, smaller);

        //        Vector2D temp = new Vector2D(RadialDist, 0.0f);

        //        Transformations.Vec2DRotateAroundOrigin(temp, (float)i * spacing);

        //        temp.x += midX; temp.y += midY;

        //        _wayPoints.Add(temp);

        //    }

        //    _curWaypoint = 0; // _wayPoints.First;

        //    return _wayPoints;
        //}

        //public void LoopOn()
        //{
        //    _looped = true;
        //}
        //public void LoopOff()
        //{
        //    _looped = false;
        //}

        //public bool GetLooped()
        //{
        //    return _looped;
        //}

        //adds a waypoint to the end of the path
        //public void AddWayPoint(Vector2D new_point)
        //{
        //    _wayPoints.Add(new_point);
        //}

        //methods for setting the path with either another Path or a list of vectors
        public void Set(List<Vector2> new_path)
        {
            _wayPoints = new_path;
            _curWaypoint = 0; // _wayPoints.First;
        }
        public void Set(Path2 path)
        {
            _wayPoints = path.GetWaypoints();
            _curWaypoint = 0; // _wayPoints.First;
        }

        public List<Vector2> GetWaypoints()
        {
            return _wayPoints;
        }

        //public LinkedListNode<Vector2D> GetCurWaypoint()
        //{
        //    return _curWaypoint;
        //}

        /// <summary>
        /// This is O(n), try to limit use!
        /// </summary>
        /// <returns></returns>
        public int GetCurWaypointIndex()
        {
            if (_wayPoints == null)
                return -1;
            //var currentWaypoint = _wayPoints.First;
            //int index = 0;
            //while (currentWaypoint != _curWaypoint)
            //{
            //    currentWaypoint = currentWaypoint.Next;
            //    index++;
            //}
            //return index;
            return _curWaypoint;
        }

        //public void SetCurrentWaypoint(Vector2D waypoint)
        //{
        //    _curWaypoint = _wayPoints.Find(waypoint);
        //    //_wayPoints.;
        //}

        /// <summary>
        /// This is O(n), try to limit use!
        /// </summary>
        /// <param name="index"></param>
        public void SetCurrentWaypoint(int waypoint)
        {
            //var currentWaypoint = _wayPoints.ElementAt(index);
            //_curWaypoint = _wayPoints.Find(currentWaypoint);
            _curWaypoint = waypoint;
        }


        //public void Clear()
        //{
        //    _wayPoints. = null;
        //}

        //public Vector2[] GetPath()
        //{
        //    return _wayPoints;
        //}

        //renders the path in orange
        //  void Render()
        //{
        //    gdi->OrangePen();

        //  std::list<Vector2D>::const_iterator it = m_WayPoints.begin();

        //  Vector2D wp = *it++;

        //  while (it != m_WayPoints.end())
        //  {
        //    gdi->Line(wp, *it);

        //    wp = *it++;
        //  }

        //  if (m_bLooped) gdi->Line(*(--it), *m_WayPoints.begin());
        //}


    }
}
