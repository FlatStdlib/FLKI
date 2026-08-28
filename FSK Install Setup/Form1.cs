using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FSK_Install_Setup
{
    public partial class Form1 : Form
    {

        public string drive = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                    dataGridView1.Rows.Add(
                        $"{drive.Name}",
                        $"{drive.DriveType}",
                        $"{drive.VolumeLabel}",
                        $"{drive.AvailableFreeSpace / 1024 / 1024 / 1024 } GB",
                        $"{drive.TotalSize / 1024 / 1024 / 1024} GB"
                    );
            }
            label1.AutoSize = true;
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string value = dataGridView1.SelectedRows[0]
                .Cells["Namee"].Value?
                .ToString()
                .Trim();
            if (value != null)
            {
                if (File.Exists($"{value}.cfg"))
                {
                    string cfg = File.ReadAllText($"{value}.cfg");
                    if (cfg.StartsWith("[@FSK]")) MessageBox.Show("FSK Config Detected!");
                    label8.Text = "Drive Results: FSK Already Installed, Ready for format!";
                } else
                {
                    label8.Text = "Drive Results: Ready!";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                string value = dataGridView1.SelectedRows[0]
                .Cells["Namee"].Value?
                .ToString()
                .Trim();


                if (value == null)
                {
                    MessageBox.Show("Select a drive first...!", "Error");
                    return;
                }

                this.drive = value;
                string drive = value.Trim().TrimEnd('\\');
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c format {drive} /FS:FAT32 /Q /Y",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                var process = Process.Start(psi);
                process.WaitForExit();

                Console.WriteLine(process.StandardOutput.ReadToEnd());
                MessageBox.Show($"Successfully Formatted {drive} !");
            }

            if(checkBox2.Checked)
            {
                File.WriteAllText($"{this.drive}.cfg", $"[@FSK]\r\n[@CreatedAt: {DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt")}]\r\n");
            }

            if(checkBox3.Checked)
            {
                Directory.CreateDirectory($"{this.drive}fs");
                Directory.CreateDirectory($"{this.drive}EFI");
                Directory.CreateDirectory($"{this.drive}EFI\\BOOT");
                Directory.CreateDirectory($"{this.drive}fs\\bins");
                Directory.CreateDirectory($"{this.drive}fs\\bins\\tools");
                Directory.CreateDirectory($"{this.drive}fs\\bins\\libs");

                if(File.Exists(@"C:\Users\bango\Desktop\Sources\FSK\build\BOOTX64.EFI"))
                {
                    File.Copy(@"C:\Users\bango\Desktop\Sources\FSK\build\BOOTX64.EFI", $"{this.drive}\\EFI\\BOOT\\BOOTX64.EFI");
                }
            }
        }
    }
}