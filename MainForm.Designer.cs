namespace CompileWatchdog {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.quitButton = new System.Windows.Forms.Button();
			this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lastOutputTextBox = new System.Windows.Forms.TextBox();
			this.lastErrorTextBox = new System.Windows.Forms.TextBox();
			this.compileCommandTextBox = new System.Windows.Forms.TextBox();
			this.compileNowButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// fileSystemWatcher1
			// 
			this.fileSystemWatcher1.EnableRaisingEvents = true;
			this.fileSystemWatcher1.SynchronizingObject = this;
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.Text = "Compile Watchdog";
			this.notifyIcon1.Click += new System.EventHandler(this.notifyIcon1_Click);
			// 
			// quitButton
			// 
			this.quitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.quitButton.Location = new System.Drawing.Point(639, 482);
			this.quitButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.quitButton.Name = "quitButton";
			this.quitButton.Size = new System.Drawing.Size(156, 57);
			this.quitButton.TabIndex = 0;
			this.quitButton.Text = "&Quit";
			this.quitButton.UseVisualStyleBackColor = true;
			this.quitButton.Click += new System.EventHandler(this.quitButton_Click);
			// 
			// checkedListBox1
			// 
			this.checkedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkedListBox1.FormattingEnabled = true;
			this.checkedListBox1.Location = new System.Drawing.Point(13, 35);
			this.checkedListBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.checkedListBox1.Name = "checkedListBox1";
			this.checkedListBox1.Size = new System.Drawing.Size(368, 436);
			this.checkedListBox1.TabIndex = 1;
			this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
			this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
			this.checkedListBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.checkedListBox1_KeyDown);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 9);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(341, 21);
			this.label1.TabIndex = 2;
			this.label1.Text = "Watched directories – drag a directory in to add:";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.lastOutputTextBox);
			this.groupBox1.Controls.Add(this.lastErrorTextBox);
			this.groupBox1.Controls.Add(this.compileCommandTextBox);
			this.groupBox1.Location = new System.Drawing.Point(389, 35);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupBox1.Size = new System.Drawing.Size(406, 437);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Compile command";
			this.groupBox1.Visible = false;
			// 
			// lastOutputTextBox
			// 
			this.lastOutputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lastOutputTextBox.Font = new System.Drawing.Font("Consolas", 14.25F);
			this.lastOutputTextBox.Location = new System.Drawing.Point(8, 143);
			this.lastOutputTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.lastOutputTextBox.Multiline = true;
			this.lastOutputTextBox.Name = "lastOutputTextBox";
			this.lastOutputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.lastOutputTextBox.Size = new System.Drawing.Size(390, 143);
			this.lastOutputTextBox.TabIndex = 2;
			// 
			// lastErrorTextBox
			// 
			this.lastErrorTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lastErrorTextBox.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lastErrorTextBox.Location = new System.Drawing.Point(8, 296);
			this.lastErrorTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.lastErrorTextBox.Multiline = true;
			this.lastErrorTextBox.Name = "lastErrorTextBox";
			this.lastErrorTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.lastErrorTextBox.Size = new System.Drawing.Size(390, 131);
			this.lastErrorTextBox.TabIndex = 1;
			// 
			// compileCommandTextBox
			// 
			this.compileCommandTextBox.Font = new System.Drawing.Font("Consolas", 14.25F);
			this.compileCommandTextBox.Location = new System.Drawing.Point(9, 31);
			this.compileCommandTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.compileCommandTextBox.Multiline = true;
			this.compileCommandTextBox.Name = "compileCommandTextBox";
			this.compileCommandTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.compileCommandTextBox.Size = new System.Drawing.Size(389, 101);
			this.compileCommandTextBox.TabIndex = 0;
			this.compileCommandTextBox.TextChanged += new System.EventHandler(this.compileCommandTextBox_TextChanged);
			// 
			// compileNowButton
			// 
			this.compileNowButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.compileNowButton.Location = new System.Drawing.Point(13, 482);
			this.compileNowButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.compileNowButton.Name = "compileNowButton";
			this.compileNowButton.Size = new System.Drawing.Size(156, 57);
			this.compileNowButton.TabIndex = 4;
			this.compileNowButton.Text = "&Compile all now";
			this.compileNowButton.UseVisualStyleBackColor = true;
			this.compileNowButton.Click += new System.EventHandler(this.compileNowButton_Click);
			// 
			// MainForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(808, 553);
			this.Controls.Add(this.compileNowButton);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkedListBox1);
			this.Controls.Add(this.quitButton);
			this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Compile Watchdog by Zdeněk Gromnica";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
			this.DragOver += new System.Windows.Forms.DragEventHandler(this.MainForm_DragOver);
			((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.IO.FileSystemWatcher fileSystemWatcher1;
		private System.Windows.Forms.NotifyIcon notifyIcon1;
		private System.Windows.Forms.Button quitButton;
		private System.Windows.Forms.CheckedListBox checkedListBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button compileNowButton;
		private System.Windows.Forms.TextBox compileCommandTextBox;
		private System.Windows.Forms.TextBox lastOutputTextBox;
		private System.Windows.Forms.TextBox lastErrorTextBox;
	}
}

