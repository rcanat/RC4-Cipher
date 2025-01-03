﻿using System;
using System.Linq;
using System.Windows;

namespace RC4_Cipher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Encrypt the plaintext when EncryptButton is clicked
        /// </summary>
        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            // get inputs from their text boxes
            string plaintext = PlaintextBox.Text;
            string key = KeytextBox.Text;

            // check if inputs are invalid
            if (string.IsNullOrEmpty(plaintext)
             || string.IsNullOrEmpty(key))
                return;

            // generate keystream and encrypt text
            byte[] keystream = PRGA(KSA(key), plaintext);
            byte[] ciphertextBytes = Encrypt(plaintext, keystream);

            // output the ciphertext
            CiphertextBox.Text = "";
            foreach (byte b in ciphertextBytes)
                CiphertextBox.Text += b.ToString("x2").ToUpper();
        }

        /// <summary>
        /// Decrypt the ciphertext when DecryptButton is clicked
        /// </summary>
        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            // get inputs from their text boxes
            string ciphertext = CiphertextBox.Text.ToUpper();
            string key = KeytextBox.Text;

            // check if inputs are invalid
            if (string.IsNullOrEmpty(ciphertext)
             || string.IsNullOrEmpty(key)
             || ciphertext.Length % 2 != 0
             || !ciphertext.All("1234567890ABCDEF".Contains))
                return;

            // generate keystream and decrypt text
            byte[] keystream = PRGA(KSA(key), ciphertext, true);
            byte[] plaintextBytes = Decrypt(ciphertext, keystream);

            // output the plaintext
            PlaintextBox.Text = "";
            foreach (byte b in plaintextBytes)
                PlaintextBox.Text += (char)b;
        }

        /// <summary>
        /// Key-Scheduling Algorithm; generates an initial permutation of all bytes used to generate the keystream.
        /// </summary>
        /// <param name="key">The key provided for encryption/decryption.</param>
        /// <returns>The byte permutation for generating the keystream.</returns>
        private static byte[] KSA(string key)
        {
            // generate an array of all possible bytes
            byte[] bytePerm = new byte[256];
            for (int i = 0; i <= byte.MaxValue; i++)
                bytePerm[i] = (byte)i;

            // shuffle the bytes into a pseudo-random permutation
            byte j = 0;
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                j = (byte)((j + bytePerm[i] + key[i % key.Length]) % 256);
                (bytePerm[i], bytePerm[j]) = (bytePerm[j], bytePerm[i]);
            }

            // return the shuffled bytes
            return bytePerm;
        }

        /// <summary>
        /// Pseudo-Random Generation Algorithm; generates the keystream using the byte permutation from KSA.
        /// </summary>
        /// <param name="bytePerm">The permutation of bytes generated by KSA.</param>
        /// <param name="text">The message being encrypted or decrypted.</param>
        /// <param name="decrypting">Whether or not message is being decrypted.</param>
        /// <returns>The keystream to be used for encryption/decryption.</returns>
        private static byte[] PRGA(byte[] bytePerm, string text, bool decrypting = false)
        {
            byte i = 0;
            byte j = 0;

            // keystream has Length/2 bytes if decrypting since 2 chars in ciphertext = 1 byte
            byte[] keystream = new byte[decrypting ? text.Length / 2 : text.Length];

            // select a byte pseudo-randomly
            for (int n = 0; n < keystream.Length; n++)
            {
                i += 1;
                j += bytePerm[i];
                (bytePerm[i], bytePerm[j]) = (bytePerm[j], bytePerm[i]);

                // add the selected byte to the keystream
                keystream[n] = bytePerm[(bytePerm[i] + bytePerm[j]) % 256];
            }

            return keystream;
        }

        /// <summary>
        /// Encrypts a message using a pre-generated keystream.
        /// </summary>
        /// <param name="plaintext">The message to encrypt.</param>
        /// <param name="keystream">The keystream to use for encryption.</param>
        /// <returns>The encrypted message.</returns>
        private static byte[] Encrypt(string plaintext, byte[] keystream)
        {
            // convert the plaintext string into bytes
            byte[] plaintextBytes = new byte[plaintext.Length];
            for (int i = 0; i < plaintext.Length; i++)
                plaintextBytes[i] = (byte)plaintext[i];

            // generate the ciphertext bytes by XORing the plaintext and keystream
            byte[] ciphertextBytes = new byte[plaintext.Length];
            for (int i = 0; i < plaintext.Length; i++)
                ciphertextBytes[i] = (byte)(plaintextBytes[i] ^ keystream[i]);

            return ciphertextBytes;
        }

        /// <summary>
        /// Decrypts a message using a pre-generated keystream.
        /// </summary>
        /// <param name="ciphertext">The message to decrypt.</param>
        /// <param name="keystream">The keystream to use for decryption.</param>
        /// <returns>The decrypted message.</returns>
        private static byte[] Decrypt(string ciphertext, byte[] keystream)
        {
            // convert the ciphertext string into bytes
            byte[] ciphertextBytes = new byte[ciphertext.Length / 2];
            for (int i = 0; i < ciphertext.Length; i += 2)
                ciphertextBytes[i / 2] = Convert.ToByte(ciphertext.Substring(i, 2), 16);

            // generate the plaintext bytes by XORing the ciphertext and keystream
            byte[] plaintextBytes = new byte[ciphertext.Length / 2];
            for (int i = 0; i < ciphertext.Length / 2; i++)
                plaintextBytes[i] = (byte)(ciphertextBytes[i] ^ keystream[i]);

            return plaintextBytes;
        }
    }
}

