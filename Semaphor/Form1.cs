using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Semaphor
{
    public partial class Form1 : Form
    {
        List<Thread> allThreads = new List<Thread>();
        ListBox allThreadsListBox;
        ListBox waitingThreadsListBox;
        ListBox threadsListBox;
        List<Thread> threads;
        //List<ProcessThread> threads;
        private Semaphore semaphore;
        private int nextThreadNum = 1;
        private int[] threadCounters;

        public Form1()
        {
            InitializeComponent();
            InitThreadsListBox();
            semaphore = new Semaphore(3, 3);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int threadNum = nextThreadNum++;
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    semaphore.WaitOne();
                    allThreadsListBox.Items.Add($"Потік {threadNum} - лічильник: {GetThreadCounter(threadNum)}");
                    Thread.Sleep(1000);
                    semaphore.Release();
                }
            });
            thread.Start();
            allThreadsListBox.Items.Add($"Створено потік {threadNum}");
        }

        private int GetThreadCounter(int threadNum)
        {
            if (threadCounters == null || threadNum > threadCounters.Length)
            {
                int oldLength = (threadCounters != null) ? threadCounters.Length : 0;
                Array.Resize(ref threadCounters, threadNum);
                for (int i = oldLength; i < threadCounters.Length; i++)
                {
                    threadCounters[i] = 0;
                }
            }
            threadCounters[threadNum - 1]++;
            return threadCounters[threadNum - 1];
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (Thread thread in threads)
                thread.Abort();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string thread = listBox1.SelectedItem.ToString();
                listBox1.Items.Remove(thread);
                waitingThreadsListBox.Items.Add(thread);
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (waitingThreadsListBox.SelectedItem != null)
            {
                string thread = waitingThreadsListBox.SelectedItem.ToString();
                waitingThreadsListBox.Items.Remove(thread);
                listBox1.Items.Add(thread);
            }
        }
        private void InitThreadsListBox()
        {

            threadsListBox = new ListBox();
            threadsListBox.Size = new Size(200, 100);
            threadsListBox.Location = new Point(50, 60);
            this.Controls.Add(threadsListBox);
            threads = new List<Thread>();
            foreach (Thread thread in Process.GetCurrentProcess().Threads)
            {
                threads.Add(thread);
            }
            UpdateThreadsListBox();
            threadsListBox.SelectedIndexChanged += listBox3_SelectedIndexChanged;
        }
        private void UpdateThreadsListBox()
        {
            threadsListBox.Items.Clear();
            foreach (Thread thread in threads)
            {
                threadsListBox.Items.Add("Потік No " + thread.ManagedThreadId);
            }
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string thread = listBox1.SelectedItem.ToString();
                int threadNum = Int32.Parse(thread.Split(' ')[1]);
                Thread targetThread = Thread.CurrentThread;
                foreach (Thread t in threads)
                {
                    if (t.ManagedThreadId == threadNum)
                    {
                        targetThread = t;
                        break;
                    }
                }
                targetThread.Abort();
                listBox1.Items.Remove(thread);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
