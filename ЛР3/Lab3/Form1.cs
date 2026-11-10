using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lab3
{
    /// <summary>
    /// Однонаправленный цепной список. Append вставляет узел в голову,
    /// поэтому перебор идёт от последнего добавленного элемента к первому.
    /// </summary>
    public class MiniList<T> : IEnumerable<T>
    {
        static int num = 0;
        public Node Top;

        public class Node
        {
            public T content;
            public Node Next = null;
        }

        public int Append(T s)
        {
            Node p = new Node();
            p.content = s;
            if (Top != null) p.Next = Top;
            Top = p;
            return num++;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class Enumerator : IEnumerator<T>
        {
            private readonly MiniList<T> list;
            private Node current;
            private bool started;

            public Enumerator(MiniList<T> list)
            {
                this.list = list;
            }

            public T Current
            {
                get
                {
                    if (!started || current == null)
                        throw new InvalidOperationException("Перечисление не начато или уже завершено.");
                    return current.content;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (!started)
                {
                    current = list.Top;
                    started = true;
                }
                else if (current != null)
                {
                    current = current.Next;
                }

                return current != null;
            }

            public void Reset()
            {
                current = null;
                started = false;
            }

            public void Dispose()
            {
            }
        }
    }

    public partial class Form1 : Form
    {
        MiniList<CheckBox> v;

        private Timer walkTimer;
        private IEnumerator walk;
        private bool genericWalk;
        private Dictionary<CheckBox, bool> savedChecked;

        public Form1()
        {
            InitializeComponent();
            walkTimer = new Timer();
            walkTimer.Interval = 300;
            walkTimer.Tick += walkTimer_Tick;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            остановитьПеребор(false);
            walkTimer.Dispose();
            base.OnFormClosed(e);
        }

        private void создатьСписокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            остановитьПеребор(false);
            panel1.Controls.Clear();
            v = new MiniList<CheckBox>();
            toolStripStatusLabel1.Text = "Список создан.";
        }

        private void добавитьЭлементToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (v != null)
            {
                CheckBox vcb = new CheckBox();
                vcb.Appearance = Appearance.Button;
                vcb.FlatStyle = FlatStyle.Standard;
                vcb.AutoSize = false;
                vcb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                vcb.Top = 50;
                vcb.Width = 30;
                vcb.Height = 30;
                vcb.Left = 40 * v.Append(vcb);
                vcb.Text = Convert.ToString(vcb.Left);
                this.panel1.Controls.Add(vcb);
                toolStripStatusLabel1.Text = "";
            }
            else toolStripStatusLabel1.Text = "Список не создан.";
        }

        // Необобщённые IEnumerable и IEnumerator: подсветка идёт по звеньям списка.
        private void переборКнопокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            начатьПеребор(false);
        }

        // Обобщённые IEnumerable<CheckBox> и IEnumerator<CheckBox>.
        private void переборОбъектовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            начатьПеребор(true);
        }

        private void начатьПеребор(bool generic)
        {
            остановитьПеребор(true);

            if (v == null)
            {
                toolStripStatusLabel1.Text = "Список не создан.";
                return;
            }

            if (v.Top == null)
            {
                toolStripStatusLabel1.Text = "Список пуст.";
                return;
            }

            savedChecked = new Dictionary<CheckBox, bool>();
            foreach (CheckBox box in v)
                savedChecked[box] = box.Checked;

            genericWalk = generic;
            walk = generic
                ? (IEnumerator)v.GetEnumerator()
                : ((IEnumerable)v).GetEnumerator();

            walkTimer.Interval = savedChecked.Count > 24 ? 80 : 300;
            if (шагПеребора())
                walkTimer.Start();
        }

        private void walkTimer_Tick(object sender, EventArgs e)
        {
            шагПеребора();
        }

        private bool шагПеребора()
        {
            if (walk == null || !walk.MoveNext())
            {
                остановитьПеребор(true);
                toolStripStatusLabel1.Text = "O.K.";
                return false;
            }

            CheckBox box = genericWalk
                ? ((IEnumerator<CheckBox>)walk).Current
                : (CheckBox)walk.Current;

            foreach (KeyValuePair<CheckBox, bool> pair in savedChecked)
                pair.Key.Checked = pair.Key == box;

            toolStripStatusLabel1.Text = genericWalk
                ? "Объект: " + box.Text
                : "Кнопка: " + box.Text;
            panel1.ScrollControlIntoView(box);
            return true;
        }

        private void остановитьПеребор(bool restore)
        {
            walkTimer.Stop();

            if (walk is IDisposable disposable)
            {
                disposable.Dispose();
                walk = null;
            }

            if (restore && savedChecked != null)
            {
                foreach (KeyValuePair<CheckBox, bool> pair in savedChecked)
                {
                    if (!pair.Key.IsDisposed)
                        pair.Key.Checked = pair.Value;
                }
            }

            savedChecked = null;
        }
    }
}
