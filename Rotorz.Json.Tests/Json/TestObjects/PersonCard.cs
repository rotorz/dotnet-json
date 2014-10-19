// Copyright (c) 2014 Rotorz Limited. All rights reserved.

namespace Rotorz.Json.Tests.TestObjects {

	public sealed class PersonCard {

		[JsonProperty]
		public string Name { get; set; }
		[JsonProperty]
		public int Age { get; set; }
		[JsonProperty]
		public int Friends { get; set; }

	}

}
