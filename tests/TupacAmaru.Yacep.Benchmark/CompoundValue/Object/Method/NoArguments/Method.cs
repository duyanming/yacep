﻿using BenchmarkDotNet.Attributes;
using System;
using System.Linq.Expressions;
using System.Reflection;
using TupacAmaru.Yacep.Extensions;

namespace TupacAmaru.Yacep.Benchmark.CompoundValue.Object.Method.NoArguments
{
    public class NoArgumentsMethodBenchmark
    {
        private static readonly FixtureForMethod fixture = new FixtureForMethod();
        private static readonly MethodInfo methodInfo;
        private static readonly IEvaluator evaluator;
        private static readonly IEvaluator<FixtureForMethod> typeEvaluator;
        private static readonly Func<FixtureForMethod, string> reader;
        private static readonly string value;
        static NoArgumentsMethodBenchmark()
        {
            var methodName = "Netyui";
            evaluator = $"this.{methodName}()".Compile();
            evaluator.Evaluate(fixture);
            typeEvaluator = $"this.{methodName}()".Compile<FixtureForMethod>();
            typeEvaluator.Evaluate(fixture);
            var obj = Expression.Parameter(typeof(FixtureForMethod), "fixture");
            methodInfo = typeof(FixtureForMethod).GetMethod(methodName);
            reader = Expression.Lambda<Func<FixtureForMethod, string>>(Expression.Call(obj, methodInfo), "InvokeObjectMethodUseDelegate", new[] { obj }).Compile();
            value = methodName.ToLower();
        }

        [Benchmark]
        public string DirectRead()
        {
            var result = fixture.Netyui();
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
            return result;
        }

        [Benchmark]
        public string UseDelegate()
        {
            var result = reader(fixture);
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
            return result;
        }

        [Benchmark]
        public string UseDynamic()
        {
            dynamic r = fixture;
            var result = r.Netyui();
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
            return result;
        }

        [Benchmark]
        public string UseReflection()
        {
            var result = methodInfo.Invoke(fixture, new object[0]) as string;
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
            return result;
        }

        [Benchmark]
        public string UseYacep()
        {
            var result = evaluator.Evaluate(fixture) as string;
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
            return result;
        }

        [Benchmark]
        public string UseTypedCompile()
        {
            var result = typeEvaluator.Evaluate(fixture) as string;
            if (!string.Equals(value, result, StringComparison.Ordinal))
                throw new Exception($"evaluate failed,result:{result},value:{value}");
            return result;
        }
    }
}
