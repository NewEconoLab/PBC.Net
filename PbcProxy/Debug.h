#pragma once

#include <pbc.h>

namespace PbcProxy {

	class Debug {
	public:
		static const size_t BUFFER_LENGTH = 512;
		static void Log(const wchar_t* format, ...);
	};

	void printElement(element_t e);

}