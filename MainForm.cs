using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;

namespace CompileWatchdog {
	public partial class MainForm : Form {
		public List<WatchedDir> watchedDirs = new List<WatchedDir>();
		List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

		DispatcherTimer timer1 = new DispatcherTimer();
		bool blLoaded = false;
		Thread compileThread;

		WatchedDir activeWD;
		String appName;
		bool threadFinished = true;

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
			appName = "Compile Watchdog v" + ver + " by Zdeněk Gromnica";
			this.Text = appName;
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
			MyRefresh();
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

		private void compileAllNowButton_Click(object sender, EventArgs e) {
			int nCompiled = 0;
			for (int i = 0; i < watchedDirs.Count; i++) {
				var wd = watchedDirs[i];
				if (wd.enabled) {
					wd.needsCompile = true;
					nCompiled++;
					//HandleCompile(wd, i);
				}
			}
			txtOuput.Text = "";
			CompileAll();
			timer1.Stop();
			//MyRefresh();
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

		/*void HandleCompile(WatchedDir wd, int index) {
			Compile(wd, index);
		}*/

		void RefocusRefresh(WatchedDir wd, int index) {
			checkedListBox1.SelectedIndex = index;
			if (popUpOnErorrCheckbox.Checked) {
				this.Show();
				this.Focus();
				notifyIcon1.Visible = false;
			}
			MyRefresh();
		}

		void MyRefresh() {
			if (checkedListBox1.SelectedIndex >= 0) {
				var wd = watchedDirs[checkedListBox1.SelectedIndex];
				if (watchedDirs[checkedListBox1.SelectedIndex].lastOutput?.Length == 0 && watchedDirs[checkedListBox1.SelectedIndex].lastError?.Length == 0) {
					lastOutputTextBox.Text = "(The output was empty.)";
				} else {
					lastOutputTextBox.Text = watchedDirs[checkedListBox1.SelectedIndex].lastOutput;
				}
				lastErrorTextBox.Text = watchedDirs[checkedListBox1.SelectedIndex].lastError;
				if (wd.lastCompile == DateTime.MinValue) {
					label2.Text = "Last standard &output:";
				} else {
					label2.Text = "Last standard &output: " + watchedDirs[checkedListBox1.SelectedIndex].lastCompile.ToString();
				}
			}
		}

		void HandleCompile(WatchedDir wd, int index) {
			// If there's no compileThread or it's finished, start a new one:
			if (compileThread == null || !compileThread.IsAlive || threadFinished) {
				this.Text = "[COMPILING] " + appName;
				if (checkedListBox1.SelectedIndex == index) {
					lastOutputTextBox.Text = "(Compiling…)";
					this.Refresh();
				}
				txtOuput.Text += "[COMPILING] " + DateTime.Now.ToString() +Environment.NewLine + wd.path + "\\" + wd.compileCommand + "\r\n";
				txtOuput.Visible = true;
				cancelCompilationButton.Text = "&Cancel compilation";
				cancelCompilationButton.Visible = true;
				compileThread = new Thread(() => CompileThread(wd,index));
				compileThread.Start();
				threadFinished = false;
			}
		}

		void CompileThread(WatchedDir wd, int index) {
			threadFinished = false;
			activeWD = wd;
			wd.lastCompile = DateTime.Now;
			// run compile command
			System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
			psi.FileName = "cmd.exe";
			psi.Arguments = "/c " + wd.compileCommand;
			psi.WorkingDirectory = wd.path;
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.OutputDataReceived += P_OutputDataReceived;
			p.ErrorDataReceived += P_ErrorDataReceived;
			p.StartInfo = psi;
			p.Start();
			wd.lastOutput = "";
			wd.lastError = "";
			p.BeginErrorReadLine();
			p.BeginOutputReadLine();
			p.WaitForExit();
			//string output = p.StandardOutput.ReadToEnd();
			//string error = p.StandardError.ReadToEnd();
			//wd.lastOutput = output;
			//wd.lastError = error;

			wd.needsCompile = false;
			
			//if (this.InvokeRequired) {
			this.Invoke(new Action(() => {
				txtOuput.Visible = false;
				cancelCompilationButton.Visible = false;
				threadFinished = true;
				if (wd.lastError.Length > 0) {
					this.Text = "[ERROR] " + appName;
					RefocusRefresh(wd, index);
				} else {
					this.Text = appName;
					MyRefresh();
					CompileAll();
				}
			}));
			//} else {
			//	RefocusRefresh(wd, index);
			//}
		}

		private void P_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e) {
			if (e.Data == null) { return; }

			this.Invoke(new Action(() => {
				activeWD.lastOutput += e.Data + "\r\n";
				txtOuput.Text += e.Data + "\r\n";
			}));
		}
		private void P_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e) {
			if (e.Data == null) { return; }

			this.Invoke(new Action(() => {
				activeWD.lastError += e.Data + "\r\n";
				txtOuput.Text += e.Data + "\r\n";
			}));
		}

		void SaveSettings() {
			if (watchedDirs == null || watchedDirs.Count == 0) {
				return;
			}

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

			try {
				if (System.IO.File.Exists(filename)) {
					System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(List<WatchedDir>));
					System.IO.Stream s = new System.IO.FileStream(filename, System.IO.FileMode.Open);
					watchedDirs = (List<WatchedDir>)xs.Deserialize(s);
					s.Close();
					foreach (WatchedDir wd in watchedDirs) {
						Add(wd, false);
					}
				}
			} catch (Exception ex) {
				txtOuput.Text = "Error reading " + filename + Environment.NewLine + Environment.NewLine + ex.Message + Environment.NewLine;
				if (ex.InnerException != null) {
					txtOuput.Text += Environment.NewLine + ex.InnerException.Message + Environment.NewLine;
				}
				txtOuput.Text += Environment.NewLine + "Try editing the file and removing the problematic part, usually the output. The settings XML file won't be saved unless you add a new watched directory and quit.";
				txtOuput.Visible = true;
				cancelCompilationButton.Text = "O&K";
				cancelCompilationButton.Visible = true;
				compileNowButton.Visible = false;
				popUpOnErorrCheckbox.Visible = false;
				minimiseButton.Visible = false;
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
			/*for (int i = 0; i < watchedDirs.Count; i++) {
				var wd = watchedDirs[i];
				if (wd.needsCompile) {
					HandleCompile(wd, i);
				}
			}*/
			txtOuput.Text = "";
			CompileAll();
			timer1.Stop();
			//MyRefresh();
		}

		void CompileAll() {
			for (int i = 0; i < watchedDirs.Count; i++) {
				var wd = watchedDirs[i];
				if (wd.enabled && wd.needsCompile) {
					HandleCompile(wd, i);
				}
			}
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

		private void cancelCompilationButton_Click(object sender, EventArgs e) {
			if (compileThread != null && compileThread.IsAlive) {
				compileThread.Abort();
				txtOuput.Text += "\r\nCompilation cancelled by user.\r\n";
			}
			for (int i = 0; i < watchedDirs.Count; i++) {
				var wd = watchedDirs[i];
				wd.needsCompile = false;
			}
			txtOuput.Visible = false;
			cancelCompilationButton.Visible = false;
			compileNowButton.Visible = true;
			popUpOnErorrCheckbox.Visible = true;
			minimiseButton.Visible = true;
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
		public DateTime lastCompile;
	}
}
