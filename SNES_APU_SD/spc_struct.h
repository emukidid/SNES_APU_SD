
typedef char s8;

typedef struct  {
	u8	header[33];
	u8	pad_26[2];
	u8	id3_tag_present;
	u8	version_minor;
} spc_header;

typedef struct {
	u8	cpu_pcl;
	u8	cpu_pch;
	u8	cpu_a;
	u8	cpu_x;
	u8	cpu_y;
	u8	cpu_psw;
	u8	cpu_sp;
	u16	reserved;
} spc_cpu_regs;

typedef struct {
	s8	song_title[32];
	s8	game_title[32];
	s8	dumper_name[16];
	s8	comments[32];
	s8	date_dumped[11];
	s8	song_length_secs[3];
	s8	fade_length_ms[5];
	s8	song_artist[32];
	u8	channel_disable;
	u8	emulator;
	u8	reserved[45];
} spc_id666_text;

typedef struct {
	s8	song_title[32];
	s8	game_title[32];
	s8	dumper_name[16];
	s8	comments[32];
	u32	date_dumped;
	u8	unused[7];
	u8	song_length_secs[3];
	u8	fade_length_ms[4];
	s8	song_artist[32];
	u8	channel_disable;
	u8	emulator;
	u8	reserved[46];
} spc_id666_bin;

typedef struct {
	u8	ram[65536];
	u8	dsp_regs[128];
	u8	unused[64];
	u8	extra_ram[64];
} spc_ram_store;

typedef struct {
	s8	song_title[256];
	s8	game_title[256];
	s8	song_artist[256];
	s8	dumper_name[256];
  s8  comments[256];
  s8  ost_title[256];
  s8  pub_name[256];
  s8  spc_filename[256];
  
	u32 date;
	u8	emulator;
	u8	ost_disc;
	u16	ost_track;
	u16	copyright;
	u32	intro_len;
	u32	fade_len;
  u16 boot_code;
} spc_idx6_table;

typedef struct {
	u8 header[4];
	u32 size;
	u8 data[65536];
} spc_idx6_header;

typedef struct {
	u8 ID;
	u8 Type;
	u16 Length;
	union {
		u8 data[4];
		u32 val;
	};
} spc_idx6_sub_header;

typedef struct {
	spc_header		header;		// 37 bytes
	spc_cpu_regs	cpu_regs;	// 9 bytes
	union { 
		spc_id666_text	tag_text; 
		spc_id666_bin	tag_binary;
	}; // 210 bytes
	// 256 bytes so far

	spc_ram_store	ram_dumps;	// 65536 bytes

	spc_idx6_table	extended;

	// the following variables aren't part of the file, 
	// but are helpful in storing information about it
	u8				tag_format;
	u32				date;	// normalized for SPC2		
	u32				song_length;
	u32				fade_length;
	u16				boot_code;
	u8				mask_file_opened;
	u8				mask[32]; //pages that are flagged as in use.
} spc_struct;

typedef struct {
  spc_header    header;   // 37 bytes
  spc_cpu_regs  cpu_regs; // 9 bytes
  union { 
    spc_id666_text  tag_text; 
    spc_id666_bin tag_binary;
  }; // 210 bytes
  // 256 bytes so far

  
  u8  dsp_regs[128];
  u8  unused[64];
  u8  extra_ram[64];
  // 512 bytes so far

  
  spc_idx6_header idx6;
} spc_sram_struct;

#define	SPC_TAG_TEXT	1
#define SPC_TAG_BINARY	2
#define SPC_TAG_PREFER_BINARY 3

enum {
	IDX6_SONGNAME = 0x1,
	IDX6_GAMENAME,
	IDX6_ARTISTNAME,
	IDX6_DUMPERNAME,
	IDX6_DATEDUMPED,
	IDX6_EMULATOR,
	IDX6_COMMENTS,
	IDX6_OSTTITLE = 0x10,
	IDX6_OSTDISC,
	IDX6_OSTTRACK,
	IDX6_PUBNAME,
	IDX6_COPYRIGHT,
	IDX6_INTROLEN = 0x30,
	IDX6_LOOPLEN,
	IDX6_ENDLEN,
	IDX6_FADELEN,
	IDX6_MUTECHAN,
	IDX6_LOOPNUM,
	IDX6_AMPVAL
};

