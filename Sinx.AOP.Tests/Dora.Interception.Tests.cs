using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dora.DynamicProxy;
using Dora.Interception;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Sinx.AOP.Tests
{
    public class Dora_Interception_Tests
    {
        private readonly IServiceProvider _sp;
        public Dora_Interception_Tests()
        {
            var sp = new ServiceCollection()
                .AddInterception()
                .AddSingleton<IFoo, Foo>()
                .BuildInterceptableServiceProvider();
            _sp = sp;
        }

        [Fact]
        public async Task Dora_Interception_Start()
        {
            var foo = _sp.GetRequiredService<IFoo>();
            var r = await foo.GetAsync();
            var dt = DateTimeOffset.Parse(Environment.GetEnvironmentVariable("Dora_Interception_Tests"));
            Assert.Equal(1, r);
            Assert.True(DateTimeOffset.Now - dt < TimeSpan.FromSeconds(2));
        }

        private interface IFoo
        {
            Task<int> GetAsync();
        }

        private class Foo : IFoo
        {
            public Task<int> GetAsync()
            {
                return Task.FromResult(1);
            }
        }

        private class DecorateAttribute : InterceptorAttribute
        {
            public override void Use(IInterceptorChainBuilder builder)
            {
                builder.Use<Decorate>(Order);
            }
        }

        private class Decorate
        {
            private readonly InterceptDelegate _next;
            public Decorate(InterceptDelegate next)
            {
                _next = next;
            }

            public Task InvokeAsync(InvocationContext context)
            {
                Environment.SetEnvironmentVariable("Dora_Interception_Tests", DateTimeOffset.Now.ToString());
                return _next(context);
            }
        }
    }
}
