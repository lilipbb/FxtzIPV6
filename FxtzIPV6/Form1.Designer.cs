namespace FxtzIPV6
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.upText = new System.Windows.Forms.TextBox();
            this.createRoomBtn = new System.Windows.Forms.Button();
            this.copyRoomBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.downText = new System.Windows.Forms.TextBox();
            this.connectRoomBtn = new System.Windows.Forms.Button();
            this.copyGameBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // upText
            // 
            this.upText.Location = new System.Drawing.Point(12, 31);
            this.upText.Name = "upText";
            this.upText.Size = new System.Drawing.Size(412, 21);
            this.upText.TabIndex = 0;
            // 
            // createRoomBtn
            // 
            this.createRoomBtn.Location = new System.Drawing.Point(112, 58);
            this.createRoomBtn.Name = "createRoomBtn";
            this.createRoomBtn.Size = new System.Drawing.Size(75, 23);
            this.createRoomBtn.TabIndex = 1;
            this.createRoomBtn.Text = "创建房间";
            this.createRoomBtn.UseVisualStyleBackColor = true;
            this.createRoomBtn.Click += new System.EventHandler(this.createRoomBtn_Click);
            // 
            // copyRoomBtn
            // 
            this.copyRoomBtn.Enabled = false;
            this.copyRoomBtn.Location = new System.Drawing.Point(234, 58);
            this.copyRoomBtn.Name = "copyRoomBtn";
            this.copyRoomBtn.Size = new System.Drawing.Size(75, 23);
            this.copyRoomBtn.TabIndex = 2;
            this.copyRoomBtn.Text = "拷贝地址";
            this.copyRoomBtn.UseVisualStyleBackColor = true;
            this.copyRoomBtn.Click += new System.EventHandler(this.copyRoomBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "主机的话请点击创建房间";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "非主机在下方粘贴ipv6地址";
            // 
            // downText
            // 
            this.downText.Location = new System.Drawing.Point(12, 126);
            this.downText.Name = "downText";
            this.downText.Size = new System.Drawing.Size(412, 21);
            this.downText.TabIndex = 4;
            // 
            // connectRoomBtn
            // 
            this.connectRoomBtn.Location = new System.Drawing.Point(112, 154);
            this.connectRoomBtn.Name = "connectRoomBtn";
            this.connectRoomBtn.Size = new System.Drawing.Size(75, 23);
            this.connectRoomBtn.TabIndex = 5;
            this.connectRoomBtn.Text = "连接房间";
            this.connectRoomBtn.UseVisualStyleBackColor = true;
            this.connectRoomBtn.Click += new System.EventHandler(this.connectRoomBtn_Click);
            // 
            // copyGameBtn
            // 
            this.copyGameBtn.Enabled = false;
            this.copyGameBtn.Location = new System.Drawing.Point(234, 154);
            this.copyGameBtn.Name = "copyGameBtn";
            this.copyGameBtn.Size = new System.Drawing.Size(75, 23);
            this.copyGameBtn.TabIndex = 6;
            this.copyGameBtn.Text = "拷贝地址";
            this.copyGameBtn.UseVisualStyleBackColor = true;
            this.copyGameBtn.Click += new System.EventHandler(this.copyGameBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 193);
            this.Controls.Add(this.copyGameBtn);
            this.Controls.Add(this.connectRoomBtn);
            this.Controls.Add(this.downText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.copyRoomBtn);
            this.Controls.Add(this.createRoomBtn);
            this.Controls.Add(this.upText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "非想天则IPV6代理 by：moyuyu";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox upText;
        private System.Windows.Forms.Button createRoomBtn;
        private System.Windows.Forms.Button copyRoomBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox downText;
        private System.Windows.Forms.Button connectRoomBtn;
        private System.Windows.Forms.Button copyGameBtn;
    }
}

