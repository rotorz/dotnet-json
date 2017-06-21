// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

namespace Rotorz.Json.Tests.TestObjects
{
    public sealed class PersonCard
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public int Age { get; set; }
        [JsonProperty]
        public int Friends { get; set; }
    }
}
