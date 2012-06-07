﻿#region (c) 2012-2012 Lokad - New BSD License 

// Copyright (c) Lokad 2012-2012, http://www.lokad.com
// This code is released as Open Source under the terms of the New BSD Licence

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace lokad_iddd_sample
{
    public interface IEventStore
    {
        EventStream LoadEventStream(IIdentity id);
        EventStream LoadEventStreamAfterVersion(IIdentity id, int version);

        void AppendToStream(IIdentity id, int expectedVersion,
            ICollection<IEvent> events);
    }

    public class EventStream
    {
        // version of the event stream returned
        public int Version;
        // all events in the stream
        public IList<IEvent> Events = new List<IEvent>();

        public static readonly EventStream Empty = new EventStream
            {
                Events = new List<IEvent>()
            };
    }

    public interface IEvent {}

    public interface ICommand {}

    public interface IIdentity {}

    public interface IEventStoreStrategy
    {
        byte[] SerializeEvent(IEvent e);
        IEvent DeserializeEvent(byte[] data);
        string IdentityToString(IIdentity id);
    }


    [Serializable]
    public class EventStoreConcurrencyException : Exception
    {

        public int ActualVersion { get; private set; }
        public int ExpectedVersion { get; private set; }
        public string EventStreamName { get; private set; }


        public EventStoreConcurrencyException(string message, int actualVersion, int expectedVersion, string eventStreamName) : base(message)
        {
            ActualVersion = actualVersion;
            ExpectedVersion = expectedVersion;
            EventStreamName = eventStreamName;
        }

        public static EventStoreConcurrencyException Create(int actual, int expected, string name)
        {
            var message = string.Format("Expected v{0} but found v{1} in stream '{2}'", expected, actual, name);
            return new EventStoreConcurrencyException(message,actual, expected, name);
        }

        protected EventStoreConcurrencyException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) {}
    }
}