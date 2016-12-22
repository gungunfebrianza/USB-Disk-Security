using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;
using System.Security.Permissions;
using System.Diagnostics;

namespace USB_Disk_Security_Crusader___Version_2._0
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //deklarasi variable
        RegistryKey Regkey;
        string lokasireg = "System\\CurrentControlSet\\Services\\USBSTOR";
        Int32 bacavalue;
        bool isAdmin;

        //menggunakan API windows untuk check user sebagai admin
        [DllImport("shell32")]
        static extern bool IsUserAnAdmin();



        #region CommonMethods
        private void progressbarloading()
        {
            //progressbar loading effect
            progressBar1.Value = 10;
            progressBar1.Value = 20;
            progressBar1.Value = 30;
            progressBar1.Value = 40;
            progressBar1.Value = 50;
            progressBar1.Value = 60;
            progressBar1.Value = 70;
            progressBar1.Value = 80;
            progressBar1.Value = 90;
            progressBar1.Value = 100;
        }

        private void restart()
        {

            //untuk menghentikan process explorer
            Process p = new Process();
            foreach (System.Diagnostics.Process exe in System.Diagnostics.Process.GetProcesses())
            {
                //jika terdapat proses explorer
                if (exe.ProcessName == "explorer")
 
                    //kill explorer
                    exe.Kill();
            }

        }
        #endregion

        // --------------------------------------- RESIDENT SHIELD MOD ----------------------
        #region ExecuteShellAlgorithm
        /// <summary>
        /// Mengeksekusi Perintah Shell
        /// </summary>
        /// <param name="command">string command</param>
        /// <returns>string, sebagai output dari perintah.</returns>
        public void ExecuteCommandSync(object command)
        {
            try
            {
                // Membuat processtartinfo menggunakan cmd sebagai program dasar untuk mengeksekusi shell
                // Dan "/c " sebagai parameter.
                // Parameter /c agar cmd.exe mengeksekusi perintah/command shell yang kita inginkan 
                // kemudian keluar dari cmd
                System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // Perintah yang dieksekusi sangat diperlukan untuk standarisasi output
                // Maksudnya perintah tersebut akan di kirimkan ke Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;

                // agar proses tidak menampilkan layar hitam dari cmd
                procStartInfo.CreateNoWindow = true;

                // Membuat proses dan menugasinya sesuai dengan perintah pada procstartinfo
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                
                // Perintah dimulai
                proc.Start();

                // mendapatkan output dan menyimpanya dalam string
                string result = proc.StandardOutput.ReadToEnd();

                // menampilkan output
                Console.WriteLine(result);
            }
            catch
            {
                // Log the exception
            }
        }
        #endregion

        #region RemoveListboxDuplicateAlgorithm
        public void RemoveListboxDuplicates(ListBox ListBox)
        {
            //deklarasi list dengan tipedata string
            List<String> Removed = new List<String>();
            for (int I = 0; I < ListBox.Items.Count; I++)
            {
                //jika list removed tidak mengandung item yang sama dlm bentuk string pada listbox target
                if (!Removed.Contains(ListBox.Items[I].ToString()))
                {
                    //tambahkan objek item kedalam listbox target
                    Removed.Add(ListBox.Items[I].ToString());
                }
            }

            //eliminasi item 
            ListBox.Items.Clear();
            
            //tambahkan item pada listbox target yang tersimpan di list removed
            foreach (String S in Removed)
            {
                ListBox.Items.Add(S);
            }

        }
        #endregion

        #region DetectUSBLetterAlgorithm
        public void DetectUSBLetterAlgorithm()
        {
            try
            {
                //dapatkn nama drive dari logical drive pada komputer
                foreach (DriveInfo drives in DriveInfo.GetDrives())
                {
                    //jika drives = removable (USB) disk tipe maka...
                    if (drives.DriveType == DriveType.Removable)
                    {
                        // menyimpan drive letter 
                        textBox1.Text = (drives.Name);
                    }

                }

            }
            catch
            {
            }
        }
        #endregion

        #region DetectAutorunAlgorithm
        public void DetectAutorunAlgorithm()
        {
            //(r) Read-only file attribute
            //(s) System file attribute
            //(h) Hidden file attribute

            // membuat attribut autorun.inf menjadi terlihat
            ExecuteCommandSync(@"attrib -r -s -h " + textBox1.Text + "autorun.inf");
            
            //membuat attribut folder RECYCLER menjadi terlihat
            ExecuteCommandSync(@"attrib -r -s -h " + textBox1.Text + "RECYCLER");
        }
        #endregion

        #region AutorunReaderAlgorithm
        public void AutorunReaderAlgorithm()
        {
            //membuka file autorun.inf dan menyimpanya dalam variable autovir
            string autorvir = File.ReadAllText(textBox1.Text + "autorun.inf");

            //menampilkan isi dari file autorun.inf
            textBox2.Text = autorvir;
        }
        #endregion

        #region KillAutoRunAlgorithm
        public void KillAutorunAlgorithm()
        {

            try
            {

                //menampilkan autorun.inf yang disembunyikan
                DetectAutorunAlgorithm();

                //jika autorun.inf terdeteksi maka..
                if (File.Exists(textBox1.Text + "autorun.inf"))
                {

                    //variable setting AutorunVirus akan menyimpan alamat path dari autorun.inf yang telah terdeteksi.
                    Properties.Settings.Default.AutorunVirus = textBox1.Text + "autorun.inf";

                    //path autorun disimpan
                    Properties.Settings.Default.Save();

                    //membaca skrip dari autorun.inf
                    AutorunReaderAlgorithm();

                    //menambahkan informasi pada kontrol listbox
                    listBox1.Items.Add("");
                    listBox1.Items.Add("Autorun Detected !!");

                    //menambahkan informasi pada kontrol listbox
                    listBox3.Items.Add("");
                    listBox3.Items.Add("Autorun Detected !!");

                    //karantina autorun.inf
                    GGFQuarantineAutorunAlgorithm();

                    //menambahkan informasi pada kontrol listbox
                    listBox1.Items.Add("Quarantined ! at " + DateTime.Now);
                    listBox1.Items.Add("");

                    //menambahkan informasi pada kontrol listbox
                    listBox3.Items.Add("Quarantined ! at " + DateTime.Now);
                    listBox3.Items.Add("");

                    //menampilkan tabpage ke 2
                    tabControl1.SelectedTab = tabPage2;

                    //membersihkan kembali variable autorun virus untuk pendeteksian lebih lanjut
                    Properties.Settings.Default.AutorunVirus = null;

                }
            }
            catch
            {
            }

        }
        #endregion

        #region GGFQuarantineAutorunAlgorithm
        public void GGFQuarantineAutorunAlgorithm()
        {
            //variable autorunvirus menyimpan alamat dari autorunvirus
            string autorunvirus = @Properties.Settings.Default.AutorunVirus;

            //variable quarantinepath menyimpan file yang dikarantina pada application startuppath
            //dengan mengubah ekstensinya menjadi .ggf
            string quarantinepath = Application.StartupPath + @"\AutorunVirus.ggf";

            //jika terdapat file yang sama pada quarantinepath
            if (File.Exists(quarantinepath))
            {
                //hapus file tersebut
                File.Delete(quarantinepath);
            }

            // autorun.inf dipindahkan ke quarantine location 
            System.IO.File.Move(autorunvirus, quarantinepath);
        }
        #endregion

        #region USBDetectorAlgorithm

        //With this message you can detect when a user adds a new USB device. Be careful though as this message can be sent out several times depending on what device is added. 
        const int WM_DEVICECHANGE = 0x0219;

        // system detected a new device
        const int DBT_DEVICEARRIVAL = 0x8000;

        //device was removed
        const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

        //device changed
        const int DBT_DEVNODES_CHANGED = 0x0007;

        // logical volume
        const int DBT_DEVTYP_VOLUME = 0x00000002;

        //see more at library microsoft http://msdn.microsoft.com/en-us/library/windows/desktop/aa363480%28v=vs.85%29.aspx
        //see more at pinvoke.ner www.pinvoke.net/default.aspx/Constants.WM

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            //jika terdeteksi penambahan device baru atau device ada yang dicabut atau terjadi perubahan pada device maka..
            if (m.Msg == WM_DEVICECHANGE && m.WParam.ToInt32() == DBT_DEVICEARRIVAL || m.WParam.ToInt32() == DBT_DEVICEREMOVECOMPLETE || m.WParam.ToInt32() == DBT_DEVNODES_CHANGED)
            {
                if (m.WParam.ToInt32() != DBT_DEVNODES_CHANGED)
                {
                    int devType = Marshal.ReadInt32(m.LParam, 4);
                    if (devType == DBT_DEVTYP_VOLUME)
                    {
                        //panggil detectusbletteralgorithm
                        DetectUSBLetterAlgorithm();

                        //tambahkan informasi pada listbox1
                        listBox1.Items.Add("");
                        listBox1.Items.Add("Device Change Detected " + " at " + DateTime.Now);

                        //panggil killautorunalgorithm
                        KillAutorunAlgorithm();
                       
                    }
                }
                else
                {
                    //tambahkan informasi pada lisbox 1
                    listBox1.Items.Add("");
                    listBox1.Items.Add("Device Change Detected " + " at " + DateTime.Now);
                }
            }

            //hapus duplikasi pesan yang sama pada lisbox1
            RemoveListboxDuplicates(listBox1);
            base.WndProc(ref m);
        }

        #endregion

        // --------------------------------------- RESIDENT SHIELD MOD ----------------------

        // --------------------------------------- USB SCAN MOD ----------------------

        #region GGFKillVirusAlgorithm
        public void GGFKillVirusAlgorithm(string virus)
        {
            try
            {
                //hapus string dari virus
                File.Delete(virus);

                //menghapus item karena file telah di hapus
                listBox2.Items.Remove((string)listBox2.SelectedItem);

                //membersihkan duplikasi item pada listbox2
                RemoveListboxDuplicates(listBox2);

                //progressbar loading effect
                progressbarloading();

                //tampilkan kotak pesan
                MessageBox.Show("Success file deleted !", "USB Security Disk Crusader version 2.0", MessageBoxButtons.OK, MessageBoxIcon.Information);

                progressBar1.Value = 0;
            }
            catch
            {
                //tampilkan kotak pesan
                MessageBox.Show("Delete Failed ! File used by another process ! ","USB Disk Security Version 2.0",MessageBoxButtons.OK ,MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region GGFQuarantineVirusAlgorithm
        public void GGFQuarantineVirusAlgorithm(string virus, string quarantinepath)
        {
           try
            {
                // jika file ada maka..
                if (File.Exists(quarantinepath))
                {
                    //hapus file
                    File.Delete(quarantinepath);
                }

                // autorun.inf dipindahkan ke quarantine location 
                System.IO.File.Move(@virus, quarantinepath);

                //menghapus item karena file telah di karantina
                listBox2.Items.Remove((string)listBox2.SelectedItem);

                //membersihkan duplikasi item pada listbox2
                RemoveListboxDuplicates(listBox2);

                //menambahkan informasi pada kontrol listbox
                listBox3.Items.Add("");
                listBox3.Items.Add("Virus Quarantined ! at " + DateTime.Now);
                listBox3.Items.Add("");

                // progressbarloading effect
                progressbarloading();

                //tampilkan kotak pesan
                MessageBox.Show("Success file quarantined !", "USB Security Disk Crusader version 2.0", MessageBoxButtons.OK, MessageBoxIcon.Information);

                progressBar1.Value = 0;
           }
          catch
            {
                //tampilkan kotak pesan
                MessageBox.Show("Quarantine Failed ! File used by another process ! ", "USB Disk Security Version 2.0", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region ScanByPatternsAlgorithm
        public void ScanByPatternsAlgorithm(string driveletter, string patterns)
        {
            try
            {
                //array extinfilepaths menyimpan alamat drive letter dan patterns file yg digunakan
                string[] extinfilePaths = Directory.GetFiles(driveletter, patterns);
               
                //jika terdapat ekstensi yang dikehendaki pada array extinfilepaths maka..
                foreach (string ext in extinfilePaths)
                {
                    //tambahkan file tersebut kedalam listbox 2
                    listBox2.Items.Add(ext);
                }
            }
            catch
            {
                //tampilkan kotak pesan
                MessageBox.Show("File not found !", "USB Disk Security Crusader v2.0", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region Scan USB
        private void pictureBox13_Click(object sender, EventArgs e)
        {
            try
            {
                //simpan setiap files dengan ekstensi exe pada array ExeinfilePaths
                string[] ExeinfilePaths = Directory.GetFiles(@textBox1.Text, "*.exe");

                //simpan setiap files dengan ekstensi pif pada array PifinfilePaths
                string[] PifinfilePaths = Directory.GetFiles(@textBox1.Text, "*.pif");
                
                foreach (string exe in ExeinfilePaths)
                {
                    //jika terdapat file dengan ekstensi executable akan ditambahkan pada lisbox
                    listBox2.Items.Add(exe);
                }

                foreach (string pif in PifinfilePaths)
                {
                    //jika terdapat file dengan ekstensi pif akan ditambahkan pada lisbox
                    listBox2.Items.Add(pif);
                }

                //membersihkan duplikasi item pada listbox2
                RemoveListboxDuplicates(listBox2);
            }
            catch
            {
                //tampilkan kotak pesan
                MessageBox.Show("Malware Virus not found !", "USB Disk Security Crusader v2.0", MessageBoxButtons .OK ,MessageBoxIcon.Information);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //scan berdasarkan patterns
            ScanByPatternsAlgorithm(@textBox1.Text, comboBox1.Text);

            //membersihkan duplikasi item pada listbox2
            RemoveListboxDuplicates(listBox2);
        }
        #endregion

        #region CMSVirus
        private void executeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //mengeksekusi file yang dipilih pada listbox 2
                Process.Start((string)listBox2.SelectedItem);
            }
            catch
            {
                //tampilkan kotak pesan
                MessageBox.Show("Can not executa file ! File used by another process ! ", "USB Disk Security Version 2.0", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void deleteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //menghapus file yang dipilih pada listbox 2
            GGFKillVirusAlgorithm((string)listBox2.SelectedItem);
        }

        private void quarantineFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //karantine file
            GGFQuarantineVirusAlgorithm(((string)listBox2.SelectedItem), Application.StartupPath + @"\Virus.ggf");
        }

        private void ignoreFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //do nothing
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            //mengeksekusi CMS execute file
            executeFileToolStripMenuItem.PerformClick();
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            //mengeksekusi CMS delete file
            deleteFileToolStripMenuItem.PerformClick();
        }

        private void pictureBox14_Click(object sender, EventArgs e)
        {
            //mengeksekusi CMS quarantine file
            quarantineFileToolStripMenuItem.PerformClick();
        }

        #endregion

        // --------------------------------------- USB SCAN MOD ----------------------

        // --------------------------------------- Quarantine ----------------------

        #region EventHandlers
        private void Form1_Load(object sender, EventArgs e)
        {

            //isuseradmin(API WINDOWS untuk cek user sebagai admin) = isadmin
            isAdmin = IsUserAnAdmin();

            //jika user bukan seorang admin
            if (isAdmin == false)
            {

                //tampilkan kotak pesan
                MessageBox.Show("You don't have proper privileges level to make changes, administrators privileges are required", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //menutup form
                Close();
            }
            else
            {

                //registry key membaca lokasireg
                Regkey = Registry.LocalMachine.OpenSubKey(lokasireg, true);
                
                //mendapatkan value yang terasosiasi dengan nama pada registry
                bacavalue = Convert.ToInt32(Regkey.GetValue("Start"));

                //membaca state pengaturan saat ini
                if (bacavalue == 3)
                {
                    //informasi state usb port akan tampil pada textbox1
                    textBox3.Text = "USB Port is Enabled";
                }
                else if (bacavalue == 4)
                {
                    //informasi state usb port akan tampil pada textbox1
                    textBox3.Text = "USB Port is Disabled";
                }
            }

            //mengubah warna tulisan logs history
            listBox3.ForeColor = System.Drawing.Color.Blue;

            try
            {
                //memuat logs history
                string[] items = File.ReadAllLines(@"C:\\Quarantinelogs.dll");

                //bersihkan lisbox
                listBox3.Items.Clear(); 
                
                //tambahkan setiap item para array items kedalam listbox 
                listBox3.Items.AddRange(items);

                //tidak ada item yang terseleksi pada listbox
                listBox3.SelectedIndex = 0;
            }
            catch (System.Exception)
            {

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //menyimpan logs history. lokasinya di drive c:\
                System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C:\\Quarantinelogs.dll");
                foreach (object item in listBox3.Items)
                
                    //write setiap item yang ada didalam listbox 3 kedalam bentuk string
                    sw.WriteLine(item.ToString());
                
                //hentikan streamwriter
                sw.Close();
            }

            catch (System.Exception)
            {

            }
        }

        private void label13_Click(object sender, EventArgs e)
        {
            //bersihkan logs history
            listBox3.Items.Clear();
        }
        #endregion

        // --------------------------------------- Quarantine ----------------------

        // --------------------------------------- Setting ----------------------

        #region GGFPortUSBManipulatorAlgorithm
        private void button3_Click(object sender, EventArgs e)
        {

            //button enable port usb

            //Regkey membaca lokasireg
            Regkey = Registry.LocalMachine.OpenSubKey(lokasireg, true);

            //tentukan nama string beserta valuenya
            Regkey.SetValue("Start", 3);

            //progressbar loading effect
            progressbarloading();

            //tampilkan kotak pesan
            MessageBox.Show("USB Port Is Enable", "USB Disk Security Crusader Version 2.0", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //mengembalikan progressbar kesemula
            progressBar1.Value = 0;

            //membutuhkan restart explorer untuk perubahan
            restart();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //button disable port usb

            //Regkey membaca lokasireg
            Regkey = Registry.LocalMachine.OpenSubKey(lokasireg, true);

            //tentukan nama string beserta valuenya
            Regkey.SetValue("Start", 4);

            //progressbar loading effect
            progressbarloading();

            //tampilkan kotak pesan
            MessageBox.Show("USB Port Is Disable", "USB Protector", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //mengembalikan progressbar kesemula
            progressBar1.Value = 0;

            //membutuhkan restart explorer untuk perubahan
            restart();
        }
        #endregion

        #region AutorunDefenderAlgorithm
        private void button4_Click(object sender, EventArgs e)
        {
            RegistryKey Rkey;

            //progressbar loading effect
            progressbarloading();

            Rkey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", true);
            Rkey.SetValue("NoDriveTypeAutoRun", 255); //disable for all media types, recommended 

            //tampilkan kotak pesan
            MessageBox.Show("Disable Autorun In All Media Type", "USB Defender V.2.0", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //mengembalikan progressbar kesemula
            progressBar1.Value = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            RegistryKey Rkey;

            //progressbar loading effect
            progressbarloading();

            Rkey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", true);
            Rkey.SetValue("NoDriveTypeAutoRun", 145); //enable

            //tampilkan kotak pesan
            MessageBox.Show("Enable Autorun In All Media Type", "USB Defender V.2.0", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //mengembalikan progressbar kesemula
            progressBar1.Value = 0;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            RegistryKey Rkey;

            //progressbar loading effect
            progressbarloading();

            Rkey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", true);
            Rkey.SetValue("NoDriveTypeAutoRun", 95); //disable

            //tampilkan kotak pesan
            MessageBox.Show("Disable Autorun USB Mode", "USB Defender V.2.0", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //mengembalikan progressbar kesemula
            progressBar1.Value = 0;
        }
        #endregion

        // --------------------------------------- Setting ----------------------

        #region etc..
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //menampilkan tabpage ke 2
            tabControl1.SelectedTab = tabPage2;
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            //menampilkan tabpage ke 3
            tabControl1.SelectedTab = tabPage3;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            //menampilkan tabpage ke 4
            tabControl1.SelectedTab = tabPage4;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            //menampilkan tabpage ke 6
            tabControl1.SelectedTab = tabPage6;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //menampilkan tabpage ke 4
            tabControl1.SelectedTab = tabPage4;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            //jika toolstrip combobox = gungunfebrianza
            if ((toolStripComboBox1.Text == "gungunfebrianza"))
            {

                //tampilkan form
                this.Visible = true;

                //bersihkan toolstrip combobox
                toolStripComboBox1.Text = "";
            }
            else
            {
                MessageBox.Show("Password Salah !", "Login Gagal!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //jika toolstrip combobox = gungunfebrianza
            if ((toolStripComboBox1.Text == "gungunfebrianza"))
            {

                //tampilkan form
                this.Visible = true;

                //bersihkan toolstrip combobox
                toolStripComboBox1.Text = "";
            }
            else
            {

                //tampilkan kotak pesan
                MessageBox.Show("Password Salah !", "Login Gagal!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {

            //sembunyikan form
            this.Visible = false;

            //tampilkan tip text dan tip title
            notifyIcon1.BalloonTipTitle = "USB Disk Security Crusader v2.0 by Gun Gun Febrianza";
            notifyIcon1.BalloonTipText = "Monitoring here..";
           
            //tampilkan durasi notifikasi icon selama 2 detik
            notifyIcon1.ShowBalloonTip(2);
        }



        #endregion

        private void label11_Click(object sender, EventArgs e)
        {

        }
    }
}
