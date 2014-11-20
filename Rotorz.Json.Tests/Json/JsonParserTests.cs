// Copyright (c) 2014 Rotorz Limited. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Rotorz.Json.Tests {

	[TestClass]
	public class JsonParserTests {

		#region Parse()

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/Empty.json")]
		public void Parse_Stream_EmptyFile() {
			// Arrange
			using (var stream = new FileStream("Empty.json", FileMode.Open, FileAccess.Read)) {
				var parser = JsonParser.Create(stream);

				// Act
				var node = parser.Parse();

				// Assert
				Assert.IsNull(node);
			}
		}

		[TestMethod]
		[DeploymentItem("Json/TestObjects/Files/JsonParser/PropertyWithMissingValue.json")]
		[ExpectedException(typeof(JsonParserException), "Unexpected end of input; expected value.")]
		public void Parse_Stream_PropertyWithMissingValue() {
			// Arrange
			using (var stream = new FileStream("PropertyWithMissingValue.json", FileMode.Open, FileAccess.Read)) {
				var parser = JsonParser.Create(stream);
				
				// Act
				parser.Parse();
			}
		}

		#endregion

	}

}
