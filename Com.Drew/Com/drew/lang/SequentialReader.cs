/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System;
using System.Text;
using JetBrains.Annotations;
using Sharpen;

namespace Com.Drew.Lang
{
    /// <summary>Base class for reading sequentially through a sequence of data encoded in a byte stream.</summary>
    /// <remarks>
    /// Concrete implementations include:
    /// <list type="bullet">
    ///   <item><see cref="SequentialByteArrayReader"/></item>
    ///   <item><see cref="SequentialStreamReader"/></item>
    /// </list>
    /// By default, the reader operates with Motorola byte order (big endianness).  This can be changed by via
    /// <see cref="IsMotorolaByteOrder"/>.
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public abstract class SequentialReader
    {
        /// <summary>Get and set the byte order of this reader. <c>true</c> by default.</summary>
        /// <remarks>
        /// <list type="bullet">
        ///   <item><c>true</c> for Motorola (or big) endianness (also known as network byte order), with MSB before LSB.</item>
        ///   <item><c>false</c> for Intel (or little) endianness, with LSB before MSB.</item>
        /// </list>
        /// </remarks>
        /// <value><c>true</c> for Motorola/big endian, <c>false</c> for Intel/little endian</value>
        public bool IsMotorolaByteOrder { set; get; }

        protected SequentialReader()
        {
            IsMotorolaByteOrder = true;
        }

        /// <summary>Gets the next byte in the sequence.</summary>
        /// <returns>The read byte value</returns>
        /// <exception cref="System.IO.IOException"/>
        protected abstract byte GetByte();

        /// <summary>Returns the required number of bytes from the sequence.</summary>
        /// <param name="count">The number of bytes to be returned</param>
        /// <returns>The requested bytes</returns>
        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public abstract byte[] GetBytes(int count);

        /// <summary>Skips forward in the sequence.</summary>
        /// <remarks>
        /// Skips forward in the sequence. If the sequence ends, an
        /// <see cref="EofException"/>
        /// is thrown.
        /// </remarks>
        /// <param name="n">the number of byte to skip. Must be zero or greater.</param>
        /// <exception cref="EofException">the end of the sequence is reached.</exception>
        /// <exception cref="System.IO.IOException">an error occurred reading from the underlying source.</exception>
        public abstract void Skip(long n);

        /// <summary>Skips forward in the sequence, returning a boolean indicating whether the skip succeeded, or whether the sequence ended.</summary>
        /// <param name="n">the number of byte to skip. Must be zero or greater.</param>
        /// <returns>a boolean indicating whether the skip succeeded, or whether the sequence ended.</returns>
        /// <exception cref="System.IO.IOException">an error occurred reading from the underlying source.</exception>
        public abstract bool TrySkip(long n);

        /// <summary>Returns an unsigned 8-bit int calculated from the next byte of the sequence.</summary>
        /// <returns>the 8 bit int value, between 0 and 255</returns>
        /// <exception cref="System.IO.IOException"/>
        public byte GetUInt8()
        {
            return GetByte();
        }

        /// <summary>Returns a signed 8-bit int calculated from the next byte the sequence.</summary>
        /// <returns>the 8 bit int value, between 0x00 and 0xFF</returns>
        /// <exception cref="System.IO.IOException"/>
        public sbyte GetInt8()
        {
            return unchecked((sbyte)GetByte());
        }

        /// <summary>Returns an unsigned 16-bit int calculated from the next two bytes of the sequence.</summary>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <exception cref="System.IO.IOException"/>
        public ushort GetUInt16()
        {
            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return (ushort)
                    (GetByte() << 8 |
                     GetByte());
            }
            // Intel ordering - LSB first
            return (ushort)
                (GetByte() |
                 GetByte() << 8);
        }

        /// <summary>Returns a signed 16-bit int calculated from two bytes of data (MSB, LSB).</summary>
        /// <returns>the 16 bit int value, between 0x0000 and 0xFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public short GetInt16()
        {
            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return (short)
                    (GetByte() << 8 |
                     GetByte());
            }
            // Intel ordering - LSB first
            return (short)
                (GetByte() |
                 GetByte() << 8);
        }

        /// <summary>Get a 32-bit unsigned integer from the buffer, returning it as a long.</summary>
        /// <returns>the unsigned 32-bit int value as a long, between 0x00000000 and 0xFFFFFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public uint GetUInt32()
        {
            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first (big endian)
                return (uint)
                    (GetByte() << 24 |
                     GetByte() << 16 |
                     GetByte() << 8  |
                     GetByte());
            }
            // Intel ordering - LSB first (little endian)
            return (uint)
                (GetByte()       |
                 GetByte() << 8  |
                 GetByte() << 16 |
                 GetByte() << 24);
        }

        /// <summary>Returns a signed 32-bit integer from four bytes of data.</summary>
        /// <returns>the signed 32 bit int value, between 0x00000000 and 0xFFFFFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public int GetInt32()
        {
            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first (big endian)
                return
                    GetByte() << 24 |
                    GetByte() << 16 |
                    GetByte() << 8  |
                    GetByte();
            }
            // Intel ordering - LSB first (little endian)
            return
                GetByte()       |
                GetByte() <<  8 |
                GetByte() << 16 |
                GetByte() << 24;
        }

        /// <summary>Get a signed 64-bit integer from the buffer.</summary>
        /// <returns>the 64 bit int value, between 0x0000000000000000 and 0xFFFFFFFFFFFFFFFF</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public long GetInt64()
        {
            if (IsMotorolaByteOrder)
            {
                // Motorola - MSB first
                return
                    (long)GetByte() << 56 |
                    (long)GetByte() << 48 |
                    (long)GetByte() << 40 |
                    (long)GetByte() << 32 |
                    (long)GetByte() << 24 |
                    (long)GetByte() << 16 |
                    (long)GetByte() << 8  |
                          GetByte();
            }
            // Intel ordering - LSB first
            return
                      GetByte()       |
                (long)GetByte() << 8  |
                (long)GetByte() << 16 |
                (long)GetByte() << 24 |
                (long)GetByte() << 32 |
                (long)GetByte() << 40 |
                (long)GetByte() << 48 |
                (long)GetByte() << 56;
        }

        /// <summary>Gets a s15.16 fixed point float from the buffer.</summary>
        /// <remarks>
        /// Gets a s15.16 fixed point float from the buffer.
        /// <para />
        /// This particular fixed point encoding has one sign bit, 15 numerator bits and 16 denominator bits.
        /// </remarks>
        /// <returns>the floating point value</returns>
        /// <exception cref="System.IO.IOException">the buffer does not contain enough bytes to service the request</exception>
        public float GetS15Fixed16()
        {
            if (IsMotorolaByteOrder)
            {
                float res = GetByte() << 8 | GetByte();
                var d = GetByte() << 8 | GetByte();
                return (float)(res + d / 65536.0);
            }
            else
            {
                // this particular branch is untested
                var d = GetByte() | GetByte() << 8;
                float res = GetByte() | GetByte() << 8;
                return (float)(res + d / 65536.0);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        public float GetFloat32()
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(GetInt32()), 0);
        }

        /// <exception cref="System.IO.IOException"/>
        public double GetDouble64()
        {
            return BitConverter.Int64BitsToDouble(GetInt64());
        }

        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public string GetString(int bytesRequested)
        {
            return GetString(bytesRequested, Encoding.UTF8);
        }

        /// <exception cref="System.IO.IOException"/>
        [NotNull]
        public string GetString(int bytesRequested, [NotNull] Encoding encoding)
        {
            return encoding.GetString(GetBytes(bytesRequested));
        }

        /// <summary>Creates a String from the stream, ending where <c>byte=='\0'</c> or where <c>length==maxLength</c>.</summary>
        /// <param name="maxLengthBytes">
        /// The maximum number of bytes to read.  If a zero-byte is not reached within this limit,
        /// reading will stop and the string will be truncated to this length.
        /// </param>
        /// <returns>The read string.</returns>
        /// <exception cref="System.IO.IOException">The buffer does not contain enough bytes to satisfy this request.</exception>
        [NotNull]
        public string GetNullTerminatedString(int maxLengthBytes)
        {
            // NOTE currently only really suited to single-byte character strings
            var bytes = new byte[maxLengthBytes];
            // Count the number of non-null bytes
            var length = 0;
            while (length < bytes.Length && (bytes[length] = GetByte()) != 0)
                length++;
            return Encoding.UTF8.GetString(bytes, 0, length);
        }
    }
}
