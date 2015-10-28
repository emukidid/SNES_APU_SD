/***************************************************************************************************
* String Functions                                                                                 *
*                                                           Copyright (C)2003 Alpha-II Productions *
***************************************************************************************************/

#include	"Types.h"

//Turn off warning "functions have no return value"
#if defined	__BORLANDC__
#pragma	inline
#pragma	warn -rvl
#elif defined	_MSC_VER
#pragma warning(disable : 4035)
#endif

//**************************************************************************************************
// Reverse Scan String for Character

s8* ScanStrR(const s8 *pStr, s8 c)
{
#ifdef	__GNUC__
	s8	*pRet;


	asm("
		xorl	%0,%0
		movl	-1,%%ecx
		repne	scasb
		decl	%%edi
		negl	%%ecx
		decl	%%ecx

		std
		movb	%2,%%al
		repne	scasb
		cld
		sete	%%al
		addl	%%edi,%0
		incl	%0

	" : "=a" (pRet) : "D" (pStr), "m" (c) : "ecx");

	return pRet;
#else
	__asm
	{
		//Find end of string -------------------
		mov		edi,[pStr]
		xor		eax,eax
		mov		ecx,-1
		repne	scasb
		dec		edi								//EDI -> NULL terminator
		neg		ecx								//ECX = Length of string, including NULL
		dec		ecx

		//Search for character -----------------
		std										//Search backwards
		mov		al,[c]
		repne	scasb
		cld
		sete	al								//EAX = 1 if character found, 0 otherwise
		add		eax,edi							//Add pointer
		inc		eax								//Increase by one to fixup post decrement
	}
#endif
}


//**************************************************************************************************
// Compare String with Length

b8 CmpStrL(const s8 *pDest, const s8 *pSrc, u32 len)
{
#ifdef	__GNUC__
	b8	ret;


	asm("
		xorb	%0,%0
		repe	cmpsb
		test	%3,%3
		setz	%%al
		
	" : "=a" (ret) : "D" (pDest), "S" (pSrc), "c" (len));

	return ret;
#else
	__asm
	{
		xor		eax,eax
		mov		edi,[pDest]
		mov		esi,[pSrc]
		mov		ecx,[len]
		repe	cmpsb
		test	ecx,ecx
		setz	al
	}
#endif
}


//**************************************************************************************************
// Compare and Copy String with Length

b8 CmpCopyL(s8 *pDest, const s8 *pSrc, u32 len)
{
#ifdef	__GNUC__
	b8	ret;


	asm("
		xorb	%0,%0

	1:
		movb	(%2),%0
		cmpb	(%1),%0
		jne		1f
		test	%0,%0
		jz		2
		incl	%2
		incl	%1
		decl	%3
		jnz		1b

	2:
		movb	$1,%%al
		jmp		0

	1:
		movb	(%2),%0
		incl	%2
		movb	%0,(%1)
		incl	%1
		testb	%0,%0
		jz		0
		decl	%%ecx
		jnz		1

		movb	$0,%0
		movb	%0,(%1)
	0:

	" : "=a" (ret) : "D" (pDest), "S" (pSrc), "c" (len));

	return ret;
#else
	__asm
	{
		mov		edi,[pDest]
		mov		esi,[pSrc]
		mov		ecx,[len]
		xor		eax,eax

	NextCCL:
		mov		al,[esi]
		cmp		al,[edi]
		jne		short NotEqual
		test	al,al
		jz		short Equal
		inc		esi
		inc		edi
		dec		ecx
		jnz		short NextCCL

	Equal:
		mov		al,1
		jmp		short DoneCCL

	NotEqual:
		mov		al,[esi]
		inc		esi
		mov		[edi],al
		inc		edi
		test	al,al
		jz		short DoneCCL
		dec		ecx
		jnz		short NotEqual

		mov		al,0
		mov		[edi],al
	DoneCCL:
	}
#endif
}


//**************************************************************************************************
// Copy String

s8* CopyStr(s8 *pDest, const s8 *pSrc)
{
#ifdef	__GNUC__
	s8	*pRet;


	asm("
	1:
		movb	(%2),%%al
		incl	%2
		movb	%%al,(%1)
		incl	%1
		testb	%%al,%%al
		jnz		1
		leal	-1(%1),%0

	" : "=a" (pRet) : "D" (pDest), "S" (pSrc));

	return pRet;
#else
	__asm
	{
		mov		esi,[pSrc]
		mov		edi,[pDest]
	NextCS:
		mov		al,[esi]
		inc		esi
		mov		[edi],al
		inc		edi
		test	al,al
		jnz		short NextCS
		lea		eax,[edi-1]
	}
#endif
}


//**************************************************************************************************
// Copy String with Length

s8* CopyStrL(s8 *pDest, const s8 *pSrc, s32 l)
{
#ifdef	__GNUC__
	s8	*pRet;

	asm("
		movl	%3,%%eax
		cdq
		xorl	%%edx,%%eax
		subl	%%edx,%%eax
		movl	%%eax,%%ecx

	1:
		movb	(%2),%%al
		incl	%2
		movb	%%al,(%1)
		testb	%%al,%%al
		jz		2
		incl	%1
		decl	%%ecx
		jnz		1
		jmp		0

	2:
		testb	$0x80,3+%3
		js		0

		movb	' ',%%al
		rep		stosb

	0:
		movb	$0,(%1)

	" : "=D" (pRet) : "D" (pDest), "S" (pSrc), "m" (l) : "eax", "edx", "ecx");

	return pRet;
#else
	__asm
	{
		mov		esi,[pSrc]
		mov		edi,[pDest]
		mov		eax,[l]
		cdq
		xor		eax,edx
		sub		eax,edx
		mov		ecx,eax

	NextCSL:
		mov		al,[esi]
		inc		esi
		mov		[edi],al
		test	al,al
		jz		short DoneCSL
		inc		edi
		dec		ecx
		jnz		short NextCSL
		jmp		short QuitCSL

	DoneCSL:
		test	byte ptr [3+l],0x80
		js		short QuitCSL

		mov		al,' '
		rep		stosb

	QuitCSL:
		mov		byte ptr [edi],0
		mov		eax,edi
	}
#endif
}


//**************************************************************************************************
// Copy String Upto Character

s8* CopyStrC(s8 *pDest, const s8 *pSrc, s8 c)
{
#ifdef	__GNUC__
	s8	*pRet;


	asm("
		jmp		2
	1:
		movb  	%%al,(%1)
		incl  	%1
	2:
		movb	(%2),%%al
		incl	%2
		cmpb	%3,%%al
		je		0
		testb	%%al,%%al
		jnz		1

	0:
		movb	$0,(%1)
		movl	%1,%0
		
	" : "=a" (pRet) : "D" (pDest), "S" (pSrc), "d" (c));

	return pRet;
#else
	__asm
	{
		mov		esi,[pSrc]
		mov		edi,[pDest]
		mov		dl,[c]
		jmp		short StartCSC

	NextCSC:
		mov		[edi],al
		inc		edi
	StartCSC:
		mov		al,[esi]
		inc		esi
		cmp		al,dl
		je		short DoneCSC
		test	al,al
		jnz		short NextCSC

	DoneCSC:
		mov		byte ptr [edi],0
		mov		eax,edi
	}
#endif
}


//**************************************************************************************************
// Find String End

s8* StrEnd(const s8 *pStr)
{
#ifdef	__GNUC__
	s8	*pRet;


	asm("
		xorl	%0,%0
		repne	scasb
		leal	-1(%1),%%eax

	" : "=a" (pRet) : "D" (pStr), "c" (-1));

	return pRet;
#else
	__asm
	{
		mov		edi,[pStr]
		xor		eax,eax
		mov		ecx,-1
		repne	scasb
		lea		eax,[edi-1]
	}
#endif
}


//**************************************************************************************************
// Convert Integer to Hex

s8*	Int2Hex(u32 i, u32 d)
{
	static s8	hex[16] = {"0123456789ABCDEF"};
	static s8	str[9];

	str[d] = 0;
	while(d--)
	{
		str[d] = hex[i & 0xF];
		i >>= 4;
	}

	return str;
}
