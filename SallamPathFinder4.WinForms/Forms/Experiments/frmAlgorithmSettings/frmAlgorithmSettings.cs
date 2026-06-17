#region File Header
/// <summary>
/// File: frmAlgorithmSettings.cs
/// Description: Dynamic form for editing algorithm parameters
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-09
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SallamPathFinder4.Core.Models.Algorithms;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmAlgorithmSettings
{
    public sealed partial class frmAlgorithmSettings : Form
    {
        #region Private Fields
        private readonly frmAlgorithmSettingsLogic _logic;
        private ToolTip _toolTip;
        private ContextMenuStrip _contextMenu;
        #endregion

        #region Properties
        public Dictionary<string, object> ModifiedValues => _logic.ModifiedValues;
        public bool ChangesApplied { get; private set; }
        #endregion

        #region Constructor
        public frmAlgorithmSettings(string algorithmName, Dictionary<string, object> currentValues = null)
        {
            _logic = new frmAlgorithmSettingsLogic(algorithmName, currentValues);
            ChangesApplied = false;
            _toolTip = new ToolTip();

            InitializeComponent();
            InitializeForm();
            LoadParameters();
            SetupContextMenu();
            SetupToolTips();
            WireEvents();
        }
        #endregion

        #region Private Methods - Initialization
        private void InitializeForm()
        {
            this.Text = $"Algorithm Settings: {_logic.AlgorithmDisplayName}";
            _lblTitle.Text = $"{_logic.AlgorithmDisplayName} - Parameter Configuration";
            _txtFormula.Text = _logic.AlgorithmFormula;
            _lblDescription.Text = _logic.AlgorithmDescription;

            // Adjust description height
            int descHeight = TextRenderer.MeasureText(_lblDescription.Text, _lblDescription.Font,
                new Size(_lblDescription.Width - 20, int.MaxValue), TextFormatFlags.WordBreak).Height;
            _lblDescription.Height = Math.Max(40, descHeight + 10);
            _lblDescription.AutoSize = false;
        }

        private void SetupToolTips()
        {
            _toolTip.SetToolTip(_btnApply, "Apply changes and close (Ctrl+Enter)");
            _toolTip.SetToolTip(_btnCancel, "Cancel without saving (Esc)");
            _toolTip.SetToolTip(_btnReset, "Reset all parameters to default values");
            _toolTip.SetToolTip(_dgvParameters, "Double-click a value to edit | Right-click for options");
        }

        private void SetupContextMenu()
        {
            _contextMenu = new ContextMenuStrip();
            _contextMenu.Items.Add("Reset to Default", null, (s, e) => ResetCurrentParameter());
            _contextMenu.Items.Add("Copy Value", null, (s, e) => CopyCurrentValue());
            _contextMenu.Items.Add("Paste Value", null, (s, e) => PasteCurrentValue());
            _contextMenu.Items.Add("-"); // Separator
            _contextMenu.Items.Add("Reset All", null, (s, e) => BtnReset_Click(s, e));
        }

        private void WireEvents()
        {
            _btnApply.Click += BtnApply_Click;
            _btnCancel.Click += BtnCancel_Click;
            _btnReset.Click += BtnReset_Click;
            _dgvParameters.CellBeginEdit += OnCellBeginEdit;
            _dgvParameters.CellValidating += OnCellValidating;
            _dgvParameters.CellFormatting += OnCellFormatting;
            _dgvParameters.MouseClick += OnDataGridViewMouseClick;
            this.KeyDown += OnFormKeyDown;
        }

        private void LoadParameters()
        {
            _dgvParameters.Rows.Clear();

            foreach (var param in _logic.ParameterDefinitions)
            {
                object currentValue = _logic.GetCurrentValue(param.Name);
                string rangeText = _logic.GetRangeText(param);
                string typeText = _logic.GetTypeText(param);
                string displayValue = _logic.FormatValueForDisplay(currentValue, param);

                int rowIndex = _dgvParameters.Rows.Add(
                    param.DisplayName,
                    displayValue,
                    rangeText,
                    typeText
                );

                var row = _dgvParameters.Rows[rowIndex];
                row.Tag = param;

                // Set tooltip for the row
                _toolTip.SetToolTip(_dgvParameters, param.Description ?? $"Click to edit {param.DisplayName}");
            }

            CustomizeParameterGrid();
        }

        private void CustomizeParameterGrid()
        {
            // Make Range and Type columns read-only
            _dgvParameters.Columns[2].ReadOnly = true;
            _dgvParameters.Columns[3].ReadOnly = true;

            // Color rows based on parameter type
            _dgvParameters.RowPrePaint += (sender, e) =>
            {
                var row = _dgvParameters.Rows[e.RowIndex];
                var param = (AlgorithmParameterDefinition)row.Tag;

                if (param.Type == ParameterType.Boolean)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
                else if (param.Type == ParameterType.Enum)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 240);
                else if (param.Type == ParameterType.Integer)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(240, 255, 240);
                else if (param.Type == ParameterType.Double)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 245);
                else
                    row.DefaultCellStyle.BackColor = Color.White;
            };

            // Set column widths
            _dgvParameters.Columns[0].Width = 180; // Parameter name
            _dgvParameters.Columns[1].Width = 150; // Value
            _dgvParameters.Columns[2].Width = 120; // Range
            _dgvParameters.Columns[3].Width = 80;  // Type
        }
        #endregion

        #region Private Methods - Cell Editing
        private void OnCellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 1)
            {
                e.Cancel = true;
                return;
            }

            var row = _dgvParameters.Rows[e.RowIndex];
            var param = (AlgorithmParameterDefinition)row.Tag;

            e.Cancel = true;

            // Open appropriate editor based on parameter type
            if (param.Type == ParameterType.Boolean)
                ShowBooleanEditor(row, param);
            else if (param.Type == ParameterType.Enum)
                ShowEnumEditor(row, param);
            else if (param.Type == ParameterType.Integer)
                ShowIntegerEditor(row, param);
            else if (param.Type == ParameterType.Double)
                ShowDoubleEditor(row, param);
            else
                ShowTextEditor(row, param);
        }

        private void ShowBooleanEditor(DataGridViewRow row, AlgorithmParameterDefinition param)
        {
            using (var form = new Form())
            {
                form.Text = $"Edit {param.DisplayName}";
                form.Size = new Size(300, 150);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                Label lbl = new Label
                {
                    Text = $"Select value for {param.DisplayName}:",
                    Location = new Point(10, 15),
                    Size = new Size(260, 25),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };

                ComboBox comboBox = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(10, 45),
                    Size = new Size(260, 25)
                };
                comboBox.Items.Add("✓ Yes");
                comboBox.Items.Add("☐ No");

                object currentValue = _logic.GetCurrentValue(param.Name);
                comboBox.SelectedItem = _logic.FormatValueForDisplay(currentValue, param);

                Button btnOk = new Button
                {
                    Text = "OK",
                    Location = new Point(100, 80),
                    Size = new Size(80, 30),
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(46, 204, 113),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                Button btnCancel = new Button
                {
                    Text = "Cancel",
                    Location = new Point(190, 80),
                    Size = new Size(80, 30),
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.FromArgb(149, 165, 166),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                form.Controls.Add(lbl);
                form.Controls.Add(comboBox);
                form.Controls.Add(btnOk);
                form.Controls.Add(btnCancel);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    bool newValue = comboBox.SelectedItem?.ToString() == "✓ Yes";
                    row.Cells[1].Value = _logic.FormatValueForDisplay(newValue, param);
                    _logic.SetModifiedValue(param.Name, newValue);
                }
            }
        }

        private void ShowEnumEditor(DataGridViewRow row, AlgorithmParameterDefinition param)
        {
            using (var form = new Form())
            {
                form.Text = $"Edit {param.DisplayName}";
                form.Size = new Size(300, 150);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                Label lbl = new Label
                {
                    Text = $"Select value for {param.DisplayName}:",
                    Location = new Point(10, 15),
                    Size = new Size(260, 25),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };

                ComboBox comboBox = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(10, 45),
                    Size = new Size(260, 25)
                };

                if (param.EnumOptions != null)
                {
                    foreach (var option in param.EnumOptions)
                        comboBox.Items.Add(option);
                }

                object currentValue = _logic.GetCurrentValue(param.Name);
                comboBox.SelectedItem = currentValue?.ToString();

                Button btnOk = new Button
                {
                    Text = "OK",
                    Location = new Point(100, 80),
                    Size = new Size(80, 30),
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(46, 204, 113),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                Button btnCancel = new Button
                {
                    Text = "Cancel",
                    Location = new Point(190, 80),
                    Size = new Size(80, 30),
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.FromArgb(149, 165, 166),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                form.Controls.Add(lbl);
                form.Controls.Add(comboBox);
                form.Controls.Add(btnOk);
                form.Controls.Add(btnCancel);

                if (form.ShowDialog() == DialogResult.OK && comboBox.SelectedItem != null)
                {
                    string newValue = comboBox.SelectedItem.ToString();
                    row.Cells[1].Value = newValue;
                    _logic.SetModifiedValue(param.Name, newValue);
                }
                
            }
        }

        private void ShowIntegerEditor(DataGridViewRow row, AlgorithmParameterDefinition param)
        {
            using (var form = new Form())
            {
                form.Text = $"Edit {param.DisplayName}";
                form.Size = new Size(350, 160);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                Label lbl = new Label
                {
                    Text = $"Enter value for {param.DisplayName} ({param.MinValue} - {param.MaxValue}):",
                    Location = new Point(10, 15),
                    Size = new Size(310, 25),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };

                NumericUpDown numericUpDown = new NumericUpDown
                {
                    Location = new Point(10, 45),
                    Size = new Size(150, 25),
                    Minimum = param.MinValue != null ? Convert.ToDecimal(param.MinValue) : -1000000,
                    Maximum = param.MaxValue != null ? Convert.ToDecimal(param.MaxValue) : 1000000,
                    DecimalPlaces = 0,
                    TextAlign = HorizontalAlignment.Right
                };

                object currentValue = _logic.GetCurrentValue(param.Name);
                numericUpDown.Value = currentValue != null ? Convert.ToDecimal(currentValue) : Convert.ToDecimal(param.DefaultValue);

                Label lblRange = new Label
                {
                    Text = $"Range: {param.MinValue} - {param.MaxValue}",
                    Location = new Point(170, 48),
                    Size = new Size(150, 20),
                    Font = new Font("Segoe UI", 8, FontStyle.Italic),
                    ForeColor = Color.Gray
                };

                Button btnOk = new Button
                {
                    Text = "OK",
                    Location = new Point(120, 85),
                    Size = new Size(80, 30),
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(46, 204, 113),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                Button btnCancel = new Button
                {
                    Text = "Cancel",
                    Location = new Point(210, 85),
                    Size = new Size(80, 30),
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.FromArgb(149, 165, 166),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                form.Controls.Add(lbl);
                form.Controls.Add(numericUpDown);
                form.Controls.Add(lblRange);
                form.Controls.Add(btnOk);
                form.Controls.Add(btnCancel);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    int newValue = (int)numericUpDown.Value;
                    row.Cells[1].Value = newValue.ToString();
                    _logic.SetModifiedValue(param.Name, newValue);
                }
            }
        }

        private void ShowDoubleEditor(DataGridViewRow row, AlgorithmParameterDefinition param)
        {
            using (var form = new Form())
            {
                form.Text = $"Edit {param.DisplayName}";
                form.Size = new Size(350, 160);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                Label lbl = new Label
                {
                    Text = $"Enter value for {param.DisplayName} ({param.MinValue} - {param.MaxValue}):",
                    Location = new Point(10, 15),
                    Size = new Size(310, 25),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };

                NumericUpDown numericUpDown = new NumericUpDown
                {
                    Location = new Point(10, 45),
                    Size = new Size(150, 25),
                    Minimum = param.MinValue != null ? Convert.ToDecimal(param.MinValue) : -1000000,
                    Maximum = param.MaxValue != null ? Convert.ToDecimal(param.MaxValue) : 1000000,
                    DecimalPlaces = 3,
                    Increment = 0.1m,
                    TextAlign = HorizontalAlignment.Right
                };

                object currentValue = _logic.GetCurrentValue(param.Name);
                numericUpDown.Value = currentValue != null ? Convert.ToDecimal(currentValue) : Convert.ToDecimal(param.DefaultValue);

                Label lblRange = new Label
                {
                    Text = $"Range: {param.MinValue} - {param.MaxValue}",
                    Location = new Point(170, 48),
                    Size = new Size(150, 20),
                    Font = new Font("Segoe UI", 8, FontStyle.Italic),
                    ForeColor = Color.Gray
                };

                Button btnOk = new Button
                {
                    Text = "OK",
                    Location = new Point(120, 85),
                    Size = new Size(80, 30),
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(46, 204, 113),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                Button btnCancel = new Button
                {
                    Text = "Cancel",
                    Location = new Point(210, 85),
                    Size = new Size(80, 30),
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.FromArgb(149, 165, 166),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                form.Controls.Add(lbl);
                form.Controls.Add(numericUpDown);
                form.Controls.Add(lblRange);
                form.Controls.Add(btnOk);
                form.Controls.Add(btnCancel);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    double newValue = (double)numericUpDown.Value;
                    row.Cells[1].Value = newValue.ToString("F3");
                    _logic.SetModifiedValue(param.Name, newValue);
                }
            }
        }

        private void ShowTextEditor(DataGridViewRow row, AlgorithmParameterDefinition param)
        {
            using (var form = new Form())
            {
                form.Text = $"Edit {param.DisplayName}";
                form.Size = new Size(350, 150);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                Label lbl = new Label
                {
                    Text = $"Enter value for {param.DisplayName}:",
                    Location = new Point(10, 15),
                    Size = new Size(310, 25),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };

                TextBox textBox = new TextBox
                {
                    Location = new Point(10, 45),
                    Size = new Size(310, 25)
                };

                object currentValue = _logic.GetCurrentValue(param.Name);
                textBox.Text = currentValue?.ToString() ?? "";

                Button btnOk = new Button
                {
                    Text = "OK",
                    Location = new Point(120, 80),
                    Size = new Size(80, 30),
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(46, 204, 113),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                Button btnCancel = new Button
                {
                    Text = "Cancel",
                    Location = new Point(210, 80),
                    Size = new Size(80, 30),
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.FromArgb(149, 165, 166),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                form.Controls.Add(lbl);
                form.Controls.Add(textBox);
                form.Controls.Add(btnOk);
                form.Controls.Add(btnCancel);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    row.Cells[1].Value = textBox.Text;
                    _logic.SetModifiedValue(param.Name, textBox.Text);
                }
            }
        }
        #endregion

        #region Private Methods - Context Menu
        private void OnDataGridViewMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hit = _dgvParameters.HitTest(e.X, e.Y);
                if (hit.RowIndex >= 0 && hit.ColumnIndex == 1)
                {
                    _dgvParameters.ClearSelection();
                    _dgvParameters.Rows[hit.RowIndex].Selected = true;
                    _contextMenu.Show(_dgvParameters, e.Location);
                }
            }
        }

        private void ResetCurrentParameter()
        {
            if (_dgvParameters.SelectedRows.Count > 0)
            {
                var row = _dgvParameters.SelectedRows[0];
                var param = (AlgorithmParameterDefinition)row.Tag;
                var defaultValue = _logic.GetDefaultValue(param.Name);

                row.Cells[1].Value = _logic.FormatValueForDisplay(defaultValue, param);
                _logic.SetModifiedValue(param.Name, defaultValue);
            }
        }

        private void CopyCurrentValue()
        {
            if (_dgvParameters.SelectedRows.Count > 0)
            {
                string value = _dgvParameters.SelectedRows[0].Cells[1].Value?.ToString();
                if (!string.IsNullOrEmpty(value))
                    Clipboard.SetText(value);
            }
        }

        private void PasteCurrentValue()
        {
            if (_dgvParameters.SelectedRows.Count > 0 && Clipboard.ContainsText())
            {
                string pastedValue = Clipboard.GetText();
                var row = _dgvParameters.SelectedRows[0];
                var param = (AlgorithmParameterDefinition)row.Tag;

                if (_logic.ValidateValue(pastedValue, param, out string error))
                {
                    row.Cells[1].Value = pastedValue;
                    object parsedValue = _logic.ParseDisplayValue(pastedValue, param);
                    _logic.SetModifiedValue(param.Name, parsedValue);
                }
                else
                {
                    MessageBox.Show(error, "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        #endregion

        #region Private Methods - Validation & Formatting
        private void OnCellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex != 1) return;

            var row = _dgvParameters.Rows[e.RowIndex];
            var param = (AlgorithmParameterDefinition)row.Tag;
            string newValue = e.FormattedValue?.ToString();

            if (param.Type == ParameterType.Boolean)
            {
                // التحقق من صحة القيمة المنطقية
                if (newValue == "✓ Yes" || newValue == "☐ No")
                {
                    // القيمة صحيحة
                }
                else if (bool.TryParse(newValue, out _))
                {
                    // مقبولة
                }
                else
                {
                    e.Cancel = true;
                    MessageBox.Show($"Please enter Yes or No for {param.DisplayName}", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else if (param.Type == ParameterType.Enum)
            {
                if (param.EnumOptions != null && !param.EnumOptions.Contains(newValue))
                {
                    e.Cancel = true;
                    MessageBox.Show($"Please select a valid option for {param.DisplayName}", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else if (param.Type == ParameterType.Integer)
            {
                if (!int.TryParse(newValue, out int intValue))
                {
                    e.Cancel = true;
                    MessageBox.Show($"Please enter a valid integer for {param.DisplayName}", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!param.IsValid(intValue))
                {
                    e.Cancel = true;
                    MessageBox.Show($"Value must be between {param.MinValue} and {param.MaxValue} for {param.DisplayName}",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else if (param.Type == ParameterType.Double)
            {
                if (!double.TryParse(newValue, out double dblValue))
                {
                    e.Cancel = true;
                    MessageBox.Show($"Please enter a valid number for {param.DisplayName}", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!param.IsValid(dblValue))
                {
                    e.Cancel = true;
                    MessageBox.Show($"Value must be between {param.MinValue} and {param.MaxValue} for {param.DisplayName}",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            object parsedValue = _logic.ParseDisplayValue(newValue, param);
            _logic.SetModifiedValue(param.Name, parsedValue);
        }
        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.Value != null)
            {
                var row = _dgvParameters.Rows[e.RowIndex];
                var param = (AlgorithmParameterDefinition)row.Tag;

                if (param.Type == ParameterType.Boolean)
                {
                    // التحقق من نوع القيمة قبل التحويل
                    if (e.Value is bool boolValue)
                    {
                        e.Value = boolValue ? "✓ Yes" : "☐ No";
                        e.FormattingApplied = true;
                    }
                    else if (e.Value is string stringValue)
                    {
                        // إذا كانت القيمة نص، نحاول تحويلها
                        if (stringValue == "✓ Yes" || stringValue == "True" || stringValue == "true")
                        {
                            e.Value = "✓ Yes";
                            e.FormattingApplied = true;
                        }
                        else if (stringValue == "☐ No" || stringValue == "False" || stringValue == "false")
                        {
                            e.Value = "☐ No";
                            e.FormattingApplied = true;
                        }
                        else
                        {
                            e.Value = "☐ No";
                            e.FormattingApplied = true;
                        }
                    }
                    else
                    {
                        e.Value = "☐ No";
                        e.FormattingApplied = true;
                    }
                }
                else if (param.Type == ParameterType.Double)
                {
                    if (e.Value is double doubleValue)
                    {
                        e.Value = doubleValue.ToString("F3");
                        e.FormattingApplied = true;
                    }
                    else if (e.Value is string stringValue && double.TryParse(stringValue, out double parsedDouble))
                    {
                        e.Value = parsedDouble.ToString("F3");
                        e.FormattingApplied = true;
                    }
                }
                else if (param.Type == ParameterType.Integer)
                {
                    if (e.Value is int intValue)
                    {
                        e.Value = intValue.ToString();
                        e.FormattingApplied = true;
                    }
                    else if (e.Value is string stringValue && int.TryParse(stringValue, out int parsedInt))
                    {
                        e.Value = parsedInt.ToString();
                        e.FormattingApplied = true;
                    }
                }
            }
        }
        #endregion

        #region Private Methods - Event Handlers
        private void BtnApply_Click(object sender, EventArgs e)
        {
            _dgvParameters.EndEdit();

            foreach (DataGridViewRow row in _dgvParameters.Rows)
            {
                var param = (AlgorithmParameterDefinition)row.Tag;
                string cellValue = row.Cells[1].Value?.ToString();

                if (param == null) continue;

                object parsedValue = null;

                switch (param.Type)
                {
                    case ParameterType.Boolean:
                        parsedValue = cellValue == "✓ Yes";
                        break;
                    case ParameterType.Integer:
                        if (int.TryParse(cellValue, out int intVal))
                            parsedValue = intVal;
                        break;
                    case ParameterType.Double:
                        if (double.TryParse(cellValue, out double dblVal))
                            parsedValue = dblVal;
                        break;
                    case ParameterType.Enum:
                        parsedValue = cellValue;
                        break;
                    default:
                        parsedValue = cellValue;
                        break;
                }
               
                if (parsedValue != null)
                    _logic.SetModifiedValue(param.Name, parsedValue);
            }
            
            _logic.ApplyChanges();
            ChangesApplied = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            ChangesApplied = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show($"Reset all parameters for {_logic.AlgorithmDisplayName} to default values?",
                "Reset to Defaults", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _logic.ResetToDefaults();
                LoadParameters();
                MessageBox.Show("Parameters reset to default values.", "Reset Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
                BtnApply_Click(sender, e);
            else if (e.KeyCode == Keys.Escape)
                BtnCancel_Click(sender, e);
        }
        #endregion
    }
}