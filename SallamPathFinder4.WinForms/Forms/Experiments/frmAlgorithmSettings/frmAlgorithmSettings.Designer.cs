#region File Header
/// <summary>
/// File: frmAlgorithmSettings.Designer.cs
/// Description: Designer file for algorithm settings form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-08
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmAlgorithmSettings
{
    partial class frmAlgorithmSettings
    {
        private System.ComponentModel.IContainer components = null;

        // UI Components
        private System.Windows.Forms.DataGridView _dgvParameters;
        private System.Windows.Forms.Panel _pnlFormula;
        private System.Windows.Forms.Panel _pnlDescription;
        private System.Windows.Forms.RichTextBox _txtFormula;
        private System.Windows.Forms.Label _lblDescription;
        private System.Windows.Forms.Button _btnApply;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.Button _btnReset;
        private System.Windows.Forms.Label _lblTitle;
        private System.Windows.Forms.Label _lblFormulaTitle;
        private System.Windows.Forms.Label _lblDescTitle;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this._dgvParameters = new System.Windows.Forms.DataGridView();
            this._pnlFormula = new System.Windows.Forms.Panel();
            this._pnlDescription = new System.Windows.Forms.Panel();
            this._txtFormula = new System.Windows.Forms.RichTextBox();
            this._lblDescription = new System.Windows.Forms.Label();
            this._btnApply = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._btnReset = new System.Windows.Forms.Button();
            this._lblTitle = new System.Windows.Forms.Label();
            this._lblFormulaTitle = new System.Windows.Forms.Label();
            this._lblDescTitle = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this._dgvParameters)).BeginInit();
            this._pnlFormula.SuspendLayout();
            this._pnlDescription.SuspendLayout();
            this.SuspendLayout();

            // 
            // _lblTitle
            // 
            this._lblTitle.AutoSize = true;
            this._lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this._lblTitle.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this._lblTitle.Location = new System.Drawing.Point(15, 15);
            this._lblTitle.Name = "_lblTitle";
            this._lblTitle.Size = new System.Drawing.Size(200, 25);
            this._lblTitle.Text = "Algorithm Settings";

            // 
            // _dgvParameters
            // 
            this._dgvParameters.AllowUserToAddRows = false;
            this._dgvParameters.AllowUserToDeleteRows = false;
            this._dgvParameters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._dgvParameters.BackgroundColor = System.Drawing.Color.White;
            this._dgvParameters.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._dgvParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvParameters.GridColor = System.Drawing.Color.FromArgb(230, 230, 230);
            this._dgvParameters.Location = new System.Drawing.Point(15, 55);
            this._dgvParameters.Name = "_dgvParameters";
            this._dgvParameters.RowHeadersVisible = false;
            this._dgvParameters.Size = new System.Drawing.Size(810, 350);
            this._dgvParameters.TabIndex = 0;

            // Add columns
            this._dgvParameters.Columns.Add("Parameter", "Parameter");
            this._dgvParameters.Columns.Add("Value", "Value");
            this._dgvParameters.Columns.Add("Range", "Range");
            this._dgvParameters.Columns.Add("Type", "Type");
            this._dgvParameters.Columns["Parameter"].Width = 200;
            this._dgvParameters.Columns["Value"].Width = 150;
            this._dgvParameters.Columns["Range"].Width = 150;
            this._dgvParameters.Columns["Type"].Width = 100;

            // 
            // _pnlFormula
            // 
            this._pnlFormula.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this._pnlFormula.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pnlFormula.Controls.Add(this._lblFormulaTitle);
            this._pnlFormula.Controls.Add(this._txtFormula);
            this._pnlFormula.Location = new System.Drawing.Point(15, 420);
            this._pnlFormula.Name = "_pnlFormula";
            this._pnlFormula.Size = new System.Drawing.Size(810, 100);
            this._pnlFormula.TabIndex = 1;

            // 
            // _lblFormulaTitle
            // 
            this._lblFormulaTitle.AutoSize = true;
            this._lblFormulaTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._lblFormulaTitle.ForeColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this._lblFormulaTitle.Location = new System.Drawing.Point(10, 8);
            this._lblFormulaTitle.Name = "_lblFormulaTitle";
            this._lblFormulaTitle.Size = new System.Drawing.Size(145, 15);
            this._lblFormulaTitle.Text = "📐 MATHEMATICAL FORMULA";

            // 
            // _txtFormula
            // 
            this._txtFormula.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this._txtFormula.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._txtFormula.Font = new System.Drawing.Font("Consolas", 10F);
            this._txtFormula.Location = new System.Drawing.Point(10, 30);
            this._txtFormula.Name = "_txtFormula";
            this._txtFormula.ReadOnly = true;
            this._txtFormula.Size = new System.Drawing.Size(785, 55);
            this._txtFormula.TabIndex = 0;
            this._txtFormula.Text = "";

            // 
            // _pnlDescription
            // 
            this._pnlDescription.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this._pnlDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pnlDescription.Controls.Add(this._lblDescTitle);
            this._pnlDescription.Controls.Add(this._lblDescription);
            this._pnlDescription.Location = new System.Drawing.Point(15, 530);
            this._pnlDescription.Name = "_pnlDescription";
            this._pnlDescription.Size = new System.Drawing.Size(810, 60);
            this._pnlDescription.TabIndex = 2;

            // 
            // _lblDescTitle
            // 
            this._lblDescTitle.AutoSize = true;
            this._lblDescTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._lblDescTitle.ForeColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this._lblDescTitle.Location = new System.Drawing.Point(10, 8);
            this._lblDescTitle.Name = "_lblDescTitle";
            this._lblDescTitle.Size = new System.Drawing.Size(95, 15);
            this._lblDescTitle.Text = "ℹ️ DESCRIPTION";

            // 
            // _lblDescription
            // 
            this._lblDescription.AutoSize = true;
            this._lblDescription.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._lblDescription.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this._lblDescription.Location = new System.Drawing.Point(10, 30);
            this._lblDescription.Name = "_lblDescription";
            this._lblDescription.Size = new System.Drawing.Size(0, 15);

            // 
            // _btnApply
            // 
            this._btnApply.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this._btnApply.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnApply.ForeColor = System.Drawing.Color.White;
            this._btnApply.Location = new System.Drawing.Point(620, 605);
            this._btnApply.Name = "_btnApply";
            this._btnApply.Size = new System.Drawing.Size(100, 35);
            this._btnApply.TabIndex = 3;
            this._btnApply.Text = "✓ Apply";
            this._btnApply.UseVisualStyleBackColor = false;

            // 
            // _btnCancel
            // 
            this._btnCancel.BackColor = System.Drawing.Color.FromArgb(149, 165, 166);
            this._btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnCancel.ForeColor = System.Drawing.Color.White;
            this._btnCancel.Location = new System.Drawing.Point(725, 605);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(100, 35);
            this._btnCancel.TabIndex = 4;
            this._btnCancel.Text = "✗ Cancel";
            this._btnCancel.UseVisualStyleBackColor = false;

            // 
            // _btnReset
            // 
            this._btnReset.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this._btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnReset.ForeColor = System.Drawing.Color.White;
            this._btnReset.Location = new System.Drawing.Point(15, 605);
            this._btnReset.Name = "_btnReset";
            this._btnReset.Size = new System.Drawing.Size(120, 35);
            this._btnReset.TabIndex = 5;
            this._btnReset.Text = "⟳ Reset to Defaults";
            this._btnReset.UseVisualStyleBackColor = false;

            // 
            // frmAlgorithmSettings
            // 
            this.ClientSize = new System.Drawing.Size(840, 660);
            this.Controls.Add(this._lblTitle);
            this.Controls.Add(this._dgvParameters);
            this.Controls.Add(this._pnlFormula);
            this.Controls.Add(this._pnlDescription);
            this.Controls.Add(this._btnApply);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnReset);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "frmAlgorithmSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Algorithm Settings";
            ((System.ComponentModel.ISupportInitialize)(this._dgvParameters)).EndInit();
            this._pnlFormula.ResumeLayout(false);
            this._pnlFormula.PerformLayout();
            this._pnlDescription.ResumeLayout(false);
            this._pnlDescription.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}