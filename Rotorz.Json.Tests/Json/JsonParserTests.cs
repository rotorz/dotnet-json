﻿// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Rotorz.Json.Tests
{
    [TestClass]
    public class JsonParserTests
    {
        #region Parse()

        [TestMethod]
        [DeploymentItem("Json/TestObjects/Files/Empty.json")]
        public void Parse_Stream_EmptyFile()
        {
            // Arrange
            using (var stream = new FileStream("Empty.json", FileMode.Open, FileAccess.Read)) {
                var parser = JsonReader.Create(stream);

                // Act
                var node = parser.Read();

                // Assert
                Assert.IsNull(node);
            }
        }

        [TestMethod]
        [DeploymentItem("Json/TestObjects/Files/JsonParser/PropertyWithMissingValue.json")]
        [ExpectedException(typeof(JsonParserException), "Unexpected end of input; expected value.")]
        public void Parse_Stream_PropertyWithMissingValue()
        {
            // Arrange
            using (var stream = new FileStream("PropertyWithMissingValue.json", FileMode.Open, FileAccess.Read)) {
                var parser = JsonReader.Create(stream);

                // Act
                parser.Read();
            }
        }

        #endregion
    }
}
