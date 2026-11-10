using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TipTop
{
  static class DemoShots
  {
    const uint PW_RENDERFULLCONTENT = 2;

    [DllImport("user32.dll")]
    static extern bool PrintWindow(IntPtr hwnd, IntPtr hdc, uint flags);

    public static int Run(string dir)
    {
      Directory.CreateDirectory(dir);
      StringBuilder log = new StringBuilder();
      Form1 form = new Form1();
      form.StartPosition = FormStartPosition.Manual;
      form.Location = new Point(40, 30);
      form.TopMost = true;
      form.Show();
      Application.DoEvents();
      try
      {
        DataGridView grid = (DataGridView)Field(form, "dataGridView2");
        DataGridView prompt = (DataGridView)Field(form, "dgwNamesPrompt");
        ToolStripStatusLabel status = (ToolStripStatusLabel)Field(form, "toolStripStatusLabel2");

        Shot(form, dir, "01-start.png");
        Note(log, "01", status, grid, prompt);

        Click(form, "создатьСписокToolStripMenuItem");
        Click(form, "подключитьСписокToolStripMenuItem");
        Click(form, "упорядочитьToolStripMenuItem");
        Application.DoEvents();
        Shot(form, dir, "02-menu-sorted.png");
        Note(log, "02", status, grid, prompt);

        Edit(grid, 0, 1);
        Shot(form, dir, "03-prompt-sorted.png");
        Note(log, "03", status, grid, prompt);

        TextBox editor = grid.EditingControl as TextBox;
        Prompt.Spec_KeyPress(editor, new KeyPressEventArgs('C'));
        editor.Text = "C";
        editor.SelectionStart = editor.Text.Length;
        Application.DoEvents();
        Shot(form, dir, "04-type-c.png");
        Note(log, "04", status, grid, prompt);

        Prompt.Spec_KeyDown(editor, new KeyEventArgs(Keys.Down));
        Application.DoEvents();
        Shot(form, dir, "05-arrow-down.png");
        Note(log, "05", status, grid, prompt);

        Prompt.Spec_KeyDown(editor, new KeyEventArgs(Keys.Enter));
        Application.DoEvents();
        Shot(form, dir, "06-enter-hint.png");
        Note(log, "06", status, grid, prompt);

        Edit(grid, 1, 1);
        editor = grid.EditingControl as TextBox;
        editor.Text = "Neon";
        editor.SelectionStart = editor.Text.Length;
        Prompt.Spec_KeyDown(editor, new KeyEventArgs(Keys.Enter));
        Application.DoEvents();
        Shot(form, dir, "07-store-new.png");
        Note(log, "07", status, grid, prompt);

        editor = grid.EditingControl as TextBox;
        Prompt.Spec_KeyDown(editor, new KeyEventArgs(Keys.Escape));
        Application.DoEvents();
        Shot(form, dir, "08-escape.png");
        Note(log, "08", status, grid, prompt);

        Edit(grid, 2, 1);
        editor = grid.EditingControl as TextBox;
        editor.Text = "";
        Prompt.Spec_KeyDown(editor, new KeyEventArgs(Keys.Enter));
        Application.DoEvents();
        Shot(form, dir, "09-empty-enter.png");
        Note(log, "09", status, grid, prompt);

        Edit(grid, 3, 1);
        if (prompt.Rows.Count > 0)
          prompt.FirstDisplayedScrollingRowIndex = 0;
        Application.DoEvents();
        Rectangle row = prompt.GetRowDisplayRectangle(0, true);
        if (row.Width <= 0 || row.Height <= 0)
          row = new Rectangle(4, 4, 40, 20);
        MethodInfo mouseDown = typeof(Prompt).GetMethod("prompt_MouseDown", BindingFlags.NonPublic | BindingFlags.Static);
        mouseDown.Invoke(null, new object[]
        {
          prompt,
          new MouseEventArgs(MouseButtons.Left, 1, Math.Max(2, row.Left + 6), Math.Max(2, row.Top + 4), 0)
        });
        Application.DoEvents();
        Shot(form, dir, "10-mouse.png");
        Note(log, "10", status, grid, prompt);

        Edit(grid, 0, 2);
        Application.DoEvents();
        Shot(form, dir, "11-type-column.png");
        Note(log, "11", status, grid, prompt);

        if (grid.EditingControl is ComboBox combo)
        {
          combo.Text = "Насекомое";
          combo.SelectionStart = combo.Text.Length;
          combo.DroppedDown = false;
          Prompt.Spec_KeyDown(combo, new KeyEventArgs(Keys.Enter));
          Application.DoEvents();
          if (!combo.IsDisposed)
            combo.DroppedDown = false;
        }
        Application.DoEvents();
        Shot(form, dir, "12-new-type.png");
        Note(log, "12", status, grid, prompt);

        Edit(grid, 0, 0);
        Application.DoEvents();
        Shot(form, dir, "13-nn-column.png");
        Note(log, "13", status, grid, prompt);
      }
      finally
      {
        form.Close();
      }
      File.WriteAllText(Path.Combine(dir, "demo.log"), log.ToString(), Encoding.UTF8);
      Console.WriteLine(log.ToString());
      return 0;
    }

    static void Edit(DataGridView grid, int row, int column)
    {
      if (grid.IsCurrentCellInEditMode)
        grid.EndEdit();
      grid.CurrentCell = grid.Rows[row].Cells[column];
      Application.DoEvents();
      grid.BeginEdit(false);
      Application.DoEvents();
    }

    static void Note(StringBuilder log, string id, ToolStripStatusLabel status, DataGridView grid, DataGridView prompt)
    {
      string cell = "";
      if (grid.CurrentCell != null)
        cell = Convert.ToString(grid.CurrentCell.Value);
      string selected = "(none)";
      if (prompt.SelectedRows.Count > 0)
        selected = Convert.ToString(prompt.SelectedRows[0].Cells[0].Value);
      log.AppendLine(id
        + " status=[" + status.Text + "]"
        + " editing=" + grid.IsCurrentCellInEditMode
        + " cell=[" + cell + "]"
        + " promptVisible=" + prompt.Visible
        + " promptRows=" + prompt.Rows.Count
        + " selected=[" + selected + "]"
        + " names=[" + string.Join(",", Form1.vns.ConvertAll(n => n.Name)) + "]");
    }

    static void Shot(Form form, string dir, string name)
    {
      form.BringToFront();
      form.Activate();
      form.Refresh();
      Application.DoEvents();
      DataGridView prompt = null;
      foreach (Control control in form.Controls)
      {
        DataGridView grid = control as DataGridView;
        if (grid != null && grid.Name == "dgwNamesPrompt")
          prompt = grid;
      }
      if (prompt != null && prompt.Visible && prompt.Rows.Count > 0 && prompt.Rows.Count <= 8)
      {
        try { prompt.FirstDisplayedScrollingRowIndex = 0; }
        catch (InvalidOperationException) { }
      }
      Rectangle bounds = form.Bounds;
      using (Bitmap bmp = new Bitmap(bounds.Width, bounds.Height))
      {
        using (Graphics g = Graphics.FromImage(bmp))
        {
          IntPtr hdc = g.GetHdc();
          PrintWindow(form.Handle, hdc, PW_RENDERFULLCONTENT);
          g.ReleaseHdc(hdc);
        }
        bmp.Save(Path.Combine(dir, name), ImageFormat.Png);
      }
    }

    static void Click(Form form, string field)
    {
      ToolStripMenuItem item = (ToolStripMenuItem)Field(form, field);
      item.PerformClick();
    }

    static object Field(Form form, string name)
    {
      return form.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(form);
    }
  }
}
