using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Resolvers.Profiles;
using Resolvers.Resolver;
using Xunit;

namespace Resolvers.Tests
{
    public interface IBase
    {
        int X { get; }
    }

    public class Derive : IBase
    {
        public int X { get; set; }
    }

    public class CustomDictionary<TKey, TValue> where TKey : struct
    {
        public TKey CustomKeyField;

        public TValue GetValue(TKey key)
        {
            /// какой-то предикат
            var predicate = key.GetHashCode() % 2 == 0;
            TValue @if = default;
            TValue @else = default;
            if (predicate)
            {
                return @if;
            }

            return @else;
        }
    }

    public class UnitTest1
    {
        public CustomDictionary<int, int> d = new CustomDictionary<int, int>();
        public void WithoutRef(Pupil p) => p = null;
        public void WithRef(ref Pupil p) => p = null;
        public void WithOut(out Pupil p) => p = null;

        [Fact]
        public void WithoutRefTest()
        {
            var p = new Pupil();
            WithoutRef(p);
            Assert.NotNull(p);
        }

        [Fact]
        public void WithRefTest()
        {
            var p = new Pupil();
            WithRef(ref p);
            Assert.Null(p);
        }

        private int A = 2;

        [Fact]
        public void Test()
        {
            String s = "a\u0304\u0308bc\u0327";
            var iterator = StringInfo.GetTextElementEnumerator(s);
            while (iterator.MoveNext())
            {
                File.AppendAllText("output.txt", $"{iterator.ElementIndex} {iterator.Current}\n\n\n");
            }
        }
    }

    public class Derived : Base, IDisposable
    {
        public void DerivedMethod()
        {
        }

        public new void Dispose()
        {
            Console.WriteLine("Dispose");
        }

        public override void VirtualMethod()
        {
            Console.WriteLine("Virtual Method");
        }
    }

    public class Base : BaseA, IDisposable
    {
        public void BaseMethod()
        {
            Console.WriteLine();
        }

        public void Dispose()
        {
            Console.WriteLine("Base");
        }

        public void Method123()
        {
        }

        public virtual void VirtualMethod()
        {
            Console.WriteLine("Virtual Method");
        }
    }

    public class BaseA
    {
        public void Dispose()
        {
            Console.WriteLine("Base");
        }
    }
}