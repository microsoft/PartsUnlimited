using System.Threading;

namespace PartsUnlimited.Models
{
    public class Request
    {
        public CancellationToken CancellationToken { get; set; }
        public int Id { get; set; }
    }
}
