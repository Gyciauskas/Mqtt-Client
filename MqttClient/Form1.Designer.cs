namespace MqttClient
{
    partial class Form1
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.UserName = new System.Windows.Forms.TextBox();
            this.Password = new System.Windows.Forms.TextBox();
            this.Connect = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.ServerPort = new System.Windows.Forms.TextBox();
            this.Disconnect_button = new System.Windows.Forms.Button();
            this.ReaderIdBox = new System.Windows.Forms.TextBox();
            this.UserIdBox = new System.Windows.Forms.TextBox();
            this.AccessGranted = new System.Windows.Forms.Button();
            this.AccessDenied = new System.Windows.Forms.Button();
            this.ReaderId = new System.Windows.Forms.Label();
            this.UserId = new System.Windows.Forms.Label();
            this.Identifier = new System.Windows.Forms.Label();
            this.IdentifierBox = new System.Windows.Forms.TextBox();
            this.IpAddress = new System.Windows.Forms.Label();
            this.IpAddressBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.PathBox2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 287);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(776, 362);
            this.textBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 152);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "User name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 184);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password";
            // 
            // UserName
            // 
            this.UserName.Location = new System.Drawing.Point(143, 147);
            this.UserName.Name = "UserName";
            this.UserName.Size = new System.Drawing.Size(100, 20);
            this.UserName.TabIndex = 3;
            this.UserName.Text = "admin";
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(143, 179);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(100, 20);
            this.Password.TabIndex = 4;
            this.Password.Text = "admin";
            this.Password.UseSystemPasswordChar = true;
            // 
            // Connect
            // 
            this.Connect.Location = new System.Drawing.Point(143, 216);
            this.Connect.Name = "Connect";
            this.Connect.Size = new System.Drawing.Size(100, 23);
            this.Connect.TabIndex = 5;
            this.Connect.Text = "Connect";
            this.Connect.UseVisualStyleBackColor = true;
            this.Connect.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Server port";
            // 
            // ServerPort
            // 
            this.ServerPort.Location = new System.Drawing.Point(143, 117);
            this.ServerPort.Name = "ServerPort";
            this.ServerPort.Size = new System.Drawing.Size(100, 20);
            this.ServerPort.TabIndex = 7;
            this.ServerPort.Text = "58032";
            // 
            // Disconnect_button
            // 
            this.Disconnect_button.Location = new System.Drawing.Point(249, 217);
            this.Disconnect_button.Name = "Disconnect_button";
            this.Disconnect_button.Size = new System.Drawing.Size(100, 23);
            this.Disconnect_button.TabIndex = 8;
            this.Disconnect_button.Text = "Disconnect";
            this.Disconnect_button.UseVisualStyleBackColor = true;
            this.Disconnect_button.Click += new System.EventHandler(this.Disconnect_button_Click);
            // 
            // ReaderIdBox
            // 
            this.ReaderIdBox.Location = new System.Drawing.Point(517, 48);
            this.ReaderIdBox.Name = "ReaderIdBox";
            this.ReaderIdBox.Size = new System.Drawing.Size(100, 20);
            this.ReaderIdBox.TabIndex = 9;
            // 
            // UserIdBox
            // 
            this.UserIdBox.Location = new System.Drawing.Point(517, 80);
            this.UserIdBox.Name = "UserIdBox";
            this.UserIdBox.Size = new System.Drawing.Size(100, 20);
            this.UserIdBox.TabIndex = 10;
            // 
            // AccessGranted
            // 
            this.AccessGranted.Location = new System.Drawing.Point(517, 117);
            this.AccessGranted.Name = "AccessGranted";
            this.AccessGranted.Size = new System.Drawing.Size(100, 23);
            this.AccessGranted.TabIndex = 11;
            this.AccessGranted.Text = "Access granted";
            this.AccessGranted.UseVisualStyleBackColor = true;
            this.AccessGranted.Click += new System.EventHandler(this.AccessGranted_Click);
            // 
            // AccessDenied
            // 
            this.AccessDenied.Location = new System.Drawing.Point(517, 146);
            this.AccessDenied.Name = "AccessDenied";
            this.AccessDenied.Size = new System.Drawing.Size(100, 23);
            this.AccessDenied.TabIndex = 12;
            this.AccessDenied.Text = "Access denied";
            this.AccessDenied.UseVisualStyleBackColor = true;
            this.AccessDenied.Click += new System.EventHandler(this.AccessDenied_Click);
            // 
            // ReaderId
            // 
            this.ReaderId.AutoSize = true;
            this.ReaderId.Location = new System.Drawing.Point(452, 51);
            this.ReaderId.Name = "ReaderId";
            this.ReaderId.Size = new System.Drawing.Size(54, 13);
            this.ReaderId.TabIndex = 13;
            this.ReaderId.Text = "Reader Id";
            // 
            // UserId
            // 
            this.UserId.AutoSize = true;
            this.UserId.Location = new System.Drawing.Point(452, 83);
            this.UserId.Name = "UserId";
            this.UserId.Size = new System.Drawing.Size(41, 13);
            this.UserId.TabIndex = 14;
            this.UserId.Text = "User Id";
            // 
            // Identifier
            // 
            this.Identifier.AutoSize = true;
            this.Identifier.Location = new System.Drawing.Point(32, 53);
            this.Identifier.Name = "Identifier";
            this.Identifier.Size = new System.Drawing.Size(47, 13);
            this.Identifier.TabIndex = 15;
            this.Identifier.Text = "Identifier";
            // 
            // IdentifierBox
            // 
            this.IdentifierBox.Location = new System.Drawing.Point(143, 48);
            this.IdentifierBox.Name = "IdentifierBox";
            this.IdentifierBox.Size = new System.Drawing.Size(100, 20);
            this.IdentifierBox.TabIndex = 16;
            this.IdentifierBox.Text = "00-05-B1-00-DD-FE";
            // 
            // IpAddress
            // 
            this.IpAddress.AutoSize = true;
            this.IpAddress.Location = new System.Drawing.Point(32, 87);
            this.IpAddress.Name = "IpAddress";
            this.IpAddress.Size = new System.Drawing.Size(91, 13);
            this.IpAddress.TabIndex = 17;
            this.IpAddress.Text = "Server IP address";
            // 
            // IpAddressBox
            // 
            this.IpAddressBox.Location = new System.Drawing.Point(143, 82);
            this.IpAddressBox.Name = "IpAddressBox";
            this.IpAddressBox.Size = new System.Drawing.Size(100, 20);
            this.IpAddressBox.TabIndex = 18;
            this.IpAddressBox.Text = "127.0.0.1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Configuration Id Path";
            // 
            // PathBox2
            // 
            this.PathBox2.Location = new System.Drawing.Point(143, 17);
            this.PathBox2.Name = "PathBox2";
            this.PathBox2.Size = new System.Drawing.Size(206, 20);
            this.PathBox2.TabIndex = 20;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 661);
            this.Controls.Add(this.PathBox2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.IpAddressBox);
            this.Controls.Add(this.IpAddress);
            this.Controls.Add(this.IdentifierBox);
            this.Controls.Add(this.Identifier);
            this.Controls.Add(this.UserId);
            this.Controls.Add(this.ReaderId);
            this.Controls.Add(this.AccessDenied);
            this.Controls.Add(this.AccessGranted);
            this.Controls.Add(this.UserIdBox);
            this.Controls.Add(this.ReaderIdBox);
            this.Controls.Add(this.Disconnect_button);
            this.Controls.Add(this.ServerPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Connect);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.UserName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Tag = "";
            this.Text = "Mobile Tester";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox UserName;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Button Connect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ServerPort;
        private System.Windows.Forms.Button Disconnect_button;
        private System.Windows.Forms.TextBox ReaderIdBox;
        private System.Windows.Forms.TextBox UserIdBox;
        private System.Windows.Forms.Button AccessGranted;
        private System.Windows.Forms.Button AccessDenied;
        private System.Windows.Forms.Label ReaderId;
        private System.Windows.Forms.Label UserId;
        private System.Windows.Forms.Label Identifier;
        private System.Windows.Forms.TextBox IdentifierBox;
        private System.Windows.Forms.Label IpAddress;
        private System.Windows.Forms.TextBox IpAddressBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PathBox2;
    }
}

