#include <stdint.h> // for intptr_t
#ifndef _WIN32
#include <sys/time.h>
#endif
#include "pbc_utils.h"

#ifdef _WIN32
typedef struct timeval {
	long tv_sec;
	long tv_usec;
} timeval;
#endif

typedef struct _FILETIME {
  unsigned long dwLowDateTime;
  unsigned long dwHighDateTime;
} FILETIME;

void __stdcall GetSystemTimeAsFileTime(FILETIME*);

int __cdecl gettimeofday(struct timeval* p, void* tz) {
  UNUSED_VAR(tz);
  union {
    long long ns100; // Time since 1 Jan 1601 in 100ns units.
    FILETIME ft;
  } _now;

  GetSystemTimeAsFileTime( &(_now.ft) );
  p->tv_usec=(long)((_now.ns100 / 10LL) % 1000000LL );
  p->tv_sec= (long)((_now.ns100-(116444736000000000LL))/10000000LL);
  return 0; // Assume success?
}

double pbc_get_time(void) {
  static struct timeval last_tv, tv;
  static int first = 1;
  static double res = 0;

  if (first) {
    gettimeofday(&last_tv, NULL);
    first = 0;
    return 0;
  } else {
    gettimeofday(&tv, NULL);
    res += tv.tv_sec - last_tv.tv_sec;
    res += (tv.tv_usec - last_tv.tv_usec) / 1000000.0;
    last_tv = tv;

    return res;
  }
}
