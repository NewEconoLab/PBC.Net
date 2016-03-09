#include "pch.h"
#include "Debug.h"

void PbcProxy::Debug::Log(const wchar_t *format, ...)
{
	wchar_t buffer[BUFFER_LENGTH];
	va_list args;
	va_start(args, format);
	_vsnwprintf_s(buffer, BUFFER_LENGTH, _TRUNCATE, format, args);
	va_end(args);
	buffer[BUFFER_LENGTH - 1] = '\0'; //prevent buffer overflow
	OutputDebugString(buffer);
}

void PbcProxy::printElement(element_t e) {
	unsigned int len = element_length_in_bytes(e);
	unsigned char* gt_bytes = new unsigned char[len];
	element_to_bytes(gt_bytes, e);

	size_t buffer_size = (len + 1) * 2;
	wchar_t* buffer = new wchar_t[buffer_size];
	buffer[buffer_size - 1] = '\0';

	for (unsigned int i = 0; i < len; ++i) {
		_snwprintf_s(buffer + i, 2, _TRUNCATE, L"%x", (unsigned int)gt_bytes[i]);
	}

	OutputDebugString(buffer);
	OutputDebugString(L"\n");

	delete gt_bytes;
	delete buffer;
} // end of print_element