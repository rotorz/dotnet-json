// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System.Runtime.Serialization;

namespace Rotorz.Json.Tests.TestObjects
{
    #region Single

    public sealed class SerializationCallback_OnSerializing
    {
        public string Result;


        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            this.Result = "OnSerializing";
        }
    }


    public sealed class SerializationCallback_OnSerialized
    {
        public string Result;


        [OnSerialized]
        private void OnSerialized(StreamingContext context)
        {
            this.Result = "OnSerialized";
        }
    }


    public sealed class SerializationCallback_OnDeserializing
    {
        public string Result;


        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Result = "OnDeserializing";
        }
    }


    public sealed class SerializationCallback_OnDeserialized
    {
        public string Result;


        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.Result = "OnDeserialized";
        }
    }

    #endregion


    #region Multiple

    public sealed class SerializationCallback_OnSerializing_Multiple
    {
        public string Result;
        public int Count;


        [OnSerializing]
        private void OnSerializing1(StreamingContext context)
        {
            this.Result = "OnSerializing";
            this.Count += 1;
        }

        [OnSerializing]
        private void OnSerializing2(StreamingContext context)
        {
            this.Result = "OnSerializing";
            this.Count += 1;
        }
    }


    public sealed class SerializationCallback_OnSerialized_Multiple
    {
        public string Result;
        public int Count;


        [OnSerialized]
        private void OnSerialized1(StreamingContext context)
        {
            this.Result = "OnSerialized";
            this.Count += 1;
        }

        [OnSerialized]
        private void OnSerialized2(StreamingContext context)
        {
            this.Result = "OnSerialized";
            this.Count += 1;
        }
    }


    public sealed class SerializationCallback_OnDeserializing_Multiple
    {
        public string Result;
        public int Count;


        [OnDeserializing]
        private void OnDeserializing1(StreamingContext context)
        {
            this.Result = "OnDeserializing";
            this.Count += 1;
        }

        [OnDeserializing]
        private void OnDeserializing2(StreamingContext context)
        {
            this.Result = "OnDeserializing";
            this.Count += 1;
        }
    }


    public sealed class SerializationCallback_OnDeserialized_Multiple
    {
        public string Result;
        public int Count;


        [OnDeserialized]
        private void OnDeserialized1(StreamingContext context)
        {
            this.Result = "OnDeserialized";
            this.Count += 1;
        }

        [OnDeserialized]
        private void OnDeserialized2(StreamingContext context)
        {
            this.Result = "OnDeserialized";
            this.Count += 1;
        }
    }

    #endregion
}
