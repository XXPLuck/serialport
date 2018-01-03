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
        Series series0 = new Series("Spline");  //添加一个序列
        Series series1 = new Series("Spline1");
        byte[] data_receive = new byte[256];
        int start  = 0;
        int Send=0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label_data.Text = "";
            chart1.Series.Clear();
            series0.ChartType = SeriesChartType.Spline;  //设定曲线类型
            chart1.Series.Add(series0);
            series1.ChartType = SeriesChartType.Spline;
            chart1.Series.Add(series1);
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
            int n;
            n = serialPort1.BytesToRead;
           // byte[] data_receive = new byte[4];
            for (int i = 0; i < n; i++)
            {
                data_receive[Send++] = (byte)serialPort1.ReadByte();
                Send = Send % 256;
            }
            serialPort1.DiscardInBuffer();

            if (((start + 4) % 256) <= Send)
            {
                textBox1.AppendText(" <");
                textBox1.AppendText((data_receive[start + 0] + data_receive[start + 1] * 256).ToString());
                textBox1.AppendText(",");
                textBox1.AppendText((data_receive[start + 2] + data_receive[start + 3] * 256).ToString());
                textBox1.AppendText("> ");

                series0.Points.AddY(data_receive[start + 0] + data_receive[start + 1] * 256);
                series1.Points.AddY(data_receive[start + 2] + data_receive[start + 3] * 256);

                start += 4;
                start = start % 256;
            }

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
                    start = 0;
                    Send = 0;
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
                    series1.Points.Clear();//波形历史数据清除
                    series0.Points.Clear();
                    textBox1.Clear();
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

        private void button1_Click(object sender, EventArgs e)
        {
            string GR_Path = @"D:";
            string fullFileName = GR_Path + "\\" + "fileName" + System.DateTime.Now.ToString("yyMMdd_HHmmss") + ".png";
            chart1.SaveImage(fullFileName, System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png);
            MessageBox.Show("保存成功！", "保存成功");
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            int high = this.Height;
            int weid = this.Width;
            chart1.Height = high / 2 - 10;
            chart1.Width = weid - 100;
            textBox1.Height = high/2-100;
            textBox1.Width = weid-50;   
            textBox1.Top = chart1.Top + high / 2 + 1;
        }
    }
}
