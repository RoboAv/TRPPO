using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Lab3;

namespace CaptureShots
{
    internal static class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdc, uint nFlags);

        [STAThread]
        private static void Main(string[] args)
        {
            string dir = args.Length > 0
                ? args[0]
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "images"));
            Directory.CreateDirectory(dir);

            ApplicationConfiguration.Initialize();
            Form1 form = new Form1();
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(40, 40);
            form.TopMost = true;
            form.Show();
            Pump(8);

            Shot(form, Path.Combine(dir, "01-start.png"));

            Click(form, "Добавить элемент");
            Pump(2);
            Shot(form, Path.Combine(dir, "02-no-list.png"));

            Click(form, "Создать список");
            Pump(2);
            Shot(form, Path.Combine(dir, "03-created.png"));

            Click(form, "Перебор кнопок");
            StopWalk(form);
            Pump(2);
            Shot(form, Path.Combine(dir, "04-empty.png"));

            for (int i = 0; i < 22; i++)
                Click(form, "Добавить элемент");
            Pump(3);

            Panel panel = form.Controls.OfType<Panel>().First();
            foreach (CheckBox box in panel.Controls.OfType<CheckBox>())
            {
                if (box.Left == 80 || (box.Left >= 160 && box.Left <= 400))
                    box.Checked = true;
            }
            Pump(2);
            Shot(form, Path.Combine(dir, "05-buttons.png"));

            Click(form, "Перебор кнопок");
            StopWalk(form);
            Pump(2);
            Shot(form, Path.Combine(dir, "06-walk-button.png"));

            for (int i = 0; i < 6; i++)
                Step(form);
            Pump(2);
            Shot(form, Path.Combine(dir, "07-walk-middle.png"));

            while (Status(form) != "O.K.")
                Step(form);
            Pump(2);
            Shot(form, Path.Combine(dir, "08-ok.png"));

            Click(form, "Перебор объектов");
            StopWalk(form);
            Step(form);
            Step(form);
            Pump(2);
            Shot(form, Path.Combine(dir, "09-walk-object.png"));

            while (Status(form) != "O.K.")
                Step(form);

            Click(form, "Создать список");
            Click(form, "Добавить элемент");
            Click(form, "Добавить элемент");
            Pump(3);
            CheckBox last = panel.Controls.OfType<CheckBox>().OrderBy(b => b.Left).Last();
            panel.ScrollControlIntoView(last);
            Pump(3);
            Shot(form, Path.Combine(dir, "10-new-list.png"));

            form.Close();
        }

        private static void Click(Form form, string text)
        {
            foreach (ToolStripItem item in form.MainMenuStrip.Items)
            {
                if (item.Text == text)
                {
                    ((ToolStripMenuItem)item).PerformClick();
                    return;
                }
            }

            throw new InvalidOperationException(text);
        }

        private static string Status(Form form)
        {
            StatusStrip strip = form.Controls.OfType<StatusStrip>().First();
            return strip.Items[0].Text;
        }

        private static void StopWalk(Form form)
        {
            FieldInfo field = typeof(Form1).GetField("walkTimer", BindingFlags.Instance | BindingFlags.NonPublic);
            System.Windows.Forms.Timer timer = (System.Windows.Forms.Timer)field.GetValue(form);
            timer.Stop();
        }

        private static void Step(Form form)
        {
            MethodInfo tick = typeof(Form1).GetMethod("walkTimer_Tick", BindingFlags.Instance | BindingFlags.NonPublic);
            tick.Invoke(form, new object[] { form, EventArgs.Empty });
        }

        private static void Pump(int times)
        {
            for (int i = 0; i < times; i++)
            {
                Application.DoEvents();
                Thread.Sleep(40);
            }
        }

        private static void Shot(Form form, string path)
        {
            form.Refresh();
            Application.DoEvents();
            GetWindowRect(form.Handle, out RECT rect);
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;
            using (Bitmap bitmap = new Bitmap(width, height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    IntPtr hdc = graphics.GetHdc();
                    PrintWindow(form.Handle, hdc, 2);
                    graphics.ReleaseHdc(hdc);
                }
                bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            }
            Console.WriteLine(path + " " + Status(form));
        }
    }
}
