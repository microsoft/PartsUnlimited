using System;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using StackExchange.Redis;

namespace PartsUnlimited.Cache
{
    public class RedisTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        private static readonly List<ConnectionFailureType> _transientTypes = new List<ConnectionFailureType>
        {
            ConnectionFailureType.UnableToConnect,
            ConnectionFailureType.Loading,
            ConnectionFailureType.ConnectionDisposed,
            ConnectionFailureType.UnableToConnect
        };

        public bool IsTransient(Exception ex)
        {
            var connectionException = ex as RedisConnectionException;
            if (connectionException != null)
            {
                return _transientTypes.Contains(connectionException.FailureType);
            }

            if (ex is TimeoutException)
            {
                return true;
            }

            return false;
        }
    }
}