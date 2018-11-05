namespace BcTool
{
    partial class Simulator
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridViewSignalTable = new System.Windows.Forms.DataGridView();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SignalID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Alarm = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSignalTable)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridViewSignalTable);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.richTextBox1);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 301;
            this.splitContainer1.TabIndex = 0;
            // 
            // dataGridViewSignalTable
            // 
            this.dataGridViewSignalTable.AllowUserToAddRows = false;
            this.dataGridViewSignalTable.AllowUserToDeleteRows = false;
            this.dataGridViewSignalTable.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dataGridViewSignalTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSignalTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SignalID,
            this.Type,
            this.Value,
            this.Alarm});
            this.dataGridViewSignalTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewSignalTable.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewSignalTable.Name = "dataGridViewSignalTable";
            this.dataGridViewSignalTable.RowHeadersVisible = false;
            this.dataGridViewSignalTable.RowTemplate.Height = 30;
            this.dataGridViewSignalTable.Size = new System.Drawing.Size(800, 301);
            this.dataGridViewSignalTable.TabIndex = 0;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(800, 145);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // SignalID
            // 
            this.SignalID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SignalID.HeaderText = "SignalID";
            this.SignalID.Name = "SignalID";
            this.SignalID.ReadOnly = true;
            this.SignalID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Type
            // 
            this.Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            this.Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            this.Value.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Alarm
            // 
            this.Alarm.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Alarm.HeaderText = "Alarm";
            this.Alarm.Name = "Alarm";
            // 
            // Simulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Simulator";
            this.Text = "Simulator";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSignalTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridViewSignalTable;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn SignalID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Alarm;
    }
}