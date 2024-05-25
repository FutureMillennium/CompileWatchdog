﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Threading;

namespace CompileWatchdog {
	public partial class MainForm : Form {
		public List<WatchedDir> watchedDirs = new List<WatchedDir>();
		List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

		DispatcherTimer timer1 = new DispatcherTimer();
		bool blLoaded = false;

		public MainForm() {
			InitializeComponent();
			LoadSettings();
			this.Icon = Properties.Resources.icon;
			this.notifyIcon1.Icon = Properties.Resources.icon;

			timer1.Interval = new TimeSpan(0, 0, 0, 0, 16);
			timer1.Tick += new EventHandler(timer1_Tick);

			var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			string ver;
			if (version.Build == 0) {
				ver = version.Major + "." + version.Minor;
			} else {
				ver = version.Major + "." + version.Minor + "." + version.Build;
			}
			this.Text = "Compile Watchdog v" + ver + " by Zdeněk Gromnica";
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
			if (e.CloseReason == CloseReason.UserClosing) {
				notifyIcon1.Visible = true;
				this.Hide();
				e.Cancel = true;
			} else {
				notifyIcon1.Visible = false;
				SaveSettings();
			}
		}

		private void quitButton_Click(object sender, EventArgs e) {
			notifyIcon1.Visible = false;
			Application.Exit();
		}

		private void MainForm_DragOver(object sender, DragEventArgs e) {
			// accept if it's a folder
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				if (files.Length == 1 && System.IO.Directory.Exists(files[0])) {
					e.Effect = DragDropEffects.Copy;
				}
			}
		}

		private void notifyIcon1_Click(object sender, EventArgs e) {
			this.Show();
			notifyIcon1.Visible = false;
		}

		private void MainForm_DragDrop(object sender, DragEventArgs e) {
			// accept if it's a folder, add to watchedDirs and checkedListBox1
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				blLoaded = false;
				for (int i = 0; i < files.Length; i++) {
					if (System.IO.Directory.Exists(files[i])) {
						WatchedDir wd = new WatchedDir();
						wd.path = files[i];
						wd.enabled = true;

						Add(wd);
					}
				}
				blLoaded = true;
			}
		}

		private void compileCommandTextBox_TextChanged(object sender, EventArgs e) {
			if (checkedListBox1.SelectedIndex >= 0) {
				watchedDirs[checkedListBox1.SelectedIndex].compileCommand = compileCommandTextBox.Text;
			}
		}

		private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e) {
			if (checkedListBox1.SelectedIndex >= 0) {
				groupBox1.Text = watchedDirs[checkedListBox1.SelectedIndex].path;
				compileCommandTextBox.Text = watchedDirs[checkedListBox1.SelectedIndex].compileCommand;
				ignoreTextBox.Text = watchedDirs[checkedListBox1.SelectedIndex].ignore;
				groupBox1.Visible = true;
			} else {
				groupBox1.Visible = false;
			}
		}

		private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e) {
			// Only un/check when clicking on the item, not on blank space
			if (blLoaded && checkedListBox1.IndexFromPoint(checkedListBox1.PointToClient(Cursor.Position).X,
			checkedListBox1.PointToClient(Cursor.Position).Y) <= -1) {
				e.NewValue = e.CurrentValue;
			}

			if (e.NewValue == CheckState.Unchecked) {
				watchedDirs[e.Index].enabled = false;
			} else {
				watchedDirs[e.Index].enabled = true;
			}
		}

		private void compileNowButton_Click(object sender, EventArgs e) {
			int nCompiled = 0;
			for (int i = 0; i < watchedDirs.Count; i++) {
				var wd = watchedDirs[i];
				if (wd.enabled) {
					nCompiled++;
					HandleCompile(wd, i);
				}
			}
			timer1.Stop();
			MyRefresh();
			if (nCompiled == 0) {
				MessageBox.Show("Nothing to compile! Add directories by dragging them into the window and make sure they're checked in the list.", "Compile Watchdog", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		void Add(WatchedDir wd, bool addToWD = true) {
			if (addToWD) {
				watchedDirs.Add(wd);
			}
			checkedListBox1.Items.Add(wd.path, wd.enabled);

			FileSystemWatcher watcher = new FileSystemWatcher();
			watcher.Path = wd.path;
			watcher.NotifyFilter = NotifyFilters.LastWrite;
			watcher.Filter = "*.*";
			watcher.IncludeSubdirectories = true;
			watcher.Changed += new FileSystemEventHandler(OnChanged);
			watcher.EnableRaisingEvents = true;
			watchers.Add(watcher);
		}

		void HandleCompile(WatchedDir wd, int index) {
			if (checkedListBox1.SelectedIndex == index) {
				lastOutputTextBox.Text = "(Compiling…)";
				this.Refresh();
			}
			Compile(wd);
			if (wd.lastError.Length > 0) {
				if (this.InvokeRequired) {
					this.Invoke(new Action(() => {
						RefocusRefresh(wd, index);
					}));
				} else {
					RefocusRefresh(wd, index);
				}
			}
		}

		void RefocusRefresh(WatchedDir wd, int index) {
			checkedListBox1.SelectedIndex = index;
			if (popUpOnErorrCheckbox.Checked) {
				this.Show();
				this.Focus();
				notifyIcon1.Visible = false;
			}
		}

		void MyRefresh() {
			if (checkedListBox1.SelectedIndex >= 0) {
				if (watchedDirs[checkedListBox1.SelectedIndex].lastOutput.Length == 0 && watchedDirs[checkedListBox1.SelectedIndex].lastError.Length == 0) {
					lastOutputTextBox.Text = "(The output was empty.)";
				} else {
					lastOutputTextBox.Text = watchedDirs[checkedListBox1.SelectedIndex].lastOutput;
				}
				lastErrorTextBox.Text = watchedDirs[checkedListBox1.SelectedIndex].lastError;
			}
		}

		void Compile(WatchedDir wd) {
			// run compile command
			System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
			psi.FileName = "cmd.exe";
			psi.Arguments = "/c " + wd.compileCommand;
			psi.WorkingDirectory = wd.path;
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;
			System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
			p.WaitForExit();
			string output = p.StandardOutput.ReadToEnd();
			string error = p.StandardError.ReadToEnd();
			wd.lastOutput = output;
			wd.lastError = error;

			wd.needsCompile = false;
		}

		void SaveSettings() {
			// filename is exe name + Settings.xml
			var filename = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
			if (filename.EndsWith(".exe")) {
				filename = filename.Substring(0, filename.Length - 4);
			}
			filename += "Settings.xml";

			System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(List<WatchedDir>));
			System.IO.Stream s = new System.IO.FileStream(filename, System.IO.FileMode.Create);
			xs.Serialize(s, watchedDirs);
			s.Close();
		}

		void LoadSettings() {
			// filename is exe name + Settings.xml
			var filename = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
			if (filename.EndsWith(".exe")) {
				filename = filename.Substring(0, filename.Length - 4);
			}
			filename += "Settings.xml";

			if (System.IO.File.Exists(filename)) {
				System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(List<WatchedDir>));
				System.IO.Stream s = new System.IO.FileStream(filename, System.IO.FileMode.Open);
				watchedDirs = (List<WatchedDir>)xs.Deserialize(s);
				s.Close();
				foreach (WatchedDir wd in watchedDirs) {
					Add(wd, false);
				}
			}
			blLoaded = true;
		}

		private void checkedListBox1_KeyDown(object sender, KeyEventArgs e) {
			// delete on [del]
			if (e.KeyCode == Keys.Delete) {
				if (checkedListBox1.SelectedIndex >= 0) {
					watchedDirs.RemoveAt(checkedListBox1.SelectedIndex);
					checkedListBox1.Items.RemoveAt(checkedListBox1.SelectedIndex);
				}
			}
		}

		private void OnChanged(object source, FileSystemEventArgs e) {
			// This event fires twice per file because of Windows behaviour.
			foreach (WatchedDir wd in watchedDirs) {
				if (wd.enabled && e.FullPath.StartsWith(wd.path)) {
					string[] ignores = wd.ignore.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string ignore in ignores) {
						if (e.FullPath.StartsWith(wd.path + "\\" + ignore)) {
							return;
						}
					}
					wd.needsCompile = true;
					timer1.Start();
					break;
				}
			}
		}
		// compilation timer
		void timer1_Tick(object sender, EventArgs e) {
			for (int i = 0; i < watchedDirs.Count; i++) {
				var wd = watchedDirs[i];
				if (wd.needsCompile) {
					HandleCompile(wd, i);
				}
			}
			timer1.Stop();
			MyRefresh();
		}

		private void minimiseButton_Click(object sender, EventArgs e) {
			notifyIcon1.Visible = true;
			this.Hide();
		}

		private void ignoreTextBox_TextChanged(object sender, EventArgs e) {
			if (checkedListBox1.SelectedIndex >= 0) {
				watchedDirs[checkedListBox1.SelectedIndex].ignore = ignoreTextBox.Text;
			}
		}
	}

	public class WatchedDir {
		public string path;
		public bool enabled;
		public string compileCommand;
		public string lastOutput;
		public string lastError;
		//public bool justFired = false;
		public bool needsCompile = false;
		public string ignore = "bin;.git";
	}
}
