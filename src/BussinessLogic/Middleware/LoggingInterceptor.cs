using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Middleware
{
    public class LoggingInterceptor : IInterceptor
    {
        private readonly ILogger<LoggingInterceptor> _logger;
        public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
            => _logger = logger;

        public void Intercept(IInvocation invocation)
        {
            var sw = Stopwatch.StartNew();
            _logger.LogInformation("Before " + invocation.Method.Name);
            try
            {
                invocation.Proceed();
                _logger.LogInformation("After {Method}", invocation.Method.Name);
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex,
                    "Method {Method} threw after {ElapsedMilliseconds}ms",
                    invocation.Method.Name,
                    sw.ElapsedMilliseconds);
                throw;
            }

            sw.Stop();
            _logger.LogInformation(
                "Finished {Method} in {ElapsedMilliseconds}ms",
                invocation.Method.Name,
                sw.ElapsedMilliseconds);
        }
    }
}
