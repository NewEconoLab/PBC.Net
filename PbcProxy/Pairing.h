#pragma once

#include <pbc.h>
#include "PbcProxy.h"

namespace PbcProxy {
	public ref class Pairing sealed
	{
	public:
		// Constructor
		Pairing();

		// Destructor
		virtual ~Pairing();

		// Initialize element from hash of the input
		Element^ elementFromHash(GroupIface^ g, String^ inputStr);

		// Obtain random element in G1
		Element^ getRandomElement(GroupIface^ g);

		// Apply bilinaer pairing to elements of
		// groups G1 and G2 to obtain an element in GT
		Element^ apply(Element^ g1, Element^ g2);

		bool isSymmetric();

	private:
		friend ref class G1;
		friend ref class G2;
		friend ref class Zn;

		pbc_param_t m_pbc_param;
		pairing_t m_pairing;
	};
}