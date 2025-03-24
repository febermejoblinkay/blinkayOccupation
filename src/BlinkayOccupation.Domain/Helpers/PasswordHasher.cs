using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace BlinkayOccupation.Domain.Helpers
{
    public static class PasswordHasher
    {
        private const int SaltSize = 24;
        private const int HashSize = 32;
        private const int Iterations = 20000;

        private static readonly int[] _invAlphabet;

        private static readonly ulong[] _powN;
        private static readonly char[] _digits;

        static PasswordHasher()
        {
            _digits = new char[16]
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F'
        };
            _invAlphabet = new int[123];
            _powN = new ulong[5] { 14776336uL, 238328uL, 3844uL, 62uL, 1uL };
            for (int i = 0; i < _invAlphabet.Length; i++)
            {
                _invAlphabet[i] = -1;
            }

            for (int j = 0; j < 62; j++)
            {
                _invAlphabet[(uint)"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"[j]] = j;
            }
        }

        /// <summary>
        /// Genera un hash seguro de la contraseña usando sal y PBKDF2.
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.");

            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[SaltSize];
            rng.GetBytes(salt);

            byte[] hash = ComputeHash(Encoding.UTF8.GetBytes(password), salt);

            byte[] combinedHash = new byte[salt.Length + hash.Length];
            Array.Copy(salt, 0, combinedHash, 0, salt.Length);
            Array.Copy(hash, 0, combinedHash, salt.Length, hash.Length);

            return Convert.ToBase64String(combinedHash);
        }

        /// <summary>
        /// Verifica si la contraseña ingresada coincide con el hash almacenado.
        /// </summary>
        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            if (string.IsNullOrEmpty(inputPassword) || string.IsNullOrEmpty(storedHash))
                return false;

            byte[] array = storedHash.FromBase62();
            byte[] array2 = new byte[24];
            Array.Copy(array, array2, 24);

            return SlowEquals(array, HashWithSaltAndCombine(Encoding.UTF8.GetBytes(inputPassword), array2));
        }

        private static byte[] HashWithSaltAndCombine(byte[] value, byte[] salt)
        {
            byte[] array = new byte[salt.Length + value.Length];
            Array.Copy(salt, array, salt.Length);
            Array.Copy(value, 0, array, salt.Length, value.Length);
            byte[] array2 = ComputeHash(array);
            using Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(array2, array2, 20000, HashAlgorithmName.SHA1);
            array2 = rfc2898DeriveBytes.GetBytes(array2.Length);
            byte[] array3 = new byte[salt.Length + array2.Length];
            Array.Copy(salt, array3, salt.Length);
            Array.Copy(array2, 0, array3, salt.Length, array2.Length);
            return array3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] ComputeHash(byte[] bytes)
        {
            using SHA256 sHA = SHA256.Create();
            return sHA.ComputeHash(bytes);
        }

        //
        // Summary:
        //     Convert a Base62 string to byte array
        //
        // Parameters:
        //   data:
        //     Base62 string
        //
        // Returns:
        //     Byte array
        public static byte[] FromBase62(this string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return Array.Empty<byte>();
            }

            int num = ((data.Length - 1) * 29 / 5 + 8) / 8 * 8;
            int num2 = num / 29 * 29;
            int num3 = num - num2;
            int num4 = num2 * 5 / 29;
            int num5 = (num3 * 5 + 29 - 1) / 29;
            ulong num6 = CharsToBits(data, num4, num5);
            if (num6 >> num3 != 0)
            {
                num += 8;
                num2 = num / 29 * 29;
                num3 = num - num2;
                num4 = num2 * 5 / 29;
                num5 = (num3 * 5 + 29 - 1) / 29;
            }

            int endInd = num4 / 5;
            byte[] array = new byte[num / 8];
            DecodeBlock(data, array, 0, endInd);
            if (num5 == 0)
            {
                return array;
            }

            ulong value = CharsToBits(data, num4, num5);
            AddBits64(array, value, num2, num3);
            return array;
        }

        private static void DecodeBlock(string src, byte[] dst, int beginInd, int endInd)
        {
            for (int i = beginInd; i < endInd; i++)
            {
                int ind = i * 5;
                int bitPos = i * 29;
                ulong value = CharsToBits(src, ind, 5);
                AddBits64(dst, value, bitPos, 29);
            }
        }

        private static void AddBits64(byte[] data, ulong value, int bitPos, int bitsCount)
        {
            int num = bitPos / 8;
            int num2 = bitPos % 8;
            int num3 = Math.Min(bitsCount, 8 - num2);
            if (num3 == 0)
            {
                return;
            }

            byte b = (byte)(value << 64 - bitsCount >> 56 + num2);
            data[num] |= b;
            num += (num2 + num3) / 8;
            num2 = (num2 + num3) % 8;
            int num4 = bitsCount - num3;
            if (num4 > 8)
            {
                num4 = 8;
            }

            while (num4 > 0)
            {
                num3 += num4;
                byte b2 = (byte)(value >> bitsCount - num3 << 8 - num4);
                data[num] |= b2;
                num += (num2 + num4) / 8;
                num2 = (num2 + num4) % 8;
                num4 = bitsCount - num3;
                if (num4 > 8)
                {
                    num4 = 8;
                }
            }
        }

        private static ulong CharsToBits(string data, int ind, int count)
        {
            ulong num = 0uL;
            for (int i = 0; i < count; i++)
            {
                num += (ulong)((long)_invAlphabet[(uint)data[ind + i]] * (long)_powN[4 - i]);
            }

            return num;
        }

        /// <summary>
        /// Computa el hash con sal usando PBKDF2.
        /// </summary>
        private static byte[] ComputeHash(byte[] password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(HashSize);
        }

        /// <summary>
        /// Compara dos arreglos de bytes de manera segura (sin vulnerabilidad de tiempo).
        /// </summary>
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)(a.Length ^ b.Length);
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }
}
