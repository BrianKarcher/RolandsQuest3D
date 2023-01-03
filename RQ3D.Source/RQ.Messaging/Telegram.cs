using System;

namespace RQ.Messaging
{
    //------------------------------------------------------------------------
    //
    //  Name:   Telegram2
    //
    //  Desc:   This defines a telegram. A telegram is a data structure that
    //          records information required to dispatch messages. Messages 
    //          are used by game agents to communicate with each other.
    //
    //------------------------------------------------------------------------

    [Serializable]
    public struct Telegram : IComparable<Telegram>
    {
        //the entity that sent this telegram
        public string SenderId;

        //the entity that is to receive this telegram
        public string ReceiverId;

        //the message itself. These are all enumerated in the file
        //"MessageTypes.h"
        public string EventName;

        //public TelegramEarlyTermination[] EarlyTermination;

        //messages can be dispatched immediately or delayed for a specified amount
        //of time. If a delay is necessary this field is stamped with the time 
        //the message should be dispatched.
        public float DispatchTime;

        //any additional information that may accompany the message
        public object ExtraInfo;

        //messages can be dispatched immediately or delayed for a specified amount
        //of time. If a delay is necessary this field is stamped with the time 
        //the message should be dispatched.
        // This is used in serialization
        public float TimeRemaining;

        //public Action<object> Act;

        //these telegrams will be stored in a priority queue. Therefore the >
        //operator needs to be overloaded so that the PQ can sort the telegrams
        //by time priority. Note how the times must be smaller than
        //SmallestDelay apart before two Telegrams are considered unique.
        //private const float SmallestDelay = 0.25f;


        //public override bool Equals(object obj)
        //{
        //    if (!(obj is Vector3Serializer))
        //        return false;
        //    var test = (Vector3Serializer)obj;

        //    if (test.x == this.x && test.y == this.y && test.z == this.z)
        //        return true;

        //    return false;
        //}

        //public override int GetHashCode()
        //{
        //    return string.Format("{0}-{1}-{2}", x, y, z).GetHashCode();
        //}


        //public Telegram()
        //{
        //    DispatchTime = -1;
        //    SenderId = -1;
        //    ReceiverId = -1;
        //    Msg = -1;
        //    ExtraInfo = null;
        //}


        //public Telegram2(float time,
        //         string sender,
        //         string receiver,
        //         int msg,
        //         object info = null) : this(time, sender, receiver, 
        //    msg, info, null)
        //{
        //    //DispatchTime = time;
        //    //SenderId = sender;
        //    //ReceiverId = receiver;
        //    //Msg = msg;
        //    //ExtraInfo = info;
        //    //EarlyTermination = earlyTermination.ToList();
        //    //TimeRemaining = 0f;
        //}

        public Telegram(float time,
         string sender,
         string receiver,
         string eventName,
         object info)
        {
            DispatchTime = time;
            SenderId = sender;
            ReceiverId = receiver;
            EventName = eventName;
            ExtraInfo = info;
            //EarlyTermination = earlyTermination == null ? new TelegramEarlyTermination[0] : earlyTermination;
            //Act = act;
            TimeRemaining = 0f;
        }

        //public static bool operator ==(Telegram t1, Telegram t2)
        //{
        //    return (UnityEngine.Mathf.Abs(t1.DispatchTime - t2.DispatchTime) < SmallestDelay) &&
        //            (t1.SenderId == t2.SenderId) &&
        //            (t1.ReceiverId == t2.ReceiverId) &&
        //            (t1.Msg == t2.Msg);
        //}

        //public static bool operator !=(Telegram t1, Telegram t2)
        //{
        //    return !(t1 == t2);
        //}

        public static bool operator <(Telegram t1, Telegram t2)
        {
            //if (t1 == t2)
            //{
            //    return false;
            //}

            //else
            //{
            return (t1.DispatchTime < t2.DispatchTime);
            //}
        }

        public static bool operator >(Telegram t1, Telegram t2)
        {
            //if (t1 == t2)
            //{
            //    return false;
            //}

            //else
            //{
            return (t1.DispatchTime > t2.DispatchTime);
            //}
        }

        // Required for SortedDictionary
        public int CompareTo(Telegram rhs)
        {
            if (this < rhs)
                return -1;
            if (this > rhs)
                return 1;
            return 0;
        }

        //    inline std::ostream& operator<<(std::ostream& os, const Telegram& t)
        //{
        //  os << "time: " << t.DispatchTime << "  Sender: " << t.Sender
        //     << "   Receiver: " << t.Receiver << "   Msg: " << t.Msg;

        //  return os;
        //}

        //handy helper function for dereferencing the ExtraInfo field of the Telegram 
        //to the required type.
        public static T DereferenceToType<T>(object p) where T : class
        {
            return p as T;
        }

    }
}
