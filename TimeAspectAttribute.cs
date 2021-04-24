using System;
using System.Linq;
using System.Threading.Tasks;
using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace dotnet_api_redis_test.AOP
{
    /// <summary>
    /// 函数调用时间计算
    /// </summary>
    public class TimeAspectAttribute : AbstractInterceptorAttribute
    {
        private readonly Type _type;

        public TimeAspectAttribute(Type type)
        {
            _type = type;
        }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var logger = context.ServiceProvider.Resolve<ILoggerFactory>().CreateLogger(_type);
            var methodName = context.ProxyMethod.Name;
            var sw = Stopwatch.StartNew();
            // 执行代理方法
            await next(context);
            sw.Stop();
            logger.LogInformation($"call {methodName} in {sw.ElapsedMilliseconds} ms");
        }
    }
}
