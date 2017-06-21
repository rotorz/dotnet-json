// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rotorz.Json.Tests.TestObjects;

namespace Rotorz.Json.Serialization.Tests
{
    [TestClass]
    public class MetaTypeTests
    {
        #region FromType(Type)

        [TestMethod]
        public void FromType_CheckCache()
        {
            // Arrange
            var type = typeof(string);

            // Act
            var metaType1 = MetaType.FromType(type);
            var metaType2 = MetaType.FromType(type);

            // Assert
            Assert.IsNotNull(metaType1);
            Assert.AreSame(metaType1, metaType2);
        }

        #endregion


        #region Serialization Callbacks (Single)

        [TestMethod]
        public void Callback_OnSerializing()
        {
            // Arrange
            var instance = new SerializationCallback_OnSerializing();

            // Act
            JsonNode.ConvertFrom(instance);

            // Assert
            Assert.AreEqual("OnSerializing", instance.Result);
        }

        [TestMethod]
        public void Callback_OnSerialized()
        {
            // Arrange
            var instance = new SerializationCallback_OnSerialized();

            // Act
            JsonNode.ConvertFrom(instance);

            // Assert
            Assert.AreEqual("OnSerialized", instance.Result);
        }

        [TestMethod]
        public void Callback_OnDeserializing()
        {
            // Arrange
            var node = new JsonObjectNode();

            // Act
            var instance = node.ConvertTo<SerializationCallback_OnDeserializing>();

            // Assert
            Assert.AreEqual("OnDeserializing", instance.Result);
        }

        [TestMethod]
        public void Callback_OnDeserialized()
        {
            // Arrange
            var node = new JsonObjectNode();

            // Act
            var instance = node.ConvertTo<SerializationCallback_OnDeserialized>();

            // Assert
            Assert.AreEqual("OnDeserialized", instance.Result);
        }

        #endregion


        #region Serialization Callbacks (Multiple)

        [TestMethod]
        public void Callback_OnSerializing_Multiple()
        {
            // Arrange
            var instance = new SerializationCallback_OnSerializing_Multiple();

            // Act
            JsonNode.ConvertFrom(instance);

            // Assert
            Assert.AreEqual("OnSerializing", instance.Result);
            Assert.AreEqual(2, instance.Count);
        }

        [TestMethod]
        public void Callback_OnSerialized_Multiple()
        {
            // Arrange
            var instance = new SerializationCallback_OnSerialized_Multiple();

            // Act
            JsonNode.ConvertFrom(instance);

            // Assert
            Assert.AreEqual("OnSerialized", instance.Result);
            Assert.AreEqual(2, instance.Count);
        }

        [TestMethod]
        public void Callback_OnDeserializing_Multiple()
        {
            // Arrange
            var node = new JsonObjectNode();

            // Act
            var instance = node.ConvertTo<SerializationCallback_OnDeserializing_Multiple>();

            // Assert
            Assert.AreEqual("OnDeserializing", instance.Result);
            Assert.AreEqual(2, instance.Count);
        }

        [TestMethod]
        public void Callback_OnDeserialized_Multiple()
        {
            // Arrange
            var node = new JsonObjectNode();

            // Act
            var instance = node.ConvertTo<SerializationCallback_OnDeserialized_Multiple>();

            // Assert
            Assert.AreEqual("OnDeserialized", instance.Result);
            Assert.AreEqual(2, instance.Count);
        }

        #endregion
    }
}
