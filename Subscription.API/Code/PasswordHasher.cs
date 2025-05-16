#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           PasswordHasher.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          05-16-2025 21:05
// Last Updated On:     05-16-2025 21:09
// *****************************************/
#endregion

using System.Security.Cryptography;

namespace Subscription.API.Code;

public static class PasswordHasher
{
    private const int SaltSize = 64; // 512 bits
    private const int HashSize = 64; // 512 bits
    private const int Iterations = 100_000;

    public static (byte[] Hash, byte[] Salt) HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = HashPassword(password, salt);
        return (hash, salt);
    }

    public static byte[] HashPassword(string password, byte[] salt)
    {
        using Rfc2898DeriveBytes pbkdf2 = new(password, salt, Iterations, HashAlgorithmName.SHA512);
        return pbkdf2.GetBytes(HashSize);
    }

    public static bool VerifyPassword(string password, byte[] expectedHash, byte[] salt)
    {
        byte[] hash = HashPassword(password, salt);
        return CryptographicOperations.FixedTimeEquals(hash, expectedHash);
    }
}