using PriorityQueues;
//using RQ.Entity.Components;
//using RQ.Entities;
//using RQ.Entities.Common;
//using RQ.Enums;
//using RQ.Model;
//using RQ.Logging;
//using RQ.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
//using RQ.Model.ObjectPool;
using UnityEngine;

namespace RQ.Messaging
{
    //[AddComponentMenu("RQ/Components/Message Dispatcher2")]
    public class MessageDispatcher
    {
        internal class Events : Dictionary<long, Action<Telegram>>
        {
            public long currentIndex = 0;
            //public List<Action<Telegram2>> AllEvents = new List<Action<Telegram2>>();
        }

        internal class EventHandler : Dictionary<string, Events>
        {
            //public EventHander()
            //{
            //    AllEvents = new List<Action<Telegram2>>();
            //}
        }

        //{
        //    public string UniqueId { get; set; }
        //    public Action<object> CallbackMethod { get; set; }
        //}
        //to make code easier to read
        //private const float SEND_MSG_IMMEDIATELY = 0.0f;
        //private const int NO_ADDITIONAL_INFO = 0;
        //private const int SENDER_ID_IRRELEVANT = -1;

        //private IMessageHandler _gameController;

        private Dictionary<string, EventHandler> _messageHandlers = new Dictionary<string, EventHandler>();

        // @todo Have this class be the container
        //private IMessageHandlerContainer _messageHandlerContainer;

        // Singleton instance
        public static readonly MessageDispatcher Instance = new MessageDispatcher();

        //a std::set is used as the container for the delayed messages
        //because of the benefit of automatic sorting and avoidance
        //of duplicates. Messages are sorted by their dispatch time.
        // todo Change this to a SortedSet when Unity finally goes to .NET 4.0......
        //private SortedDictionary<Telegram, bool> PriorityQ;
        //private List<Telegram> PriorityQ;
        // Inserts and removals are very fast with a Linked List, which we
        // will be using often with the messaging system
        //private LinkedList<Telegram> PriorityQ = new LinkedList<Telegram>();
        private PriorityQueue<Telegram> PriorityQ = new PriorityQueue<Telegram>();
        //private List<Telegram> _itemsToRemove = new List<Telegram>();

        //public void SetGameController(IMessageHandler gameController)
        //{
        //    _gameController = gameController;
        //}

        //public void RegisterMessageHandlerContainer(IMessageHandlerContainer messageHandlerContainer)
        //{
        //    _messageHandlerContainer = messageHandlerContainer;
        //}

        public void ResetAll()
        {
            ResetQueue();
            ResetMessageHandlerList();
        }

        public void ResetMessageHandlerList()
        {
            _messageHandlers.Clear();
        }

        public virtual long StartListening(string eventName, string id, Action<Telegram> callbackMethod)
        {
            //////if (_messageHandlers.ContainsKey(messageHandler.UniqueId))
            //////    return;
            //throw new Exception("MessageDispatcher - Unique Id " + messageHandler.UniqueId + " already exists");
            //if (!_messageHandlers.ContainsKey(messageHandler.UniqueId))
            if (callbackMethod == null)
            {
                Debug.LogError($"(MessageDispatcher2) No callback method for {eventName} for entity {id}");
            }
            EventHandler eventHandler;
            if (!_messageHandlers.TryGetValue(eventName, out eventHandler))
            {
                eventHandler = new EventHandler();
                _messageHandlers.Add(eventName, eventHandler);
            }
            // Multiple events may be registered to the same id
            Events events;
            if (!eventHandler.TryGetValue(id, out events))
            {
                events = new Events();
                eventHandler.Add(id, events);
            }
            events.currentIndex++;
            if (!events.ContainsKey(events.currentIndex))
            {
                events.Add(events.currentIndex, callbackMethod);
                //events.AllEvents.Add(callbackMethod);
            }
            return events.currentIndex;
        }

        public bool IsListening(string eventName, string id)
        {
            if (!_messageHandlers.ContainsKey(eventName))
                return false;
            return _messageHandlers[eventName].ContainsKey(id);
        }

        public void StopListening(string eventName, string id, long index)
        {
            // Note: We do not remove the event even when nobody is listening to it. Just keep the dictionary empty.
            // This is a quasi pooling system so we can reuse it in the future and it does not get Garbage Collected.

            //EventHandler eventHandler;
            if (_messageHandlers.TryGetValue(eventName, out var eventHandler))
            {
                //Events events;
                if (eventHandler.TryGetValue(id, out var events))
                {
                    if (index == -1)
                    {
                        events.Clear();
                        //events.AllEvents.Clear();
                    }
                    else if (events.ContainsKey(index))
                    {
                        //events.AllEvents.Remove(events[index]);
                        events.Remove(index);
                    }
                }
                //if (events != null && events.Count == 0)
                //{
                //    eventHandler.Remove(id);
                //}
                //eventHandler.Remove(id);
            }
        }

        public void ChangeListener(string oldId, string newId)
        {
            if (oldId == newId)
                return;
            //var eventNames = _messageHandlers.Keys.ToList();
            //foreach (var eventName in eventNames)
            //{
            //var eventNames = _messageHandlers.Keys.ToList();
            foreach (var eventName in _messageHandlers.Keys)
            {
                EventHandler eventHandler;
                if (!_messageHandlers.TryGetValue(eventName, out eventHandler))
                    continue;

                Events ourEvents;
                // See if the listener is listening to this event, if so get the delegate
                if (!eventHandler.TryGetValue(oldId, out ourEvents))
                    continue;
                // Remove the old key
                eventHandler.Remove(oldId);
                if (eventHandler.ContainsKey(newId))
                    Debug.LogError("(" + eventName + ") key already present in dictionary " + newId);
                // Assign the retreived delegate to the new key
                eventHandler.Add(newId, ourEvents);

            }
        }

        //this method is utilized by DispatchMsg or DispatchDelayedMessages.
        //This method calls the message handling member function of the receiving
        //entity, pReceiver, with the newly created telegram
        //private List<Dictionary<string, Events>> eventHandlers;

        private Queue<List<Events>> _eventListQueue = new Queue<List<Events>>();

        private List<Events> PullEventListFromQueue()
        {
            //_eventListQueue.
            if (_eventListQueue.Count == 0)
            {
                return new List<Events>();
            }
            return _eventListQueue.Dequeue();
        }

        private void PushEventListToQueue(List<Events> events)
        {
            _eventListQueue.Enqueue(events);
        }

        private bool Discharge(string receiverId, Telegram telegram)
        {
            //get a pointer to the receiver
            //IMessageHandler messageHandler = EntityController._instance.GetEntity(receiverId); //EntityMgr->GetEntityFromID(receiver);
            //IMessageHandler messageHandler = _messageHandlerContainer.GetEntityForHandler(receiverId);
            EventHandler eventHandler;
            if (!_messageHandlers.TryGetValue(telegram.EventName, out eventHandler))
            {
                //isSuccess = false;
                //throw new Exception("Could not locate id " + receiverId + " in the messaging system for message " + telegram.Msg + ".");
                return false;
            }
            // If receiver is null, send telegram to all listeners for this event type
            if (receiverId == null)
            {
                //var events = ObjectPool.Instance.PullFromPool<List<Events>>(ObjectPoolType.ListOfMessageEvents);
                var events = PullEventListFromQueue();
                events.Clear();
                //var keys = eventHandler.Keys;
                //for (int i = keys.Count - 1; i >= 0; i--)
                //{
                //    var key = keys[i];
                foreach (var @event in eventHandler)
                {
                    events.Add(@event.Value);
                }

                foreach (var @event in events)
                {
                    //Events event;
                    //if (eventHandler.TryGetValue(events.Key, out var @event))
                    //{
                    Discharge(@event, telegram);
                    //}
                }

                PushEventListToQueue(events);
                //ObjectPool.Instance.ReleaseToPool(ObjectPoolType.ListOfMessageEvents, events);
                //foreach (var action in eventHandler)
                //{
                //    action.Value(telegram);
                //}
            }
            else
            {
                // Single receiver
                Events events;
                if (!eventHandler.TryGetValue(receiverId, out events))
                {
                    //Debug.LogError("Could not locate receiver id " + receiverId + " for " + telegram.EventName);
                    return false;
                }
                Discharge(events, telegram);
            }
            //make sure the receiver is valid
            //if (action == null)
            //{
            //    //throw new Exception("Warning! No Receiver with ID of " + receiverId + " found");


            //    return false;
            //}

            //isSuccess = true;
            //return messageHandler.HandleMessage(telegram);

            return true;
        }

        // Only instantiates once so it only causes one GC alloc for the entire game, vs once per call if this was inside a function
        // Clear this before use!
        //private List<Action<Telegram2>> tempActionsList = new List<Action<Telegram2>>();
        private readonly Queue<Queue<Action<Telegram>>> _actionTelegramQueue = new Queue<Queue<Action<Telegram>>>();

        private Queue<Action<Telegram>> PullActionTelegramFromQueue()
        {
            //_eventListQueue.
            if (_actionTelegramQueue.Count == 0)
            {
                return new Queue<Action<Telegram>>();
            }
            return _actionTelegramQueue.Dequeue();
        }

        private void PushActionTelegramToQueue(Queue<Action<Telegram>> actionTelegarms)
        {
            _actionTelegramQueue.Enqueue(actionTelegarms);
        }

        private void Discharge(Events events, Telegram telegram)
        {
            if (events == null)
                return;

            //var tempEvents = new Events();

            // Pooling this heap object so we limit GC
            // We pool instead of keep a class object because, through recursion, this can be called multiple times at once
            //if (ObjectPool.Instance == null)
            //    return;
            //var tempEvents = ObjectPool.Instance.PullFromPool<Queue<Action<Telegram2>>>(ObjectPoolType.QueueActionTelegram2);
            var tempEvents = PullActionTelegramFromQueue();
            tempEvents.Clear();
            //var tempEvents = new Queue<Action<Telegram2>>();
            foreach (var thisEvent in events)
            //for (int i = 0; i < events.AllEvents.Count; i++)
            {
                //var thisEvent = events[i];
                tempEvents.Enqueue(thisEvent.Value);
                //tempEvents.Add(thisevent.Key, thisevent.Value);
            }
            //var keys = events.Keys;
            //events.

            while (tempEvents.Count > 0)
            {
                var hmm = tempEvents.Peek();
                if (hmm == null)
                {
                    int i = 1;
                }
                var _event = tempEvents.Dequeue();
                _event(telegram);
            }

            //foreach (var tempEvent in tempEvents)
            //    tempEvent.Value(telegram);
            //ObjectPool.Instance.ReleaseToPool(ObjectPoolType.QueueActionTelegram2, tempEvents);
            PushActionTelegramToQueue(tempEvents);
        }

        // Private constructor enforces the singleton
        private MessageDispatcher()
        {

        }

        public virtual void Awake()
        {
            //PriorityQ = new SortedDictionary<Telegram, bool>();
            //PriorityQ = new List<Telegram>();
            //PriorityQ = new LinkedList<Telegram>();
            //_messageHandlers = new Dictionary<string, IMessagingObject>();
        }

        public void ResetQueue()
        {
            //while()
            //PriorityQ.Dequeue
            PriorityQ.Clear();
        }

        /// <summary>
        /// Removes the messages destined for an entity from the queue
        /// </summary>
        /// <param name="entityId"></param>
        public void RemoveMessagesForReceiverId(string receiverId)
        {
            PriorityQ.RemoveAll(i => i.ReceiverId == receiverId);
            //LinkedListNode<Telegram> node = PriorityQ.First;

            //while (node != null)
            //{
            //    //var nextNode = node.Next;

            //    if (node.Value.ReceiverId == entityId)
            //        PriorityQ.Remove(node);

            //    //node = nextNode;
            //    node = node.Next;
            //}
        }

        public void RemoveMessages(string eventName, string receiverId)
        {
            PriorityQ.RemoveAll(i => i.ReceiverId == receiverId && i.EventName == eventName);
        }

        //private bool filterMessagesByEventNameAndReceiver<T>(string eventName, string receiverId)
        //    where T : Telegram2
        //{
        //    return 
        //}

        //copy ctor and assignment should be private
        //private MessageDispatcher(MessageDispatcher&);
        //private static MessageDispatcher operator =(MessageDispatcher rhs) {}

        public bool DispatchMsgToList(string eventName, float delay,
                 string senderId,
                 IEnumerable<string> receiverIds,
                 object ExtraInfo)
        {
            foreach (var receiverId in receiverIds)
            {
                DispatchMsg(eventName, delay, senderId, receiverId, ExtraInfo);
            }
            return false;
        }

        //send a message to another agent. Receiving agent is referenced by ID.
        //public bool DispatchMsg(string eventName, float delay,
        //                 string senderId,
        //                 string receiverId,
        //                 object ExtraInfo)
        //{
        //    return DispatchMsgWithEarlyTermination(eventName, delay, senderId, receiverId, ExtraInfo);
        //}

        public void DispatchMsg(string eventName,
                 float delay,
                 string senderId,
                 string receiverId, Action<object> p)
        {
            DispatchMsgWithEarlyTermination(eventName, delay, senderId, receiverId, p);
        }

        public bool DispatchMsg(string eventName,
                 float delay,
                 string senderId,
                 string receiverId,
                 object ExtraInfo
                 )
        {
            return DispatchMsgWithEarlyTermination(eventName, delay, senderId, receiverId, ExtraInfo);
        }

        public bool DispatchMsgWithEarlyTermination(string eventName, float delay,
                 string senderId,
                 string receiverId,
                 object ExtraInfo)
        {
            if (eventName == "VictoryPose")
            {
                Debug.LogError("(MessageDispatcher) VictoryPose called");
                int i = 1;
            }
            bool isDispatched = false;
            //create the telegram
            var telegram = CreateTelegram(delay, senderId, receiverId, eventName, ExtraInfo); //new Telegram(0, senderId, receiverId, msg, ExtraInfo, earlyTermination);

            //if there is no delay, route telegram immediately
            if (delay <= 0.0)
            {
                //Log.Info("Telegram dispatched at time: " + Time.time
                //   + " by " + senderId + " for " + receiverId
                //   + ". Msg is " + msg);
                //#ifdef SHOW_MESSAGING_INFO
                //debug_con << "\nTelegram dispatched at time: " << TickCounter->GetCurrentFrame()
                //     << " by " << sender << " for " << receiver 
                //     << ". Msg is " << msg << "";
                //#endif

                //send the telegram to the recipient
                //bool isSuccess;
                isDispatched = Discharge(receiverId, telegram);
            }

            //else calculate the time when the telegram should be dispatched
            else
            {
                float CurrentTime = UnityEngine.Time.time; //TickCounter->GetCurrentFrame(); 

                telegram.DispatchTime = CurrentTime + delay;

                //and put it in the queue
                PriorityQ.Enqueue(telegram);
                //PriorityQ.AddLast(telegram);
                //Log.Info("Adding item from message queue, count = " + PriorityQ.Count);

                //if (PriorityQ.ContainsKey(telegram))
                //{
                //    Log.Error("Telegram " + telegram.Msg.ToString() + " already exists");
                //}
                //else
                //{
                //    PriorityQ.Add(telegram, true);
                //}
                //PriorityQ.insert(telegram);   

                //#ifdef SHOW_MESSAGING_INFO
                //debug_con << "\nDelayed telegram from " << sender << " recorded at time " 
                //        << TickCounter->GetCurrentFrame() << " for " << receiver
                //        << ". Msg is " << msg << "";
                //#endif
            }
            return isDispatched;
        }

        //public bool DispatchMsgToRelay(float delay,
        //         string senderId,
        //         IMessageRelayComponent receiver,
        //         Telegrams msg,
        //         object ExtraInfo,
        //         params TelegramEarlyTermination[] earlyTermination)
        //{
        //    return DispatchMsg(delay, senderId, receiver.UniqueId, msg, ExtraInfo, earlyTermination);
        //}

        //send a message to all agents.
        //public void DispatchMsgToAll(float delay,
        //                 string senderId,
        //                 Telegrams msg,
        //                 object ExtraInfo,
        //                 params TelegramEarlyTermination[] earlyTermination)
        //{
        //    foreach (var messageHandler in _messageHandlers)
        //    {
        //        DispatchMsg(delay, senderId, messageHandler.Key, msg, ExtraInfo, earlyTermination);
        //    }
        //}

        public Telegram CreateTelegram(float delay,
                         string senderId,
                         string receiverId,
                         string eventName,
                         object ExtraInfo)
        {
            return new Telegram(0, senderId, receiverId, eventName, ExtraInfo);
        }

        //send out any delayed messages. This method is called each time through   
        //the main game loop.
        //  This function dispatches any telegrams with a timestamp that has
        //  expired. Any dispatched telegrams are removed from the queue
        public void DispatchDelayedMessages()
        {
            //RemoveTaggedMessages();
            //first get current time
            float CurrentTime = UnityEngine.Time.time; //TickCounter->GetCurrentFrame(); 

            //List<Telegram> telegramsToRemove = new List<Telegram>();
            //var ListPriorityQ = PriorityQ.Keys.ToList();
            //now peek at the queue to see if any telegrams need dispatching.
            //remove all telegrams from the front of the queue that have gone
            //past their sell by date

            // Iterate through the list
            //var iteration = PriorityQ.First;

            //for (int i = 0; i < ListPriorityQ.Count; i++)
            while (PriorityQ.Count() != 0 && PriorityQ.Peek().DispatchTime < CurrentTime)
            //while (iteration != null)
            //foreach (var item in PriorityQ)
            {
                //var nextIteration = iteration.Next;
                //var telegram = iteration.Value;
                var telegram = PriorityQ.Dequeue();
                //var telegram = ListPriorityQ[i];
                //Telegram telegram = item;
                //if (telegram.DispatchTime < CurrentTime)
                //{
                //if (telegram.DispatchTime > 0)
                //{
                //find the recipient
                //BaseGameEntity pReceiver = EntityController._instance.GetEntity(telegram.ReceiverId); //EntityMgr->GetEntityFromID(telegram.Receiver);

                //send the telegram to the recipient
                //bool isSuccess;
                Discharge(telegram.ReceiverId, telegram);
            }

            // Remove the telegrams marked for deletion
            //foreach (var item in telegramsToRemove)
            //{
            //    PriorityQ.Remove(item);
            //}
        }

        //public void RemoveByEarlyTermination(string receiverId, Enums.TelegramEarlyTermination earlyTermination)
        //{
        //    // Doing it this way only flips through the list once.
        //    var iteration = PriorityQ.First;

        //    while (iteration != null)
        //    {
        //        var telegram = iteration.Value;
        //        if (telegram.ReceiverId == receiverId && telegram.EarlyTermination.Contains(earlyTermination))
        //        {
        //            PriorityQ.Remove(iteration); // This is an O(1) operation. Do NOT remove telegram, that would be an O(n) operation.
        //        }
        //        iteration = iteration.Next;
        //    }
        //    //var telegramsToRemove = PriorityQ.Where(i => i.ReceiverId == receiverId && i.EarlyTermination == earlyTermination);
        //    //foreach (var telegram in telegramsToRemove)
        //    //{
        //    //    PriorityQ.Remove(telegram);
        //    //}
        //}

        //public void RemoveByEarlyTermination(Enums.TelegramEarlyTermination earlyTermination)
        //{
        //    // Doing it this way only flips through the list once.
        //    var iteration = PriorityQ.First;

        //    while (iteration != null)
        //    {
        //        var telegram = iteration.Value;
        //        if (telegram.EarlyTermination.Contains(earlyTermination))
        //        {
        //            PriorityQ.Remove(iteration); // This is an O(1) operation. Do NOT remove telegram, that would be an O(n) operation.
        //        }
        //        iteration = iteration.Next;
        //    }
        //    //var telegramsToRemove = PriorityQ.Where(i => i.ReceiverId == receiverId && i.EarlyTermination == earlyTermination);
        //    //foreach (var telegram in telegramsToRemove)
        //    //{
        //    //    PriorityQ.Remove(telegram);
        //    //}
        //}

        //public void RemoveFromQueue(Func<Telegram, bool> removeQuery)
        //{
        //    var itemsToRemove = PriorityQ.Where(removeQuery);
        //    _itemsToRemove.AddRange(itemsToRemove);
        //    //foreach(var item in itemsToRemove)
        //    //    PriorityQ.Remove(item);
        //}
    }
}
