using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vietlott_crawler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void crawl_data(int start_num, int end_num, int total_num)
        {
            string output_path = Application.StartupPath + @"\output.txt";

            // renew the text file
            if (File.Exists(output_path))
            {
                File.Delete(output_path);
            }

            // append to text file
            using (StreamWriter w = File.AppendText(output_path))
            {
                w.WriteLine("Start time : " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
            }

            int cnt = 0;
            string period = "";
            string date = "";
            string lottery = "";

            for (int i = start_num; i <= end_num; i++)
            {
                period = i.ToString().PadLeft(5, '0');

                using (WebClient client = new WebClient())
                {
                    string url = "https://vietlott.vn/vi/trung-thuong/ket-qua-trung-thuong/655?id=" + period + "&nocatche=1";

                    // update a UI component from a method in a separate thread
                    textBox3.Invoke((Action)delegate
                    {
                        textBox3.Text = url;
                    });

                    string html = client.DownloadString(url);
                    string shorted_html = html.Between("ng <b>", "btn_chuyendulieu").Replace(" ", "").Replace("   ", "");

                    date = shorted_html.Between("y<b>", "</b></h5>");
                    lottery = shorted_html.Between("bong_tronsmall\"", "/span></div><divclass=\"btn_chuyendulieu").Replace("</span><spanclass=\"bong_tronsmall\"", "").Replace("</span><i>|</i><spanclass=\"bong_tronsmallno-margin-rightactive\"", "").Replace("<", "");
                }

                // append to text file
                using (StreamWriter w = File.AppendText(output_path))
                {
                    w.WriteLine($"Kỳ:{period} Ngày:{date} Số:{lottery}");
                }

                cnt++;

                // update a UI component from a method in a separate thread
                label4.Invoke((Action)delegate
                {
                    label4.Text = $"Fetched : {cnt}/{total_num}";
                });
            }

            // append to text file
            using (StreamWriter w = File.AppendText(output_path))
            {
                w.WriteLine("End time : " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
            }

            MessageBox.Show("Done");
        }

        private void buttonCrawl_Click(object sender, EventArgs e)
        {
            int start_num = Int32.Parse(textBox1.Text);
            int end_num = Int32.Parse(textBox2.Text);
            int total_num = end_num - start_num + 1;
            label4.Text = $"Fetched : 0/{total_num}";

            new Thread(delegate () {
                crawl_data(start_num, end_num, total_num);
            }).Start();
        }
    }
}
