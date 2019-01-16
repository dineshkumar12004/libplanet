using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Libplanet.Crypto;
using Libplanet.Tests.Common.Action;
using Libplanet.Tx;
using Xunit;
using static Libplanet.Tests.TestUtils;

namespace Libplanet.Tests.Tx
{
    public class TransactionTest
    {
        private readonly TxFixture _fx;

        public TransactionTest()
        {
            _fx = new TxFixture();
        }

        [Fact]
        public void CanMake()
        {
            PrivateKey privateKey = new PrivateKey(
                ByteUtil.ParseHex(
                    "cf36ecf9e47c879a0dbf46b2ecd83fd276182ade0265825e3b8c6ba214467b76")
            );
            var recipient = new Address(privateKey.PublicKey);
            var timestamp = new DateTime(2018, 11, 21);
            Transaction<BaseAction> tx = Transaction<BaseAction>.Make(
                privateKey,
                recipient,
                new List<BaseAction>(),
                timestamp
            );

            Assert.Equal(
                new Address(privateKey.PublicKey),
                tx.Sender
            );
            Assert.Equal(recipient, tx.Recipient);
            Assert.Equal(privateKey.PublicKey, tx.PublicKey);
            Assert.Equal(timestamp, tx.Timestamp);
            AssertBytesEqual(
                new byte[]
                {
                    0x30, 0x45, 0x02, 0x21, 0x00, 0x9b, 0x8e, 0xb8, 0xb8, 0x6b,
                    0x31, 0x8d, 0xb4, 0x86, 0xb5, 0x9a, 0x4f, 0x8e, 0x54, 0xea,
                    0xa6, 0x8f, 0x88, 0x73, 0x94, 0x63, 0xa9, 0x19, 0x36, 0x1a,
                    0x4b, 0x1a, 0x32, 0xcf, 0x22, 0xf2, 0x1e, 0x02, 0x20, 0x76,
                    0xbe, 0x7f, 0xbf, 0x76, 0xa2, 0x09, 0x71, 0xfe, 0xf9, 0x28,
                    0xc6, 0x44, 0x0e, 0xdf, 0xda, 0xf3, 0x82, 0x29, 0x7b, 0x0f,
                    0x09, 0xf4, 0x50, 0x9f, 0xb1, 0xb1, 0x1e, 0xab, 0x11, 0x4b,
                    0x3d,
                },
                tx.Signature
            );
            AssertBytesEqual(
                new TxId(
                    new byte[]
                    {
                        0xfd, 0x5d, 0x3b, 0x46, 0x18, 0x01, 0xcd, 0x9b, 0xa8,
                        0xdc, 0x86, 0x49, 0xa7, 0x6b, 0x65, 0x14, 0xc8, 0x7a,
                        0x47, 0x4f, 0x11, 0x28, 0x37, 0xb8, 0xa7, 0x37, 0x69,
                        0x63, 0x3b, 0x8c, 0x54, 0xbb,
                    }
                ),
                tx.Id
            );
        }

        [Fact]
        public void CanSerialize()
        {
            var expected = new byte[]
            {
                0x64, 0x37, 0x3a, 0x61, 0x63, 0x74, 0x69, 0x6f, 0x6e, 0x73,
                0x6c, 0x65, 0x31, 0x30, 0x3a, 0x70, 0x75, 0x62, 0x6c, 0x69,
                0x63, 0x5f, 0x6b, 0x65, 0x79, 0x36, 0x35, 0x3a, 0x04, 0x46,
                0x11, 0x5b, 0x01, 0x31, 0xba, 0xcc, 0xf9, 0x4a, 0x58, 0x56,
                0xed, 0xe8, 0x71, 0x29, 0x5f, 0x6f, 0x3d, 0x35, 0x2e, 0x68,
                0x47, 0xcd, 0xa9, 0xc0, 0x3e, 0x89, 0xfe, 0x09, 0xf7, 0x32,
                0x80, 0x87, 0x11, 0xec, 0x97, 0xaf, 0x6e, 0x34, 0x1f, 0x11,
                0x0a, 0x32, 0x6d, 0xa1, 0xbd, 0xb8, 0x1f, 0x5a, 0xe3, 0xba,
                0xdf, 0x76, 0xa9, 0x0b, 0x22, 0xc8, 0xc4, 0x91, 0xae, 0xd3,
                0xaa, 0xa2, 0x96, 0x39, 0x3a, 0x72, 0x65, 0x63, 0x69, 0x70,
                0x69, 0x65, 0x6e, 0x74, 0x32, 0x30, 0x3a, 0xc2, 0xa8, 0x60,
                0x14, 0x07, 0x3d, 0x66, 0x2a, 0x4a, 0x9b, 0xfc, 0xf9, 0xcb,
                0x54, 0x26, 0x3d, 0xfa, 0x4f, 0x5c, 0xbc, 0x36, 0x3a, 0x73,
                0x65, 0x6e, 0x64, 0x65, 0x72, 0x32, 0x30, 0x3a, 0xc2, 0xa8,
                0x60, 0x14, 0x07, 0x3d, 0x66, 0x2a, 0x4a, 0x9b, 0xfc, 0xf9,
                0xcb, 0x54, 0x26, 0x3d, 0xfa, 0x4f, 0x5c, 0xbc, 0x39, 0x3a,
                0x73, 0x69, 0x67, 0x6e, 0x61, 0x74, 0x75, 0x72, 0x65, 0x37,
                0x31, 0x3a, 0x30, 0x45, 0x02, 0x21, 0x00, 0x9b, 0x8e, 0xb8,
                0xb8, 0x6b, 0x31, 0x8d, 0xb4, 0x86, 0xb5, 0x9a, 0x4f, 0x8e,
                0x54, 0xea, 0xa6, 0x8f, 0x88, 0x73, 0x94, 0x63, 0xa9, 0x19,
                0x36, 0x1a, 0x4b, 0x1a, 0x32, 0xcf, 0x22, 0xf2, 0x1e, 0x02,
                0x20, 0x76, 0xbe, 0x7f, 0xbf, 0x76, 0xa2, 0x09, 0x71, 0xfe,
                0xf9, 0x28, 0xc6, 0x44, 0x0e, 0xdf, 0xda, 0xf3, 0x82, 0x29,
                0x7b, 0x0f, 0x09, 0xf4, 0x50, 0x9f, 0xb1, 0xb1, 0x1e, 0xab,
                0x11, 0x4b, 0x3d, 0x39, 0x3a, 0x74, 0x69, 0x6d, 0x65, 0x73,
                0x74, 0x61, 0x6d, 0x70, 0x75, 0x32, 0x37, 0x3a, 0x32, 0x30,
                0x31, 0x38, 0x2d, 0x31, 0x31, 0x2d, 0x32, 0x31, 0x54, 0x30,
                0x30, 0x3a, 0x30, 0x30, 0x3a, 0x30, 0x30, 0x2e, 0x30, 0x30,
                0x30, 0x30, 0x30, 0x30, 0x5a, 0x65,
            };

            AssertBytesEqual(expected, _fx.Tx.Bencode(true));
        }

        [Fact]
        public void CanSerializeWithActions()
        {
            var expected = new byte[]
            {
                0x64, 0x37, 0x3a, 0x61, 0x63, 0x74, 0x69, 0x6f, 0x6e, 0x73,
                0x6c, 0x64, 0x75, 0x37, 0x3a, 0x74, 0x79, 0x70, 0x65, 0x5f,
                0x69, 0x64, 0x75, 0x36, 0x3a, 0x61, 0x74, 0x74, 0x61, 0x63,
                0x6b, 0x75, 0x36, 0x3a, 0x76, 0x61, 0x6c, 0x75, 0x65, 0x73,
                0x64, 0x75, 0x36, 0x3a, 0x74, 0x61, 0x72, 0x67, 0x65, 0x74,
                0x75, 0x33, 0x3a, 0x6f, 0x72, 0x63, 0x75, 0x36, 0x3a, 0x77,
                0x65, 0x61, 0x70, 0x6f, 0x6e, 0x75, 0x34, 0x3a, 0x77, 0x61,
                0x6e, 0x64, 0x65, 0x65, 0x64, 0x75, 0x37, 0x3a, 0x74, 0x79,
                0x70, 0x65, 0x5f, 0x69, 0x64, 0x75, 0x35, 0x3a, 0x73, 0x6c,
                0x65, 0x65, 0x70, 0x75, 0x36, 0x3a, 0x76, 0x61, 0x6c, 0x75,
                0x65, 0x73, 0x64, 0x75, 0x37, 0x3a, 0x7a, 0x6f, 0x6e, 0x65,
                0x5f, 0x69, 0x64, 0x69, 0x31, 0x30, 0x65, 0x65, 0x65, 0x65,
                0x31, 0x30, 0x3a, 0x70, 0x75, 0x62, 0x6c, 0x69, 0x63, 0x5f,
                0x6b, 0x65, 0x79, 0x36, 0x35, 0x3a, 0x04, 0x46, 0x11, 0x5b,
                0x01, 0x31, 0xba, 0xcc, 0xf9, 0x4a, 0x58, 0x56, 0xed, 0xe8,
                0x71, 0x29, 0x5f, 0x6f, 0x3d, 0x35, 0x2e, 0x68, 0x47, 0xcd,
                0xa9, 0xc0, 0x3e, 0x89, 0xfe, 0x09, 0xf7, 0x32, 0x80, 0x87,
                0x11, 0xec, 0x97, 0xaf, 0x6e, 0x34, 0x1f, 0x11, 0x0a, 0x32,
                0x6d, 0xa1, 0xbd, 0xb8, 0x1f, 0x5a, 0xe3, 0xba, 0xdf, 0x76,
                0xa9, 0x0b, 0x22, 0xc8, 0xc4, 0x91, 0xae, 0xd3, 0xaa, 0xa2,
                0x96, 0x39, 0x3a, 0x72, 0x65, 0x63, 0x69, 0x70, 0x69, 0x65,
                0x6e, 0x74, 0x32, 0x30, 0x3a, 0xc2, 0xa8, 0x60, 0x14, 0x07,
                0x3d, 0x66, 0x2a, 0x4a, 0x9b, 0xfc, 0xf9, 0xcb, 0x54, 0x26,
                0x3d, 0xfa, 0x4f, 0x5c, 0xbc, 0x36, 0x3a, 0x73, 0x65, 0x6e,
                0x64, 0x65, 0x72, 0x32, 0x30, 0x3a, 0xc2, 0xa8, 0x60, 0x14,
                0x07, 0x3d, 0x66, 0x2a, 0x4a, 0x9b, 0xfc, 0xf9, 0xcb, 0x54,
                0x26, 0x3d, 0xfa, 0x4f, 0x5c, 0xbc, 0x39, 0x3a, 0x73, 0x69,
                0x67, 0x6e, 0x61, 0x74, 0x75, 0x72, 0x65, 0x37, 0x30, 0x3a,
                0x30, 0x44, 0x02, 0x21, 0x00, 0xf4, 0xf5, 0xaa, 0xe0, 0xda,
                0xc6, 0x0d, 0x36, 0x4b, 0x5c, 0xb5, 0x76, 0x52, 0xfa, 0x9d,
                0x0c, 0x14, 0x81, 0x10, 0x55, 0xc0, 0x09, 0xbb, 0x03, 0xd0,
                0x2f, 0x3b, 0x8b, 0xd2, 0x63, 0x6b, 0x46, 0x02, 0x1f, 0x76,
                0x64, 0x35, 0x9a, 0x15, 0x2f, 0x00, 0xd8, 0x53, 0x24, 0x27,
                0x3c, 0xbd, 0x88, 0x14, 0x3b, 0x8d, 0x9c, 0x9b, 0x5c, 0x37,
                0x74, 0xc5, 0x63, 0xcd, 0x78, 0x00, 0xc9, 0x55, 0x7f, 0x27,
                0x39, 0x3a, 0x74, 0x69, 0x6d, 0x65, 0x73, 0x74, 0x61, 0x6d,
                0x70, 0x75, 0x32, 0x37, 0x3a, 0x32, 0x30, 0x31, 0x38, 0x2d,
                0x31, 0x31, 0x2d, 0x32, 0x31, 0x54, 0x30, 0x30, 0x3a, 0x30,
                0x30, 0x3a, 0x30, 0x30, 0x2e, 0x30, 0x30, 0x30, 0x30, 0x30,
                0x30, 0x5a, 0x65,
            };

            AssertBytesEqual(expected, _fx.TxWithActions.Bencode(true));
        }

        [Fact]
        public void CanBeDeserialized()
        {
            byte[] bytes = ByteUtil.ParseHex(
                "64373a616374696f6e736c6531303a7075626c69635f6b657936353a0446115b0131baccf94a5856ede871295f6f3d352e6847cda9c03e89fe09f732808711ec97af6e341f110a326da1bdb81f5ae3badf76a90b22c8c491aed3aaa296393a726563697069656e7432303ac2a86014073d662a4a9bfcf9cb54263dfa4f5cbc363a73656e64657232303ac2a86014073d662a4a9bfcf9cb54263dfa4f5cbc393a7369676e617475726537313a3045022100d3009449764f77e5e3de701451f16e6555f0ab7d1fcb1533f1c8977a1af099100220254b158567b4b285d2a31bf3a922596ec8deeffc32e4f2d5e5982f4030478f4d393a74696d657374616d7032373a323031382d31312d32315430303a30303a30302e3030303030305a65"
            );
            PublicKey publicKey = new PrivateKey(
                ByteUtil.ParseHex(
                    "cf36ecf9e47c879a0dbf46b2ecd83fd276182ade0265825e3b8c6ba214467b76"
                )
            ).PublicKey;
            Transaction<BaseAction> tx = Transaction<BaseAction>.FromBencoded(bytes);

            Assert.Equal(publicKey, tx.PublicKey);
            Assert.Equal(new Address(publicKey), tx.Recipient);
            Assert.Equal(new Address(publicKey), tx.Sender);
            Assert.Equal(new DateTime(2018, 11, 21), tx.Timestamp);
            Assert.Equal(
                ByteUtil.ParseHex(
                    "3045022100d3009449764f77e5e3de701451f16e6555f0ab7d1fcb1533f1c8977a1af099100220254b158567b4b285d2a31bf3a922596ec8deeffc32e4f2d5e5982f4030478f4d"
                ),
                tx.Signature
            );
            AssertBytesEqual(
                new TxId(
                    new byte[]
                    {
                        0x88, 0xe9, 0x11, 0xe1, 0x7b, 0x1f, 0x3a, 0x3b, 0x53,
                        0x15, 0xf0, 0x8f, 0x21, 0x63, 0x3c, 0xc2, 0x7e, 0x31,
                        0xac, 0x8c, 0xf3, 0x1e, 0xc0, 0xd7, 0x5b, 0x90, 0x6b,
                        0x10, 0xce, 0x7a, 0x82, 0x01,
                    }
                ),
                tx.Id
            );
        }

        [Fact]
        public void CanBeDeserializedWithActions()
        {
            var bytes = new byte[]
            {
                0x64, 0x75, 0x37, 0x3a, 0x61, 0x63, 0x74, 0x69, 0x6f, 0x6e,
                0x73, 0x6c, 0x64, 0x75, 0x37, 0x3a, 0x74, 0x79, 0x70, 0x65,
                0x5f, 0x69, 0x64, 0x75, 0x36, 0x3a, 0x61, 0x74, 0x74, 0x61,
                0x63, 0x6b, 0x75, 0x36, 0x3a, 0x76, 0x61, 0x6c, 0x75, 0x65,
                0x73, 0x64, 0x75, 0x36, 0x3a, 0x74, 0x61, 0x72, 0x67, 0x65,
                0x74, 0x75, 0x33, 0x3a, 0x6f, 0x72, 0x63, 0x75, 0x36, 0x3a,
                0x77, 0x65, 0x61, 0x70, 0x6f, 0x6e, 0x75, 0x34, 0x3a, 0x77,
                0x61, 0x6e, 0x64, 0x65, 0x65, 0x64, 0x75, 0x37, 0x3a, 0x74,
                0x79, 0x70, 0x65, 0x5f, 0x69, 0x64, 0x75, 0x35, 0x3a, 0x73,
                0x6c, 0x65, 0x65, 0x70, 0x75, 0x36, 0x3a, 0x76, 0x61, 0x6c,
                0x75, 0x65, 0x73, 0x64, 0x75, 0x37, 0x3a, 0x7a, 0x6f, 0x6e,
                0x65, 0x5f, 0x69, 0x64, 0x69, 0x31, 0x30, 0x65, 0x65, 0x65,
                0x65, 0x75, 0x31, 0x30, 0x3a, 0x70, 0x75, 0x62, 0x6c, 0x69,
                0x63, 0x5f, 0x6b, 0x65, 0x79, 0x36, 0x35, 0x3a, 0x04, 0x46,
                0x11, 0x5b, 0x01, 0x31, 0xba, 0xcc, 0xf9, 0x4a, 0x58, 0x56,
                0xed, 0xe8, 0x71, 0x29, 0x5f, 0x6f, 0x3d, 0x35, 0x2e, 0x68,
                0x47, 0xcd, 0xa9, 0xc0, 0x3e, 0x89, 0xfe, 0x09, 0xf7, 0x32,
                0x80, 0x87, 0x11, 0xec, 0x97, 0xaf, 0x6e, 0x34, 0x1f, 0x11,
                0x0a, 0x32, 0x6d, 0xa1, 0xbd, 0xb8, 0x1f, 0x5a, 0xe3, 0xba,
                0xdf, 0x76, 0xa9, 0x0b, 0x22, 0xc8, 0xc4, 0x91, 0xae, 0xd3,
                0xaa, 0xa2, 0x96, 0x75, 0x39, 0x3a, 0x72, 0x65, 0x63, 0x69,
                0x70, 0x69, 0x65, 0x6e, 0x74, 0x32, 0x30, 0x3a, 0xc2, 0xa8,
                0x60, 0x14, 0x07, 0x3d, 0x66, 0x2a, 0x4a, 0x9b, 0xfc, 0xf9,
                0xcb, 0x54, 0x26, 0x3d, 0xfa, 0x4f, 0x5c, 0xbc, 0x75, 0x36,
                0x3a, 0x73, 0x65, 0x6e, 0x64, 0x65, 0x72, 0x32, 0x30, 0x3a,
                0xc2, 0xa8, 0x60, 0x14, 0x07, 0x3d, 0x66, 0x2a, 0x4a, 0x9b,
                0xfc, 0xf9, 0xcb, 0x54, 0x26, 0x3d, 0xfa, 0x4f, 0x5c, 0xbc,
                0x75, 0x39, 0x3a, 0x73, 0x69, 0x67, 0x6e, 0x61, 0x74, 0x75,
                0x72, 0x65, 0x37, 0x30, 0x3a, 0x30, 0x44, 0x02, 0x20, 0x3a,
                0x3f, 0xf2, 0xe1, 0x74, 0xc6, 0x4e, 0x91, 0xfb, 0x98, 0xfd,
                0x8d, 0xdd, 0x7b, 0x4f, 0xa8, 0x5f, 0xef, 0x7e, 0xc4, 0x34,
                0x83, 0xed, 0x40, 0x85, 0xb2, 0x13, 0xfc, 0xa9, 0xae, 0x73,
                0xe8, 0x02, 0x20, 0x40, 0x1c, 0xae, 0x21, 0xa6, 0x54, 0x51,
                0x28, 0x0e, 0x7b, 0xb3, 0x89, 0x07, 0x42, 0x31, 0xfe, 0x4b,
                0x78, 0x86, 0x1a, 0xb4, 0x74, 0x17, 0xfe, 0x9a, 0x5a, 0xe0,
                0xdb, 0xb2, 0xc0, 0xa7, 0xd4, 0x75, 0x39, 0x3a, 0x74, 0x69,
                0x6d, 0x65, 0x73, 0x74, 0x61, 0x6d, 0x70, 0x32, 0x37, 0x3a,
                0x32, 0x30, 0x31, 0x38, 0x2d, 0x31, 0x31, 0x2d, 0x32, 0x31,
                0x54, 0x30, 0x30, 0x3a, 0x30, 0x30, 0x3a, 0x30, 0x30, 0x2e,
                0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x5a, 0x65,
            };
            PublicKey publicKey = new PrivateKey(
                ByteUtil.ParseHex(
                    "cf36ecf9e47c879a0dbf46b2ecd83fd276182ade0265825e3b8c6ba214467b76"
                )
            ).PublicKey;
            Transaction<BaseAction> tx = Transaction<BaseAction>.FromBencoded(bytes);

            Assert.Equal(publicKey, tx.PublicKey);
            Assert.Equal(new Address(publicKey), tx.Recipient);
            Assert.Equal(new Address(publicKey), tx.Sender);
            Assert.Equal(new DateTime(2018, 11, 21), tx.Timestamp);
            Assert.Equal(
                ByteUtil.ParseHex(
                    "304402203a3ff2e174c64e91fb98fd8ddd7b4fa85fef7ec43483ed4085b213fca9ae73e80220401cae21a65451280e7bb389074231fe4b78861ab47417fe9a5ae0dbb2c0a7d4"
                ),
                tx.Signature
            );
            AssertBytesEqual(
                new TxId(
                    new byte[]
                    {
                        0x0f, 0x88, 0xa5, 0x60, 0xe1, 0x14, 0xf1, 0x47, 0x92,
                        0x44, 0x86, 0x24, 0xc5, 0x43, 0x03, 0x40, 0xe3, 0x02,
                        0xb0, 0x34, 0x1d, 0xca, 0xf4, 0x7a, 0x53, 0x3c, 0x6b,
                        0xf4, 0xe5, 0xcf, 0xb0, 0x82,
                    }
                ),
                tx.Id
            );

            Assert.Equal(2, tx.Actions.Count());
            Assert.IsType<Attack>(tx.Actions[0]);
            Assert.Equal(
                new Dictionary<string, object>()
                {
                    { "weapon", "wand" },
                    { "target", "orc" },
                },
                tx.Actions[0].PlainValue
            );
            Assert.IsType<Sleep>(tx.Actions[1]);
            Assert.Equal(
                new Dictionary<string, object>()
                {
                    { "zone_id", 10 },
                },
                tx.Actions[1].PlainValue
            );
        }

        [Fact]
        public void CanValidate()
        {
            PrivateKey privateKey = new PrivateKey(
                ByteUtil.ParseHex(
                    "cf36ecf9e47c879a0dbf46b2ecd83fd276182ade0265825e3b8c6ba214467b76"
                )
            );
            var recipient = new Address(privateKey.PublicKey);
            var timestamp = new DateTime(2018, 11, 21);
            Transaction<BaseAction> tx = Transaction<BaseAction>.Make(
                privateKey,
                recipient,
                new List<BaseAction>(),
                timestamp
            );

            tx.Validate();
        }

        [Fact]
        public void CanDetectBadSignature()
        {
            Transaction<BaseAction> tx = Transaction<BaseAction>.FromBencoded(
                ByteUtil.ParseHex(
                    "64373a616374696f6e736c6531303a7075626c69635f6b657936353a0446115b0131baccf94a5856ede871295f6f3d352e6847cda9c03e89fe09f732808711ec97af6e341f110a326da1bdb81f5ae3badf76a90b22c8c491aed3aaa296393a726563697069656e7432303ac2a86014073d662a4a9bfcf9cb54263dfa4f5cbc363a73656e64657232303ac2a86014073d662a4a9bfcf9cb54263dfa4f5cbc393a7369676e617475726537313a3045022100d3009449764f77e5e3de701451f16e6555f0ab7d1fcb1533f1c8977b1af099100220254b158567b4b285d2a31bf3a922596ec8deeffc32e4f2d5e5982f4030478f4d393a74696d657374616d7032373a323031382d31312d32315430303a30303a30302e3030303030305a65"
                )
            );

            Assert.Throws<InvalidTxSignatureException>(() => { tx.Validate(); });
        }

        [Fact]
        public void CanDetectAddressMismatch()
        {
            RawTransaction rawTx = GetExpectedRawTransaction(true);
            var mismatchedAddress = new byte[]
            {
                0x45, 0xa2, 0x21, 0x87, 0xe2, 0xd8, 0x85, 0x0b, 0xb3, 0x57,
                0x88, 0x69, 0x58, 0xbc, 0x3e, 0x85, 0x60, 0x92, 0x9c, 0x01,
            };
            var signature = new byte[]
            {
                0x30, 0x45, 0x02, 0x21, 0x00, 0xc0, 0xd3, 0xa0, 0xa5, 0x77,
                0x4e, 0x77, 0x62, 0x08, 0xd5, 0x1b, 0xfa, 0x06, 0xc5, 0xd0,
                0x13, 0x75, 0x31, 0x49, 0xed, 0xb5, 0x67, 0x4a, 0xdc, 0x15,
                0xe5, 0x63, 0xf2, 0x64, 0xe5, 0x37, 0x7d, 0x02, 0x20, 0x47,
                0xaf, 0x52, 0x70, 0xec, 0x6f, 0x9a, 0x99, 0xc3, 0x33, 0x4c,
                0x0a, 0x9c, 0x97, 0x28, 0x41, 0x6f, 0xd8, 0xd7, 0xfc, 0x11,
                0x54, 0x49, 0x4b, 0x09, 0xed, 0x83, 0x92, 0xd1, 0xad, 0xc5,
                0xa4,
            };
            var rawTxWithMismatchedAddress = new RawTransaction(
                sender: mismatchedAddress,
                recipient: rawTx.Recipient,
                publicKey: rawTx.PublicKey,
                timestamp: rawTx.Timestamp,
                actions: rawTx.Actions,
                signature: signature
            );
            var tx = new Transaction<BaseAction>(rawTxWithMismatchedAddress);

            Assert.Throws<InvalidTxPublicKeyException>(() => { tx.Validate(); });
        }

        [Fact]
        public void CanConvertToRaw()
        {
            PrivateKey privateKey = new PrivateKey(
                ByteUtil.ParseHex(
                    "cf36ecf9e47c879a0dbf46b2ecd83fd276182ade0265825e3b8c6ba214467b76"
                )
            );
            var recipient = new Address(privateKey.PublicKey);
            var timestamp = new DateTime(2018, 11, 21);
            Transaction<BaseAction> tx = Transaction<BaseAction>.Make(
                privateKey,
                recipient,
                new List<BaseAction>(),
                timestamp
            );

            Assert.Equal(
                GetExpectedRawTransaction(false),
                tx.ToRawTransaction(false)
            );
            Assert.Equal(
                GetExpectedRawTransaction(true),
                tx.ToRawTransaction(true)
            );
        }

        [Fact]
        public void CanConvertFromRawTransaction()
        {
            RawTransaction rawTx = GetExpectedRawTransaction(true);
            var tx = new Transaction<BaseAction>(rawTx);
            tx.Validate();
        }

        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.ReadabilityRules",
            "SA1118",
            Justification = "Long array literals should be multiline.")]
        internal RawTransaction GetExpectedRawTransaction(bool includeSingature)
        {
            var tx = new RawTransaction(
                sender: new byte[]
                {
                    0xc2, 0xa8, 0x60, 0x14, 0x07, 0x3d, 0x66, 0x2a, 0x4a, 0x9b,
                    0xfc, 0xf9, 0xcb, 0x54, 0x26, 0x3d, 0xfa, 0x4f, 0x5c, 0xbc,
                },
                recipient: new byte[]
                {
                    0xc2, 0xa8, 0x60, 0x14, 0x07, 0x3d, 0x66, 0x2a, 0x4a, 0x9b,
                    0xfc, 0xf9, 0xcb, 0x54, 0x26, 0x3d, 0xfa, 0x4f, 0x5c, 0xbc,
                },
                publicKey: new byte[]
                {
                    0x04, 0x46, 0x11, 0x5b, 0x01, 0x31, 0xba, 0xcc, 0xf9, 0x4a,
                    0x58, 0x56, 0xed, 0xe8, 0x71, 0x29, 0x5f, 0x6f, 0x3d, 0x35,
                    0x2e, 0x68, 0x47, 0xcd, 0xa9, 0xc0, 0x3e, 0x89, 0xfe, 0x09,
                    0xf7, 0x32, 0x80, 0x87, 0x11, 0xec, 0x97, 0xaf, 0x6e, 0x34,
                    0x1f, 0x11, 0x0a, 0x32, 0x6d, 0xa1, 0xbd, 0xb8, 0x1f, 0x5a,
                    0xe3, 0xba, 0xdf, 0x76, 0xa9, 0x0b, 0x22, 0xc8, 0xc4, 0x91,
                    0xae, 0xd3, 0xaa, 0xa2, 0x96,
                },
                timestamp: "2018-11-21T00:00:00.000000Z",
                actions: new List<IDictionary<string, object>>()
            );
            if (!includeSingature)
            {
                return tx;
            }

            byte[] signature = new byte[]
            {
                0x30, 0x45, 0x02, 0x21, 0x00, 0x9b, 0x8e, 0xb8, 0xb8, 0x6b,
                0x31, 0x8d, 0xb4, 0x86, 0xb5, 0x9a, 0x4f, 0x8e, 0x54, 0xea,
                0xa6, 0x8f, 0x88, 0x73, 0x94, 0x63, 0xa9, 0x19, 0x36, 0x1a,
                0x4b, 0x1a, 0x32, 0xcf, 0x22, 0xf2, 0x1e, 0x02, 0x20, 0x76,
                0xbe, 0x7f, 0xbf, 0x76, 0xa2, 0x09, 0x71, 0xfe, 0xf9, 0x28,
                0xc6, 0x44, 0x0e, 0xdf, 0xda, 0xf3, 0x82, 0x29, 0x7b, 0x0f,
                0x09, 0xf4, 0x50, 0x9f, 0xb1, 0xb1, 0x1e, 0xab, 0x11, 0x4b,
                0x3d,
            };
            return tx.AddSignature(signature);
        }
    }
}
