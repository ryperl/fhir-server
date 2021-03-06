﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Health.Fhir.CosmosDb.Configs;

namespace Microsoft.Health.Fhir.CosmosDb.Features.Storage
{
    /// <summary>
    /// Provides methods for creating a DocumentClient instance and initializing a collection.
    /// </summary>
    public interface IDocumentClientInitializer
    {
        /// <summary>
        /// Creates am unopened <see cref="IDocumentClient"/> based on the given <see cref="CosmosDataStoreConfiguration"/>.
        /// </summary>
        /// <param name="configuration">The endpoint and collection settings</param>
        /// <returns>A <see cref="IDocumentClient"/> instance</returns>
        IDocumentClient CreateDocumentClient(CosmosDataStoreConfiguration configuration);

        /// <summary>
        /// Perform a trivial query to establish a connection.
        /// DocumentClient.OpenAsync() is not supported when a token is used as the access key.
        /// </summary>
        /// <param name="client">The document client</param>
        /// <param name="configuration">The data store config</param>
        Task OpenDocumentClient(IDocumentClient client, CosmosDataStoreConfiguration configuration);

        /// <summary>
        /// Ensures that the necessary database and collection exist with the proper indexing policy and stored procedures
        /// </summary>
        /// <param name="documentClient">The <see cref="IDocumentClient"/> instance to use for initialization.</param>
        /// <param name="cosmosDataStoreConfiguration">The data store configuration.</param>
        /// <returns>A task</returns>
        Task InitializeDataStore(IDocumentClient documentClient, CosmosDataStoreConfiguration cosmosDataStoreConfiguration);
    }
}
