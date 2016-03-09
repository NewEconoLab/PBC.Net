#include "pch.h"
#include <pbc.h>
#include "Random.h"

using namespace Platform;
using Windows::Security::Cryptography::CryptographicBuffer;
using Windows::Storage::Streams::IBuffer;

/// PBC implementation

static void getRandomBytes(byte* buffer, size_t len)
{
	IBuffer^ randBuffer = CryptographicBuffer::GenerateRandom(len);
	Array<byte>^ pUnmanagedArray;
	CryptographicBuffer::CopyToByteArray(randBuffer, &pUnmanagedArray);

	// TODO: optimize by copying the whole buffer using a function
	for (unsigned int i = 0; i < len; ++i) {
		buffer[i] = pUnmanagedArray->get(i);
	}
}

void win10_mpz_random(mpz_t r, mpz_t limit, void *data) {
	UNUSED_VAR(data);

	int n, bytecount, leftover;
	unsigned char *bytes;
	mpz_t z;
	mpz_init(z);
	n = mpz_sizeinbase(limit, 2);
	bytecount = (n + 7) / 8;
	leftover = n % 8;
	bytes = (unsigned char *)pbc_malloc(bytecount);
	for (;;) {
		getRandomBytes(bytes, bytecount);
		if (leftover) {
			*bytes = *bytes % (1 << leftover);
		}
		mpz_import(z, bytecount, 1, 1, 0, 0, bytes);
		if (mpz_cmp(z, limit) < 0) break;
	}
	mpz_set(r, z);
	mpz_clear(z);
	pbc_free(bytes);
}