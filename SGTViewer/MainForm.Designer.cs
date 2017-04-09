namespace SGTViewer
{
    partial class MainForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainMenu = new System.Windows.Forms.ToolStrip();
            this.openMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.openSgtMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btGridPanelSwitch = new System.Windows.Forms.ToolStripButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.gridSgtFile = new SourceGrid.Grid();
            this.dgvSgtFile = new System.Windows.Forms.DataGridView();
            this.mainMenu.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSgtFile)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.AutoSize = false;
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMenu,
            this.toolStripSeparator1,
            this.btGridPanelSwitch});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(1212, 56);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "toolStrip1";
            // 
            // openMenu
            // 
            this.openMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSgtMenuItem});
            this.openMenu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.openMenu.Name = "openMenu";
            this.openMenu.Size = new System.Drawing.Size(67, 53);
            this.openMenu.Text = "Открыть";
            this.openMenu.Click += new System.EventHandler(this.openMenu_Click);
            // 
            // openSgtMenuItem
            // 
            this.openSgtMenuItem.Name = "openSgtMenuItem";
            this.openSgtMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openSgtMenuItem.Text = "Открыть SGT";
            this.openSgtMenuItem.Click += new System.EventHandler(this.openSgtMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 56);
            // 
            // btGridPanelSwitch
            // 
            this.btGridPanelSwitch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btGridPanelSwitch.Image = ((System.Drawing.Image)(resources.GetObject("btGridPanelSwitch.Image")));
            this.btGridPanelSwitch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btGridPanelSwitch.Name = "btGridPanelSwitch";
            this.btGridPanelSwitch.Size = new System.Drawing.Size(102, 53);
            this.btGridPanelSwitch.Text = "К другой панели";
            this.btGridPanelSwitch.Click += new System.EventHandler(this.btGridPanelSwitch_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 35);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1214, 678);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.gridSgtFile);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1206, 652);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dgvSgtFile);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1206, 652);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // gridSgtFile
            // 
            this.gridSgtFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gridSgtFile.EnableSort = true;
            this.gridSgtFile.Location = new System.Drawing.Point(8, 9);
            this.gridSgtFile.Name = "gridSgtFile";
            this.gridSgtFile.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.gridSgtFile.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.gridSgtFile.Size = new System.Drawing.Size(1190, 640);
            this.gridSgtFile.TabIndex = 10;
            this.gridSgtFile.TabStop = true;
            this.gridSgtFile.ToolTipText = "";
            // 
            // dgvSgtFile
            // 
            this.dgvSgtFile.AllowUserToAddRows = false;
            this.dgvSgtFile.AllowUserToDeleteRows = false;
            this.dgvSgtFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvSgtFile.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvSgtFile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSgtFile.Location = new System.Drawing.Point(8, 6);
            this.dgvSgtFile.Name = "dgvSgtFile";
            this.dgvSgtFile.RowHeadersVisible = false;
            this.dgvSgtFile.Size = new System.Drawing.Size(1190, 640);
            this.dgvSgtFile.TabIndex = 8;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1212, 715);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainForm";
            this.Text = "Mainform";
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSgtFile)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip mainMenu;
        private System.Windows.Forms.ToolStripDropDownButton openMenu;
        private System.Windows.Forms.ToolStripMenuItem openSgtMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btGridPanelSwitch;
        private SourceGrid.Grid gridSgtFile;
        private System.Windows.Forms.DataGridView dgvSgtFile;
    }
}

