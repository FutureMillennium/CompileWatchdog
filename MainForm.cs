using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompileWatchdog {
	public partial class MainForm : Form {
		public List<WatchedDir> watchedDirs = new List<WatchedDir>();
		public List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

		public MainForm() {
			InitializeComponent();
			LoadSettings();
			this.Icon = Properties.Resources.icon;
			this.notifyIcon1.Icon = Properties.Resources.icon;
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
				if (files.Length == 1 && System.IO.Directory.Exists(files[0])) {
					WatchedDir wd = new WatchedDir();
					wd.path = files[0];
					wd.enabled = true;
					//watchedDirs.Add(wd);
					//// add and check
					//checkedListBox1.Items.Add(wd.path, true);

					Add(wd);

					//checkedListBox1.SetItemChecked(checkedListBox1.Items.Count - 1, true);
					//RefreshWatchedDirs();
				}
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
				groupBox1.Visible = true;
			} else {
				groupBox1.Visible = false;
			}
		}

		private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e) {
			if (e.NewValue == CheckState.Unchecked) {
				watchedDirs[e.Index].enabled = false;
			} else {
				watchedDirs[e.Index].enabled = true;
			}
		}

		private void compileNowButton_Click(object sender, EventArgs e) {
			// compile all checked dirs
			foreach (WatchedDir wd in watchedDirs) {
				if (wd.enabled) {
					HandleCompile(wd);
				}
			}
		}

		void Add(WatchedDir wd, bool addToWD = true) {
			if (addToWD) {
				watchedDirs.Add(wd);
			}
			checkedListBox1.Items.Add(wd.path, wd.enabled);

			// add a watcher
			FileSystemWatcher watcher = new FileSystemWatcher();
			watcher.Path = wd.path;
			watcher.NotifyFilter = NotifyFilters.LastWrite;
			watcher.Filter = "*.*";
			watcher.IncludeSubdirectories = true;
			watcher.Changed += new FileSystemEventHandler(OnChanged);
			watcher.EnableRaisingEvents = true;
			watchers.Add(watcher);
		}

		void HandleCompile(WatchedDir wd) {
			Compile(wd);
			if (wd.lastError.Length > 0) {
				if (this.InvokeRequired) {
					this.Invoke(new Action(() => {
						RefocusRefresh(wd);
					}));
				} else {
					RefocusRefresh(wd);
				}
			}
		}

		void RefocusRefresh(WatchedDir wd) {
			if (checkedListBox1.SelectedIndex >= 0) {
				lastOutputTextBox.Text = watchedDirs[checkedListBox1.SelectedIndex].lastOutput;
				lastErrorTextBox.Text = watchedDirs[checkedListBox1.SelectedIndex].lastError;
			}
			this.Show();
			this.Focus();
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
			/*if (output.Length > 0) {
				MessageBox.Show(output);
			}
			if (error.Length > 0) {
				MessageBox.Show(error);
			}*/
		}

		void SaveSettings() {
			// save watchedDirs to settings.xml
			System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(List<WatchedDir>));
			System.IO.Stream s = new System.IO.FileStream("settings.xml", System.IO.FileMode.Create);
			xs.Serialize(s, watchedDirs);
			s.Close();
		}

		void LoadSettings() {
			// load watchedDirs from settings.xml
			if (System.IO.File.Exists("settings.xml")) {
				System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(List<WatchedDir>));
				System.IO.Stream s = new System.IO.FileStream("settings.xml", System.IO.FileMode.Open);
				watchedDirs = (List<WatchedDir>)xs.Deserialize(s);
				s.Close();
				foreach (WatchedDir wd in watchedDirs) {
					Add(wd, false);
					//checkedListBox1.Items.Add(wd.path, wd.enabled);
					wd.justFired = false;
				}
			}
		
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
			//MessageBox.Show("File: " + e.FullPath + " " + e.ChangeType);
			// compile
			foreach (WatchedDir wd in watchedDirs) {
				if (e.FullPath.StartsWith(wd.path)) {
					if (wd.justFired) { // Assuming the event fires twice because of Windows behaviour.
						wd.justFired = false;
					} else {
						HandleCompile(wd);
						wd.justFired = true;
					}
				}
			}
		}
	}

	public class WatchedDir {
		public string path;
		public bool enabled;
		public string compileCommand;
		public string lastOutput;
		public string lastError;
		public bool justFired = false;
	}
}
