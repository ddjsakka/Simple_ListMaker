using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ListMaker
{
    /*
     Telegram Username : https://t.me/Sinnerx9
     Telegram Channel : https://t.me/Sinnersx
         */
    #region Custom List Of String
    class SinnerList<String> : List<String>
    {
        public event EventHandler OnAdd;

        public new void Add(String item)
        {
            if (OnAdd != null)
            {
                OnAdd(this, null);
            }
            base.Add(item);
        }
    }
    #endregion
    public partial class MainWindow : Window
    {
        #region variables
        SinnerList<String> list = new SinnerList<string>();
        bool isStrated { get; set; }
        Thread[] tasks = new Thread[2];
        Random rnd = new Random();
        object obj = new object();
        int sleep = 0;
        Mode mode = Mode.Normal;

        public enum Mode
        {
            Normal,
            ComboList
        }
        #endregion
        #region main
        

        public MainWindow()
        {
            InitializeComponent();
            radiobtn1.IsChecked = true;
            list.OnAdd += AddEvent;
        }

        private void AddEvent(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() => lbl1.Text = ((SinnerList<String>)sender).Count.ToString()));
        }
        #endregion
        #region realwork
        
        private void Generatebtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.isStrated)
            {
                this.isStrated = false;
                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        this.tasks[i].Abort();
                    }
                    catch { }
                }
                
                Generatebtn.Content = "Start";
                return;
            }
            String pattren = pattrenbox.Text;
            list.Clear();
            lbl1.Text = "0";
            Generatebtn.Content = "Stop";
            this.mode = (bool)radiobtn1.IsChecked ? Mode.ComboList : Mode.Normal;
            this.isStrated = true;
            if(!int.TryParse(sleepbox.Text,out this.sleep))
            {
                this.sleep = 0;
            }
            for (int i = 0; i < 2; i++)
            {
                this.tasks[i] = new Thread(() => Generate(pattren));
                this.tasks[i].Start();
            }
        }

        private void Generate(string pattren)
        {
            while (this.isStrated)
            {
                string result = null;
                for (int i = 0; i < pattren.Length; i++)
                {
                    if (pattren[i] == 'x')
                        result += rnd.Next(0, 9).ToString();
                    else if (int.TryParse(pattren[i].ToString(), out _))
                        result += pattren[i];
                }
                lock (obj)
                {
                    if (this.mode == Mode.Normal)
                        list.Add(result);
                    else if (this.mode == Mode.ComboList)
                        list.Add($"{result}:{result}");
                }
                if (this.sleep > 0)
                    Thread.Sleep(this.sleep);
            }
        }

        private void Savebtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sdialog = new SaveFileDialog();
            sdialog.Filter = "Text file (*.txt)|*.txt";
            if (sdialog.ShowDialog() == true)
            {
                File.WriteAllLines(sdialog.FileName, this.list.ToArray());
            }
            MessageBox.Show("Saved Successfuly...");
        }
        #endregion
    }
}
