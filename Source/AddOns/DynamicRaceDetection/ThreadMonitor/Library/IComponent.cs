// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the repo root for full license information.
// ------------------------------------------------------------------------------------------------

using Microsoft.ExtendedReflection.ComponentModel;

namespace Microsoft.PSharp.Monitoring.ComponentModel
{
    /// <summary>
    /// A P# cop component.
    /// </summary>
    internal interface ICopComponent : IComponent
    {
        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>The services.</value>
        new ICopComponentServices Services { get; }
    }
}
