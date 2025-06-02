using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

using Commons.Resources;
using Commons.ErrorsHandlings;  // GlobalServiceMessage

public class LoggingInterceptor : IInterceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;
    private  string DefaultError = GlobalServiceMessage.UNKNOWN_ERROR;

    // Cached MethodInfos for reflection
    private static readonly MethodInfo _syncFailureFactory = typeof(ServiceResult<>)
        .GetMethod(nameof(ServiceResult<object>.Failure), new[] { typeof(string) })!;
    private static readonly MethodInfo _asyncHelperDef = typeof(LoggingInterceptor)
        .GetMethod(nameof(InterceptAsyncGeneric), BindingFlags.NonPublic | BindingFlags.Instance)!;

    public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
        => _logger = logger;

    public void Intercept(IInvocation invocation)
    {
        var sw = Stopwatch.StartNew();
        var returnType = invocation.Method.ReturnType;
        var name = invocation.Method.Name;

        _logger.LogInformation("→ Entering {Method}", name);

        // 1) sync: ServiceResult<T>
        if (IsSyncServiceResult(returnType))
        {
            try
            {
                invocation.Proceed();
                _logger.LogInformation("√ {Method} succeeded in {Elapsed}ms", name, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "× {Method} threw after {Elapsed}ms", name, sw.ElapsedMilliseconds);
                invocation.ReturnValue = CreateSyncFailure(returnType, DefaultError);
            }
            return;
        }

        // 2) async: Task<ServiceResult<T>>
        if (IsAsyncServiceResult(returnType))
        {
            invocation.Proceed();

            // build Task<ServiceResult<T>> wrapper
            var task = (Task)invocation.ReturnValue!;
            var serviceT = returnType.GetGenericArguments()[0];   // ServiceResult<T>
            var resultType = serviceT.GetGenericArguments()[0];     // T
            var helper = _asyncHelperDef.MakeGenericMethod(resultType);

            invocation.ReturnValue = helper.Invoke(this, new object[] { task, name, sw })!;
            return;
        }

        // 3) non-ServiceResult → pass-through
        invocation.Proceed();
        sw.Stop();
        _logger.LogInformation("→ Exiting {Method} in {Elapsed}ms", name, sw.ElapsedMilliseconds);
    }

    private object CreateSyncFailure(Type serviceResultType, string message)
    {
        // ServiceResult<T>.Failure(message)
        var resultType = serviceResultType.GetGenericArguments()[0];
        var generic = _syncFailureFactory.DeclaringType!.MakeGenericType(resultType);
        var failure = generic.GetMethod(_syncFailureFactory.Name, new[] { typeof(string) })!;
        return failure.Invoke(null, new object[] { message })!;
    }

    private async Task<ServiceResult<T>> InterceptAsyncGeneric<T>(
        Task originalTask, string methodName, Stopwatch sw)
    {
        try
        {
            // cast & await the underlying Task<ServiceResult<T>>
            var typedTask = (Task<ServiceResult<T>>)originalTask;
            var result = await typedTask.ConfigureAwait(false);

            _logger.LogInformation("√ {Method} succeeded in {Elapsed}ms", methodName, sw.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "× {Method} threw after {Elapsed}ms", methodName, sw.ElapsedMilliseconds);
            
            return ServiceResult<T>.Failure(DefaultError);
        }
        finally
        {
            if (sw.IsRunning) sw.Stop();
            _logger.LogInformation("→ Finished {Method} in {Elapsed}ms", methodName, sw.ElapsedMilliseconds);
        }
    }

    private static bool IsSyncServiceResult(Type t) =>
        t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ServiceResult<>);

    private static bool IsAsyncServiceResult(Type t) =>
        typeof(Task).IsAssignableFrom(t)
        && t.IsGenericType
        && t.GetGenericArguments()[0].IsGenericType
        && t.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(ServiceResult<>);
}
