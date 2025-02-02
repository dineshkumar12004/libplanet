using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Blockchain.Renderers;
using Libplanet.Blockchain.Renderers.Debug;
using Libplanet.Blocks;
using Libplanet.Crypto;
using Libplanet.Net.Protocols;
using Libplanet.Store;
using Libplanet.Tx;
using Xunit;
using static Libplanet.Blockchain.KeyConverters;
using Random = System.Random;

namespace Libplanet.Tests
{
    public static class TestUtils
    {
        public static readonly Address GenesisMinerAddress =
            new Address("21744f4f08db23e044178dafb8273aeb5ebe6644");

        // These keys are used to generate a situation that two adjacent peers generated by these
        // keys are in different buckets in routing table of Kademlia Protocol.
        public static readonly PrivateKey[] AdjacentKeys = new PrivateKey[10]
        {
            new PrivateKey(new byte[]
            {
                0x98, 0x66, 0x98, 0x50, 0x72, 0x8c, 0x6c, 0x41, 0x0b, 0xf4,
                0x2c, 0x45, 0xfe, 0x7c, 0x49, 0x23, 0x2d, 0x14, 0xcf, 0xb5,
                0x5b, 0x78, 0x4d, 0x81, 0x35, 0xae, 0x40, 0x4c, 0x7c, 0x24,
                0x3f, 0xc7,
            }),
            new PrivateKey(new byte[]
            {
                0xd2, 0x47, 0x6f, 0xf3, 0x1a, 0xf3, 0x4f, 0x00, 0x5a, 0xe2,
                0xd9, 0x24, 0x18, 0x60, 0xe9, 0xb9, 0xd0, 0x42, 0x9a, 0x30,
                0x67, 0x81, 0x2b, 0x00, 0xf0, 0x45, 0x87, 0x70, 0x3f, 0xd5,
                0x51, 0x93,
            }),
            new PrivateKey(new byte[]
            {
                0x9e, 0xd4, 0xdb, 0x20, 0xfd, 0x4d, 0x1c, 0x52, 0x55, 0x24,
                0x80, 0x52, 0xc6, 0x1f, 0x95, 0x1c, 0xf1, 0x49, 0x4a, 0xd6,
                0xf9, 0x1d, 0x29, 0xb9, 0xa3, 0x0b, 0x0e, 0x0c, 0xc8, 0xaa,
                0xb0, 0x79,
            }),
            new PrivateKey(new byte[]
            {
                0x0a, 0x4f, 0x84, 0xeb, 0x69, 0x4d, 0xc1, 0xf0, 0xf3, 0x15,
                0x97, 0xcc, 0x95, 0x53, 0x66, 0x01, 0x27, 0x2a, 0xc1, 0xcd,
                0x0f, 0xf6, 0x02, 0x6f, 0x08, 0x29, 0x1d, 0xd0, 0x79, 0xda,
                0xcc, 0x36,
            }),
            new PrivateKey(new byte[]
            {
                0x68, 0xbd, 0xc3, 0xda, 0xf1, 0xa1, 0x67, 0x9c, 0xa1, 0x1e,
                0x5a, 0x64, 0x10, 0xe6, 0x74, 0x95, 0x77, 0xbc, 0x47, 0x1c,
                0x55, 0xd7, 0x38, 0xa3, 0x67, 0x48, 0x73, 0x08, 0xcd, 0x74,
                0x3c, 0x4b,
            }),
            new PrivateKey(new byte[]
            {
                0x02, 0x40, 0xa6, 0x72, 0xdd, 0xc0, 0x65, 0x04, 0x54, 0xfb,
                0x34, 0x29, 0x05, 0xaa, 0xa6, 0x1e, 0x94, 0x30, 0x89, 0x26,
                0xfd, 0x30, 0xd1, 0x61, 0x8c, 0x1b, 0x75, 0x79, 0x86, 0xf8,
                0x8a, 0x6a,
            }),
            new PrivateKey(new byte[]
            {
                0x74, 0x6d, 0x07, 0xb0, 0xb9, 0x7e, 0x0d, 0xb9, 0x1f, 0x96,
                0x59, 0xe1, 0x20, 0x8d, 0x31, 0xac, 0x94, 0xcd, 0xc8, 0xaa,
                0x0c, 0x0d, 0xeb, 0x35, 0xab, 0x93, 0x95, 0x65, 0xae, 0x5f,
                0xc1, 0x4b,
            }),
            new PrivateKey(new byte[]
            {
                0xc5, 0x1e, 0xc4, 0x6c, 0x81, 0x6a, 0x9d, 0x41, 0xc2, 0xae,
                0x61, 0x51, 0x0f, 0x97, 0xd0, 0x0e, 0x3a, 0x7b, 0x86, 0xd3,
                0xbd, 0xf1, 0x1e, 0xfe, 0x55, 0x67, 0x8f, 0x31, 0x92, 0xc1,
                0xdf, 0xe4,
            }),
            new PrivateKey(new byte[]
            {
                0x0a, 0x9e, 0x67, 0x59, 0x4b, 0xfc, 0xdd, 0x81, 0xcd, 0x86,
                0xa0, 0xf1, 0x79, 0x74, 0x56, 0x0f, 0x56, 0x85, 0xff, 0x3f,
                0x75, 0xd3, 0xbc, 0xaa, 0xf0, 0xa2, 0xec, 0xdb, 0x05, 0xa5,
                0x59, 0x79,
            }),
            new PrivateKey(new byte[]
            {
                0x3d, 0xae, 0x2d, 0x2f, 0x87, 0x1b, 0x11, 0xaa, 0x41, 0xbd,
                0xec, 0x81, 0x4c, 0x4d, 0x27, 0xf3, 0xba, 0xd9, 0x1f, 0x61,
                0xc3, 0x57, 0xab, 0x43, 0xa0, 0x0c, 0x63, 0x1b, 0x2b, 0x15,
                0x13, 0xf2,
            }),
        };

        private static readonly Random _random = new Random();

        public static void AssertBytesEqual(byte[] expected, byte[] actual)
        {
            string msg;
            if (expected.LongLength < 1024 && actual.LongLength < 1024 &&
                expected.All(b => b < 0x80) && actual.All(b => b < 0x80))
            {
                // If both arrays can be ASCII encoding, print them directly.
                string expectedStr = Encoding.ASCII.GetString(expected);
                string actualStr = Encoding.ASCII.GetString(actual);
                msg = $@"Two byte arrays do not equal
Expected: ({expected.LongLength}) {expectedStr}
Actual:   ({actual.LongLength}) {actualStr}";
            }
            else
            {
                string expectedRepr = Repr(expected);
                string actualRepr = Repr(actual);
                msg = $@"Two byte arrays do not equal
Expected: new byte[{expected.LongLength}] {{ {expectedRepr} }}
Actual:   new byte[{actual.LongLength}] {{ {actualRepr} }}";
            }

            Assert.True(expected.SequenceEqual(actual), msg);

            string Repr(byte[] bytes)
            {
                const int limit = 1024;
                if (bytes.LongLength > limit)
                {
                    bytes = bytes.Take(limit).ToArray();
                }

                string s = string.Join(
                    ", ",
                    bytes.Select(b => b < 0x10 ? $"0x0{b:x}" : $"0x{b:x}")
                );
                return bytes.LongLength > limit ? $"{s}, ..." : s;
            }
        }

        public static void AssertBytesEqual(
            ImmutableArray<byte> expected,
            ImmutableArray<byte> actual
        )
        {
            AssertBytesEqual(expected.ToArray(), actual.ToArray());
        }

        public static void AssertBytesEqual(TxId expected, TxId actual)
        {
            AssertBytesEqual(expected.ToByteArray(), actual.ToByteArray());
        }

        public static void AssertBytesEqual(BlockHash expected, BlockHash actual) =>
            AssertBytesEqual(expected.ToByteArray(), actual.ToByteArray());

        public static void AssertBytesEqual<T>(
            HashDigest<T> expected,
            HashDigest<T> actual
        )
            where T : HashAlgorithm
        {
            AssertBytesEqual(expected.ToByteArray(), actual.ToByteArray());
        }

        public static byte[] GetRandomBytes(int size)
        {
            var bytes = new byte[size];
            _random.NextBytes(bytes);

            return bytes;
        }

        public static Block<T> MineGenesis<T>(
            HashAlgorithmGetter hashAlgorithmGetter,
            Address? miner = null,
            IReadOnlyList<Transaction<T>> transactions = null,
            DateTimeOffset? timestamp = null,
            int protocolVersion = Block<T>.CurrentProtocolVersion
        )
            where T : IAction, new()
        {
            if (transactions is null)
            {
                transactions = new List<Transaction<T>>();
            }

            var block = new Block<T>(
                index: 0,
                difficulty: 0,
                totalDifficulty: 0,
                nonce: new Nonce(new byte[] { 0x01, 0x00, 0x00, 0x00 }),
                miner: miner ?? GenesisMinerAddress,
                previousHash: null,
                timestamp: timestamp ?? new DateTimeOffset(2018, 11, 29, 0, 0, 0, TimeSpan.Zero),
                transactions: transactions,
                hashAlgorithm: hashAlgorithmGetter(0),
                protocolVersion: protocolVersion
            );

            return block;
        }

        public static Block<T> MineNext<T>(
            Block<T> previousBlock,
            HashAlgorithmGetter hashAlgorithmGetter,
            IReadOnlyList<Transaction<T>> txs = null,
            byte[] nonce = null,
            long difficulty = 1,
            Address? miner = null,
            TimeSpan? blockInterval = null,
            int protocolVersion = Block<T>.CurrentProtocolVersion
        )
            where T : IAction, new()
        {
            if (txs == null)
            {
                txs = new List<Transaction<T>>();
            }

            long index = previousBlock.Index + 1;
            BlockHash previousHash = previousBlock.Hash;
            DateTimeOffset timestamp =
                previousBlock.Timestamp.Add(blockInterval ?? TimeSpan.FromSeconds(15));

            Block<T> block;
            if (nonce == null)
            {
                block = Block<T>.Mine(
                    index: index,
                    hashAlgorithm: hashAlgorithmGetter(index),
                    difficulty: difficulty,
                    previousTotalDifficulty: previousBlock.TotalDifficulty,
                    miner: miner ?? previousBlock.Miner.Value,
                    previousHash: previousHash,
                    timestamp: timestamp,
                    transactions: txs,
                    protocolVersion: protocolVersion
                );
            }
            else
            {
                block = new Block<T>(
                    index: index,
                    difficulty: difficulty,
                    totalDifficulty: previousBlock.TotalDifficulty + difficulty,
                    nonce: new Nonce(nonce),
                    miner: miner ?? previousBlock.Miner.Value,
                    previousHash: previousHash,
                    timestamp: timestamp,
                    transactions: txs,
                    hashAlgorithm: hashAlgorithmGetter(index),
                    protocolVersion: protocolVersion
                );
            }

            block.Validate(hashAlgorithmGetter(index), DateTimeOffset.Now);

            return block;
        }

        public static Block<T> AttachStateRootHash<T>(
            this Block<T> block,
            IStateStore stateStore,
            IBlockPolicy<T> policy
        )
            where T : IAction, new() =>
                AttachStateRootHash(block, policy.GetHashAlgorithm, stateStore, policy.BlockAction);

        public static Block<T> AttachStateRootHash<T>(
            this Block<T> block,
            HashAlgorithmType hashAlgorithm,
            IStateStore stateStore,
            IAction blockAction
        )
            where T : IAction, new() =>
                AttachStateRootHash(block, _ => hashAlgorithm, stateStore, blockAction);

        public static string ToString(BitArray bitArray)
        {
            return new string(
                bitArray.OfType<bool>().Select(b => b ? '1' : '0').ToArray()
            );
        }

        public static BlockChain<T> MakeBlockChain<T>(
            IBlockPolicy<T> policy,
            IStore store,
            IStateStore stateStore,
            IEnumerable<T> actions = null,
            PrivateKey privateKey = null,
            DateTimeOffset? timestamp = null,
            IEnumerable<IRenderer<T>> renderers = null,
            Block<T> genesisBlock = null,
            int protocolVersion = Block<T>.CurrentProtocolVersion
        )
            where T : IAction, new()
        {
            actions = actions ?? ImmutableArray<T>.Empty;
            privateKey = privateKey ?? new PrivateKey(
                new byte[]
                {
                    0xcf, 0x36, 0xec, 0xf9, 0xe4, 0x7c, 0x87, 0x9a, 0x0d, 0xbf,
                    0x46, 0xb2, 0xec, 0xd8, 0x3f, 0xd2, 0x76, 0x18, 0x2a, 0xde,
                    0x02, 0x65, 0x82, 0x5e, 0x3b, 0x8c, 0x6b, 0xa2, 0x14, 0x46,
                    0x7b, 0x76,
                }
            );

            var tx = Transaction<T>.Create(
                0,
                privateKey,
                null,
                actions,
                timestamp: timestamp ?? DateTimeOffset.MinValue);
            genesisBlock = genesisBlock ?? new Block<T>(
                0,
                0,
                0,
                new Nonce(new byte[] { 0x01, 0x00, 0x00, 0x00 }),
                GenesisMinerAddress,
                null,
                timestamp ?? DateTimeOffset.MinValue,
                new[] { tx },
                hashAlgorithm: policy.GetHashAlgorithm(0),
                protocolVersion: protocolVersion
            );
            genesisBlock = genesisBlock.AttachStateRootHash(
                policy.GetHashAlgorithm,
                stateStore,
                policy.BlockAction
            );
            ValidatingActionRenderer<T> validator = null;
#pragma warning disable S1121
            var chain = new BlockChain<T>(
                policy,
                new VolatileStagePolicy<T>(),
                store,
                stateStore,
                genesisBlock,
                renderers: renderers ?? new[] { validator = new ValidatingActionRenderer<T>() }
            );
#pragma warning restore S1121

            if (validator != null)
            {
                validator.BlockChain = chain;
            }

            return chain;
        }

        public static PrivateKey GeneratePrivateKeyOfBucketIndex(Address tableAddress, int target)
        {
            var table = new RoutingTable(tableAddress);
            PrivateKey privateKey;
            do
            {
                privateKey = new PrivateKey();
            }
            while (table.GetBucketIndexOf(privateKey.ToAddress()) != target);

            return privateKey;
        }

        private static Block<T> AttachStateRootHash<T>(
            this Block<T> block,
            HashAlgorithmGetter hashAlgorithmGetter,
            IStateStore stateStore,
            IAction blockAction
        )
            where T : IAction, new()
        {
            IValue StateGetter(
                Address address, BlockHash? blockHash, StateCompleter<T> stateCompleter) =>
                blockHash is null
                    ? null
                    : stateStore.GetState(ToStateKey(address), blockHash.Value);

            FungibleAssetValue FungibleAssetValueGetter(
                Address address,
                Currency currency,
                BlockHash? blockHash,
                FungibleAssetStateCompleter<T> stateCompleter)
            {
                if (blockHash is null)
                {
                    return FungibleAssetValue.FromRawValue(currency, 0);
                }

                IValue value = stateStore.GetState(
                    ToFungibleAssetKey(address, currency), blockHash.Value);
                return FungibleAssetValue.FromRawValue(
                    currency,
                    value is Bencodex.Types.Integer i ? i.Value : 0);
            }

            var actionEvaluator = new ActionEvaluator<T>(
                hashAlgorithmGetter: hashAlgorithmGetter,
                policyBlockAction: blockAction,
                stateGetter: StateGetter,
                balanceGetter: FungibleAssetValueGetter,
                trieGetter: null
            );
            var actionEvaluationResult = actionEvaluator
                .Evaluate(block, StateCompleterSet<T>.Reject)
                .GetTotalDelta(ToStateKey, ToFungibleAssetKey);
            stateStore.SetStates(block, actionEvaluationResult);
            if (stateStore is TrieStateStore trieStateStore)
            {
                block = new Block<T>(block, trieStateStore.GetRootHash(block.Hash));
                stateStore.SetStates(block, actionEvaluationResult);
            }

            return block;
        }
    }
}
