using System;
using System.Collections.Generic;

namespace RespServer.Protocol
{
    class RespParser
    {
        class RespState
        {
            public readonly List<object> CurrentMembers;
            public int CurrentMembersRemaining;

            public RespState(int remaining)
            {
                CurrentMembersRemaining = remaining;
                CurrentMembers = new List<object>();
            }

            public int AddMember(object member)
            {
                CurrentMembers.Add(member);
                return --CurrentMembersRemaining;
            }
        }

        readonly Stack<RespState> _stack = new Stack<RespState>();
        private RespState _currentState;
        
        private object MessageHandleInternal(RespPart message)
        {
            object ret = null;
            if (message.Marker.Type == RespMarker.MarkerType.Array)
            {
                if (_currentState != null)
                {
                    _stack.Push(_currentState);
                }
                _currentState = new RespState(message.Marker.Length);
            }
            else
            {
                if (_currentState == null)
                {
                    if (_stack.Count == 0)
                    {
                        if (message.Marker.Type == RespMarker.MarkerType.Error)
                        {
                            return null;
                        }
                        throw new Exception("Must be an array on the outer");
                    }

                    _currentState = _stack.Pop();
                }

                if (_currentState.AddMember(message.DeserializeScalar()) == 0)
                {
                    if (_stack.Count != 0)
                    {
                        int remaining;
                        do
                        {
                            var cs = _currentState;
                            _currentState = _stack.Pop();
                            remaining = _currentState.AddMember(cs.CurrentMembers);
                            if (_stack.Count == 0)
                            {
                                break;
                            }
                        } while (remaining == 0);
                    }
                    if (_stack.Count == 0)
                    {
                        ret = _currentState.CurrentMembers;
                    }
                }
            }
            return ret;
        }

        public List<object> MessageHandle(RespPart message)
        {
            var ret =  MessageHandleInternal(message) as List<object>;
            if (ret != null)
            {
                _currentState = null;
                _stack.Clear();
            }
            return ret;
        }
    }
}
