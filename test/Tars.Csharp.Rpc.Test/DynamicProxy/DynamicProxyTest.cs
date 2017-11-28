using System;
using System.Threading.Tasks;
using Tars.Csharp.Rpc.DynamicProxy;
using Xunit;

namespace Tars.Csharp.Rpc.Test.DynamicProxy
{
    public class DynamicProxyTest
    {
        private static IDynamicProxyGenerator generator = new DynamicProxyGenerator();

        [Fact]
        public void DynamicProxyCallHelloStringShouldNoError()
        {
            var context = new DynamicProxyContext()
            {
                Invoke = (c, s, ps) => $"{s}({ps[0]},{ps[1]})"
            };
            var proxy = generator.CreateInterfaceProxy<IHelloRpc>(context);
            var str = proxy.HelloString(3, "Vic");
            Assert.Equal("HelloString(3,Vic)", str);
        }

        [Fact]
        public void DynamicProxyCallHelloVoidShouldNoError()
        {
            var context = new DynamicProxyContext()
            {
                Invoke = (c, s, ps) => null
            };
            var proxy = generator.CreateInterfaceProxy<IHelloRpc>(context);
            proxy.HelloVoid(3, "Vic");
        }

        [Fact]
        public async void DynamicProxyCallHelloTaskShouldNoError()
        {
            var context = new DynamicProxyContext()
            {
                Invoke = (c, s, ps) => Task.FromResult($"{s}({ps[0]},{ps[1]})")
            };
            var proxy = generator.CreateInterfaceProxy<IHelloRpc>(context);
            var str = await proxy.HelloTask(3, "Vic");
            Assert.Equal("HelloTask(3,Vic)", str);
        }

        [Fact]
        public async void DynamicProxyCallHelloValueTaskShouldNoError()
        {
            var context = new DynamicProxyContext()
            {
                Invoke = (c, s, ps) => Task.FromResult($"{s}({ps[0]},{ps[1]},{ps[2]})")
            };
            var proxy = generator.CreateInterfaceProxy<IHelloRpc>(context);
            var str = await proxy.HelloValueTask(3, "Vic", context);
            Assert.Equal("HelloValueTask(3,Vic,Tars.Csharp.Rpc.DynamicProxy.DynamicProxyContext)", str);
        }

        [Fact]
        public void DynamicProxyCallNullShouldNull()
        {
            var context = new DynamicProxyContext();
            var proxy = generator.CreateInterfaceProxy<IHelloRpc>(context);
            var str = proxy.HelloString(3, "Vic");
            Assert.Null(str);
        }

        [Fact]
        public async void DynamicProxyCallNullTaskShouldNull()
        {
            try
            {
                var context = new DynamicProxyContext();
                var proxy = generator.CreateInterfaceProxy<IHelloRpc>(context);
                var str = await proxy.HelloTask(3, "Vic");
            }
            catch (NullReferenceException e)
            {
                Assert.NotNull(e);
            }
        }
    }

    public interface IHelloRpc
    {
        string HelloString(int no, string name);

        void HelloVoid(int no, string name);

        Task<string> HelloTask(int no, string name);

        ValueTask<string> HelloValueTask(int no, string name, DynamicProxyContext context);
    }
}