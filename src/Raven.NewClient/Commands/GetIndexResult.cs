//-----------------------------------------------------------------------
// <copyright file="PutResult.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Sparrow.Json;

namespace Raven.NewClient.Client.Commands
{
    public class GetIndexResult
    {
        public BlittableJsonReaderArray Results { get; set; }
    }
}