using System.Drawing;
using System.Windows.Forms;

namespace TipTop
{
  partial class Form1
  {
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
        components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      components = new System.ComponentModel.Container();
      menuStrip1 = new MenuStrip();
      создатьСписокToolStripMenuItem = new ToolStripMenuItem();
      подключитьСписокToolStripMenuItem = new ToolStripMenuItem();
      упорядочитьToolStripMenuItem = new ToolStripMenuItem();
      statusStrip1 = new StatusStrip();
      toolStripStatusLabel2 = new ToolStripStatusLabel();
      dataGridView2 = new DataGridView();
      dgwNamesPrompt = new DataGridView();
      menuStrip1.SuspendLayout();
      statusStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
      ((System.ComponentModel.ISupportInitialize)dgwNamesPrompt).BeginInit();
      SuspendLayout();

      menuStrip1.Items.AddRange(new ToolStripItem[]
      {
        создатьСписокToolStripMenuItem,
        подключитьСписокToolStripMenuItem,
        упорядочитьToolStripMenuItem
      });
      menuStrip1.Location = new Point(0, 0);
      menuStrip1.Name = "menuStrip1";
      menuStrip1.Size = new Size(980, 24);
      menuStrip1.TabIndex = 0;
      menuStrip1.Text = "menuStrip1";

      создатьСписокToolStripMenuItem.Name = "создатьСписокToolStripMenuItem";
      создатьСписокToolStripMenuItem.Size = new Size(109, 20);
      создатьСписокToolStripMenuItem.Text = "Создать список";
      создатьСписокToolStripMenuItem.Click += создатьСписокToolStripMenuItem_Click;

      подключитьСписокToolStripMenuItem.Name = "подключитьСписокToolStripMenuItem";
      подключитьСписокToolStripMenuItem.Size = new Size(129, 20);
      подключитьСписокToolStripMenuItem.Text = "Подключить список";
      подключитьСписокToolStripMenuItem.Click += подключитьСписокToolStripMenuItem_Click;

      упорядочитьToolStripMenuItem.Name = "упорядочитьToolStripMenuItem";
      упорядочитьToolStripMenuItem.Size = new Size(97, 20);
      упорядочитьToolStripMenuItem.Text = "Упорядочить";
      упорядочитьToolStripMenuItem.Click += упорядочитьToolStripMenuItem_Click;

      statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel2 });
      statusStrip1.Location = new Point(0, 528);
      statusStrip1.Name = "statusStrip1";
      statusStrip1.Size = new Size(980, 22);
      statusStrip1.TabIndex = 1;
      statusStrip1.Text = "statusStrip1";

      toolStripStatusLabel2.Name = "toolStripStatusLabel2";
      toolStripStatusLabel2.Spring = true;
      toolStripStatusLabel2.TextAlign = ContentAlignment.MiddleLeft;
      toolStripStatusLabel2.Text = "Готово";

      dataGridView2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      dataGridView2.Location = new Point(12, 36);
      dataGridView2.Name = "dataGridView2";
      dataGridView2.RowHeadersWidth = 51;
      dataGridView2.Size = new Size(956, 480);
      dataGridView2.TabIndex = 2;
      dataGridView2.CellBeginEdit += dataGridView2_CellBeginEdit;
      dataGridView2.CellEndEdit += dataGridView2_CellEndEdit;
      dataGridView2.CellMouseDown += dataGridView2_CellMouseDown;
      dataGridView2.CellValidating += dataGridView2_CellValidating;
      dataGridView2.DataError += dataGridView2_DataError;
      dataGridView2.EditingControlShowing += dataGridView2_EditingControlShowing;
      dataGridView2.Scroll += dataGridView2_Scroll;

      dgwNamesPrompt.ColumnHeadersVisible = false;
      dgwNamesPrompt.RowHeadersVisible = false;
      dgwNamesPrompt.Visible = false;
      dgwNamesPrompt.AllowUserToAddRows = false;
      dgwNamesPrompt.AllowUserToDeleteRows = false;
      dgwNamesPrompt.AllowUserToResizeColumns = false;
      dgwNamesPrompt.AllowUserToResizeRows = false;
      dgwNamesPrompt.BackgroundColor = Color.White;
      dgwNamesPrompt.BorderStyle = BorderStyle.FixedSingle;
      dgwNamesPrompt.CausesValidation = false;
      dgwNamesPrompt.EditMode = DataGridViewEditMode.EditProgrammatically;
      dgwNamesPrompt.Location = new Point(40, 80);
      dgwNamesPrompt.MultiSelect = false;
      dgwNamesPrompt.Name = "dgwNamesPrompt";
      dgwNamesPrompt.ReadOnly = true;
      dgwNamesPrompt.RowTemplate.Height = 24;
      dgwNamesPrompt.ScrollBars = ScrollBars.Vertical;
      dgwNamesPrompt.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      dgwNamesPrompt.Size = new Size(180, 28);
      dgwNamesPrompt.TabIndex = 3;
      dgwNamesPrompt.TabStop = false;

      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(980, 550);
      Controls.Add(dgwNamesPrompt);
      Controls.Add(dataGridView2);
      Controls.Add(statusStrip1);
      Controls.Add(menuStrip1);
      MainMenuStrip = menuStrip1;
      MinimumSize = new Size(720, 420);
      Name = "Form1";
      StartPosition = FormStartPosition.CenterScreen;
      Text = "ЛР4 — подсказка в экранной таблице";
      menuStrip1.ResumeLayout(false);
      menuStrip1.PerformLayout();
      statusStrip1.ResumeLayout(false);
      statusStrip1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
      ((System.ComponentModel.ISupportInitialize)dgwNamesPrompt).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    private MenuStrip menuStrip1;
    private ToolStripMenuItem создатьСписокToolStripMenuItem;
    private ToolStripMenuItem подключитьСписокToolStripMenuItem;
    private ToolStripMenuItem упорядочитьToolStripMenuItem;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel toolStripStatusLabel2;
    private DataGridView dataGridView2;
    private DataGridView dgwNamesPrompt;
  }
}
