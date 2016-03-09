#include "pch.h"
#include "PbcProxy.h"

using namespace PbcProxy;

/// Element class implementation

PbcProxy::Element::Element()
	: m_element()
{}

PbcProxy::Element::~Element()
{
	element_clear(m_element);
}

Element^ PbcProxy::Element::mul(Element^ other)
{
	Element^ result = ref new Element();
	element_init_same_as(result->m_element, this->m_element);
	element_mul(result->m_element, m_element, other->m_element);
	return result;
}

Element^ PbcProxy::Element::powZn(Element^ other)
{
	Element^ result = ref new Element();
	element_init_same_as(result->m_element, this->m_element);
	element_pow_zn(result->m_element, m_element, other->m_element);
	return result;
}

Array<byte>^ PbcProxy::Element::toBuffer()
{
	int len = element_length_in_bytes(m_element);
	byte* buffer = new byte[len];
	element_to_bytes(buffer, m_element);
	Array<byte>^ retval = ref new Array<byte>(buffer, len);
	delete buffer; // TODO: check that we can indeed free that buffer
	return retval;
}