using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace 波形记录
{
    public partial class Form1 : Form
    {
        Series series = new Series("Spline");
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label_data.Text = "";
            chart1.Series.Clear();
            series.ChartType = SeriesChartType.Spline;
            chart1.Series.Add(series);
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            SearchAndAddSerialToCombobox(serialPort1, comboBox1);
            //comboBox2.Items.Add("1200");
            //comboBox2.Items.Add("2400");
            comboBox2.Items.Add("4800");
            comboBox2.Items.Add("9600");
            comboBox2.Items.Add("19200");
            comboBox2.Items.Add("38400");
            comboBox2.Items.Add("43000");
            comboBox2.Items.Add("115200");
            comboBox2.SelectedIndex = 3;  //默认显示
            comboBox1.SelectedIndex = 0;

        }

        /// <summary>
        /// 这段代码功能是寻找可用的串口并添加到comboBox下拉选项中。原理是逐个测试串口是否可用，一般来说电脑1-20就足够了，如果超过20个，可修改。
        /// </summary>
        /// <param name="Myport"></param>
        /// <param name="Mybox"></param>
        private void SearchAndAddSerialToCombobox(SerialPort Myport, ComboBox Mybox)
        {
            string Buffer;
            Mybox.Items.Clear();
            for (int i = 1; i < 20; i++)
            {
                try
                {
                    Buffer = "COM" + i.ToString();
                    Myport.PortName = Buffer;
                    Myport.Open();
                    Mybox.Items.Add(Buffer);
                    Myport.Close();
                }
                catch
                { }
            }
        }
        /// <summary>
        /// data_receive[3]的原因是每个字节发送完毕后会多出13 10 有的串口调试助手发送单字节会在末尾自动加/r/n
        /// 本块代码需根据实际传送的数据加以修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] data_receive = new byte[2];
            data_receive[0] = (byte)serialPort1.ReadByte();
            data_receive[1] = (byte)serialPort1.ReadByte();
           // data_receive[2] = (byte)serialPort1.ReadByte();

            textBox1.AppendText((data_receive[0] + data_receive[1] * 256).ToString());
            textBox1.AppendText(" " );
          //  textBox1.ScrollToCaret();
           // textBox1.AppendText(data_receive[0].ToString()+" ");
            //textBox1.AppendText(data_receive[1].ToString());
          //  textBox1.AppendText(data_receive[2].ToString()+"\r\n");
            series.Points.AddY(data_receive[0] + data_receive[1] * 256);
            /*  int data_receive = serialPort1.ReadByte();
              textBox1.AppendText(data_receive.ToString()+" ");
              series.Points.AddY(data_receive);*/

        }

        private void btn_scan_Click(object sender, EventArgs e)
        {
            SearchAndAddSerialToCombobox(serialPort1, comboBox1);
        }

        private void btn_open_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Close();
                    btn_open.Text = "打开";
                }
                catch
                { }   
            }
            else
            {
                try
                {
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text.Trim());
                    serialPort1.Open();
                    btn_open.Text = "关闭";
                }
                catch
                {
                    MessageBox.Show("串口打开失败！", "错误");
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
