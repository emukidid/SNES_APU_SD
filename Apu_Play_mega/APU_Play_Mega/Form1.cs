using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace APU_Play_Mega
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string ReadLine(int timeout = 1000)
        {
            serialPort.ReadTimeout = timeout;
            try
            {
                return serialPort.ReadLine();
            }
            catch
            {
                serialPort.ReadTimeout = 5000;
                return string.Empty;
            }
            
        }

        

        private static string IntToHex(int val,bool bytelen)
        {
            return val.ToString(bytelen ? "X2" : "X4");
        }

        private int _filetreedepth;
        private const int MaxDepth = 20;
        private readonly Dictionary<int, string>[] _direntries = new Dictionary<int, string>[MaxDepth];
        private bool _autoPlaySet;

        private void MakeDirectoryListing(string entry)
        {
            if (!entry.StartsWith("----START OF DIRECTORY LISTING----")) return;
            
            _direntries[_filetreedepth].Clear();
            var direntry = ReadLine();
            var dircount = 0;
            while (!direntry.StartsWith("----END OF DIRECTORY LISTING----"))
            {
                _direntries[_filetreedepth].Add(dircount++, direntry.Split('|')[1]);
                direntry = ReadLine();
            }
            PopulateDirTree();
        }

        private void PopulateDirTree()
        {
            DirectoryView.Nodes.Clear();
            for (var i = 0; i < _direntries[_filetreedepth].Count; i++)
            {
                DirectoryView.Nodes.Add(IntToHex(i, _direntries[_filetreedepth].Count <= 256), _direntries[_filetreedepth][i]);
            }
            DirectoryView.Sort();
            if (_filetreedepth > 0)
                DirectoryView.Nodes.Insert(0, "..", "..");
        }

        private bool IsSerialPortClosed()
        {
            return OpenSerialPort.Text == @"Open Serial Port";
        }

        private void OpenSerialPort_Click(object sender, EventArgs e)
        {
            if (IsSerialPortClosed())
            {
                timer1.Enabled = false;
                serialPort.PortName = comPortSelector.SelectedItem.ToString();
                try
                {
                    serialPort.Open();
                }
                catch
                {
                    textOutput.Text = @"Selected serial port is currently in use. Close whatever application is currently using it and try again.";
                    return;
                }

                
                try
                {
                    serialPort.ReadTimeout = 5000;
                    Text = serialPort.ReadLine();
                    serialPort.ReadTimeout = 1000;
                    while (!serialPort.ReadLine().Contains("Initializing SD card"))
                    {
                    }
                }
                catch
                {
                    Text = "";
                    serialPort.Close();
                    textOutput.Text = @"Sorry, this loader only works on the APU Mega shield. :(";
                    return;
                }
                MakeDirectoryListing(ReadLine());
                
                

                //50 milliseconds for reading a bunch of lines for the directory entries.
                //5 seconds for uploading spcs, and waiting for a response, and during initial serial port open
                //  as it takes 2 seconds for the arduino bootloader to jump into the sketch code.

                textOutput.Text = @"Serial Port " + comPortSelector.SelectedItem + @" Opened Successfully" + Environment.NewLine;
                OpenSerialPort.Text = @"Close Serial Port";
                comPortSelector.Enabled = false;
                RefreshSerialPorts.Enabled = false;

                timer1.Enabled = true;
            }
            else
            {
                _filetreedepth = 0;
                DirectoryView.Nodes.Clear();
                _direntries[0].Clear();
                serialPort.Close();
                OpenSerialPort.Text = @"Open Serial Port";
                comPortSelector.Enabled = true;
                RefreshSerialPorts.Enabled = true;
            }

        }

        private void UploadSPCRam(BinaryReader filestream)
        {
            timer1.Enabled = false;
            var dataBytes = new byte[66048];
            var tagBytes = new byte[0x7E02];
            filestream.Read(dataBytes, 0x10000, 0x100);
            filestream.Read(dataBytes, 0, 0x10000);
            filestream.Read(dataBytes, 0x10100, 0x100);
            var tagSize = filestream.Read(tagBytes, 2, 0x7E00);
            tagBytes[0] = (byte)(tagSize >> 8);
            tagBytes[1] = (byte) (tagSize & 0xFF);


            InitProgressBar(66, 0, 0);

            filestream.Close();
            serialPort.Write("D0U");

            for (var i = 0; i < 256; i += 4)
            {
                UpdateProgressBar();
                serialPort.Write(dataBytes, i * 0x100, 0x400);
            }
            serialPort.Write(dataBytes, 0x10000, 0x200);
            UpdateProgressBar();

            serialPort.Write(tagBytes, 0, tagSize + 2);
            UpdateProgressBar();

            serialPort.Write("D1");

            HideProgressBar();

            timer1.Enabled = true;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private void ClearTags()
        {
            OSTDiscTrackLabel.Text = "";
            GameTitleLabel.Text = "";
            SongArtistLabel.Text = "";
            CommentsLabel.Text = "";
            SongTitleLabel.Text = "";
            DumperLabel.Text = "";
            OSTTitleLabel.Text = "";
            PublisherLabel.Text = "";
            CopyrightYearLabel.Text = "";
            FadeoutTimeLabel.Text = "";
            PlayTimeLabel.Text = "";
        }

        private void PlaySPC()
        {
            serialPort.Write("P");
        }

        private void LoadAndPlaySPC_Click(object sender, EventArgs e)
        {
            if (IsSerialPortClosed()) return;
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            UploadSPCRam(new BinaryReader(openFileDialog.OpenFile()));
            PlaySPC();
            textOutput.AppendText("File " + openFileDialog.FileName + " is now playing" + Environment.NewLine);
        }

        private void LoadSPC_Click(object sender, EventArgs e)
        {
            if (IsSerialPortClosed()) return;
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            UploadSPCRam(new BinaryReader(openFileDialog.OpenFile()));
            serialPort.Write(autoPlay.Checked ? "A1" : "A0");
            if (autoPlay.Checked == _autoPlaySet) ReadLine();
            _autoPlaySet = autoPlay.Checked;
            textOutput.AppendText("File " + openFileDialog.FileName + " loaded into SRAM" + Environment.NewLine);
        }

        private void PlayLoadedSPC_Click(object sender, EventArgs e)
        {
            if (IsSerialPortClosed()) return;
            PlaySPC();
        }

        private void ResetAPU_Click(object sender, EventArgs e)
        {
            if (IsSerialPortClosed()) return;
            serialPort.Write("sA0");
            textOutput.AppendText(ReadLine() + Environment.NewLine);
            if (autoPlay.Checked != _autoPlaySet) ReadLine();
            _autoPlaySet = false;
        }

        private void InitProgressBar(int maximum, int discardlines, int discardbytes)
        {
            uploadlabel.Visible = true;
            uploadprogress.Visible = true;
            uploadprogress.Value = 0;
            uploadprogress.Maximum = maximum;
            for (var i = 0; i < discardlines; i++)
                ReadLine();
            for (var i = 0; i < discardbytes; i++)
                serialPort.ReadByte();
        }

        private void UpdateProgressBar()
        {
            if (uploadprogress.Value == uploadprogress.Maximum) return;
            try
            {
                var chr = (char)serialPort.ReadChar();
                if (chr != '.')
                {
                    textOutput.AppendText(chr.ToString());
                    uploadprogress.Value = uploadprogress.Maximum;
                    HideProgressBar();
                    return;
                }
            }
            catch
            {
                return;
            }
            uploadprogress.Value++;
            Application.DoEvents();
        }

        private void HideProgressBar()
        {
            uploadlabel.Visible = false;
            uploadprogress.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (IsSerialPortClosed()) return;

            if (uploadlabel.Visible)
            {
                
                while ((serialPort.BytesToRead > 0) && (uploadprogress.Value < uploadprogress.Maximum))
                {
                    UpdateProgressBar();
                }
                if (uploadprogress.Value < uploadprogress.Maximum) return;
                if (!uploadprogress.Visible) return;    //There was an error uploading the spc. Abort and output.
                Application.DoEvents();
                ReadLine();
                ReadLine();
                HideProgressBar();
            }
            else
            {
                var str = string.Empty;

                while (serialPort.BytesToRead > 0)
                {
                    var line = ReadLine();

                    if (line.StartsWith("----START OF SPC TAGS----"))
                    {
                        while (!line.StartsWith("----END OF SPC TAGS----")  && (line != string.Empty))
                        {
                            var prevLine = line;
                            line = ReadLine();
                            if (line.StartsWith("----END OF SPC TAGS----") || (line == string.Empty)) break;
                            if (!line.Contains(':'))
                            {
                                textOutput.AppendText(prevLine + Environment.NewLine);
                                textOutput.AppendText(line + Environment.NewLine);
                                line = string.Empty;
                                continue;
                            }
                            switch (line.Split(':')[0])
                            {
                                case "OST Disc/Track #":
                                    OSTDiscTrackLabel.Text =
                                        line.Split(new[] {"OST Disc/Track #: "}, StringSplitOptions.None)[1];
                                    continue;
                                case "SPC File name":
                                    textOutput.AppendText("File " +
                                                          line.Split(new[] {"SPC File name: "},
                                                              StringSplitOptions.None)
                                                              [1] +
                                                          " Now playing from SD" + Environment.NewLine);
                                    continue;
                                case "Song Name":
                                    GameTitleLabel.Text =
                                        line.Split(new[] {"Song Name: "}, StringSplitOptions.None)[1];
                                    continue;
                                case "Game":
                                    SongTitleLabel.Text = line.Split(new[] {"Game: "}, StringSplitOptions.None)[1];
                                    continue;
                                case "Artists":
                                    SongArtistLabel.Text =
                                        line.Split(new[] {"Artists: "}, StringSplitOptions.None)[1];
                                    continue;
                                case "Dumper":
                                    DumperLabel.Text = line.Split(new[] {"Dumper: "}, StringSplitOptions.None)[1];
                                    continue;
                                case "Comments":
                                    CommentsLabel.Text =
                                        line.Split(new[] {"Comments: "}, StringSplitOptions.None)[1];
                                    line = ReadLine();
                                    while (!line.StartsWith("----End of Comments----"))
                                    {
                                        CommentsLabel.Text += Environment.NewLine + line;
                                        line = ReadLine();
                                    }
                                    continue;
                                case "Original Soundtrack Title":
                                    OSTTitleLabel.Text =
                                        line.Split(new[] {"Original Soundtrack Title: "}, StringSplitOptions.None)[1
                                            ];
                                    continue;
                                case "Publisher name":
                                    PublisherLabel.Text =
                                        line.Split(new[] {"Publisher name: "}, StringSplitOptions.None)[1];
                                    continue;
                                case "Copyright Year":
                                    CopyrightYearLabel.Text =
                                        line.Split(new[] {"Copyright Year: "}, StringSplitOptions.None)[1];
                                    continue;
                                case "Play Time":
                                    PlayTimeLabel.Text =
                                        line.Split(new[] {"Play Time: "}, StringSplitOptions.None)[1];
                                    continue;
                                case "Fadeout Time":
                                    FadeoutTimeLabel.Text =
                                        line.Split(new[] {"Fadeout Time: "}, StringSplitOptions.None)[1];
                                    continue;
                                default:
                                    textOutput.AppendText(prevLine + Environment.NewLine);
                                    textOutput.AppendText(line + Environment.NewLine);
                                    line = string.Empty;
                                    break;
                            }
                        }
                        continue;
                    }


                    if (line.Contains("]|") && line.StartsWith("["))
                    {
                        var key = line.Split('[')[1].Split(']')[0];
                        var text = line.Split('|')[1];
                        var nodes = DirectoryView.Nodes.Find(key, false);
                        foreach (var node in nodes.Where(node => node.Text == text))
                        {
                            DirectoryView.SelectedNode = node;
                            break;
                        }
                        continue;
                    }


                    if (!line.Contains("APU Reset Complete"))
                        str += line + Environment.NewLine;
                    else
                    {
                        InitProgressBar(63,2,17);
                        ClearTags();
                        break;
                    }
                }
                textOutput.AppendText(str);
                Application.DoEvents();
            }

            
        }

        private void DirectoryView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (IsSerialPortClosed()) return;
            if (DirectoryView.SelectedNode == null) return;

            timer1.Enabled = false;
            var leaving = DirectoryView.SelectedNode.Name == "..";
                serialPort.Write( leaving ?
                    "D0lD1" :
                    (autoPlay.Checked ? "E" : "e") + DirectoryView.SelectedNode.Name);



            if (leaving)
            {
                _filetreedepth--;
                PopulateDirTree();
            }
            else
            {
                ReadLine();
                var command = ReadLine();
                if (command.StartsWith("----LOAD AND PLAY SPC----"))
                {
                    timer1.Enabled = true;
                }
                else if (command.StartsWith("----START OF DIRECTORY LISTING----"))
                {
                    _filetreedepth++;
                    MakeDirectoryListing(command);
                }
            }

            timer1.Enabled = true;
        }

        private void RefreshPorts_Click(object sender, EventArgs e)
        {
            var ports = SerialPort.GetPortNames();
            comPortSelector.Items.Clear();
            foreach (var port in ports)
                comPortSelector.Items.Add(port);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HideProgressBar();
            for (var i = 0; i < MaxDepth; i++)
                _direntries[i] = new Dictionary<int, string>();
            RefreshPorts_Click(sender, e);
            DirectoryView.Nodes.Clear();
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
