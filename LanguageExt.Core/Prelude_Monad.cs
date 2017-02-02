﻿using LanguageExt.ClassInstances;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Reader Writer State monad constructor
        /// </summary>
        /// <returns>Rws monad</returns>
        [Pure]
        public static Rws<Env, Out, S, T> Rws<Env, Out, S, T>(T value) => rws =>
              new RwsResult<Out, S, T>(new Out[0], rws.Item2, value);

        /// <summary>
        /// RWS monad 'tell'
        /// Adds an item to the writer's output
        /// </summary>
        /// <returns>Rws monad</returns>
        [Pure]
        public static Rws<Env, Out, S, Unit> tell<Env, Out, S>(Out value) =>
            rws => new RwsResult<Out, S, Unit>(new Out[1] { value }, rws.Item2, unit);

        /// <summary>
        /// RWS monad 'ask'
        /// Gets the 'environment' so it can be used 
        /// </summary>
        /// <returns>Rws monad with the environment in as the wrapped value</returns>
        [Pure]
        public static Rws<Env, Out, S, Env> ask<Env, Out, S>() =>
            rws => new RwsResult<Out, S, Env>(new Out[0], rws.Item2, rws.Item1);

        /// <summary>
        /// RWS monad 'ask'
        /// Gets the 'environment' and maps it
        /// </summary>
        /// <returns>Rws monad with the mapped environment in as the wrapped value</returns>
        [Pure]
        public static Rws<Env, Out, S, Ret> ask<Env, Out, S, Ret>(Func<Env, Ret> map) =>
            rws => new RwsResult<Out, S, Ret>(new Out[0], rws.Item2, map(rws.Item1));

        /// <summary>
        /// Get the state from monad into its wrapped value (RWS)
        /// </summary>
        /// <returns>Rws monad with state in the value</returns>
        [Pure]
        public static Rws<Env, Out, S, S> get<Env, Out, S>() =>
            rws => new RwsResult<Out, S, S>(new Out[0], rws.Item2, rws.Item2);

        /// <summary>
        /// Set the state (RWS)
        /// </summary>
        /// <returns>Rws monad with state set and with a Unit value</returns>
        [Pure]
        public static Rws<Env, Out, S, Unit> put<Env,Out,S>(S state) =>
            _ => new RwsResult<Out, S, Unit>(new Out[0], state, unit);

        /// <summary>
        /// Writer monad constructor
        /// </summary>
        /// <typeparam name="Out">Writer output</typeparam>
        /// <typeparam name="T">Wrapped type</typeparam>
        /// <param name="value">Wrapped value</param>
        /// <returns>Writer monad</returns>
        [Pure]
        public static Writer<Out, T> Writer<Out, T>(T value) => () =>
              new WriterResult<Out, T>(value, new Out[0]);

        /// <summary>
        /// Writer monad constructor
        /// </summary>
        /// <typeparam name="Out">Writer output</typeparam>
        /// <typeparam name="T">Wrapped type</typeparam>
        /// <param name="value">Wrapped value</param>
        /// <param name="ws">Writer log</param>
        /// <returns>Writer monad</returns>
        [Pure]
        public static Writer<Out, T> Writer<Out, T>(T value, IEnumerable<Out> ws) => () =>
              new WriterResult<Out, T>(value, ws);

        /// <summary>
        /// Writer monad 'tell'
        /// Adds an item to the writer's output
        /// </summary>
        /// <typeparam name="W"></typeparam>
        /// <param name="value"></param>
        /// <returns>Writer monad</returns>
        [Pure]
        public static Writer<Out, Unit> tell<Out>(Out value) => () => 
            new WriterResult<Out, Unit>(unit, new Out[1] { value });

        [Pure]
        public static Writer<Out, T> trywrite<Out, T>(Func<Writer<Out, T>> tryDel) => () =>
        {
            try
            {
                return (from x in tryDel()
                        select x)();
            }
            catch
            {
                return new WriterResult<Out, T>(default(T), new Out[0], true);
            }
        };

        [Pure]
        public static Try<Reader<Env, T>> tryfun<Env, T>(Func<Reader<Env, T>> tryDel) => () => 
            from x in tryDel()
            select x;

        [Pure]
        public static Try<State<S, T>> tryfun<S, T>(Func<State<S, T>> tryDel) => () => 
            from x in tryDel()
            select x;

        [Pure]
        public static Try<Writer<Out, T>> tryfun<Out, T>(Func<Writer<Out, T>> tryDel) => () => 
            from x in tryDel()
            select x;
    }
}
