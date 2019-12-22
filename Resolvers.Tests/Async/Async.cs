using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Resolvers.Tests.Tests;

namespace Resolvers.Tests
{
    namespace Tests
    {
        [AsyncMethodBuilder(typeof(AsyncMaybeBuilder<>))]
        public class MaybeAwaitable<T> : INotifyCompletion
        {
            private Maybe<T>.MaybeForStateBuilder<T> _maybe;

            public MaybeAwaitable(Maybe<T>.MaybeForStateBuilder<T> maybe)
            {
                _maybe = maybe;
            }

            public void OnCompleted(Action continuation)
            {
                if (_maybe)
                {
                    continuation.Invoke();
                }
            }

            public MaybeAwaitable<T> GetAwaiter() => this;

            public bool IsCompleted => false;

            public T GetResult()
            {
                if (IsCompleted)
                    throw new ArgumentException();
                var st = new StackTrace();
                
                return _maybe.GetValue();
            }

            public void SetResult(T t)
            {
                _maybe.SetResult(t);
            }
        }

        public class AsyncMaybeBuilder<T>
        {
            public static AsyncMaybeBuilder<T> Create() => new AsyncMaybeBuilder<T>();

            public AsyncMaybeBuilder()
            {
                Task = new MaybeAwaitable<T>(new Maybe<T>.MaybeForStateBuilder<T>());
            }

            // public static MyAwaitableTaskMethodBuilder<T> Create() 
            // => new MyAwaitableTaskMethodBuilder<T>();

            public void Start<TStateMachine>(ref TStateMachine stateMachine)
                where TStateMachine : IAsyncStateMachine
            {
                stateMachine.MoveNext();
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
            }

            public void SetException(Exception exception)
            {
                throw exception;
            }
            // => this.Task.SetException(exception);

            public void SetResult(T result)
            {
                Task.SetResult(result);
            }
            // => this.Task.SetResult(result);

            public void AwaitOnCompleted<TAwaiter, TStateMachine>(
                ref TAwaiter awaiter,
                ref TStateMachine stateMachine)
                where TAwaiter : INotifyCompletion
                where TStateMachine : IAsyncStateMachine
            {
                GenericAwaitOnCompleted(ref awaiter, ref stateMachine);
            }

            public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
                ref TAwaiter awaiter,
                ref TStateMachine stateMachine)
                where TAwaiter : ICriticalNotifyCompletion
                where TStateMachine : IAsyncStateMachine
            {
                if (stateMachine is AsyncMaybeBuilder)
                {
                    new AsyncTaskMethodBuilder().AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
                    return;
                }

                GenericAwaitOnCompleted(ref awaiter, ref stateMachine);
            }

            public void GenericAwaitOnCompleted<TAwaiter, TStateMachine>(
                ref TAwaiter awaiter,
                ref TStateMachine stateMachine)
                where TAwaiter : INotifyCompletion
                where TStateMachine : IAsyncStateMachine
            {
                
                awaiter.OnCompleted(stateMachine.MoveNext);
            }

            public MaybeAwaitable<T> Task { get; }
        }

        public class Maybe<T>
        {
            private T _value;
            private bool IsNothing;

            public Maybe(T v)
            {
                _value = v;
            }

            protected Maybe()
            {
            }

            public static Maybe<T> Nothing => new Maybe<T>() {IsNothing = true};

            public static implicit operator bool(Maybe<T> maybe)
            {
                return !maybe.IsNothing;
            }

            public class MaybeForStateBuilder<T> : Maybe<T>
            {
                public void SetResult(T result)
                {
                    _value = result;
                }

                public MaybeForStateBuilder()
                {
                    
                }
                public MaybeForStateBuilder(Maybe<T> maybe)
                {
                    if (maybe)
                    {
                        _value = maybe._value;
                    }
                    else
                    {
                        IsNothing = true;
                    }
                }

                public T GetValue()
                {
                    if (this)
                    {
                        return _value;
                    }

                    throw new ArgumentException();
                }
            }
        }
    }

    public static class MaybeExtensions
    {
        public static Maybe<T> ToJust<T>(this T v) => new Maybe<T>(v);

        public static MaybeAwaitable<T> ToAsync<T>(this Maybe<T> v)
        {
            return new MaybeAwaitable<T>(new Maybe<T>.MaybeForStateBuilder<T>(v));
        }
    }
}