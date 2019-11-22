using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;

namespace Brierley.FrameWork.LWIntegration.IPFiltering
{
    public class IPRange
    {
        #region Enumerations
        private enum Mode : byte
        {
            Class = 0,
            Classless,
        }

        private enum IPClass : byte
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8
        }
        #endregion

        #region Fields
        private static readonly char[] _wildcardChars = new char[] { '*', 'x', 'X' };
        private readonly uint _addressMask;
        private readonly byte _maskData;
        private readonly Mode _mode;
        #endregion

        #region Construction/Initialization
        private IPRange(uint mask, byte maskData, Mode mode)
        {
            _addressMask = mask;
            _maskData = maskData;
            _mode = mode;
        }
                
        public static IPRange ParseRange(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("value");
            }

            try
            {
                if (value.IndexOf("*") >= 0)
                {
                    // this is wild carded ip address
                    return Parse(value);
                }
                else
                {
                    // try the host names first
                    IPAddress[] list = Dns.GetHostAddresses(value);
                    // grab the first Ipv4 address
                    IPAddress v4Addr = list.FirstOrDefault(i => i.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    return Parse(INetUtil.GetStringIpAddress(v4Addr));
                }
            }
            catch (FormatException)
            {
                // this is an ip address
                return Parse(value);
            }            
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Determines whether the specified address is a match.
        /// </summary>
        /// <param name="ipRanges">The ip ranges.</param>
        /// <param name="address">The address.</param>
        /// <returns>
        /// 	<c>true</c> if the specified ip ranges is match; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsMatch(IList<IPRange> ipRanges, IPAddress address)
        {
            for (int i = 0; i < ipRanges.Count; i++)
            {
                if (ipRanges[i].IsMatch(address))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified address is a match.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>
        /// 	<c>true</c> if the specified address is a match; otherwise, <c>false</c>.
        /// </returns>
        private bool IsMatch(IPAddress address)
        {
            // Address is deprecated and but should not make a difference for ipv4 addresses though
            //return IsMatch((uint)(address.Address));
            return IsMatch(INetUtil.ConvertIpv4ToUint(address));
        }
        /// <summary>
        /// Determines whether the specified address is a match.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>
        /// 	<c>true</c> if the specified address is a match; otherwise, <c>false</c>.
        /// </returns>
        private bool IsMatch(uint address)
        {
            if (_mode == Mode.Class)
            {
                // Check the mask.
                if ((address & _addressMask) != _addressMask)
                {
                    return false;
                }

                // Check for zeros in mask
                IPClass ipClasses = (IPClass)_maskData;
                if ((ipClasses & IPClass.A) != IPClass.A &&
                    (0xFF & _addressMask) == 0 && (0xFF & address) != 0)
                {
                    return false;
                }
                if ((ipClasses & IPClass.B) != IPClass.B &&
                    (0xFF00 & _addressMask) == 0 && (0xFF00 & address) != 0)
                {
                    return false;
                }
                if ((ipClasses & IPClass.C) != IPClass.C &&
                    (0xFF0000 & _addressMask) == 0 && (0xFF0000 & address) != 0)
                {
                    return false;
                }
                if ((ipClasses & IPClass.D) != IPClass.D &&
                    (0xFF000000 & _addressMask) == 0 && (0xFF000000 & address) != 0)
                {
                    return false;
                }
                return true;

            }
            // Shift over the host identifier and so we are only comparing the network identifier portion of the ip address
            int shift = 32 - _maskData;
            return (_addressMask << shift) == (address << shift);
        }
        /// <summary>
        /// Gets all the address values in the range.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<uint> GetAddressValues()
        {
            if (_mode == Mode.Class)
            {
                IPClass classWildcard = (IPClass)_maskData;

                int aStart, aEnd, bStart, bEnd, cStart, cEnd, dStart, dEnd;
                GetClassRange(IPClass.A, out aStart, out aEnd);
                GetClassRange(IPClass.B, out bStart, out bEnd);
                GetClassRange(IPClass.C, out cStart, out cEnd);
                GetClassRange(IPClass.D, out dStart, out dEnd);

                for (int a = aStart; a <= aEnd; a++)
                {
                    for (int b = bStart; b <= bEnd; b++)
                    {
                        for (int c = cStart; c <= cEnd; c++)
                        {
                            for (int d = dStart; d <= dEnd; d++)
                            {
                                yield return (uint)(a | b << 8 | c << 16 | d << 24);
                            }
                        }
                    }
                }
            }
            else
            {
                int maxValue = Count;
                for (int i = 0; i < maxValue; i++)
                {
                    yield return (uint)(((uint)i << _maskData) | _addressMask);
                }
            }
        }
        /// <summary>
        /// Gets all the ip addresses in the range.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IPAddress> GetAddresses()
        {
            foreach (uint address in GetAddressValues())
            {
                yield return new IPAddress((long)address);
            }
        }
        
        private void GetClassRange(IPClass ipClass, out int start, out int end)
        {
            if ((ipClass & (IPClass)_maskData) == ipClass)
            {
                start = 0;
                end = 255;
            }
            else
            {
                int shift = ((byte)ipClass - 1) * 8;
                start = end = (_maskData >> shift) & 0xFF;
            }
        }

        /// <summary>
        /// Parses a string in CIDR notation or a wildcard address like 192.168.1.*
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static IPRange Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("value");
            }

            // Check if is a wildcard.
            if (IsWildCard(value))
            {
                return new IPRange(0, 0xF, Mode.Class);
            }

            int index = value.IndexOf('/');
            Mode mode;
            byte classData;
            uint mask;

            if (index > -1)
            {
                ParseAddress(value.Substring(0, index), out classData, out mask);
                mode = Mode.Classless;
                if (classData != 0)
                {
                    throw new ArgumentException(
                        string.Format("You can use a CIDR notation and wildcards.  '{0}' is invalid'.", value),
                        "value");
                }

                if (!byte.TryParse(value.Substring(index + 1), out classData))
                {
                    throw new ArgumentException(
                        string.Format("The '{0}' is invalid'.", value),
                        "value");
                }
                if (classData > 32)
                {
                    throw new ArgumentException("The subnet mask length must be less than 32.", "value");
                }

                // Remove any bits that are not apart of the network identifier
                mask &= (uint)((1 << classData) - 1);
            }
            else
            {
                ParseAddress(value, out classData, out mask);
                mode = Mode.Class;
            }
            return new IPRange(mask, classData, mode);
        }

        private static void ParseAddress(string value, out byte ipClassWildcards, out uint address)
        {
            string[] values = value.Split('.');
            if (values.Length != 4)
            {
                throw new ArgumentException(string.Format("The ip address is in invalid format {0}", value), "value");
            }

            byte ipValue;
            byte classIndex = 1;
            int maskIndex = 0;

            address = 0;
            ipClassWildcards = 0;

            for (int i = 0; i < values.Length; i++)
            {
                if (IsWildCard(values[i]))
                {
                    ipClassWildcards |= classIndex;
                }
                else
                {
                    if (!byte.TryParse(values[i], out ipValue))
                    {
                        throw new ArgumentException(string.Format("The ip address is in invalid format {0}", value), "value");
                    }
                    address |= (uint)(ipValue << maskIndex);
                }
                maskIndex += 8;
                classIndex <<= 1;
            }
        }

        private static bool IsWildCard(string value)
        {
            if (value.Length != 1)
            {
                return false;
            }
            return _wildcardChars.Contains(value[0]);
        }

        private static byte GetMaskSize(uint mask)
        {
            bool complete = false;
            byte count = 0;
            for (int i = 0; i < 32; i++)
            {
                if ((mask & 1) != 0)
                {
                    if (complete)
                    {
                        throw new ArgumentException(
                            string.Format("The netmask is invalid.  Value : 0x{0:x}", mask),
                            "mask"
                            );
                    }
                    count++;
                }
                else
                {
                    complete = true;
                }
                mask >>= 1;
            }
            return count;
        }

        private static string GetDotDecimal(uint address, IPClass ipClassWildcards)
        {
            return string.Format("{0}.{1}.{2}.{3}",
                (ipClassWildcards & IPClass.A) == IPClass.A ? "*" : (address & 0xFF).ToString(),
                (ipClassWildcards & IPClass.B) == IPClass.B ? "*" : ((address >> 8) & 0xFF).ToString(),
                (ipClassWildcards & IPClass.C) == IPClass.C ? "*" : ((address >> 16) & 0xFF).ToString(),
                (ipClassWildcards & IPClass.D) == IPClass.D ? "*" : ((address >> 24) & 0xFF).ToString()
                );
        }        
        #endregion

        #region Public Methods 
       
        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is IPRange))
            {
                return false;
            }
            IPRange range = (IPRange)obj;
            return range._addressMask == _addressMask && range._maskData == _maskData && range._mode == _mode;
        }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return ((int)_mode) |
                ((int)(_addressMask ^ (_maskData << 24)) << 1);
        }
        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>
        /// </returns>
        public override string ToString()
        {
            if (_mode == Mode.Class)
            {
                return GetDotDecimal(_addressMask, (IPClass)_maskData);
            }
            return GetDotDecimal(_addressMask, (IPClass)0) + "/" + _maskData.ToString();
        }

        /// <summary>
        /// The number of addresses in the ip range.
        /// </summary>
        public int Count
        {
            get
            {
                if (_mode == Mode.Classless)
                {
                    return (1 << (32 - _maskData));
                }
                int count = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (((1 << i) & _maskData) != 0)
                    {
                        count += 1;
                    }
                }
                return (1 << (count * 8));
            }
        }
        /// <summary>
        /// The address mask
        /// </summary>
        public uint AddressMask
        {
            get
            {
                return _addressMask;
            }
        }
        /// <summary>
        /// The mask data.
        /// </summary>
        public byte MaskData
        {
            get
            {
                return _maskData;
            }
        }

        public static bool IsAddressInRange(IList<IPRange> ipRange, IPAddress rAddress)
        {
            foreach (IPRange range in ipRange)
            {
                if (range.IsMatch(INetUtil.GetIPv4Address(rAddress)))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
