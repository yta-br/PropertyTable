namespace PropertyTable
{
  partial class PropertyDataGridView
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.dataGridView = new System.Windows.Forms.DataGridView();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
      this.SuspendLayout();
      // 
      // dataGridView
      // 
      this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dataGridView.CausesValidation = false;
      this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
      this.dataGridView.Location = new System.Drawing.Point(0, 0);
      this.dataGridView.Name = "dataGridView";
      this.dataGridView.RowHeadersVisible = false;
      this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dataGridView.Size = new System.Drawing.Size(531, 150);
      this.dataGridView.TabIndex = 0;
      this.dataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_RowEnter);
      this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
      // 
      // PropertyDataGridView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.dataGridView);
      this.Name = "PropertyDataGridView";
      this.Size = new System.Drawing.Size(531, 150);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView;

  }
}
