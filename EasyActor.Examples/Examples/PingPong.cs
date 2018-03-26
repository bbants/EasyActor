﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace EasyActor.Examples
{

    public class PingPong 
    {
        private readonly ITestOutputHelper _Output;

        public PingPong(ITestOutputHelper output = null)
        {
            _Output = output;
        }

        private static readonly FactoryBuilder _FactoryBuilder = new FactoryBuilder();

        public static IEnumerable<object[]> GetFactories()
        {
            yield return new object[] { _FactoryBuilder.GetFactory() };
            yield return new object[] { _FactoryBuilder.GetFactory(shared:true) };
            yield return new object[] { _FactoryBuilder.GetTaskBasedFactory() };
            yield return new object[] { _FactoryBuilder.GetThreadPoolFactory() };        
        }

        [Theory]
        [MemberData(nameof(GetFactories))]
        public async Task Test(IActorFactory fact)
        {
            Output(fact.Type.ToString());

            var one = new PingPongerAsync("Bjorg");
            var actor1 = fact.Build<IPingPongerAsync>(one);

            var two = new PingPongerAsync("Lendl");
            var actor2 = fact.Build<IPingPongerAsync>(two);

            one.PongerAsync = actor2;
            two.PongerAsync = actor1;

            var watch = Stopwatch.StartNew();

            await actor1.Ping();
            Thread.Sleep(10000);

            await fact.DisposeAsync();

            watch.Stop();

            Output($"Total Ping:{one.Count}, Total Pong:{two.Count} Total Time: {watch.ElapsedMilliseconds} ms");
            Output($"Operation/ms:{(one.Count + two.Count) / watch.ElapsedMilliseconds}");
        }

        [Theory]
        [MemberData(nameof(GetFactories))]
        public async Task TestNoTask(IActorFactory fact)
        {
            Output(fact.Type.ToString());

            var one = new PingPongerSimple("Bjorg");
            var actor1 = fact.Build<IPingPonger>(one);

            var two = new PingPongerSimple("Lendl");
            var actor2 = fact.Build<IPingPonger>(two);

            one.Ponger = actor2;
            two.Ponger = actor1;

            var watch = Stopwatch.StartNew();

            actor1.Ping();
            Thread.Sleep(10000);

            await fact.DisposeAsync();
            watch.Stop();

            Output($"Total Ping:{one.Count}, Total Pong:{two.Count} Total Time: {watch.ElapsedMilliseconds} ms");
            Output($"Operation/ms:{(one.Count + two.Count) / watch.ElapsedMilliseconds}");
        }

        private void Output(string message)
        {
            Console.WriteLine(message);
            _Output?.WriteLine(message);
        }
    }
}
