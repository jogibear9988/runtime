// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Converters;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json
{
    public static partial class JsonSerializer
    {
        /// <summary>
        /// Reads the UTF-8 encoded text representing a single JSON value into a <typeparamref name="TValue"/>.
        /// The Stream will be read to completion.
        /// </summary>
        /// <typeparam name="TValue">The type to deserialize the JSON value into.</typeparam>
        /// <returns>A <typeparamref name="TValue"/> representation of the JSON value.</returns>
        /// <param name="utf8Json">JSON data to parse.</param>
        /// <param name="options">Options to control the behavior during reading.</param>
        /// <param name="cancellationToken">
        /// The <see cref="System.Threading.CancellationToken"/> that can be used to cancel the read operation.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="utf8Json"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="JsonException">
        /// The JSON is invalid,
        /// <typeparamref name="TValue"/> is not compatible with the JSON,
        /// or when there is remaining data in the Stream.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// There is no compatible <see cref="System.Text.Json.Serialization.JsonConverter"/>
        /// for <typeparamref name="TValue"/> or its serializable members.
        /// </exception>
        [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
        [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
        public static ValueTask<TValue?> DeserializeAsync<TValue>(
            Stream utf8Json,
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (utf8Json is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(utf8Json));
            }

            JsonTypeInfo jsonTypeInfo = GetTypeInfo(options, typeof(TValue));
            return ReadFromStreamAsync<TValue>(utf8Json, jsonTypeInfo, cancellationToken);
        }

        /// <summary>
        /// Reads the UTF-8 encoded text representing a single JSON value into a <typeparamref name="TValue"/>.
        /// The Stream will be read to completion.
        /// </summary>
        /// <typeparam name="TValue">The type to deserialize the JSON value into.</typeparam>
        /// <returns>A <typeparamref name="TValue"/> representation of the JSON value.</returns>
        /// <param name="utf8Json">JSON data to parse.</param>
        /// <param name="options">Options to control the behavior during reading.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="utf8Json"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="JsonException">
        /// The JSON is invalid,
        /// <typeparamref name="TValue"/> is not compatible with the JSON,
        /// or when there is remaining data in the Stream.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// There is no compatible <see cref="System.Text.Json.Serialization.JsonConverter"/>
        /// for <typeparamref name="TValue"/> or its serializable members.
        /// </exception>
        [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
        [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
        public static TValue? Deserialize<TValue>(
            Stream utf8Json,
            JsonSerializerOptions? options = null)
        {
            if (utf8Json is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(utf8Json));
            }

            JsonTypeInfo jsonTypeInfo = GetTypeInfo(options, typeof(TValue));
            return ReadFromStream<TValue>(utf8Json, jsonTypeInfo);
        }

        /// <summary>
        /// Reads the UTF-8 encoded text representing a single JSON value into a <paramref name="returnType"/>.
        /// The Stream will be read to completion.
        /// </summary>
        /// <returns>A <paramref name="returnType"/> representation of the JSON value.</returns>
        /// <param name="utf8Json">JSON data to parse.</param>
        /// <param name="returnType">The type of the object to convert to and return.</param>
        /// <param name="options">Options to control the behavior during reading.</param>
        /// <param name="cancellationToken">
        /// The <see cref="System.Threading.CancellationToken"/> that can be used to cancel the read operation.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="utf8Json"/> or <paramref name="returnType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="JsonException">
        /// The JSON is invalid,
        /// the <paramref name="returnType"/> is not compatible with the JSON,
        /// or when there is remaining data in the Stream.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// There is no compatible <see cref="System.Text.Json.Serialization.JsonConverter"/>
        /// for <paramref name="returnType"/> or its serializable members.
        /// </exception>
        [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
        [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
        public static ValueTask<object?> DeserializeAsync(
            Stream utf8Json,
            Type returnType,
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (utf8Json is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(utf8Json));
            }
            if (returnType is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(returnType));
            }

            JsonTypeInfo jsonTypeInfo = GetTypeInfo(options, returnType);
            return ReadFromStreamAsync<object?>(utf8Json, jsonTypeInfo, cancellationToken);
        }

        /// <summary>
        /// Reads the UTF-8 encoded text representing a single JSON value into a <paramref name="returnType"/>.
        /// The Stream will be read to completion.
        /// </summary>
        /// <returns>A <paramref name="returnType"/> representation of the JSON value.</returns>
        /// <param name="utf8Json">JSON data to parse.</param>
        /// <param name="returnType">The type of the object to convert to and return.</param>
        /// <param name="options">Options to control the behavior during reading.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="utf8Json"/> or <paramref name="returnType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="JsonException">
        /// The JSON is invalid,
        /// the <paramref name="returnType"/> is not compatible with the JSON,
        /// or when there is remaining data in the Stream.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// There is no compatible <see cref="System.Text.Json.Serialization.JsonConverter"/>
        /// for <paramref name="returnType"/> or its serializable members.
        /// </exception>
        [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
        [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
        public static object? Deserialize(
            Stream utf8Json,
            Type returnType,
            JsonSerializerOptions? options = null)
        {
            if (utf8Json is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(utf8Json));
            }
            if (returnType is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(returnType));
            }

            JsonTypeInfo jsonTypeInfo = GetTypeInfo(options, returnType);
            return ReadFromStream<object>(utf8Json, jsonTypeInfo);
        }

        /// <summary>
        /// Reads the UTF-8 encoded text representing a single JSON value into a <typeparamref name="TValue"/>.
        /// The Stream will be read to completion.
        /// </summary>
        /// <typeparam name="TValue">The type to deserialize the JSON value into.</typeparam>
        /// <returns>A <typeparamref name="TValue"/> representation of the JSON value.</returns>
        /// <param name="utf8Json">JSON data to parse.</param>
        /// <param name="jsonTypeInfo">Metadata about the type to convert.</param>
        /// <param name="cancellationToken">
        /// The <see cref="System.Threading.CancellationToken"/> that can be used to cancel the read operation.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="utf8Json"/> or <paramref name="jsonTypeInfo"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="JsonException">
        /// The JSON is invalid,
        /// <typeparamref name="TValue"/> is not compatible with the JSON,
        /// or when there is remaining data in the Stream.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// There is no compatible <see cref="System.Text.Json.Serialization.JsonConverter"/>
        /// for <typeparamref name="TValue"/> or its serializable members.
        /// </exception>
        public static ValueTask<TValue?> DeserializeAsync<TValue>(
            Stream utf8Json,
            JsonTypeInfo<TValue> jsonTypeInfo,
            CancellationToken cancellationToken = default)
        {
            if (utf8Json is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(utf8Json));
            }
            if (jsonTypeInfo is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(jsonTypeInfo));
            }

            jsonTypeInfo.EnsureConfigured();
            return ReadFromStreamAsync<TValue>(utf8Json, jsonTypeInfo, cancellationToken);
        }

        /// <summary>
        /// Reads the UTF-8 encoded text representing a single JSON value into a <typeparamref name="TValue"/>.
        /// The Stream will be read to completion.
        /// </summary>
        /// <typeparam name="TValue">The type to deserialize the JSON value into.</typeparam>
        /// <returns>A <typeparamref name="TValue"/> representation of the JSON value.</returns>
        /// <param name="utf8Json">JSON data to parse.</param>
        /// <param name="jsonTypeInfo">Metadata about the type to convert.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="utf8Json"/> or <paramref name="jsonTypeInfo"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="JsonException">
        /// The JSON is invalid,
        /// <typeparamref name="TValue"/> is not compatible with the JSON,
        /// or when there is remaining data in the Stream.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// There is no compatible <see cref="System.Text.Json.Serialization.JsonConverter"/>
        /// for <typeparamref name="TValue"/> or its serializable members.
        /// </exception>
        public static TValue? Deserialize<TValue>(
            Stream utf8Json,
            JsonTypeInfo<TValue> jsonTypeInfo)
        {
            if (utf8Json is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(utf8Json));
            }
            if (jsonTypeInfo is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(jsonTypeInfo));
            }

            jsonTypeInfo.EnsureConfigured();
            return ReadFromStream<TValue>(utf8Json, jsonTypeInfo);
        }

        /// <summary>
        /// Reads the UTF-8 encoded text representing a single JSON value into a <paramref name="returnType"/>.
        /// The Stream will be read to completion.
        /// </summary>
        /// <returns>A <paramref name="returnType"/> representation of the JSON value.</returns>
        /// <param name="utf8Json">JSON data to parse.</param>
        /// <param name="returnType">The type of the object to convert to and return.</param>
        /// <param name="context">A metadata provider for serializable types.</param>
        /// <param name="cancellationToken">
        /// The <see cref="System.Threading.CancellationToken"/> that can be used to cancel the read operation.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="utf8Json"/>, <paramref name="returnType"/>, or <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="JsonException">
        /// The JSON is invalid,
        /// the <paramref name="returnType"/> is not compatible with the JSON,
        /// or when there is remaining data in the Stream.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// There is no compatible <see cref="System.Text.Json.Serialization.JsonConverter"/>
        /// for <paramref name="returnType"/> or its serializable members.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="JsonSerializerContext.GetTypeInfo(Type)"/> method on the provided <paramref name="context"/>
        /// did not return a compatible <see cref="JsonTypeInfo"/> for <paramref name="returnType"/>.
        /// </exception>
        public static ValueTask<object?> DeserializeAsync(
            Stream utf8Json,
            Type returnType,
            JsonSerializerContext context,
            CancellationToken cancellationToken = default)
        {
            if (utf8Json is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(utf8Json));
            }
            if (returnType is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(returnType));
            }
            if (context is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(context));
            }

            JsonTypeInfo jsonTypeInfo = GetTypeInfo(context, returnType);
            return ReadFromStreamAsync<object>(utf8Json, jsonTypeInfo, cancellationToken);
        }

        /// <summary>
        /// Reads the UTF-8 encoded text representing a single JSON value into a <paramref name="returnType"/>.
        /// The Stream will be read to completion.
        /// </summary>
        /// <returns>A <paramref name="returnType"/> representation of the JSON value.</returns>
        /// <param name="utf8Json">JSON data to parse.</param>
        /// <param name="returnType">The type of the object to convert to and return.</param>
        /// <param name="context">A metadata provider for serializable types.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="utf8Json"/>, <paramref name="returnType"/>, or <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="JsonException">
        /// The JSON is invalid,
        /// the <paramref name="returnType"/> is not compatible with the JSON,
        /// or when there is remaining data in the Stream.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// There is no compatible <see cref="System.Text.Json.Serialization.JsonConverter"/>
        /// for <paramref name="returnType"/> or its serializable members.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="JsonSerializerContext.GetTypeInfo(Type)"/> method on the provided <paramref name="context"/>
        /// did not return a compatible <see cref="JsonTypeInfo"/> for <paramref name="returnType"/>.
        /// </exception>
        public static object? Deserialize(
            Stream utf8Json,
            Type returnType,
            JsonSerializerContext context)
        {
            if (utf8Json is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(utf8Json));
            }
            if (returnType is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(returnType));
            }
            if (context is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(context));
            }

            JsonTypeInfo jsonTypeInfo = GetTypeInfo(context, returnType);
            return ReadFromStream<object>(utf8Json, jsonTypeInfo);
        }

        /// <summary>
        /// Wraps the UTF-8 encoded text into an <see cref="IAsyncEnumerable{TValue}" />
        /// that can be used to deserialize root-level JSON arrays in a streaming manner.
        /// </summary>
        /// <typeparam name="TValue">The element type to deserialize asynchronously.</typeparam>
        /// <returns>An <see cref="IAsyncEnumerable{TValue}" /> representation of the provided JSON array.</returns>
        /// <param name="utf8Json">JSON data to parse.</param>
        /// <param name="options">Options to control the behavior during reading.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> that can be used to cancel the read operation.</param>
        /// <returns>An <typeparamref name="TValue"/> representation of the JSON value.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="utf8Json"/> is <see langword="null"/>.
        /// </exception>
        [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
        [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
        public static IAsyncEnumerable<TValue?> DeserializeAsyncEnumerable<TValue>(
            Stream utf8Json,
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (utf8Json is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(utf8Json));
            }

            JsonTypeInfo jsonTypeInfo = GetTypeInfo(options, typeof(TValue));
            return CreateAsyncEnumerableDeserializer(utf8Json, CreateQueueTypeInfo<TValue>(jsonTypeInfo), cancellationToken);
        }

        /// <summary>
        /// Wraps the UTF-8 encoded text into an <see cref="IAsyncEnumerable{TValue}" />
        /// that can be used to deserialize root-level JSON arrays in a streaming manner.
        /// </summary>
        /// <typeparam name="TValue">The element type to deserialize asynchronously.</typeparam>
        /// <returns>An <see cref="IAsyncEnumerable{TValue}" /> representation of the provided JSON array.</returns>
        /// <param name="utf8Json">JSON data to parse.</param>
        /// <param name="jsonTypeInfo">Metadata about the element type to convert.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> that can be used to cancel the read operation.</param>
        /// <returns>An <typeparamref name="TValue"/> representation of the JSON value.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="utf8Json"/> or <paramref name="jsonTypeInfo"/> is <see langword="null"/>.
        /// </exception>
        public static IAsyncEnumerable<TValue?> DeserializeAsyncEnumerable<TValue>(
            Stream utf8Json,
            JsonTypeInfo<TValue> jsonTypeInfo,
            CancellationToken cancellationToken = default)
        {
            if (utf8Json is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(utf8Json));
            }

            if (jsonTypeInfo is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(jsonTypeInfo));
            }

            return CreateAsyncEnumerableDeserializer(utf8Json, CreateQueueTypeInfo<TValue>(jsonTypeInfo), cancellationToken);
        }

        private static JsonTypeInfo<Queue<TValue>> CreateQueueTypeInfo<TValue>(JsonTypeInfo jsonTypeInfo)
        {
            JsonTypeInfo<Queue<TValue>> queueTypeInfo = JsonMetadataServices.CreateQueueInfo<Queue<TValue>, TValue>(
                options: jsonTypeInfo.Options,
                collectionInfo: new()
                {
                    ObjectCreator = static () => new Queue<TValue>(),
                    ElementInfo = jsonTypeInfo,
                    NumberHandling = jsonTypeInfo.Options.NumberHandling
                });

            queueTypeInfo.EnsureConfigured();
            return queueTypeInfo;
        }

        private static async IAsyncEnumerable<TValue> CreateAsyncEnumerableDeserializer<TValue>(
            Stream utf8Json,
            JsonTypeInfo<Queue<TValue>> queueTypeInfo,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Debug.Assert(queueTypeInfo.IsConfigured);
            JsonSerializerOptions options = queueTypeInfo.Options;
            var bufferState = new ReadBufferState(options.DefaultBufferSize);
            ReadStack readStack = new ReadStack
            {
                SupportContinuation = true
            };

            readStack.Initialize(queueTypeInfo);

            var jsonReaderState = new JsonReaderState(options.GetReaderOptions());

            try
            {
                do
                {
                    bufferState = await bufferState.ReadFromStreamAsync(utf8Json, cancellationToken, fillBuffer: false).ConfigureAwait(false);
                    ContinueDeserialize<Queue<TValue>>(
                        ref bufferState,
                        ref jsonReaderState,
                        ref readStack,
                        queueTypeInfo);

                    if (readStack.Current.ReturnValue is Queue<TValue> queue)
                    {
                        while (queue.Count > 0)
                        {
                            yield return queue.Dequeue();
                        }
                    }
                }
                while (!bufferState.IsFinalBlock);
            }
            finally
            {
                bufferState.Dispose();
            }
        }

        internal static async ValueTask<TValue?> ReadFromStreamAsync<TValue>(
            Stream utf8Json,
            JsonTypeInfo jsonTypeInfo,
            CancellationToken cancellationToken)
        {
            Debug.Assert(jsonTypeInfo.IsConfigured);
            JsonSerializerOptions options = jsonTypeInfo.Options;
            var bufferState = new ReadBufferState(options.DefaultBufferSize);
            ReadStack readStack = new ReadStack
            {
                SupportContinuation = true
            };
            readStack.Initialize(jsonTypeInfo);
            var jsonReaderState = new JsonReaderState(options.GetReaderOptions());

            try
            {
                while (true)
                {
                    bufferState = await bufferState.ReadFromStreamAsync(utf8Json, cancellationToken).ConfigureAwait(false);
                    TValue? value = ContinueDeserialize<TValue>(ref bufferState, ref jsonReaderState, ref readStack, jsonTypeInfo);

                    if (bufferState.IsFinalBlock)
                    {
                        return value;
                    }
                }
            }
            finally
            {
                bufferState.Dispose();
            }
        }

        internal static TValue? ReadFromStream<TValue>(
            Stream utf8Json,
            JsonTypeInfo jsonTypeInfo)
        {
            Debug.Assert(jsonTypeInfo.IsConfigured);
            JsonSerializerOptions options = jsonTypeInfo.Options;
            var bufferState = new ReadBufferState(options.DefaultBufferSize);
            ReadStack readStack = new ReadStack
            {
                SupportContinuation = true
            };
            readStack.Initialize(jsonTypeInfo);
            var jsonReaderState = new JsonReaderState(options.GetReaderOptions());

            try
            {
                while (true)
                {
                    bufferState.ReadFromStream(utf8Json);
                    TValue? value = ContinueDeserialize<TValue>(ref bufferState, ref jsonReaderState, ref readStack, jsonTypeInfo);

                    if (bufferState.IsFinalBlock)
                    {
                        return value;
                    }
                }
            }
            finally
            {
                bufferState.Dispose();
            }
        }

        internal static TValue? ContinueDeserialize<TValue>(
            ref ReadBufferState bufferState,
            ref JsonReaderState jsonReaderState,
            ref ReadStack readStack,
            JsonTypeInfo jsonTypeInfo)
        {
            var reader = new Utf8JsonReader(bufferState.Bytes, bufferState.IsFinalBlock, jsonReaderState);

            // If we haven't read in the entire stream's payload we'll need to signify that we want
            // to enable read ahead behaviors to ensure we have complete json objects and arrays
            // ({}, []) when needed. (Notably to successfully parse JsonElement via JsonDocument
            // to assign to object and JsonElement properties in the constructed .NET object.)
            readStack.ReadAhead = !bufferState.IsFinalBlock;
            readStack.BytesConsumed = 0;

            TValue? value = ReadCore<TValue>(ref reader, jsonTypeInfo, ref readStack);
            Debug.Assert(readStack.BytesConsumed <= bufferState.Bytes.Length);
            bufferState.AdvanceBuffer((int)readStack.BytesConsumed);
            jsonReaderState = reader.CurrentState;
            return value;
        }
    }
}
