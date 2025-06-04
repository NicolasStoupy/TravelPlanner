using System.Diagnostics;
using System.Reflection;
using Castle.DynamicProxy;
using Commons.ErrorsHandlings;
using Microsoft.Extensions.Logging;

namespace BussinessLogic.Middleware;

/// <summary>
/// Interceptor using Castle DynamicProxy to log method entry, exit, and exceptions for service methods returning ServiceResult or Task<ServiceResult>.
/// </summary>
public class LoggingInterceptor : IInterceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;
    private readonly string _defaultError = "Une erreur inattendue est survenue.";

    // Retrieves MethodInfo for ServiceResult<T>.Failure(string)
    private static readonly MethodInfo _failureMethodDef = typeof(ServiceResult<>)
        .GetMethod(nameof(ServiceResult<object>.Failure), new[] { typeof(string) })!;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingInterceptor"/> class.
    /// </summary>
    /// <param name="logger">The logger used to record method execution details and errors.</param>
    public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Intercepts method calls, logging entry, execution time, and handling exceptions for methods returning ServiceResult or Task<ServiceResult>.
    /// </summary>
    /// <param name="invocation">The invocation information for the intercepted method.</param>
    public void Intercept(IInvocation invocation)
    {
        var sw = Stopwatch.StartNew();
        var methodName = invocation.Method.Name;
        var returnType = invocation.Method.ReturnType;

        _logger.LogInformation("Entering {Method}", methodName);

        // 1) Méthodes asynchrones retournant Task<ServiceResult<T>>
        if (IsAsyncServiceResult(returnType))
        {
            try
            {
                invocation.Proceed(); // invocation.ReturnValue est alors Task<ServiceResult<T>>
                invocation.ReturnValue = WrapAsync((Task)invocation.ReturnValue!, methodName, sw, returnType);
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, " {Method} threw synchronously after {Elapsed}ms", methodName, sw.ElapsedMilliseconds);
                invocation.ReturnValue = CreateFaultedTask(returnType, _defaultError);
            }
            return;
        }

        // 2) Méthodes synchrones retournant ServiceResult<T>
        if (IsSyncServiceResult(returnType))
        {
            try
            {
                invocation.Proceed(); // invocation.ReturnValue est ServiceResult<T>
                sw.Stop();
                _logger.LogInformation("{Method} succeeded in {Elapsed}ms", methodName, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, " {Method} threw after {Elapsed}ms", methodName, sw.ElapsedMilliseconds);
                invocation.ReturnValue = CreateSyncFailure(returnType, _defaultError);
            }
            return;
        }

        try
        {
            invocation.Proceed();
            sw.Stop();
            _logger.LogInformation(" {Method} succeeded in {Elapsed}ms", methodName, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, " {Method} threw after {Elapsed}ms", methodName, sw.ElapsedMilliseconds);
            throw; // on relaie l’exception
        }
    }

    /// <summary>
    /// Wraps an asynchronous Task<ServiceResult> to log completion or exception and return a ServiceResult&lt;T&gt;.
    /// </summary>
    /// <param name="task">The original Task<ServiceResult> to await.</param>
    /// <param name="methodName">The name of the intercepted method.</param>
    /// <param name="sw">The stopwatch tracking execution time.</param>
    /// <param name="returnType">The return type of the intercepted method (Task<ServiceResult>).</param>
    /// <returns>A Task<ServiceResult> that completes with the mapped result or a failure ServiceResult on exception.</returns>
    private object WrapAsync(Task task, string methodName, Stopwatch sw, Type returnType)
    {
        // On génère un appel à HandleAsync<T>(Task<ServiceResult<T>>, string, Stopwatch)
        var serviceResultType = returnType.GetGenericArguments()[0];      // ex. ServiceResult<MyDto>
        var resultType = serviceResultType.GetGenericArguments()[0];      // ex. MyDto
        var helper = typeof(LoggingInterceptor)
            .GetMethod(nameof(HandleAsync), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(resultType);

        return helper.Invoke(this, new object[] { task, methodName, sw })!;
    }

    // HandleAsync<T> attend la Task<ServiceResult<T>>, logue puis retourne un ServiceResult<T>
    private async Task<ServiceResult<T>> HandleAsync<T>(Task originalTask, string methodName, Stopwatch sw)
    {
        try
        {
            var typedTask = (Task<ServiceResult<T>>)originalTask;
            var result = await typedTask.ConfigureAwait(false);
            sw.Stop();
            _logger.LogInformation(" {Method} succeeded in {Elapsed}ms", methodName, sw.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, " {Method} threw after {Elapsed}ms", methodName, sw.ElapsedMilliseconds);
            return ServiceResult<T>.Failure(_defaultError);
        }
    }

    // Crée un Task<ServiceResult<T>> déjà en échec
    private object CreateFaultedTask(Type taskResultType, string message)
    {
        // taskResultType == typeof(Task<ServiceResult<T>>)
        var serviceResultType = taskResultType.GetGenericArguments()[0]; // ServiceResult<T>
        var resultType = serviceResultType.GetGenericArguments()[0];     // T

        // Appel de ServiceResult<T>.Failure(message)
        var genericFailure = _failureMethodDef.DeclaringType!.MakeGenericType(resultType);
        var failureMethod = genericFailure.GetMethod(_failureMethodDef.Name, new[] { typeof(string) })!;
        var failureResult = failureMethod.Invoke(null, new object[] { message });

        // Crée Task.FromResult<ServiceResult<T>>( failureResult )
        var fromResult = typeof(Task)
            .GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(serviceResultType)
            .Invoke(null, new[] { failureResult })!;

        return fromResult;
    }

    // Crée un ServiceResult<T> en échec pour le synchrone
    private object CreateSyncFailure(Type serviceResultType, string message)
    {
        // serviceResultType == typeof(ServiceResult<T>)
        var resultType = serviceResultType.GetGenericArguments()[0];
        var genericFailure = _failureMethodDef.DeclaringType!.MakeGenericType(resultType);
        var failureMethod = genericFailure.GetMethod(_failureMethodDef.Name, new[] { typeof(string) })!;
        return failureMethod.Invoke(null, new object[] { message })!;
    }

    private static bool IsSyncServiceResult(Type t) =>
        t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ServiceResult<>);

    private static bool IsAsyncServiceResult(Type t)
    {
        // true si t hérite de Task<ServiceResult<T>>
        if (!typeof(Task).IsAssignableFrom(t) || !t.IsGenericType) return false;
        var inner = t.GetGenericArguments()[0];
        return inner.IsGenericType && inner.GetGenericTypeDefinition() == typeof(ServiceResult<>);
    }
}