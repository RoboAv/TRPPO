namespace Lab3
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.создатьСписокToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.добавитьЭлементToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.переборКнопокToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.переборОбъектовToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStrip1
            //
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.создатьСписокToolStripMenuItem,
                this.добавитьЭлементToolStripMenuItem,
                this.переборКнопокToolStripMenuItem,
                this.переборОбъектовToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            //
            // создатьСписокToolStripMenuItem
            //
            this.создатьСписокToolStripMenuItem.Name = "создатьСписокToolStripMenuItem";
            this.создатьСписокToolStripMenuItem.Size = new System.Drawing.Size(109, 20);
            this.создатьСписокToolStripMenuItem.Text = "Создать список";
            this.создатьСписокToolStripMenuItem.Click += new System.EventHandler(this.создатьСписокToolStripMenuItem_Click);
            //
            // добавитьЭлементToolStripMenuItem
            //
            this.добавитьЭлементToolStripMenuItem.Name = "добавитьЭлементToolStripMenuItem";
            this.добавитьЭлементToolStripMenuItem.Size = new System.Drawing.Size(124, 20);
            this.добавитьЭлементToolStripMenuItem.Text = "Добавить элемент";
            this.добавитьЭлементToolStripMenuItem.Click += new System.EventHandler(this.добавитьЭлементToolStripMenuItem_Click);
            //
            // переборКнопокToolStripMenuItem
            //
            this.переборКнопокToolStripMenuItem.Name = "переборКнопокToolStripMenuItem";
            this.переборКнопокToolStripMenuItem.Size = new System.Drawing.Size(112, 20);
            this.переборКнопокToolStripMenuItem.Text = "Перебор кнопок";
            this.переборКнопокToolStripMenuItem.Click += new System.EventHandler(this.переборКнопокToolStripMenuItem_Click);
            //
            // переборОбъектовToolStripMenuItem
            //
            this.переборОбъектовToolStripMenuItem.Name = "переборОбъектовToolStripMenuItem";
            this.переборОбъектовToolStripMenuItem.Size = new System.Drawing.Size(129, 20);
            this.переборОбъектовToolStripMenuItem.Text = "Перебор объектов";
            this.переборОбъектовToolStripMenuItem.Click += new System.EventHandler(this.переборОбъектовToolStripMenuItem_Click);
            //
            // panel1
            //
            this.panel1.AutoScroll = true;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(784, 415);
            this.panel1.TabIndex = 1;
            //
            // statusStrip1
            //
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 439);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            //
            // toolStripStatusLabel1
            //
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            this.toolStripStatusLabel1.Text = "";
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(480, 280);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem создатьСписокToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem добавитьЭлементToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem переборКнопокToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem переборОбъектовToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}
