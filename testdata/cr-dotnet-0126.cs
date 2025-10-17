using System;
using System.ServiceModel;
using System.Web;
using System.Threading.Tasks;

namespace CloudStatefulMiddlewareExample
{
    // ❌ Violating Example: WCF service using session-based state management
    [ServiceContract(SessionMode = SessionMode.Required)] // ❌ Requires stateful session
    public interface IStatefulWcfService
    {
        [OperationContract(IsInitiating = true)]
        void StartSession();

        [OperationContract]
        void StoreValue(string key, string value);

        [OperationContract(IsTerminating = true)]
        string GetValue(string key);
    }

    // ❌ Violating Example: Implementation relies on in-memory session data
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class StatefulWcfService : IStatefulWcfService
    {
        private readonly System.Collections.Generic.Dictionary<string, string> _sessionData =
            new System.Collections.Generic.Dictionary<string, string>();

        public void StartSession()
        {
            Console.WriteLine("[Violation] Session started for WCF client.");
        }

        public void StoreValue(string key, string value)
        {
            _sessionData[key] = value;
            Console.WriteLine($"[Violation] Stored session value: {key} = {value}");
        }

        public string GetValue(string key)
        {
            _sessionData.TryGetValue(key, out string value);
            Console.WriteLine($"[Violation] Retrieved session value: {key} = {value}");
            return value;
        }
    }

    // ❌ Violating Example: Using IIS session state (sticky session dependency)
    public class StickySessionHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            // Violation: Depends on IIS in-memory session (not distributed)
            context.Session["UserData"] = "User123";
            Console.WriteLine($"[Violation] Using IIS session for state persistence: {context.Session["UserData"]}");
        }

        public bool IsReusable => false;
    }

    // ✅ Compliant Example: Stateless web API using external distributed cache
    public class StatelessApiController
    {
        // ✅ Uses distributed cache or database to store state externally
        private readonly IDistributedCache _cache;

        public StatelessApiController(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SaveStateAsync(string key, string value)
        {
            Console.WriteLine("[Compliant] Saving state in distributed cache...");
            await _cache.SetStringAsync(key, value);
        }

        public async Task<string> GetStateAsync(string key)
        {
            string value = await _cache.GetStringAsync(key);
            Console.WriteLine($"[Compliant] Retrieved state from cache: {value}");
            return value;
        }
    }

    // ✅ Compliant Example: Stateless microservice using message-based communication
    public class StatelessMicroservice
    {
        public async Task HandleMessageAsync(string message)
        {
            // No in-memory or per-session state maintained
            Console.WriteLine($"[Compliant] Processing message: {message}");
            await Task.Delay(200); // Simulate async stateless work
        }
    }

    // Mock distributed cache interface (for demo only)
    public interface IDistributedCache
    {
        Task SetStringAsync(string key, string value);
        Task<string> GetStringAsync(string key);
    }
}
