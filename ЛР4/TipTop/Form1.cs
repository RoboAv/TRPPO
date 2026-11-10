// Лабораторная работа 4.
// Экранная таблица: ввод с клавиатуры, ComboBox-столбец и расширяемая подсказка.
// Подсказка ведётся отдельно для каждого столбца и пополняется новыми строками.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TipTop
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
      BuildTable();
      Stat = toolStripStatusLabel2;
      toolStripStatusLabel2.Text = "Создайте список, подключите его и редактируйте ячейки. Стрелки и Enter выбирают подсказку, Esc отменяет.";
      Resize += delegate { if (Prompt.Opened) Prompt.AdjustPrompt(); };
      dataGridView2.CurrentCellChanged += dataGridView2_CurrentCellChanged;
    }

    private void BuildTable()
    {
      dataGridView2.AutoGenerateColumns = false;
      dataGridView2.AllowUserToAddRows = true;
      dataGridView2.MultiSelect = false;
      dataGridView2.SelectionMode = DataGridViewSelectionMode.CellSelect;
      dataGridView2.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
      dataGridView2.Columns.Clear();

      dataGridView2.Columns.Add(new DataGridViewTextBoxColumn
      {
        Name = "colNN",
        HeaderText = "NN",
        Width = 80
      });
      dataGridView2.Columns.Add(new DataGridViewTextBoxColumn
      {
        Name = "colName",
        HeaderText = "Название",
        Width = 240
      });

      DataGridViewComboBoxColumn typeColumn = new DataGridViewComboBoxColumn
      {
        Name = "colType",
        HeaderText = "Тип",
        Width = 160,
        FlatStyle = FlatStyle.Flat,
        DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
      };
      typeColumn.Items.AddRange(new object[] { "Зверь", "Птица", "Рыба", "Гад" });
      dataGridView2.Columns.Add(typeColumn);

      for (int i = 0; i < 8; i++)
        dataGridView2.Rows.Add();
    }

    private void создатьСписокToolStripMenuItem_Click(object sender, EventArgs e)
    {
      AddUnique(vns, "Argon");
      AddUnique(vns, "Xenon");
      AddUnique(vns, "Cripton");
      if (ReferenceEquals(vbs.DataSource, vns))
        vbs.ResetBindings(false);
      Prompt.NotifyListChanged(1);
      toolStripStatusLabel2.Text = "Список создан: Argon, Xenon, Cripton";
    }

    private void подключитьСписокToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (dgwNamesPrompt.DataSource == null)
        dgwNamesPrompt.Rows.Clear();
      dgwNamesPrompt.DataSource = vbs;
      vbs.DataSource = vns;
      Prompt.NotifyListChanged(1);
      toolStripStatusLabel2.Text = "Список подключён (" + vns.Count + ")";
    }

    private void упорядочитьToolStripMenuItem_Click(object sender, EventArgs e)
    {
      IEnumerable<Names> query = vns.OrderBy(nmes => nmes.Name);
      List<Names> vons = new List<Names>();
      foreach (Names nm in query)
        vons.Add(new Names { Name = nm.Name });
      vns = vons;
      vbs.DataSource = vons;
      Prompt.NotifyListChanged(1);
      toolStripStatusLabel2.Text = "Список упорядочен";
    }

    public static List<Names> vns = new List<Names>();
    public static BindingSource vbs = new BindingSource();
    public static ToolStripStatusLabel Stat;
    static Dictionary<int, List<Names>> columnLists = new Dictionary<int, List<Names>>();
    static Dictionary<int, BindingSource> columnSources = new Dictionary<int, BindingSource>();

    public static List<Names> GetList(int col)
    {
      if (col == 1)
        return vns;
      List<Names> list;
      if (!columnLists.TryGetValue(col, out list))
      {
        list = new List<Names>();
        columnLists[col] = list;
      }
      return list;
    }

    public static void SetList(int col, List<Names> list)
    {
      if (col == 1)
        vns = list;
      else
        columnLists[col] = list;
      GetSource(col).DataSource = list;
    }

    public static BindingSource GetSource(int col)
    {
      if (col == 1)
        return vbs;
      BindingSource bs;
      if (!columnSources.TryGetValue(col, out bs))
      {
        bs = new BindingSource();
        columnSources[col] = bs;
      }
      return bs;
    }

    static void AddUnique(List<Names> list, string name)
    {
      foreach (Names nm in list)
      {
        if (string.Equals((nm.Name ?? "").Trim(), name, StringComparison.CurrentCulture))
          return;
      }
      list.Add(new Names { Name = name });
    }

    private void dataGridView2_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
      if (e.RowIndex < 0 || e.ColumnIndex < 0)
        return;
      Prompt.OpenPrompt(toolStripStatusLabel2, dgwNamesPrompt, dataGridView2, e);
    }

    private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
      Prompt.DetachEditControl();
      if (Prompt.Active && !Prompt.FromPrompt)
        return;
      if (!Prompt.Cancelled && !Prompt.FromPrompt)
        Prompt.StoreNewCellValue(e, dataGridView2, dgwNamesPrompt);
      if (!Prompt.KeepOpen)
        Prompt.ForceClose();
      Prompt.Cancelled = false;
      Prompt.FromPrompt = false;
      Prompt.KeepOpen = false;
      Prompt.Active = false;
    }

    private void dataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
    {
      Prompt.Attach(e.Control);
      if (Prompt.CaretAtStart || Control.MouseButtons == MouseButtons.Left)
        Prompt.PlaceCaretAtStart();
      Prompt.CaretAtStart = false;
      if (e.Control is ComboBox combo)
      {
        combo.DropDownStyle = ComboBoxStyle.DropDown;
        combo.AutoCompleteMode = AutoCompleteMode.None;
        combo.AutoCompleteSource = AutoCompleteSource.None;
        combo.DroppedDown = false;
        BeginInvoke(new Action(delegate
        {
          if (!combo.IsDisposed)
            combo.DroppedDown = false;
        }));
      }
      toolStripStatusLabel2.Text = "table ED is initiated";
    }

    private void dataGridView2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left || e.RowIndex < 0 || e.ColumnIndex < 0)
        return;
      bool wasCurrent = dataGridView2.CurrentCell != null
        && dataGridView2.CurrentCell.RowIndex == e.RowIndex
        && dataGridView2.CurrentCell.ColumnIndex == e.ColumnIndex;
      if (!wasCurrent || dataGridView2.IsCurrentCellInEditMode)
        return;
      Prompt.CaretAtStart = true;
      BeginInvoke(new Action(delegate
      {
        if (!dataGridView2.IsCurrentCellInEditMode
          && dataGridView2.CurrentCell != null
          && dataGridView2.CurrentCell.RowIndex == e.RowIndex
          && dataGridView2.CurrentCell.ColumnIndex == e.ColumnIndex)
          dataGridView2.BeginEdit(false);
      }));
    }

    private void dataGridView2_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
    {
      if (dataGridView2.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn col)
        Prompt.EnsureComboItem(col, Convert.ToString(e.FormattedValue));
    }

    private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
    {
      e.ThrowException = false;
    }

    private void dataGridView2_Scroll(object sender, ScrollEventArgs e)
    {
      if (Prompt.Opened)
        Prompt.AdjustPrompt();
    }

    private void dataGridView2_CurrentCellChanged(object sender, EventArgs e)
    {
      if (!Prompt.Opened || dataGridView2.IsCurrentCellInEditMode)
        return;
      DataGridViewCell cell = dataGridView2.CurrentCell;
      bool same = cell != null && cell.RowIndex == Prompt.curRow && cell.ColumnIndex == Prompt.curCell;
      if (same || (Prompt.Sticky && cell == null))
        return;
      Prompt.ForceClose();
    }
  }

  public class Names
  {
    public string Name { get; set; }
  }

  class Prompt
  {
    public static TextBox txt;
    public static Control edit;
    public static Rectangle vr;
    public static int curRow;
    public static int curCell;
    public static bool Active = false;
    public static bool Opened = false;
    public static bool KeepOpen = false;
    public static bool Cancelled = false;
    public static bool FromPrompt = false;
    public static bool CaretAtStart = false;
    public static bool Sticky = false;
    public static int CurPromptrow = 0;
    static bool mouseApplyQueued;
    static bool navigatedByArrows;
    static bool wired;
    static DataGridView vDGW;
    public static DataGridView vPrompt;
    public static ToolStripStatusLabel vStatus;

    public static void OpenPrompt(ToolStripStatusLabel status, DataGridView dgwPrompt, DataGridView dgw, DataGridViewCellCancelEventArgs e)
    {
      curRow = e.RowIndex;
      curCell = e.ColumnIndex;
      vDGW = dgw;
      vPrompt = dgwPrompt;
      vStatus = status;
      CurPromptrow = 0;
      navigatedByArrows = false;
      Cancelled = false;
      FromPrompt = false;
      Active = false;
      KeepOpen = false;
      Sticky = false;
      BindColumn(curCell);
      Wire();
      AdjustPrompt();
      if (vPrompt.Rows.Count > 0)
        vPrompt.ClearSelection();
      Opened = true;
      vPrompt.Visible = true;
      vPrompt.BringToFront();
      if (vStatus != null)
        vStatus.Text = "Подсказка открыта: " + dgw.Columns[curCell].HeaderText;
    }

    static void BindColumn(int col)
    {
      List<Names> list = Form1.GetList(col);
      if (list.Count == 0 && vDGW.Columns[col] is DataGridViewComboBoxColumn combo)
      {
        foreach (object item in combo.Items)
        {
          string text = Convert.ToString(item);
          if (!string.IsNullOrWhiteSpace(text))
            list.Add(new Names { Name = text.Trim() });
        }
      }
      BindingSource bs = Form1.GetSource(col);
      bs.DataSource = list;
      vPrompt.DataSource = bs;
      if (vPrompt.Columns.Count > 0)
        vPrompt.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
    }

    public static void AdjustPrompt()
    {
      if (vDGW == null || vPrompt == null || vPrompt.Parent == null)
        return;
      Rectangle cell = vDGW.GetCellDisplayRectangle(curCell, curRow, true);
      if (cell.Width <= 0 || cell.Height <= 0)
      {
        vPrompt.Visible = false;
        return;
      }
      Point below = vPrompt.Parent.PointToClient(vDGW.PointToScreen(new Point(cell.Left, cell.Bottom)));
      int width = Math.Max(cell.Width, 120);
      int rowH = vPrompt.RowTemplate.Height > 0 ? vPrompt.RowTemplate.Height : 24;
      if (vPrompt.Rows.Count > 0 && vPrompt.Rows[0].Height > 0)
        rowH = vPrompt.Rows[0].Height;
      int visibleRows = vPrompt.Rows.Count == 0 ? 1 : Math.Min(vPrompt.Rows.Count, 8);
      int height = visibleRows * rowH + 8;
      vPrompt.ScrollBars = vPrompt.Rows.Count <= 8 ? ScrollBars.None : ScrollBars.Vertical;
      int minTop = 0;
      int maxBottom = vPrompt.Parent.ClientSize.Height;
      foreach (Control sibling in vPrompt.Parent.Controls)
      {
        if (sibling is MenuStrip menu && menu.Visible)
          minTop = Math.Max(minTop, menu.Bottom);
        if (sibling is StatusStrip status && status.Visible)
          maxBottom = Math.Min(maxBottom, status.Top);
      }
      int left = below.X;
      int top = below.Y;
      if (top + height > maxBottom)
      {
        int above = vPrompt.Parent.PointToClient(vDGW.PointToScreen(new Point(cell.Left, cell.Top))).Y - height;
        if (above >= minTop)
          top = above;
      }
      if (top < minTop)
        top = minTop;
      if (left + width > vPrompt.Parent.ClientSize.Width)
        left = Math.Max(0, vPrompt.Parent.ClientSize.Width - width);
      if (left < 0)
        left = 0;
      vPrompt.Bounds = new Rectangle(left, top, width, height);
      vr = vPrompt.Bounds;
      if (Opened)
        vPrompt.Visible = true;
    }

    public static void NotifyListChanged(int col)
    {
      if (!Opened || curCell != col)
        return;
      AdjustPrompt();
    }

    public static void ClosePrompt()
    {
      if (Active)
        return;
      ForceClose();
    }

    public static void ForceClose()
    {
      Unwire();
      DetachEditControl();
      Opened = false;
      Active = false;
      KeepOpen = false;
      Sticky = false;
      mouseApplyQueued = false;
      navigatedByArrows = false;
      if (vPrompt != null)
        vPrompt.Visible = false;
    }

    static void Wire()
    {
      if (wired || vPrompt == null)
        return;
      vPrompt.KeyDown += prompt_KeyDown;
      vPrompt.CellMouseClick += prompt_CellMouseClick;
      vPrompt.MouseDown += prompt_MouseDown;
      vPrompt.MouseEnter += prompt_MouseEnter;
      vPrompt.MouseLeave += prompt_MouseLeave;
      wired = true;
    }

    static void Unwire()
    {
      if (!wired || vPrompt == null)
        return;
      vPrompt.KeyDown -= prompt_KeyDown;
      vPrompt.CellMouseClick -= prompt_CellMouseClick;
      vPrompt.MouseDown -= prompt_MouseDown;
      vPrompt.MouseEnter -= prompt_MouseEnter;
      vPrompt.MouseLeave -= prompt_MouseLeave;
      wired = false;
    }

    static void prompt_MouseEnter(object sender, EventArgs e)
    {
      Active = true;
      if (vStatus != null)
        vStatus.Text = "Mouse enters prompt";
    }

    static void prompt_MouseLeave(object sender, EventArgs e)
    {
      Active = false;
      if (FromPrompt || !Opened)
        return;
      if (vStatus != null)
        vStatus.Text = "Mouse leaves prompt";
      if (vDGW != null && !vDGW.IsCurrentCellInEditMode && curRow >= 0 && curRow < vDGW.Rows.Count)
      {
        string loc = Convert.ToString(vDGW.Rows[curRow].Cells[curCell].Value);
        StoreText(curCell, loc);
      }
    }

    static void prompt_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left || vPrompt == null || vDGW == null)
        return;
      DataGridView.HitTestInfo hit = vPrompt.HitTest(e.X, e.Y);
      if (hit.RowIndex < 0)
        return;
      string value = CellText(hit.RowIndex);
      if (value.Length == 0)
        return;
      mouseApplyQueued = true;
      FromPrompt = true;
      Active = true;
      if (edit != null)
        EditText = value;
      if (curCell >= 0 && curCell < vDGW.Columns.Count)
        EnsureComboItem(vDGW.Columns[curCell] as DataGridViewComboBoxColumn, value);
      if (vStatus != null)
        vStatus.Text = "write from prompt to table";
      vDGW.BeginInvoke(new Action(delegate
      {
        mouseApplyQueued = false;
        ApplyToCell(value);
      }));
    }

    static void prompt_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      if (mouseApplyQueued || e.RowIndex < 0 || !Opened)
        return;
      ApplyToCell(CellText(e.RowIndex));
    }

    static void prompt_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter && vPrompt != null && vPrompt.CurrentRow != null)
      {
        ApplyToCell(Convert.ToString(vPrompt.CurrentRow.Cells[0].Value));
        e.Handled = true;
      }
      else if (e.KeyCode == Keys.Escape)
      {
        CancelEditAndHide();
        e.Handled = true;
      }
    }

    public static void Spec_PreKeyDown(object sender, PreviewKeyDownEventArgs e)
    {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
        e.IsInputKey = true;
    }

    public static void Spec_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Down)
      {
        MovePrompt(1);
        e.Handled = true;
        e.SuppressKeyPress = true;
      }
      else if (e.KeyCode == Keys.Up)
      {
        MovePrompt(-1);
        e.Handled = true;
        e.SuppressKeyPress = true;
      }
      else if (e.KeyCode == Keys.Enter)
      {
        e.Handled = true;
        e.SuppressKeyPress = true;
        if (navigatedByArrows && vPrompt != null && vPrompt.Rows.Count > 0)
          ApplyToCell(CellText(CurPromptrow));
        else
          CommitTypedText();
      }
      else if (e.KeyCode == Keys.Escape)
      {
        e.Handled = true;
        e.SuppressKeyPress = true;
        CancelEditAndHide();
      }
    }

    public static void Spec_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (char.IsControl(e.KeyChar))
        return;
      if (SelectionLength > 0)
        EditText = "";
      string vcur = EditText + e.KeyChar;
      navigatedByArrows = false;
      Advance(vcur);
    }

    public static void Spec_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
      {
        navigatedByArrows = false;
        Advance(EditText.Trim());
      }
    }

    static void CommitTypedText()
    {
      string text = (EditText ?? "").Trim();
      if (text.Length == 0)
      {
        Cancelled = true;
        KeepOpen = true;
        Sticky = true;
        Active = false;
        if (vStatus != null)
          vStatus.Text = "Ячейка закрыта, подсказка видима";
        if (vDGW != null && vDGW.IsCurrentCellInEditMode)
          vDGW.EndEdit();
        else
          Opened = true;
        return;
      }
      StoreText(curCell, text);
      navigatedByArrows = false;
      CurPromptrow = 0;
      KeepOpen = true;
      Opened = true;
      ClearPromptSelection();
      if (vPrompt != null)
      {
        vPrompt.Visible = true;
        vPrompt.BringToFront();
      }
      AdjustPrompt();
      if (vStatus != null)
        vStatus.Text = "Строка занесена в подсказку";
    }

    static void CancelEditAndHide()
    {
      Cancelled = true;
      Active = false;
      KeepOpen = false;
      Sticky = false;
      if (vDGW != null && vDGW.IsCurrentCellInEditMode)
      {
        try
        {
          RestoreEditorToCell();
          vDGW.CancelEdit();
          if (vDGW.IsCurrentCellInEditMode)
            vDGW.EndEdit();
        }
        catch (InvalidOperationException)
        {
          vDGW.BeginInvoke(new Action(FinishCancel));
          return;
        }
      }
      ForceClose();
      if (vStatus != null)
        vStatus.Text = "Подсказка скрыта";
    }

    static void FinishCancel()
    {
      try
      {
        if (vDGW != null && vDGW.IsCurrentCellInEditMode)
        {
          RestoreEditorToCell();
          vDGW.CancelEdit();
          vDGW.EndEdit();
        }
      }
      catch (InvalidOperationException)
      {
      }
      ForceClose();
      if (vStatus != null)
        vStatus.Text = "Подсказка скрыта";
    }

    static void RestoreEditorToCell()
    {
      if (vDGW == null || curRow < 0 || curRow >= vDGW.Rows.Count)
        return;
      string original = Convert.ToString(vDGW.Rows[curRow].Cells[curCell].Value) ?? "";
      if (edit != null)
        EditText = original;
    }

    static void MovePrompt(int delta)
    {
      if (vPrompt == null || vPrompt.Rows.Count == 0)
        return;
      HideComboDropDown();
      if (!navigatedByArrows && delta > 0 && !HasSelection())
        CurPromptrow = 0;
      else
        CurPromptrow += delta;
      if (CurPromptrow < 0)
        CurPromptrow = 0;
      if (CurPromptrow >= vPrompt.Rows.Count)
        CurPromptrow = vPrompt.Rows.Count - 1;
      navigatedByArrows = true;
      Active = true;
      vPrompt.ClearSelection();
      vPrompt.Rows[CurPromptrow].Selected = true;
      vPrompt.CurrentCell = vPrompt.Rows[CurPromptrow].Cells[0];
      if (vStatus != null)
        vStatus.Text = "Подсказка: " + CellText(CurPromptrow);
    }

    static bool HasSelection()
    {
      return vPrompt != null && vPrompt.SelectedRows.Count > 0;
    }

    static void Advance(string vcur)
    {
      if (vPrompt == null || vPrompt.Rows.Count == 0)
        return;
      vcur = vcur ?? "";
      vPrompt.ClearSelection();
      if (vcur.Trim().Length == 0)
      {
        CurPromptrow = 0;
        return;
      }
      CurPromptrow = 0;
      while (CurPromptrow < vPrompt.Rows.Count
        && string.Compare(vcur, CellText(CurPromptrow), StringComparison.CurrentCulture) > 0)
        CurPromptrow++;
      if (CurPromptrow < vPrompt.Rows.Count
        && string.Compare(vcur, CellText(CurPromptrow), StringComparison.CurrentCulture) <= 0)
      {
        vPrompt.Rows[CurPromptrow].Selected = true;
        vPrompt.CurrentCell = vPrompt.Rows[CurPromptrow].Cells[0];
        if (vStatus != null)
          vStatus.Text = "Подсказка: " + CellText(CurPromptrow);
      }
      else
        CurPromptrow = 0;
    }

    public static void StoreNewCellValue(DataGridViewCellEventArgs e, DataGridView dgw, DataGridView dgwPrompt)
    {
      if (e.RowIndex < 0 || e.RowIndex >= dgw.Rows.Count)
        return;
      string locnm = Convert.ToString(dgw.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
      StoreText(e.ColumnIndex, locnm);
    }

    public static void StoreText(int column, string value)
    {
      value = (value ?? "").Trim();
      if (value.Length == 0)
        return;
      List<Names> list = Form1.GetList(column);
      bool found = false;
      foreach (Names nm in list)
      {
        if ((nm.Name ?? "").Trim() == value)
        {
          found = true;
          break;
        }
      }
      if (!found)
        list.Add(new Names { Name = value });
      List<Names> sorted = list
        .OrderBy(nmes => nmes.Name ?? "", StringComparer.CurrentCulture)
        .Select(nm => new Names { Name = nm.Name })
        .ToList();
      Form1.SetList(column, sorted);
      if (vDGW != null && column >= 0 && column < vDGW.Columns.Count)
        EnsureComboItem(vDGW.Columns[column] as DataGridViewComboBoxColumn, value);
      if (Opened && curCell == column)
        AdjustPrompt();
    }

    public static void EnsureComboItem(DataGridViewComboBoxColumn col, string value)
    {
      value = (value ?? "").Trim();
      if (col == null || value.Length == 0)
        return;
      foreach (object item in col.Items)
      {
        if (Convert.ToString(item) == value)
          return;
      }
      col.Items.Add(value);
    }

    static void ApplyToCell(string value)
    {
      value = (value ?? "").Trim();
      if (value.Length == 0 || vDGW == null)
        return;
      if (curCell >= 0 && curCell < vDGW.Columns.Count)
        EnsureComboItem(vDGW.Columns[curCell] as DataGridViewComboBoxColumn, value);
      FromPrompt = true;
      Active = false;
      KeepOpen = false;
      Cancelled = false;
      if (edit != null)
        EditText = value;
      if (vStatus != null)
        vStatus.Text = "write from prompt to table";
      if (vDGW.IsCurrentCellInEditMode)
        vDGW.EndEdit();
      if (curRow >= 0 && curRow < vDGW.Rows.Count)
        vDGW.Rows[curRow].Cells[curCell].Value = value;
      ForceClose();
    }

    public static void Attach(Control control)
    {
      DetachEditControl();
      edit = control;
      txt = control as TextBox;
      if (edit == null)
        return;
      edit.PreviewKeyDown += Spec_PreKeyDown;
      edit.KeyDown += Spec_KeyDown;
      edit.KeyPress += Spec_KeyPress;
      edit.KeyUp += Spec_KeyUp;
    }

    public static void DetachEditControl()
    {
      if (edit == null)
        return;
      edit.PreviewKeyDown -= Spec_PreKeyDown;
      edit.KeyDown -= Spec_KeyDown;
      edit.KeyPress -= Spec_KeyPress;
      edit.KeyUp -= Spec_KeyUp;
      edit = null;
      txt = null;
    }

    public static void PlaceCaretAtStart()
    {
      if (edit is TextBoxBase text)
        text.Select(0, 0);
      else if (edit is ComboBox combo)
      {
        combo.SelectionStart = 0;
        combo.SelectionLength = 0;
        combo.DroppedDown = false;
      }
    }

    static void HideComboDropDown()
    {
      ComboBox combo = edit as ComboBox;
      if (combo != null)
        combo.DroppedDown = false;
    }

    static string EditText
    {
      get
      {
        if (edit is TextBoxBase text)
          return text.Text ?? "";
        ComboBox combo = edit as ComboBox;
        return combo != null ? (combo.Text ?? "") : "";
      }
      set
      {
        if (edit is TextBoxBase text)
          text.Text = value ?? "";
        else if (edit is ComboBox combo)
          combo.Text = value ?? "";
      }
    }

    static int SelectionLength
    {
      get
      {
        if (edit is TextBoxBase text)
          return text.SelectionLength;
        ComboBox combo = edit as ComboBox;
        return combo != null ? combo.SelectionLength : 0;
      }
    }

    static string CellText(int row)
    {
      if (vPrompt == null || row < 0 || row >= vPrompt.Rows.Count)
        return "";
      return (Convert.ToString(vPrompt.Rows[row].Cells[0].Value) ?? "").Trim();
    }

    static void ClearPromptSelection()
    {
      if (vPrompt == null)
        return;
      vPrompt.ClearSelection();
      try
      {
        vPrompt.CurrentCell = null;
      }
      catch (InvalidOperationException)
      {
      }
    }
  }
}
