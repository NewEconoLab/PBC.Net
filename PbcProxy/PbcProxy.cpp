#include "pch.h"
#include "Random.h"
#include "Debug.h"
#include "Params.h"
#include "PbcProxy.h"
#include "Pairing.h"

using namespace PbcProxy;
using namespace Platform;


void PbcProxy::PBC::init()
{
	pbc_random_set_function(win10_mpz_random, NULL);
}

void PbcProxy::PBC::init(unsigned int seed)
{
	pbc_random_set_deterministic(seed);
}

void PbcProxy::PBC::test()
{
	pbc_param_t pbc_param;
	pairing_t pairing;
	
	pbc_param_init_set_str(pbc_param, type_A_params);
	pairing_init_pbc_param(pairing, pbc_param);

	element_t g1, h1, g2, gt;
	element_init_G1(g1, pairing);
	element_init_G1(h1, pairing);
	element_init_G2(g2, pairing);
	element_init_GT(gt, pairing);

	element_random(g1);
	
	element_t n;
	element_init_Zr(n, pairing);
	element_random(n);
	element_pow_zn(h1, g1, n);

	element_random(g2);

	pairing_apply(gt, h1, g2, pairing);
	printElement(gt);

	element_pow_zn(g2, g2, n);
	pairing_apply(gt, g1, g2, pairing);
	printElement(gt);

	element_clear(g1);
	element_clear(g2);
	element_clear(h1);
	element_clear(gt);
}

/// Group classes implementation

void PbcProxy::G1::initElement(Element^ element, Pairing^ pairing)
{
	element_init_G1(element->m_element, pairing->m_pairing);
}

void PbcProxy::G2::initElement(Element^ element, Pairing^ pairing)
{
	element_init_G2(element->m_element, pairing->m_pairing);
}

void PbcProxy::Zn::initElement(Element^ element, Pairing^ pairing)
{
	element_init_Zr(element->m_element, pairing->m_pairing);
}