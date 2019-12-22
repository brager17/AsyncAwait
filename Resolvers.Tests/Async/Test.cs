using System;
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
            var value = await AsyncFact();
        }

        [Fact]
        public async MaybeAwaitable<int> AsyncFact()
        {
            await Task.Delay(10000);
            var o = await Method1();
            var t = await Method2();
            // var nh = await Method4_Nth();
            var th = await Method3();
            _testOutputHelper.WriteLine(th.ToString());
            return th;
        }
    }
}