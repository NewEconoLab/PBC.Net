#include "pch.h"
#include "Pairing.h"
#include "Params.h"

using namespace PbcProxy;

/// Pairing class implementation

PbcProxy::Pairing::Pairing()
	: m_pbc_param(), m_pairing()
{
	// initialize the pairing
	pbc_param_init_set_str(m_pbc_param, type_A_params);
	pairing_init_pbc_param(m_pairing, m_pbc_param);
}

PbcProxy::Pairing::~Pairing()
{
	pairing_clear(m_pairing);
}

Element^ PbcProxy::Pairing::
elementFromHash(GroupIface^ g, String^ inputStr)
{
	Element^ element = ref new Element();
	g->initElement(element, this);

	// set element using an input string hash
	element_from_hash(element->m_element, (void*)inputStr->Data(),
		inputStr->Length() * sizeof(wchar_t));

	return element;
}

Element^ PbcProxy::Pairing::
getRandomElement(GroupIface^ g)
{
	Element^ element = ref new Element();
	g->initElement(element, this);
	element_random(element->m_element);
	return element;
}

Element^ PbcProxy::Pairing::apply(Element^ g1, Element^ g2)
{
	Element^ gt = ref new Element();
	element_init_GT(gt->m_element, m_pairing);
	pairing_apply(gt->m_element, g1->m_element, g2->m_element, m_pairing);
	return gt;
}

bool PbcProxy::Pairing::isSymmetric()
{
	return pairing_is_symmetric(m_pairing) != 0;
}