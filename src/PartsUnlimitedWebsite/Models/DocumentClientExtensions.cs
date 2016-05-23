using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;

namespace PartsUnlimited.Models
{
    public static class DocumentClientExtensions
    {
        /// <summary>
        /// Read all items of type <typeparamref name="T"/> from the document feed identitifed by <paramref name="collectionId"/>
        /// </summary>
        public static async Task<IList<T>> ReadEntireDocumentFeedAsync<T>(this DocumentClient client, Uri collectionId)
        {
            var documents = new List<T>();
            string continuation = null;

            do
            {
                var feedResult = await client.ReadDocumentFeedAsync(collectionId, new FeedOptions { RequestContinuation = continuation });

                foreach (dynamic document in feedResult)
                {
                    // Note: {document} is an instance of Microsoft.Azure.Documents.Document
                    // As a dynamic object it supports casting in order to deserialize into an arbitrary type:
                    documents.Add((T) document);
                }

                continuation = feedResult.ResponseContinuation;
            }
            while (continuation != null);

            return documents;
        }
    }
}