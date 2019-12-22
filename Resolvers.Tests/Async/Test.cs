using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Resolvers.Tests.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Resolvers.Tests
{
    public class Test
    {
        private readonly ITestOutputHelper _testOutputHelper;


        static MaybeAwaitable<int> Method1()
        {
            return 1.ToJust().ToAsync();
        }

        static MaybeAwaitable<int> Method2()
        {
            return 2.ToJust().ToAsync();
        }

        static MaybeAwaitable<int> Method3()
        {
            return 3.ToJust().ToAsync();
        }

        static MaybeAwaitable<int> Method4_Nth()
        {
            return Maybe<int>.Nothing.ToAsync();
        }

        public Test(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task MaybeTest()
        {
            await Task.Delay(10000);
            var value = await AsyncFact();
            _testOutputHelper.WriteLine(value.ToString());
        }

        [Fact]
        public async MaybeAwaitable<int> AsyncFact()
        {
            await Task.Delay(100000);
            var a = await Method2();
            var b = await Method3();
            return a + b;
        }
    }
}