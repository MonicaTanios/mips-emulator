namespace MipsEmulator
{
    partial class MipsEmulator
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
            this.MipsRegistersDataGrid = new System.Windows.Forms.DataGridView();
            this.runOneCycle = new System.Windows.Forms.Button();
            this.initialize = new System.Windows.Forms.Button();
            this.PCVal = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.userCode = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.PipelineRegisters = new System.Windows.Forms.DataGridView();
            this.DataMemory = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.MipsRegistersDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PipelineRegisters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataMemory)).BeginInit();
            this.SuspendLayout();
            // 
            // MipsRegistersDataGrid
            // 
            this.MipsRegistersDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MipsRegistersDataGrid.Location = new System.Drawing.Point(433, 37);
            this.MipsRegistersDataGrid.Name = "MipsRegistersDataGrid";
            this.MipsRegistersDataGrid.RowHeadersWidth = 51;
            this.MipsRegistersDataGrid.RowTemplate.Height = 24;
            this.MipsRegistersDataGrid.Size = new System.Drawing.Size(290, 402);
            this.MipsRegistersDataGrid.TabIndex = 36;
            // 
            // runOneCycle
            // 
            this.runOneCycle.Location = new System.Drawing.Point(433, 467);
            this.runOneCycle.Name = "runOneCycle";
            this.runOneCycle.Size = new System.Drawing.Size(161, 53);
            this.runOneCycle.TabIndex = 35;
            this.runOneCycle.Text = "Run 1 Cycle";
            this.runOneCycle.UseVisualStyleBackColor = true;
            this.runOneCycle.Click += new System.EventHandler(this.runOneCycle_Click);
            // 
            // initialize
            // 
            this.initialize.Location = new System.Drawing.Point(247, 467);
            this.initialize.Name = "initialize";
            this.initialize.Size = new System.Drawing.Size(161, 53);
            this.initialize.TabIndex = 34;
            this.initialize.Text = "Initialize";
            this.initialize.UseVisualStyleBackColor = true;
            this.initialize.Click += new System.EventHandler(this.initialize_Click);
            // 
            // PCVal
            // 
            this.PCVal.AcceptsTab = true;
            this.PCVal.Location = new System.Drawing.Point(62, 481);
            this.PCVal.Name = "PCVal";
            this.PCVal.Size = new System.Drawing.Size(117, 22);
            this.PCVal.TabIndex = 33;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 481);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 17);
            this.label5.TabIndex = 32;
            this.label5.Text = "PC";
            // 
            // userCode
            // 
            this.userCode.Location = new System.Drawing.Point(33, 37);
            this.userCode.Name = "userCode";
            this.userCode.Size = new System.Drawing.Size(375, 402);
            this.userCode.TabIndex = 31;
            this.userCode.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1044, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 17);
            this.label3.TabIndex = 30;
            this.label3.Text = "DATA Memory";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(744, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 17);
            this.label4.TabIndex = 29;
            this.label4.Text = "Pipeline Registers";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(430, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 17);
            this.label2.TabIndex = 28;
            this.label2.Text = "MIPS Registers";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 17);
            this.label1.TabIndex = 27;
            this.label1.Text = "User Code";
            // 
            // PipelineRegisters
            // 
            this.PipelineRegisters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PipelineRegisters.Location = new System.Drawing.Point(747, 37);
            this.PipelineRegisters.Name = "PipelineRegisters";
            this.PipelineRegisters.RowHeadersWidth = 51;
            this.PipelineRegisters.RowTemplate.Height = 24;
            this.PipelineRegisters.Size = new System.Drawing.Size(271, 402);
            this.PipelineRegisters.TabIndex = 37;
            // 
            // DataMemory
            // 
            this.DataMemory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataMemory.Location = new System.Drawing.Point(1047, 37);
            this.DataMemory.Name = "DataMemory";
            this.DataMemory.RowHeadersWidth = 51;
            this.DataMemory.RowTemplate.Height = 24;
            this.DataMemory.Size = new System.Drawing.Size(273, 402);
            this.DataMemory.TabIndex = 38;
            // 
            // MipsEmulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1344, 568);
            this.Controls.Add(this.DataMemory);
            this.Controls.Add(this.PipelineRegisters);
            this.Controls.Add(this.MipsRegistersDataGrid);
            this.Controls.Add(this.runOneCycle);
            this.Controls.Add(this.initialize);
            this.Controls.Add(this.PCVal);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.userCode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "MipsEmulator";
            this.Text = "MipsEmulator";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MipsRegistersDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PipelineRegisters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataMemory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView MipsRegistersDataGrid;
        private System.Windows.Forms.Button runOneCycle;
        private System.Windows.Forms.Button initialize;
        private System.Windows.Forms.TextBox PCVal;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox userCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView PipelineRegisters;
        private System.Windows.Forms.DataGridView DataMemory;
    }
}

