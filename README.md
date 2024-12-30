# RC4-Cipher
 Implementation of the RC4 stream cipher in a C# WPF application (see https://en.wikipedia.org/wiki/RC4)

## Instructions
Download RC4_Cipher.zip from the latest release, extract the contents, and run the executable.

To encrypt a message, enter the plaintext and key and press "Encrypt". To decrypt, enter the ciphertext and key and press "Decrypt". The plaintext and key may be any string value. The ciphertext must be in hexadecimal format with no spaces. Do NOT use hexadecimal for the plaintext or key - these are already converted to hexadecimal during the encryption/decryption process.

## How It Works
The RC4 cipher is a stream cipher, meaning that the encryption key is used to generate a stream of bytes called a keystream. This keystream is combined with the plaintext to encrypt it or with the ciphertext to decrypt it.

First, the key-scheduling algorithm (KSA) is used to generate an initial permutation of all 256 possible bytes based on the key. Then, the pseudo-random generation algorithm (PRGA) is used to select bytes from the permutation to form the keystream. The permutation of bytes is also shuffled during this process. Finally, the keystream is XORed with the plaintext or ciphertext to yield the other.

RC4 is no longer widely-used because of security concerns. Researchers have discovered certain biases in the keystream, especially in the first few bytes, increasing its susceptibility to cryptanalysis. Additionally, if the same key is reused for multiple messages, attackers can XOR the ciphertexts to gain information about the key and plaintexts.
