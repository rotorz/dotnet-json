// Copyright (c) Rotorz Limited. All rights reserved.

namespace Rotorz.Json.Tests.TestObjects {

	internal static class JsonObjectGraphs {

		public static JsonObjectNode CreateSimpleObject() {
			var obj = new JsonObjectNode();
			obj["EvenNumbers"] = CreateNumericArrayWithEvenNumbers();
			obj["Cards"] = CreateIdentityCardArray();
			return obj;
		}

		public static JsonArrayNode CreateNumericArrayWithEvenNumbers() {
			var arr = new JsonArrayNode();
			arr.Add(new JsonIntegerNode(0));
			arr.Add(new JsonIntegerNode(2));
			arr.Add(new JsonIntegerNode(4));
			arr.Add(new JsonIntegerNode(6));
			arr.Add(new JsonIntegerNode(8));
			arr.Add(new JsonIntegerNode(10));
			return arr;
		}

		public static JsonObjectNode CreateIdentityCard(string name) {
			var card = new JsonObjectNode();
			card["Name"] = new JsonStringNode(name);
			card["Age"] = new JsonIntegerNode(22);
			return card;
		}

		public static JsonArrayNode CreateIdentityCardArray() {
			var cards = new JsonArrayNode();
			cards.Add(CreateIdentityCard("Bob"));
			cards.Add(CreateIdentityCard("Jessica"));
			return cards;
		}

	}

}
