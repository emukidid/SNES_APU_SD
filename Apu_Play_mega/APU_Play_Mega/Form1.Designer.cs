namespace APU_Play_Mega
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.OpenSerialPort = new System.Windows.Forms.Button();
            this.LoadAndPlaySPC = new System.Windows.Forms.Button();
            this.textOutput = new System.Windows.Forms.TextBox();
            this.LoadSPC = new System.Windows.Forms.Button();
            this.PlayLoadedSPC = new System.Windows.Forms.Button();
            this.ResetAPU = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.DirectoryView = new System.Windows.Forms.TreeView();
            this.autoPlay = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.comPortSelector = new System.Windows.Forms.ComboBox();
            this.RefreshSerialPorts = new System.Windows.Forms.Button();
            this.uploadlabel = new System.Windows.Forms.Label();
            this.uploadprogress = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label11 = new System.Windows.Forms.Label();
            this.CommentsLabel = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.OSTTitleLabel = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SongArtistLabel = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SongTitleLabel = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.GameTitleLabel = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.PublisherLabel = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.CopyrightYearLabel = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.DumperLabel = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.PlayTimeLabel = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.FadeoutTimeLabel = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.OSTDiscTrackLabel = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // serialPort
            // 
            this.serialPort.BaudRate = 250000;
            this.serialPort.DtrEnable = true;
            this.serialPort.RtsEnable = true;
            this.serialPort.WriteBufferSize = 32768;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.Filter = "SPC Files|*.spc|All Files|*.*";
            // 
            // OpenSerialPort
            // 
            this.OpenSerialPort.Location = new System.Drawing.Point(3, 3);
            this.OpenSerialPort.Name = "OpenSerialPort";
            this.OpenSerialPort.Size = new System.Drawing.Size(119, 21);
            this.OpenSerialPort.TabIndex = 0;
            this.OpenSerialPort.Text = "Open Serial Port";
            this.OpenSerialPort.UseVisualStyleBackColor = true;
            this.OpenSerialPort.Click += new System.EventHandler(this.OpenSerialPort_Click);
            // 
            // LoadAndPlaySPC
            // 
            this.LoadAndPlaySPC.Location = new System.Drawing.Point(3, 3);
            this.LoadAndPlaySPC.Name = "LoadAndPlaySPC";
            this.LoadAndPlaySPC.Size = new System.Drawing.Size(119, 23);
            this.LoadAndPlaySPC.TabIndex = 2;
            this.LoadAndPlaySPC.Text = "Load and Play SPC";
            this.LoadAndPlaySPC.UseVisualStyleBackColor = true;
            this.LoadAndPlaySPC.Click += new System.EventHandler(this.LoadAndPlaySPC_Click);
            // 
            // textOutput
            // 
            this.textOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textOutput.Location = new System.Drawing.Point(3, 393);
            this.textOutput.Multiline = true;
            this.textOutput.Name = "textOutput";
            this.textOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textOutput.Size = new System.Drawing.Size(671, 140);
            this.textOutput.TabIndex = 3;
            // 
            // LoadSPC
            // 
            this.LoadSPC.Location = new System.Drawing.Point(128, 3);
            this.LoadSPC.Name = "LoadSPC";
            this.LoadSPC.Size = new System.Drawing.Size(118, 23);
            this.LoadSPC.TabIndex = 4;
            this.LoadSPC.Text = "Load Next SPC";
            this.LoadSPC.UseVisualStyleBackColor = true;
            this.LoadSPC.Click += new System.EventHandler(this.LoadSPC_Click);
            // 
            // PlayLoadedSPC
            // 
            this.PlayLoadedSPC.Location = new System.Drawing.Point(252, 3);
            this.PlayLoadedSPC.Name = "PlayLoadedSPC";
            this.PlayLoadedSPC.Size = new System.Drawing.Size(105, 23);
            this.PlayLoadedSPC.TabIndex = 5;
            this.PlayLoadedSPC.Text = "Play SPC";
            this.PlayLoadedSPC.UseVisualStyleBackColor = true;
            this.PlayLoadedSPC.Click += new System.EventHandler(this.PlayLoadedSPC_Click);
            // 
            // ResetAPU
            // 
            this.ResetAPU.Location = new System.Drawing.Point(363, 3);
            this.ResetAPU.Name = "ResetAPU";
            this.ResetAPU.Size = new System.Drawing.Size(87, 23);
            this.ResetAPU.TabIndex = 6;
            this.ResetAPU.Text = "Reset APU";
            this.ResetAPU.UseVisualStyleBackColor = true;
            this.ResetAPU.Click += new System.EventHandler(this.ResetAPU_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // DirectoryView
            // 
            this.DirectoryView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DirectoryView.Location = new System.Drawing.Point(680, 73);
            this.DirectoryView.Name = "DirectoryView";
            this.tableLayoutPanel2.SetRowSpan(this.DirectoryView, 2);
            this.DirectoryView.Size = new System.Drawing.Size(333, 460);
            this.DirectoryView.TabIndex = 7;
            this.DirectoryView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.DirectoryView_NodeMouseDoubleClick);
            // 
            // autoPlay
            // 
            this.autoPlay.AutoSize = true;
            this.autoPlay.Location = new System.Drawing.Point(353, 3);
            this.autoPlay.Name = "autoPlay";
            this.autoPlay.Size = new System.Drawing.Size(71, 17);
            this.autoPlay.TabIndex = 8;
            this.autoPlay.Text = "Auto Play";
            this.autoPlay.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this.OpenSerialPort);
            this.flowLayoutPanel1.Controls.Add(this.comPortSelector);
            this.flowLayoutPanel1.Controls.Add(this.RefreshSerialPorts);
            this.flowLayoutPanel1.Controls.Add(this.autoPlay);
            this.flowLayoutPanel1.Controls.Add(this.uploadlabel);
            this.flowLayoutPanel1.Controls.Add(this.uploadprogress);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1010, 29);
            this.flowLayoutPanel1.TabIndex = 9;
            // 
            // comPortSelector
            // 
            this.comPortSelector.FormattingEnabled = true;
            this.comPortSelector.Location = new System.Drawing.Point(128, 3);
            this.comPortSelector.Name = "comPortSelector";
            this.comPortSelector.Size = new System.Drawing.Size(118, 21);
            this.comPortSelector.TabIndex = 0;
            // 
            // RefreshSerialPorts
            // 
            this.RefreshSerialPorts.Location = new System.Drawing.Point(252, 3);
            this.RefreshSerialPorts.Name = "RefreshSerialPorts";
            this.RefreshSerialPorts.Size = new System.Drawing.Size(95, 21);
            this.RefreshSerialPorts.TabIndex = 10;
            this.RefreshSerialPorts.Text = "Refresh Ports";
            this.RefreshSerialPorts.UseVisualStyleBackColor = true;
            this.RefreshSerialPorts.Click += new System.EventHandler(this.RefreshPorts_Click);
            // 
            // uploadlabel
            // 
            this.uploadlabel.Location = new System.Drawing.Point(430, 0);
            this.uploadlabel.Name = "uploadlabel";
            this.uploadlabel.Size = new System.Drawing.Size(88, 24);
            this.uploadlabel.TabIndex = 7;
            this.uploadlabel.Text = "Upload Progress:";
            this.uploadlabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uploadprogress
            // 
            this.uploadprogress.Location = new System.Drawing.Point(524, 3);
            this.uploadprogress.Maximum = 80;
            this.uploadprogress.Name = "uploadprogress";
            this.uploadprogress.Size = new System.Drawing.Size(227, 23);
            this.uploadprogress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.uploadprogress.TabIndex = 11;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.67F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.DirectoryView, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.textOutput, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 68.88412F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 31.11588F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1016, 536);
            this.tableLayoutPanel2.TabIndex = 10;
            this.tableLayoutPanel2.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel2_Paint);
            // 
            // flowLayoutPanel2
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.flowLayoutPanel2, 2);
            this.flowLayoutPanel2.Controls.Add(this.LoadAndPlaySPC);
            this.flowLayoutPanel2.Controls.Add(this.LoadSPC);
            this.flowLayoutPanel2.Controls.Add(this.PlayLoadedSPC);
            this.flowLayoutPanel2.Controls.Add(this.ResetAPU);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 38);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(1010, 29);
            this.flowLayoutPanel2.TabIndex = 11;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.CommentsLabel, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label5, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.OSTTitleLabel, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.SongArtistLabel, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.SongTitleLabel, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.GameTitleLabel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label6, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.PublisherLabel, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.label7, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.CopyrightYearLabel, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.DumperLabel, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.PlayTimeLabel, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label9, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.FadeoutTimeLabel, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.label10, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.OSTDiscTrackLabel, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 73);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(671, 314);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Location = new System.Drawing.Point(4, 181);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(69, 132);
            this.label11.TabIndex = 10;
            this.label11.Text = "Comments:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CommentsLabel
            // 
            this.CommentsLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel1.SetColumnSpan(this.CommentsLabel, 3);
            this.CommentsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CommentsLabel.Location = new System.Drawing.Point(80, 184);
            this.CommentsLabel.Multiline = true;
            this.CommentsLabel.Name = "CommentsLabel";
            this.CommentsLabel.ReadOnly = true;
            this.CommentsLabel.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.CommentsLabel.Size = new System.Drawing.Size(587, 126);
            this.CommentsLabel.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(344, 1);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 35);
            this.label5.TabIndex = 4;
            this.label5.Text = "OST Title:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // OSTTitleLabel
            // 
            this.OSTTitleLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.OSTTitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OSTTitleLabel.Location = new System.Drawing.Point(410, 4);
            this.OSTTitleLabel.Multiline = true;
            this.OSTTitleLabel.Name = "OSTTitleLabel";
            this.OSTTitleLabel.ReadOnly = true;
            this.OSTTitleLabel.Size = new System.Drawing.Size(257, 29);
            this.OSTTitleLabel.TabIndex = 25;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(4, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 35);
            this.label3.TabIndex = 2;
            this.label3.Text = "Song Artist:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SongArtistLabel
            // 
            this.SongArtistLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SongArtistLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SongArtistLabel.Location = new System.Drawing.Point(80, 112);
            this.SongArtistLabel.Multiline = true;
            this.SongArtistLabel.Name = "SongArtistLabel";
            this.SongArtistLabel.ReadOnly = true;
            this.SongArtistLabel.Size = new System.Drawing.Size(257, 29);
            this.SongArtistLabel.TabIndex = 23;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(4, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 35);
            this.label2.TabIndex = 1;
            this.label2.Text = "Song Title:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SongTitleLabel
            // 
            this.SongTitleLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SongTitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SongTitleLabel.Location = new System.Drawing.Point(80, 76);
            this.SongTitleLabel.Multiline = true;
            this.SongTitleLabel.Name = "SongTitleLabel";
            this.SongTitleLabel.ReadOnly = true;
            this.SongTitleLabel.Size = new System.Drawing.Size(257, 29);
            this.SongTitleLabel.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(4, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "Game Title:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GameTitleLabel
            // 
            this.GameTitleLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GameTitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GameTitleLabel.Location = new System.Drawing.Point(80, 40);
            this.GameTitleLabel.Multiline = true;
            this.GameTitleLabel.Name = "GameTitleLabel";
            this.GameTitleLabel.ReadOnly = true;
            this.GameTitleLabel.Size = new System.Drawing.Size(257, 29);
            this.GameTitleLabel.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(344, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 35);
            this.label6.TabIndex = 12;
            this.label6.Text = "Publisher:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PublisherLabel
            // 
            this.PublisherLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PublisherLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PublisherLabel.Location = new System.Drawing.Point(410, 40);
            this.PublisherLabel.Multiline = true;
            this.PublisherLabel.Name = "PublisherLabel";
            this.PublisherLabel.ReadOnly = true;
            this.PublisherLabel.Size = new System.Drawing.Size(257, 29);
            this.PublisherLabel.TabIndex = 26;
            // 
            // label7
            // 
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(344, 73);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 35);
            this.label7.TabIndex = 13;
            this.label7.Text = "Copyright Year:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CopyrightYearLabel
            // 
            this.CopyrightYearLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CopyrightYearLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CopyrightYearLabel.Location = new System.Drawing.Point(410, 76);
            this.CopyrightYearLabel.Multiline = true;
            this.CopyrightYearLabel.Name = "CopyrightYearLabel";
            this.CopyrightYearLabel.ReadOnly = true;
            this.CopyrightYearLabel.Size = new System.Drawing.Size(257, 29);
            this.CopyrightYearLabel.TabIndex = 27;
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(344, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 35);
            this.label4.TabIndex = 3;
            this.label4.Text = "Dumper:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DumperLabel
            // 
            this.DumperLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DumperLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DumperLabel.Location = new System.Drawing.Point(410, 112);
            this.DumperLabel.Multiline = true;
            this.DumperLabel.Name = "DumperLabel";
            this.DumperLabel.ReadOnly = true;
            this.DumperLabel.Size = new System.Drawing.Size(257, 29);
            this.DumperLabel.TabIndex = 24;
            // 
            // label8
            // 
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(4, 145);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 35);
            this.label8.TabIndex = 14;
            this.label8.Text = "Play Time:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PlayTimeLabel
            // 
            this.PlayTimeLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PlayTimeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlayTimeLabel.Location = new System.Drawing.Point(80, 148);
            this.PlayTimeLabel.Multiline = true;
            this.PlayTimeLabel.Name = "PlayTimeLabel";
            this.PlayTimeLabel.ReadOnly = true;
            this.PlayTimeLabel.Size = new System.Drawing.Size(257, 29);
            this.PlayTimeLabel.TabIndex = 28;
            // 
            // label9
            // 
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Location = new System.Drawing.Point(344, 145);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 35);
            this.label9.TabIndex = 15;
            this.label9.Text = "Fadeout Time:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FadeoutTimeLabel
            // 
            this.FadeoutTimeLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FadeoutTimeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FadeoutTimeLabel.Location = new System.Drawing.Point(410, 148);
            this.FadeoutTimeLabel.Multiline = true;
            this.FadeoutTimeLabel.Name = "FadeoutTimeLabel";
            this.FadeoutTimeLabel.ReadOnly = true;
            this.FadeoutTimeLabel.Size = new System.Drawing.Size(257, 29);
            this.FadeoutTimeLabel.TabIndex = 29;
            // 
            // label10
            // 
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Location = new System.Drawing.Point(4, 1);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(69, 35);
            this.label10.TabIndex = 30;
            this.label10.Text = "OST Disc / Track #:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // OSTDiscTrackLabel
            // 
            this.OSTDiscTrackLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.OSTDiscTrackLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OSTDiscTrackLabel.Location = new System.Drawing.Point(80, 4);
            this.OSTDiscTrackLabel.Multiline = true;
            this.OSTDiscTrackLabel.Name = "OSTDiscTrackLabel";
            this.OSTDiscTrackLabel.ReadOnly = true;
            this.OSTDiscTrackLabel.Size = new System.Drawing.Size(257, 29);
            this.OSTDiscTrackLabel.TabIndex = 31;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 536);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button OpenSerialPort;
        private System.Windows.Forms.Button LoadAndPlaySPC;
        private System.Windows.Forms.TextBox textOutput;
        private System.Windows.Forms.Button LoadSPC;
        private System.Windows.Forms.Button PlayLoadedSPC;
        private System.Windows.Forms.Button ResetAPU;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TreeView DirectoryView;
        private System.Windows.Forms.CheckBox autoPlay;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ComboBox comPortSelector;
        private System.Windows.Forms.Button RefreshSerialPorts;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label uploadlabel;
        private System.Windows.Forms.ProgressBar uploadprogress;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox CommentsLabel;
        private System.Windows.Forms.TextBox GameTitleLabel;
        private System.Windows.Forms.TextBox SongTitleLabel;
        private System.Windows.Forms.TextBox SongArtistLabel;
        private System.Windows.Forms.TextBox DumperLabel;
        private System.Windows.Forms.TextBox OSTTitleLabel;
        private System.Windows.Forms.TextBox PublisherLabel;
        private System.Windows.Forms.TextBox CopyrightYearLabel;
        private System.Windows.Forms.TextBox PlayTimeLabel;
        private System.Windows.Forms.TextBox FadeoutTimeLabel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox OSTDiscTrackLabel;
    }
}

