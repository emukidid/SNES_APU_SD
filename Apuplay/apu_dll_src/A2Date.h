/***************************************************************************************************
* Class:      Date                                                                                 *
* Platform:   C++                                                                                  *
* Programmer: Anti Resonance                                                                       *
*                                                                                                  *
* This program is free software; you can redistribute it and/or modify it under the terms of the   *
* GNU General Public License as published by the Free Software Foundation; either version 2 of     *
* the License, or (at your option) any later version.                                              *
*                                                                                                  *
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;        *
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.        *
* See the GNU General Public License for more details.                                             *
*                                                                                                  *
* You should have received a copy of the GNU General Public License along with this program;       *
* if not, write to the Free Software Foundation, Inc.                                              *
* 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.                                        *
*                                                                                                  *
*                                                        Copyright (C)2003-04 Alpha-II Productions *
***************************************************************************************************/


//**************************************************************************************************
// Public Variables
#include "Types.h"

#ifndef _A2DATE_H
#define _A2DATE_H 1

class A2Date
{
private:
	A2Date*	__fastcall Add(s32);

	struct
	{
		u8	d;
		u8	m;
		s16	y;
	} date;

public:
	A2Date();
	A2Date(s32 year, u32 month, u32 day);
	~A2Date();

	//**********************************************************************************************
	// Overloaded operators

	inline b8 operator<(A2Date &a2date)  {return *(s32*)&date < *(s32*)&a2date.date;}
	inline b8 operator<=(A2Date &a2date) {return *(s32*)&date <= *(s32*)&a2date.date;}
	inline b8 operator==(A2Date &a2date) {return *(s32*)&date == *(s32*)&a2date.date;}
	inline b8 operator>=(A2Date &a2date) {return *(s32*)&date >= *(s32*)&a2date.date;}
	inline b8 operator>(A2Date &a2date)  {return *(s32*)&date > *(s32*)&a2date.date;}
	inline b8 operator!=(A2Date &a2date) {return *(s32*)&date != *(s32*)&a2date.date;}

	A2Date*	__fastcall operator--();
	A2Date*	__fastcall operator-=(s32);
	A2Date*	__fastcall operator+=(s32);
	A2Date*	__fastcall operator++();


	//**********************************************************************************************
	// Set Date

	inline A2Date* operator=(const A2Date &a2date) {*(s32*)&date = *(s32*)&a2date.date;	return this;}

	//------------------------------------------
	// In:
	//    Packed s32

	inline A2Date* operator=(s32 pd) {*(s32*)&date = pd; return this;}


	//------------------------------------------
	// In:
	//    year  = Year (1 - 9999)
	//    month = Month (1 - 12)
	//    day   = Day (1 - 31)
	//
	// Out:
	//    true, if date was valid

	b8	__fastcall SetDate(s32 year, s32 month, s32 day);


	//------------------------------------------
	// In:
	//    days = Number of days since 1 Jan 1
	//
	// Out:
	//    true, if date was valid

	b8	__fastcall SetDate(s32 days);


	//------------------------------------------
	// Invalidates the date

	inline v0 Invalidate() {*(s32*)&date = 0;}


	//**********************************************************************************************
	// Get Date

	// -----------------------------------------
	// In:
	//    year  -> Year
	//    month -> Month
	//    day   -> Day
	//
	// Out:
	//    Fields will contain 0's, if object doesn't contain a valid date

	inline operator s32() {return *(s32*)&date;}//Returns the date represented as a packed s32
	v0	__fastcall GetDate(s32 &year, s32 &month, s32 &day);
	u32	__fastcall GetDays();


	//**********************************************************************************************
	// Convert Date to String
	//
	// In:
	//    str -> String to store date
	//
	// Out:
	//    true, if a date was copied

	b8	__fastcall ToStr(s8 *str);
};

#endif
