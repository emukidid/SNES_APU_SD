/***************************************************************************************************
* ID666 Tag Manager                                                                                *
*                                                      Copyright (C)2001-2003 Alpha-II Productions *
***************************************************************************************************/

#include	<string.h>
#include	<stdio.h>
#include	<stdlib.h>

#include	"Types.h"
#include	"A2Date.h"
#include	"A2Str.h"
#include	"ID666.h"

//File header text
const s8			emuList[XID6_EMULIST][12] =
		{"unknown","ZSNES","Snes9x","ZST2SPC","ETC","SNEShout","ZSNESW"};
static const s8 	spcID[] = {"SNES-SPC700 Sound File Data v0.30"};
static const s8 	spcTerm[] = {0x1a,0x1a,0x1a,0x1e};
static const s8 	xid6[4] = {'x','i','d','6'};
static const s8 	zstID[] = {"ZSNES Save State File"};
static const s8		sfcID[] = {"GAME DOCTOR SF 3"};
static const u32	romTag[] = {0x7FC0, 0xFFC0, 0x81C0, 0x101C0};
#if ID666_ROM
extern const char romMaker[][32];
#endif

static ROMHdr		romID;

u32	ID666::defSong;
u32	ID666::defFade;
b8	ID666::preferBin;


////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructors and Destructors

//**************************************************************************************************
ID666::ID666()
{
	bin = false;

	file[0] = 0;

	mute = 0;
	amp = 0;
}


//**************************************************************************************************
ID666::~ID666() {}


////////////////////////////////////////////////////////////////////////////////////////////////////
// Private Functions

//**************************************************************************************************
v0 __fastcall ID666::GetID666(SPCHdr &spc)
{
	s32	i,j,k;
	u32	d,m,y;
	s8	str[36],*t;
	s8	songStr[4];								//Song in seconds
	s8	fadeStr[6];								//Fade in ms


	//Copy known fields ------------------------
	str[32] = 0;
	memcpy(song, spc.song, 32);
	song[32] = 0;
	memcpy(game, spc.game, 32);
	game[32] = 0;
	memcpy(dumper, spc.dumper, 16);
	dumper[16] = 0;
	memcpy(comment, spc.comment, 32);
	comment[32] = 0;
	memcpy(songStr, spc.songLen, 3);
	memcpy(fadeStr, spc.fadeLen, 5);
	mute = 0;
	amp = 0;
	date.Invalidate();

	//Determine tag type -----------------------
	i = IsText(songStr, 3);
	j = IsText(fadeStr, 5);
	k = IsText(spc.date, 11);
	if (!(i | j | k))							//If no times or date, use default
	{
		if (spc.chnDis == 1 && spc.emulator == 0)	//Was the file dumped with ZSNES?
			bin = true;
		else
			bin = preferBin;
	}
	else
	if (i != -1 && j != -1)						//If no time, or time is text
	{
		if (k > 0)
			bin = false;						//If date is text, then tag is text
		else
		if (k == 0)								//No date
			bin = preferBin;					//Times could still be binary (ex. 56 bin = '8' txt)
		else
		if (k == -1)							//Date contains invalid characters
		{
			if (!((u32*)&spc.date)[1])			//If bytes 4-7 are 0's, it's probably a ZSNES dump
				bin = true;
			else
				bin = false;					//Otherwise date may contain alpha characters
		}
	}
	else
		bin = true;								//If time is not text, tag is binary

	//Get fields dependant on tag type ---------
	if (bin)
	{
		j = *(u32*)(fadeStr);
		if (j > 59999) j = 59999;

		i = *(u16*)(songStr);
		if (i > 959) i = 959;

		intro = i;
		fade = j;

		memcpy(artist, spc.artist-1, 32);
		artist[32] = 0;
		emu = spc.chnDis;

		datetxt[0] = 0;
		y = *(u16*)(&spc.date[2]);
		m = *(u8*)(&spc.date[1]);
		d = *(u8*)(&spc.date[0]);
	}
	else
	{
		intro = atoi(songStr);
		fade = atoi(fadeStr);
		if (intro > 959) intro = 959;
		if (fade > 59999) fade = 59999;

		memcpy(artist,spc.artist,32);
		artist[32] = 0;
		emu = spc.emulator;
		if (emu>=0x30 && emu<=0x39)
			emu -= 0x30;
		else
			emu = 0;

		memcpy(datetxt,spc.date,11);
		y = 0;
		strcpy(str,datetxt);
		if (str[0] != 0)
		{
			t = strchr(str,'/');
			if (t && *(++t)!=0)
			{
				d = atoi(t);
				t = strchr(t,'/');
				if (t && *(++t)!=0)
				{
					y = atoi(t);
					m = atoi(str);
				}
			}
		}
	}
	SetDate(y,m,d);

	//Change song/fade length to ticks ---------
	intro *= XID6_TICKSSEC;
	fade <<= 6;

	if (!intro) fade = 0;
}


//**************************************************************************************************
b8 __fastcall ID666::GetXID6(XID6Chk *pTag, s32 size)
{
	s32		i,r;
#ifdef	TIDY
	s32		sb;
	XID6Chk	*ptb;
	b8		align;


	ptb = pTag;
	sb = size;
	align = 0;

TryAgain:
	pTag = ptb;
	size = sb;
#endif
	while(size > 0)								//While there's data left to be read
	{
#ifdef	TIDY
		r = pTag->val;
		if (align) r = (r + 3) & ~3;
#else
		r = (pTag->val + 3) & ~3;
#endif

		size -= 4;

		switch(pTag->type)
		{
		case(XID6_TSTR):
			if (pTag->val < 1 || pTag->val > 0x100) break;

			switch(pTag->id)
			{
			case(XID6_SONG):
				memcpy(song, pTag->data, size > r ? r : size);
				song[pTag->val-1] = 0;		//Force string to null terminate, incase tag is bad
				break;

			case(XID6_GAME):
				memcpy(game, pTag->data, size > r ? r : size);
				game[pTag->val-1] = 0;
				break;

			case(XID6_ARTIST):
				memcpy(artist, pTag->data, size > r ? r : size);
				artist[pTag->val-1] = 0;
				break;

			case(XID6_PUB):
				memcpy(pub, pTag->data, size > r ? r : size);
				pub[pTag->val-1] = 0;
				break;

			case(XID6_OST):
				memcpy(ost, pTag->data, size > r ? r : size);
				ost[pTag->val-1] = 0;
				break;

			case(XID6_DUMPER):
				memcpy(dumper, pTag->data, size > r ? r : size);
				dumper[pTag->val-1] = 0;
				break;

			case(XID6_CMNTS):
				memcpy(comment, pTag->data, size > r ? r : size);
				comment[pTag->val-1] = 0;
				break;
#ifdef	TIDY
			default:
				if (align && size == 4) goto Done;
#endif
			}
			break;

		case(XID6_TINT):
			if (pTag->val != 4) break;

			switch(pTag->id)
			{
			case(XID6_DATE):
				i = *pTag->data;
				SetDate(i>>16, (i>>8) & 0xFF, i & 0xFF);
				break;

			case(XID6_INTRO):
				intro = *pTag->data;
				if (intro > XID6_MAXTICKS) intro = XID6_MAXTICKS;
				break;

			case(XID6_LOOP):
				loop = *pTag->data;
				if (loop > XID6_MAXTICKS) loop = XID6_MAXTICKS;
				break;

			case(XID6_END):
				end = *pTag->data;
				if (end > XID6_MAXTICKS) end = XID6_MAXTICKS;
				break;

			case(XID6_FADE):
				fade = *pTag->data;
				if (fade > XID6_TICKSMIN-1) fade = XID6_TICKSMIN-1;
				break;

			case(XID6_AMP):
				amp = *pTag->data;
				if (amp < 32768) amp = 32768;
				if (amp > 524288) amp = 524288;
				break;
#ifdef	TIDY
			default:
				if (align && size == 4) goto Done;
#endif
			}
			break;

		case(XID6_TVAL):
			switch(pTag->id)
			{
			case(XID6_EMU):
				emu = (s8)pTag->val;
				break;

			case(XID6_DISC):
				disc = (u8)pTag->val;
				if (disc > 9) disc = 9;
				break;

			case(XID6_TRACK):
				track = pTag->val;
				if (((track>>8)-1) > 98) track = 0;
				break;

			case(XID6_COPY):
				copy = pTag->val;
				break;

			case(XID6_MUTE):
				mute = (u8)pTag->val;
				break;

			case(XID6_LOOPX):
				loopx = pTag->val;
				if (loopx < 1) loopx = 1;
				if (loopx > 9) loopx = 9;
				break;

			case(XID6_AMP):						//Old way of storing
				amp = (u32)pTag->val << 12;
				if (amp < 32768) amp = 32768;
				if (amp > 524288) amp = 524288;
				break;
#ifdef	TIDY
			default:
				if (align && size == 4) goto Done;
#endif
			}
			break;
#ifdef	TIDY
		default:
			if (align && size == 4) goto Done;
#endif
		}

		if (pTag->type != XID6_TVAL)
		{
			pTag = (XID6Chk*)((s32)pTag + r);
			size -= r;
		}

		pTag++;									//Skip sub-chunk header
	}

Done:
	if (end < -((s32)loop))						//If end is negative and |end| > loop
		end = loop ? (end % (s32)loop) : 0;

#ifdef	TIDY
	if (size != 0)
	{
		if (!align)
		{
			align = 1;
			goto TryAgain;
		}

		if (size == 4) printf("Bad tail?  ");
		else           printf("**corrupt  ");
	}
#endif

	return (size == 0);
}


//**************************************************************************************************
s32 __fastcall ID666::IsText(s8 *str, u32 length)
{
	u32	c = 0;

	while (c<length && ((str[c]>=0x30 && str[c]<=0x39) || str[c]=='/')) c++;
	if (c==length || str[c]==0)
		return c;
	else
		return -1;
}


//**************************************************************************************************
XID6Chk* __fastcall ID666::WriteVal(XID6Chk *pChk, u8 id, u16 val)
{
	if (val != 0)
	{
		pChk->id = id;
		pChk->type = XID6_TVAL;
		pChk->val = val;
		pChk++;
	}

	return pChk;
}


//**************************************************************************************************
XID6Chk* __fastcall ID666::WriteInt(XID6Chk *pChk, u8 id, u32 data)
{
	if (data != 0)
	{
		pChk->id = id;
		pChk->type = XID6_TINT;
		pChk->val = 4;

		((u32*)pChk)[1] = data;
		pChk += 2;
	}

	return pChk;
}


//**************************************************************************************************
XID6Chk* __fastcall ID666::WriteStr(XID6Chk *pChk, u8 id, const s8 *str)
{
	u32		i,j;


	i = strlen(str);
	if (i != 0)
	{
		j = (i + 4) >> 2;

		pChk->id = id;
		pChk->type = XID6_TSTR;
		pChk->val = i + 1;

		((u32*)pChk)[j] = 0;
		memcpy(&pChk[1], str, i);
		pChk += j + 1;
	}

	return pChk;
}


////////////////////////////////////////////////////////////////////////////////////////////////////
// Public Functions

//**************************************************************************************************
ID6Type __fastcall ID666::LoadTag(const s8 *fn, const u32 size)
{
	SPCHdr	spc;
	ID6Type	type;
	FILE	*fh;
	u32		i,n;
	XID6Chk	*pTag;


	if (!size)
		type = IsSPC(fn, fh);
	else
		type = IsSPC(fn, size);

	if (type > 0)
	{
		if (!size) strcpy(file, fn);

		if (type==ID6_SPC || type==ID6_EXT)
		{
			bin = false;

			if (!size)
			{
				fread(&spc, sizeof(SPCHdr), 1, fh);
				GetID666(spc);
			}
			else
				GetID666(*(SPCHdr*)fn);

			ost[0]	= 0;
			pub[0]	= 0;
			copy	= 0;
			disc	= 0;
			track	= 0;
			amp		= 0;
			loop	= 0;
			end		= 0;
			loopx	= 1;
			mute	= 0;

			if (type == ID6_EXT)
			{
				if (!size)
				{
					fseek(fh, 0x10204, SEEK_SET);
					fread(&i, 1, 4, fh);

					//Truncate header size, if it's corrupt
					fseek(fh, 0, SEEK_END);
					n = ftell(fh) - 0x10208;
					if (i > n) i = n;

					//Load xid6 chunk into memory
					pTag = (XID6Chk*)malloc((i + 3) & ~3);
					fseek(fh, 0x10208, SEEK_SET);
					fread(pTag, i, 1, fh);
					GetXID6(pTag, i);
					free(pTag);
				}
				else
				{
					i = *(u32*)&fn[0x10204];
					if (i > (size - 0x10208)) i = size - 0x10208;

					GetXID6((XID6Chk*)&fn[0x10208], i);
				}
			}
		}
		else
		{
			song[0]		= 0;
			game[0]		= 0;
			artist[0]	= 0;
			dumper[0]	= 0;
			comment[0]	= 0;
			ost[0]		= 0;
			pub[0]		= 0;
			datetxt[0]	= 0;
			date.Invalidate();
			copy		= 0;
			track		= 0;
			disc		= 0;
			amp			= 0;
			mute		= 0;
			emu			= 0;

			intro		= defSong;
			loop		= 0;
			loopx		= 1;
			end			= 0;
			fade		= defFade;

			if (type>=ID6_ROM && type<=ID6_SF3)
			{
				game[21] = 0;
				memcpy(game, romID.name, 21);
#if	ID666_ROM
				strcpy(pub, romMaker[romID.maker]);
#else
				sprintf(pub, "%i", romID.maker);
#endif
			}
		}

		if (!size) fclose(fh);
	}

	return type;
}


//**************************************************************************************************
b8 __fastcall ID666::SaveTag(b8 bin, b8 ext)
{
	u32		xbuf[512];
	u32		*pBuf;
	XID6Chk	*p;
	FILE	*fh;
	u32		size,i;


	//Load SPC ---------------------------------
	fh = fopen(file,"rb");
	if (!fh) return 0;
	fseek(fh, 0, SEEK_END);
	size = ftell(fh);
	fseek(fh, 0, SEEK_SET);
	pBuf = (u32*)malloc(size);
	fread(pBuf, size, 1, fh);
	fclose(fh);

	//Write ID666 tag --------------------------
	ToSPC(*(SPCHdr*)pBuf, bin);					//Create SPC header

	//Write SPC back out -----------------------
	fh = fopen(file,"wb");
	if (!fh) return 0;
	fwrite(pBuf, 0x10200, 1, fh);

	//Write extended tag -----------------------
	if (ext)
	{
		p = (XID6Chk*)&xbuf[2];
		if (strlen(song) > 32)		p = WriteStr(p, XID6_SONG, song);
		if (strlen(game) > 32)		p = WriteStr(p, XID6_GAME, game);
		if (strlen(artist) > 32)	p = WriteStr(p, XID6_ARTIST, artist);
		if (strlen(dumper) > 16)	p = WriteStr(p, XID6_DUMPER, dumper);
		if (strlen(comment) > 32 ||
			strchr(comment,'\n'))	p = WriteStr(p, XID6_CMNTS, comment);
		//*** Date and emulator will always be written in the original tag ***
//		p = WriteXChk(p, XID6_EMU, id6.emu);
//		id6.date.GetDate(y,m,d);
//		p = WriteInt(p, XID6_DATE, (y<<16) | ((m&0xFF)<<8) | (d&0xFF));

		if (ost) p = WriteStr(p, XID6_OST, ost);
		p = WriteVal(p, XID6_DISC, disc);
		p = WriteVal(p, XID6_TRACK, track);
		p = WriteVal(p, XID6_COPY, copy);
		p = WriteStr(p, XID6_PUB, pub);
		p = WriteVal(p, XID6_MUTE, mute);
		p = WriteInt(p, XID6_AMP, amp);

		if (end < (s32)(0 - loop))				//If 'end' is negative, it can't be more than loop
			end = loop ? (end % (s32)loop) : 0;

		p = WriteInt(p, XID6_INTRO, intro);
		p = WriteInt(p, XID6_LOOP, loop);
		p = WriteInt(p, XID6_END, end);
		p = WriteInt(p, XID6_FADE, fade);
		if (loopx > 1)
			p = WriteVal(p, XID6_LOOPX, loopx);

		i = (u32)p - (u32)xbuf - 8;
		if (i)
		{
			xbuf[0] = *(u32*)xid6;				//Chunk ID
			xbuf[1] = i;						//Chunk size
			fwrite(xbuf, 1, i+8, fh);
		}
	}

	//Write out any extra data -----------------
	if (size > 0x10200)							//Was file larger than regular SPC size?
	{
		i = 0x10200;
		if (pBuf[0x10200 >> 2] == *(u32*)xid6)	//Did file have an extended ID666 tag?
			i += pBuf[0x10204 >> 2] + 8;		//Skip past tag chunk
		if (i < size) fwrite(&pBuf[i >> 2], 1, size-i, fh);	//If more data remains, write it out
	}

	fclose(fh);
	free(pBuf);

	return 1;
}


//**************************************************************************************************
ID666* __fastcall ID666::operator=(const ID666 &id6)
{
	strcpy(file, id6.file);

	bin		= id6.bin;

	strcpy(song, id6.song);
	strcpy(game, id6.game);
	strcpy(artist, id6.artist);
	strcpy(dumper, id6.dumper);
	strcpy(datetxt, id6.datetxt);
	date	= id6.date;
	emu		= id6.emu;
	strcpy(comment, id6.comment);
	strcpy(ost,id6.ost);
	strcpy(pub, id6.pub);
	copy	= id6.copy;
	disc	= id6.disc;
	track	= id6.track;
	mute	= id6.mute;
	amp		= id6.amp;

	intro	= id6.intro;
	loop	= id6.loop;
	end		= id6.end;
	fade	= id6.fade;
	loopx	= id6.loopx;

	return this;
}


//**************************************************************************************************
u32 ID666::GetSong()
{
	u32 time = intro + (loop * loopx) + end;

	return time ? time : defSong;
}


//**************************************************************************************************
u32 ID666::GetFade()
{
	return (intro | loop | end) ? fade : defFade;
}


//**************************************************************************************************
u32 ID666::GetTotal()
{
	u32	time = intro + (loop * loopx) + end + fade;

	return time ? time : defSong+defFade;
}


//**************************************************************************************************
b8 __fastcall ID666::FixDate(A2Date &date, u32 year, u8 month, u8 day)
{
	if (year < 100)								//Add century, if not included in year
	{
		year += 1900;
		if (year < 1998) year += 100;
	}

	if (year < 1998 || year > 9999) year = 0;	//Invaild year
	if (month<1 || month>12) year = 0;			//Invalid month
	if (day<1 || day>31) year = 0;				//Invalid day
	if (year == 1998)	  						//SPC's didn't exist before 15 Apr 1998
		if ((month==4 && day<15) || month<4) year = 0;

	if (year)
	{
/*
		if (month > 2)
		{
			if (!(year&3) && year%100) day++;	//Add a day if the year is a leap year
			else
			if (!(year%400)) day++;
		}
		month--;
		year--;
		year = year*365 + year/4 - year/100 + year/400;	//Convert years into days

		date = s32(year + aDays[month] + day - 693594);	//eDate = Number of days since 30 Dec 1899
*/
		date.SetDate(year,month,day);
		return true;
	}
	else
	{
		date.Invalidate();
		return false;
	}
}


//**************************************************************************************************
v0 __fastcall ID666::ToSPC(SPCHdr &spc, b8 bin)
{
	s8	*cr;
	s32	d,m,y;
	u32	i;

	strcpy(spc.fTag,spcID);
	memcpy(spc.tTerm,spcTerm,4);
	memset(spc.song,0,0xa5);

	if (bin)
	{
		i = strlen(artist);
		if (i > 32) i = 32;
		memcpy(spc.artist-1, artist, i);

		*(u32*)(&spc.songLen[0]) = intro / XID6_TICKSSEC;
		*(u32*)(&spc.fadeLen[0]) = fade >> 6;

		if ((s32)date)
		{
			*(u32*)&spc.date = (u32)date;
		}

		spc.chnDis = emu;
		spc.artist[31] = 0;
	}
	else
	{
		if ((s32)date)
		{
			d = m = y = 0;
			date.GetDate(y,m,d);
			sprintf(spc.date,"%i/%i/%i",m,d,y);
		}
		else
			strcpy(spc.date,datetxt);

		if (HasTime())
		{
			sprintf(spc.songLen,"%i",intro/XID6_TICKSSEC);
			sprintf(spc.fadeLen,"%i",fade>>6);
		}

		i = strlen(artist);
		if (i > 32) i = 32;
		memcpy(spc.artist,artist,i);
		spc.emulator = emu + 0x30;
		spc.chnDis = 0;
	}

	i = strlen(song);
	if (i > 32) i = 32;
	memcpy(spc.song,song,i);

	i = strlen(game);
	if (i > 32) i = 32;
	memcpy(spc.game,game,i);

	i = strlen(dumper);
	if (i > 16) i = 16;
	memcpy(spc.dumper,dumper,i);

	i = strlen(comment);
	if (i > 32) i = 32;
	memcpy(spc.comment,comment,i);

	cr = strchr(spc.comment,'\n');
	if (cr) *cr = 0;
}


//**************************************************************************************************
b8 ID666::IsExt()
{
	return	(strlen(song) > 32) ||
			(strlen(game) > 32) ||
			(strlen(artist) > 32) ||
			(strlen(dumper) > 32) ||
			(strlen(comment) > 32) || //TODO: Check for CR
			(intro % XID6_TICKSSEC) ||
			(ost[0]	!= 0) ||
			(pub[0]	!= 0) ||
			(copy	!= 0) ||
			(disc	!= 0) ||
			(track	!= 0) ||
			(mute	!= 0) ||
			(amp	!= 0) ||
			(loop	!= 0) ||
			(end	!= 0);
}


//**************************************************************************************************
b8 ID666::HasTime()
{
	return (intro | loop | end | fade) != 0;
}


//**************************************************************************************************
// Parse Title Format String

//--------------------------------------------------------------------------------------------------
// Copy ID666 Field
//
// Copies the string form of an ID666 field to a destination string.  A specific number of
// characters will be copied if the user specified a constraint.
//
// In:
//    pDest -> Destination string
//    pSrc  -> String to copy
//    ppFmt -> Pointer to character after field specifier in format string
//
// Out:
//    -> end of pDest string

static s8* CopyField(s8 *pDest, const s8 *pSrc, s8 **ppFmt)
{
	u32	i;
	s8	*s,f;


	i = 0;										//Required length = 0
	f = **ppFmt;								//f = length type

	if (f=='<' || f=='=' || f=='>')				//Is a length specified?
	{
		s = (*ppFmt)+1;
		i = atoi(s);							//i = number of characters to copy
		while (*s >= '0' && *s <= '9') s++;		//Skip *len past digits
		*ppFmt = s;
	}

	if (!i) return CopyStr(pDest, pSrc);		//If no length, perform a normal copy

	switch(f)
	{
	case('<'):
		return CopyStrL(pDest, pSrc, 0-i);

	case('>'):
		pDest = CopyStrL(pDest, pSrc, i);
		if (strlen(pSrc) <= i) return pDest;
		return CopyStr(pDest, pSrc+i);

	case('='):
		return CopyStrL(pDest, pSrc, i);
	}

	return NULL;
}


//--------------------------------------------------------------------------------------------------
// Find End of Conditional Statement
// 
// In:
//    str   -> Format string
//    pIf   -> Length of "if" portion of condition
//    pElse -> Length of "else" portion of condition
//
// Out:
//    Number of characters in condition

static u32 FindEnd(s8 *str, u32 *pIf, u32 *pElse)
{
	u32	len,i,j;
	s8	c;
	b8	or;


	*pIf = 0;
	*pElse = 0;

	len = 0;
	or = 0;
	while (*str)
	{
		switch(*str++)
		{
		case('%'):
			c = *str++ | 0x20;
			len += 2;

			if (*str == '[' && ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'd')))
			{
				len++;
				i = FindEnd(++str, &j, &j);
				len += i;
				str += i;
			}
			break;

		case(']'):
			if (or)
			{
				*pElse = len;
				len += *pIf + 2;
			}
			else
			{
				*pIf = len;
				len += 1;
			}

			return len;

		case('|'):
			if (!or)
			{
				*pIf = len;
				len = 0;
				or = 1;
				break;
			}

		default:
			len++;
		}
	}

	return 0;
}

#define	_CopyPath() \
			if (cnd) \
			{ \
				cpy |= ToStr(&str, pFmt, f); \
				pFmt += cnd; \
				cnd = 0; \
			}\
			else

#define	_CopyField(_field, _string, _printf) \
			if (cnd) \
			{ \
				cpy |= (_field) ? ToStr(&str, pFmt, f) : ToStr(&str, &pFmt[f+1], e); \
				pFmt += cnd; \
				cnd = 0;\
			} \
			else \
			if (_field) \
			{ \
				_printf; \
				str = CopyField(str, _string, &pFmt); \
				cpy |= 1; \
			} \
			else \
			{ \
				*str++ = '?'; \
			}

b8 __fastcall ID666::ToStr(s8 **ppStr, s8 *pFmt, const u32 len)
{
	s8		temp[12];
	u32		i;
	u32		cnd,f,e;						//Length of statement, and contained if and else
	s8		*str,*end,c;
	b8		cpy=0;							//True, if any fields were copied


	cnd = 0;								//Length of conditional statement is 0
	str = *ppStr;
	end = &pFmt[len];
	while (pFmt < end)
	{
		if (*pFmt != '%')					//Is current character a field delimiter?
		{
			*str++ = *pFmt++;
			continue;
		}

		c = pFmt[1];
		pFmt += 2;							//Move pointer to first char after field specifier

		//Check for special characters -----
		switch(c)
		{
		case('%'):
			*str++ = '%';
			continue;

		case('<'):
			*str++ = '<';
			continue;

		case('='):
			*str++ = '=';
			continue;

		case('>'):
			*str++ = '>';
			continue;

		case('['):
			*str++ = '[';
			continue;

		case(']'):
			*str++ = ']';
			continue;

		case('|'):
			*str++ = '|';
			continue;
		}

		//Check for unknown format specifier ---
		c |= 0x20;
		if ((c < '0' || c > '9') && (c < 'a' || c > 'd'))
		{
			*str++ = '%';
			pFmt--;
			continue;
		}

		if (pFmt[0] == '[') cnd = FindEnd(++pFmt, &f, &e);

		switch(c)
		{
		case('0'):							//Path
			_CopyPath()
			{
				CopyField(str, file, &pFmt);
				str = ScanStrR(str, '\\');
				cpy |= 1;
			}
			break;

		case('1'):							//Filename
			_CopyPath()
			{
				str = CopyField(str, ScanStrR(file, '\\'), &pFmt);
				cpy |= 1;
			}
			break;

		case('2'):							//Song str
			_CopyField(song[0], song,);
			break;

		case('3'):							//Game str
			_CopyField(game[0], game,);
			break;

		case('4'):							//Artist
			_CopyField(artist[0], artist,);
			break;

		case('5'):							//Publisher
			_CopyField(pub[0], pub,);
			break;

		case('6'):							//Copyright
			_CopyField(copy, temp, sprintf(temp, "©%i", copy));
			break;

		case('7'):							//OST str
			_CopyField(ost[0], ost,);
			break;

		case('8'):							//OST disc
			_CopyField(disc, temp, sprintf(temp, "%i", disc));
			break;

		case('9'):							//OST track
			_CopyField(track, temp, sprintf(temp, "%i%c", track >> 8, track & 0xFF));
			break;

		case('a'):							//Dumper
			_CopyField(dumper[0], dumper,);
			break;

		case('b'):							//Date dumped
			if (cnd)
			{
				cpy |= (s32)date ? ToStr(&str, pFmt, f) : ToStr(&str, &pFmt[f+1], e);
				pFmt += cnd;
				cnd = 0;
			}
			else
			if ((s32)date)
			{
				str += date.ToStr(str);
				cpy |= 1;
			}
			else
				_CopyField(datetxt[0], datetxt,);
			break;

		case('c'):							//Emulator used
			_CopyField(emu <= 6, emuList[emu],);
			break;

		case('d'):							//Comment
			_CopyField(comment[0], comment,);
			break;
		}
	}

	*str = 0;

	if (cpy)
	{
		*ppStr = str;
    	return 1;
	}

	return 0;
}

b8 __fastcall ID666::ToStr(s8 *str, s8 *format)
{
	if (ToStr(&str, format, strlen(format))) return 1;

	CopyStr(str, ScanStrR(file, '\\'));
	return 0;
}


//**************************************************************************************************
// Does File Contain SPC Data?

ID6Type __fastcall IsSPC(const s8 *fn, FILE* &fh)
{
	s8		header[28];
	s32		i,l;


	//Open file and read header ----------------
	fh = fopen(fn,"rb");
	if (!fh) return ID6_ERR;

	fread(header, 28, 1, fh);
	fseek(fh, 0, SEEK_END);
	l = ftell(fh);
	fseek(fh, 0 ,SEEK_SET);

	if (l < 0x10200) goto Done;					//If file is too small to be anything

	//Look for SPC header ----------------------
	if (CmpStrL(header, spcID, 28))
	{
		if (l < 0x1020C) return ID6_SPC;		//Return normal SPC

		fseek(fh, 0x10200, SEEK_SET);			//Look for extended information
		fread(&header, 1, 8, fh);
		fseek(fh, 0, SEEK_SET);

		if (*(u32*)&header[0] == *(u32*)xid6 &&
			*(u32*)&header[4] >= 4) return ID6_EXT;

		return ID6_SPC;
	}

	//Look for ZST header ----------------------
	if (CmpStrL(header, zstID, 21))
	{
		if (l < 266879)	goto Done;				//Return invalid, if file is too small

		return ID6_ZST;
	}

	//Is it a ROM image? -----------------------
	if (l < 0x40000) goto Done;					//If file is too small to be a valid ROM (2Mbit)

	for (i=0; i<4; i++)
	{
		fseek(fh, romTag[i], SEEK_SET);
		fread(&romID, 1, 32, fh);

		if ((romID.crc^romID.icrc) == 0xFFFF &&
			(romID.makeup & 0xF) == (i & 1)) break;
	}
	if (i == 4) goto Done;

	fseek(fh,0,SEEK_SET);

	if (i < 2) return ID6_ROM;

	if (*(u32*)&header[8] == 0x4BBAA) return ID6_SWC;

	if (CmpStrL(header, sfcID, 16)) return ID6_SF3;

	if ((header[2] ==   64 || header[2] == 0) &&
		(header[3] == -128 || header[3] == 0)) return ID6_FIG;

	return ID6_ROMH;

Done:
	fclose(fh);
	fh = NULL;

	return ID6_UNK;
}

ID6Type __fastcall IsSPC(const s8 *pFile, u32 size)
{
	s32	i;

	
	if (size < 0x10200) goto Done;

	//Look for SPC header ----------------------
	if (CmpStrL(pFile, spcID, 28))
	{
		if (size < 0x1020C) return ID6_SPC;

		if (*(u32*)&pFile[0x10200] == *(u32*)xid6 &&
			*(u32*)&pFile[0x10204] >= 4) return ID6_EXT;

		return ID6_SPC;
	}

	//Look for ZST header ----------------------
	if (CmpStrL(pFile, zstID, 21))
	{
		if (size < 266879) goto Done;

		return ID6_ZST;
	}

	//Is it a ROM image? -----------------------
	if (size < 0x40000) goto Done;

	for (i=0; i<4; i++)
	{
		memcpy(&romID, &pFile[romTag[i]], sizeof(ROMHdr));

		if ((romID.crc^romID.icrc) == 0xFFFF &&
			(romID.makeup & 0xF) == (i & 1)) break;
	}
	if (i == 4) goto Done;

	if (i < 2) return ID6_ROM;

	if (*(u32*)&pFile[8] == 0x4BBAA) return ID6_SWC;

	if (CmpStrL(pFile, sfcID, 16)) return ID6_SF3;

	if ((pFile[2] ==   64 || pFile[2] == 0) &&
		(pFile[3] == -128 || pFile[3] == 0)) return ID6_FIG;

	return ID6_ROMH;

Done:
	return ID6_UNK;
}


//**************************************************************************************************
// String to Ticks

u32 __fastcall Str2Ticks(const s8 *cstr)
{
	s8	str[4],*d;
	s32	s,m;


	s = atoi(cstr);								//Get integer value of number left of decimal
	if (s < 0) return 0;						//Time can't be negative
	if (s > 59) return XID6_TICKSMIN - 1;		//Time can't be greater than 1 minute

	m = 0;
	d = strchr((char*)cstr,'.');
	if (d++)									//If there's a decimal point, get fraction
	{
		*(u32*)&str[0] = 0x303030;				//Initialize str to "000"

		for (m=0; m<3; m++)
		{
			if (*d >= '0' && *d <= '9') str[m] = *d++;
			else break;
		}

		m = atoi(str);

		return (s * XID6_TICKSSEC) + (m * XID6_TICKSMS);
	}
	else
	{
		d = strchr((char*)cstr,':');
		if (d)
		{
			m = atoi(d+1);
			if (m < 0) m = 0;
			if (m > XID6_TICKSSEC - 1) m = XID6_TICKSSEC - 1;
		}

		return (s * XID6_TICKSSEC) + m;
	}
}

#if ID666_ROM
const char romMaker[][32]={
	"N/A",
	"Nintendo",
	"(2)",
	"Imagineer-Zoom",
	"(4)",
	"Zamuse",
	"Falcom",
	"(7)",
	"Capcom",
	"HOT-B",
	"Jaleco",
	"Coconuts",
	"Rage Software",
	"(13)",
	"Technos",
	"Mebio Software",
	"(16)",
	"(17)",
	"Gremlin Graphics",
	"Electronic Arts",
	"(20)",
	"COBRA Team",
	"Human/Field",
	"KOEI",
	"Hudson Soft",
	"(25)",
	"Yanoman",
	"(27)",
	"Tecmo",
	"(29)",
	"Open System",
	"Virgin Games",
	"KSS",
	"Sunsoft",
	"POW",
	"Micro World",
	"(36)",
	"(37)",
	"Enix",
	"Loriciel/Electro Brain",
	"Kemco",
	"Seta Co.,Ltd.",
	"(42)",
	"(43)",
	"(44)",
	"Visit Co.,Ltd.",
	"(46)",
	"(47)",
	"(48)",
	"Carrozzeria",
	"Dynamic",
	"Nintendo",
	"Magifact",
	"Hect",
	"(54)",
	"(55)",
	"(56)",
	"(57)",
	"(58)",
	"(59)",
	"Empire Software",
	"Loriciel",
	"(62)",
	"(63)",
	"Seika Corp.",
	"UBI Soft",
	"(66)",
	"(67)",
	"(68)",
	"(69)",
	"System 3",
	"Spectrum Holobyte",
	"(72)",
	"Irem",
	"(74)",
	"Raya Systems/Sculptured Software",
	"Renovation Products",
	"Malibu Games/Black Pearl",
	"(78)",
	"U.S. Gold",
	"Absolute Entertainment",
	"Acclaim",
	"Activision",
	"American Sammy",
	"GameTek",
	"Hi Tech Expressions",
	"LJN Toys",
	"(87)",
	"(88)",
	"(89)",
	"Mindscape",
	"(91)",
	"(92)",
	"Tradewest",
	"(94)",
	"American Softworks Corp.",
	"Titus",
	"Virgin Interactive Entertainment",
	"Maxis",
	"(99)",
	"(100)",
	"(101)",
	"(102)",
	"Ocean",
	"(104)",
	"Electronic Arts",
	"(106)",
	"Laser Beam",
	"(108)",
	"(109)",
	"Elite",
	"Electro Brain",
	"Infogrames",
	"Interplay",
	"LucasArts",
	"Parker Brothers",
	"(116)",
	"STORM",
	"(118)",
	"(119)",
	"THQ Software",
	"Accolade Inc.",
	"Triffix Entertainment",
	"(123)",
	"Microprose",
	"(125)",
	"(126)",
	"Kemco",
	"Misawa",
	"Teichio",
	"Namco Ltd.",
	"Lozc",
	"Koei",
	"(133)",
	"Tokuma Shoten Intermedia",
	"(135)",
	"DATAM-Polystar",
	"(137)",
	"(138)",
	"Bullet-Proof Software",
	"Vic Tokai",
	"(141)",
	"Character Soft",
	"I''Max",
	"Takara",
	"CHUN Soft",
	"Video System Co., Ltd.",
	"BEC",
	"(148)",
	"Varie",
	"(150)",
	"Kaneco",
	"(152)",
	"Pack in Video",
	"Nichibutsu",
	"TECMO",
	"Imagineer Co.",
	"(157)",
	"(158)",
	"(159)",
	"Telenet",
	"(161)",
	"(162)",
	"(163)",
	"Konami",
	"K.Amusement Leasing Co.",
	"(166)",
	"Takara",
	"(168)",
	"Technos Jap.",
	"JVC",
	"(171)",
	"Toei Animation",
	"Toho",
	"(174)",
	"Namco Ltd.",
	"(176)",
	"ASCII Co. Activison",
	"BanDai America",
	"(179)",
	"Enix",
	"(181)",
	"Halken",
	"(183)",
	"(184)",
	"(185)",
	"Culture Brain",
	"Sunsoft",
	"Toshiba EMI",
	"Sony Imagesoft",
	"(190)",
	"Sammy",
	"Taito",
	"(193)",
	"Kemco",
	"Square",
	"Tokuma Soft",
	"Data East",
	"Tonkin House",
	"(199)",
	"KOEI",
	"(201)",
	"Konami USA",
	"NTVIC",
	"(204)",
	"Meldac",
	"Pony Canyon",
	"Sotsu Agency/Sunrise",
	"Disco/Taito",
	"Sofel",
	"Quest Corp.",
	"Sigma",
	"(212)",
	"(213)",
	"Naxat",
	"(215)",
	"Capcom Co., Ltd.",
	"Banpresto",
	"Tomy",
	"Acclaim",
	"(220)",
	"NCS",
	"Human Entertainment",
	"Altron",
	"Jaleco",
	"(225)",
	"Yutaka",
	"(227)",
	"T&ESoft",
	"EPOCH Co.,Ltd.",
	"(230)",
	"Athena",
	"Asmik",
	"Natsume",
	"King Records",
	"Atlus",
	"Sony Music Entertainment",
	"(237)",
	"IGS",
	"(239)",
	"(240)",
	"Motown Software",
	"Left Field Entertainment",
	"Beam Software",
	"Tec Magik",
	"(245)",
	"(246)",
	"(247)",
	"(248)",
	"Cybersoft",
	"(250)",
	"(251)",
	"(252)",
	"(253)",
	"(254)",
	"Hudson Soft"};
#endif

void * __stdcall CreateID666()
{
	return new ID666;
}

void __stdcall DestroyID666(void * objptr)
{
	ID666 *id666 = (ID666 *) objptr;
	if(id666)
		delete id666;
}

b8 __stdcall ID666_LoadTag(void * objptr, const s8 *file, const u32 size)
{
	ID666 *id666 = (ID666 *) objptr;
	if(id666)
	{
		return id666->LoadTag(file,size);
	}
	else
		return ID6_ERR;
}

b8 __stdcall ID666_ToStr(void * objptr, s8 *str, s8 *format)
{
	ID666 *id666 = (ID666 *) objptr;
	if(id666)
	{
		return id666->ToStr(str,format);
	}
	else
		return 0;
}