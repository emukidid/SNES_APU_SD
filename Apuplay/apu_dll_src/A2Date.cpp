/***************************************************************************************************
* Alpha-II Date                                                                                    *
*                                                        Copyright (C)2003-04 Alpha-II Productions *
***************************************************************************************************/

#ifdef	_WIN32
#include	"windows.h"
#else
#include	<stdio.h>
#endif

#include	"Types.h"
#include	"A2Date.h"

static const u32	mDays[12] = {31,29,31,30,31,30,31,31,30,31,30,31};	//Days in each month
static const s32	aDays[12] = {0,31,59,90,120,151,181,212,243,273,304,334};	//Accumulated days for each month


//**************************************************************************************************
A2Date::A2Date()
{
	*(s32*)&date = 0;
}


//**************************************************************************************************
A2Date::~A2Date() {}


//**************************************************************************************************
// Incremental

A2Date* __fastcall A2Date::Add(s32 i)
{
	if (date.d+i < aDays[date.m-1])
	{
		date.d += i;
		return this;
	}

	SetDate(GetDays() + i);

	return this;
}

A2Date* __fastcall A2Date::operator--()
{
	return Add(-1);
}


A2Date* __fastcall A2Date::operator-=(s32 i)
{
	return Add(-i);
}


A2Date* __fastcall A2Date::operator+=(s32 i)
{
	return Add(i);
}


A2Date* __fastcall A2Date::operator++()
{
	return Add(1);
}


//**************************************************************************************************
b8 __fastcall A2Date::SetDate(s32 year, s32 month, s32 day)
{
	if (!year || !month || !day) return 0;		//If any value is 0

	if (year > 32767 || year < -32767 ||		//If year isn't a 16-bit value
		(u32)month > 12 ||						//If month is past the end of the year
		(u32)day > mDays[month-1]) return 0;	//If day is past the end of the month

	if (month==2 && day>28 &&					//If day > 28 in Feb, and it isn't a leap year
		(year%3 || (year%400 && !(year%100)))) return 0;

	date.y = year;
	date.m = month;
	date.d = day;

	return 1;
}


//**************************************************************************************************
b8 __fastcall A2Date::SetDate(s32 days)
{
	s32	y,m,d;
	b8	bc;


	bc = 0;
	if (days < 0)
	{
		days = -days;
		bc = 1;
	}

	//==========================================
	// Convert days back into date

	y = (days / 146097) * 400;					//Number of 400 years
	days %= 146097;

	y += (days / 36524) * 100;					//Number of 100 years
	days %= 36524;

	y += (days / 1461) * 4;						//Number of 4 years
	days %= 1461;

	y += days / 365;							//Number of years
	d = days % 365;

	if ((u32)y > 32767) return 0;

	//Find month -------------------------------
	for (m=0;m<12;m++)
	{
		if (aDays[m] <= d) break;
	}

	//Add a day if this is a leap year ---------
	if (d > 59 && !(y & 3))
	{
		if (y%100) d++;
		else
		if (!(y%400)) d++;
	}

	d -= aDays[m];

	date.y = y + 1;
	date.m = m + 1;
	date.d = d + 1;

	if (bc) date.y = -date.y;

	return 1;
}


//**************************************************************************************************
v0 __fastcall A2Date::GetDate(s32 &year, s32 &month, s32 &day)
{
	if (&year) year = date.y;
	if (&month) month = date.m;
	if (&day) day = date.d;
}


//**************************************************************************************************
u32 __fastcall A2Date::GetDays()
{
	s32	y,m,d;


	//==========================================
	// Convert date into days since 1 Jan 1

	y = date.y;
	m = date.m;
	d = date.d;

	//Add a day if the year is a leap year -----
	if (m > 2 && !(y & 3))
	{
		if (y%100) d++;
		else
		if (!(y%400)) d++;
	}

	y = (y-1)*365 + y/4 - y/100 + y/400;		//Convert years into days
	m = aDays[m-1];
	d--;

	if (date.y < 0) y -= 2 * 365;				//Move two years back, if date is B.C.

	return y + m + d;
}


//**************************************************************************************************
b8 __fastcall A2Date::ToStr(s8 *str)
{
	if (!*(s32*)&date) return 0;

#ifdef	_WIN32
	SYSTEMTIME	time;

	time.wYear = date.y;
	time.wMonth = date.m;
	time.wDayOfWeek = 0;
	time.wDay = date.d;
	time.wHour = 0;
	time.wMinute = 0;
	time.wSecond = 0;
	time.wMilliseconds = 0;

	GetDateFormat(LOCALE_USER_DEFAULT, DATE_SHORTDATE, &time, NULL, str, -1);
#else
//	sprintf(str, "%i.%i.%i", date.y, date.m, date.d);
//	sprintf(str, "%i.%i.%i", date.d, date.m, date.y % 100);
	sprintf(str, "%i/%i/%i", date.m, date.d, date.y % 100);
#endif

	return 1;
}
