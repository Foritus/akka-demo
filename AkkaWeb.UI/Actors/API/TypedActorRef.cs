using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Util;

namespace AkkaWeb.UI.Actors.API
{
    public class TypedActorRef<TActor> : IActorRef
    {
        public ActorPath Path => _wrappedRef.Path;

        private readonly IActorRef _wrappedRef;

        public TypedActorRef(IActorRef wrappedRef)
        {
            _wrappedRef = wrappedRef ?? throw new ArgumentNullException(nameof(wrappedRef));
        }

        public async Task<TResponse> Ask<TResponse>(object message)
        {
            return (TResponse)(await _wrappedRef.Ask(message));
        }

        public int CompareTo(IActorRef other)
        {
            return _wrappedRef.CompareTo(other);
        }

        public int CompareTo(object obj)
        {
            return _wrappedRef.CompareTo(obj);
        }

        public bool Equals(IActorRef other)
        {
            return _wrappedRef.Equals(other);
        }

        public void Tell(object message)
        {
            Tell(message, null);
        }

        public void Tell(object message, IActorRef sender)
        {
            _wrappedRef.Tell(message);
        }

        public ISurrogate ToSurrogate(ActorSystem system)
        {
            return _wrappedRef.ToSurrogate(system);
        }
    }
}
