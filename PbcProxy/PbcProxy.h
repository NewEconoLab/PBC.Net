#pragma once

#include <mpir.h>
#include <pbc.h>

using namespace Platform;

namespace PbcProxy
{
	public ref class PBC sealed
	{
	public:
		// The init() function must be called in the beginning to supply
		// a random generator to the PBC library
		static void init();

		// Initialize PBC to use a deterministic pseudo-random function
		// using the provided seed
		static void init(unsigned int seed);

		// Test PBC by computing two simple pairings
		// and comparing the results
		static void test();
	};

	ref class Pairing;

	public ref class Element sealed
	{
	public:
		// Destructor
		virtual ~Element();

		// Multiply by another element
		Element^ mul(Element^ other);

		// Raise element to the power of an integer in Zn
		Element^ powZn(Element^ other);

		Array<byte>^ toBuffer();

	private:
		friend ref class Pairing;
		friend ref class G1;
		friend ref class G2;
		friend ref class Zn;

		Element();

		element_t m_element;
	};

	public interface class GroupIface
	{
	public:
		void initElement(Element^ element, Pairing^ pairing);
	};

	public ref class G1 sealed : GroupIface
	{
	public:
		virtual void initElement(Element^ element, Pairing^ pairing);
	};

	public ref class G2 sealed : GroupIface
	{
	public:
		virtual void initElement(Element^ element, Pairing^ pairing);
	};

	public ref class Zn sealed : GroupIface
	{
	public:
		virtual void initElement(Element^ element, Pairing^ pairing);
	};
}
