namespace Client.MainForm
{
    partial class ChatForm
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
            this.panelLeft = new System.Windows.Forms.Panel();
            this.listBoxFriend = new System.Windows.Forms.ListBox();
            this.btnAddUID = new System.Windows.Forms.Button();
            this.txtUID = new System.Windows.Forms.TextBox();
            this.panelRights = new System.Windows.Forms.Panel();
            this.richTextBoxMessages = new System.Windows.Forms.RichTextBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtChatWith = new System.Windows.Forms.TextBox();
            this.panelLeft.SuspendLayout();
            this.panelRights.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.listBoxFriend);
            this.panelLeft.Controls.Add(this.btnAddUID);
            this.panelLeft.Controls.Add(this.txtUID);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Margin = new System.Windows.Forms.Padding(6);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(272, 681);
            this.panelLeft.TabIndex = 0;
            // 
            // listBoxFriend
            // 
            this.listBoxFriend.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxFriend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxFriend.FormattingEnabled = true;
            this.listBoxFriend.ItemHeight = 25;
            this.listBoxFriend.Location = new System.Drawing.Point(0, 77);
            this.listBoxFriend.Margin = new System.Windows.Forms.Padding(6);
            this.listBoxFriend.Name = "listBoxFriend";
            this.listBoxFriend.Size = new System.Drawing.Size(272, 604);
            this.listBoxFriend.TabIndex = 2;
            this.listBoxFriend.SelectedIndexChanged += new System.EventHandler(this.listBoxFriend_SelectedIndexChanged);
            // 
            // btnAddUID
            // 
            this.btnAddUID.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddUID.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAddUID.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddUID.Location = new System.Drawing.Point(0, 33);
            this.btnAddUID.Margin = new System.Windows.Forms.Padding(6);
            this.btnAddUID.Name = "btnAddUID";
            this.btnAddUID.Size = new System.Drawing.Size(272, 44);
            this.btnAddUID.TabIndex = 1;
            this.btnAddUID.Text = "Thêm";
            this.btnAddUID.UseVisualStyleBackColor = true;
            this.btnAddUID.Click += new System.EventHandler(this.btnAddUID_Click);
            // 
            // txtUID
            // 
            this.txtUID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUID.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtUID.Location = new System.Drawing.Point(0, 0);
            this.txtUID.Margin = new System.Windows.Forms.Padding(6);
            this.txtUID.Name = "txtUID";
            this.txtUID.Size = new System.Drawing.Size(272, 33);
            this.txtUID.TabIndex = 0;
            // 
            // panelRights
            // 
            this.panelRights.Controls.Add(this.richTextBoxMessages);
            this.panelRights.Controls.Add(this.txtMessage);
            this.panelRights.Controls.Add(this.btnSend);
            this.panelRights.Controls.Add(this.txtChatWith);
            this.panelRights.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRights.Location = new System.Drawing.Point(272, 0);
            this.panelRights.Margin = new System.Windows.Forms.Padding(6);
            this.panelRights.Name = "panelRights";
            this.panelRights.Size = new System.Drawing.Size(637, 681);
            this.panelRights.TabIndex = 1;
            // 
            // richTextBoxMessages
            // 
            this.richTextBoxMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxMessages.Location = new System.Drawing.Point(0, 58);
            this.richTextBoxMessages.Name = "richTextBoxMessages";
            this.richTextBoxMessages.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.richTextBoxMessages.Size = new System.Drawing.Size(637, 511);
            this.richTextBoxMessages.TabIndex = 4;
            this.richTextBoxMessages.Text = "";
            // 
            // txtMessage
            // 
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtMessage.Location = new System.Drawing.Point(0, 569);
            this.txtMessage.Margin = new System.Windows.Forms.Padding(6);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(637, 68);
            this.txtMessage.TabIndex = 2;
            // 
            // btnSend
            // 
            this.btnSend.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSend.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Location = new System.Drawing.Point(0, 637);
            this.btnSend.Margin = new System.Windows.Forms.Padding(6);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(637, 44);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Gửi";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtChatWith
            // 
            this.txtChatWith.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtChatWith.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtChatWith.Enabled = false;
            this.txtChatWith.Location = new System.Drawing.Point(0, 0);
            this.txtChatWith.Margin = new System.Windows.Forms.Padding(6);
            this.txtChatWith.Multiline = true;
            this.txtChatWith.Name = "txtChatWith";
            this.txtChatWith.ReadOnly = true;
            this.txtChatWith.Size = new System.Drawing.Size(637, 58);
            this.txtChatWith.TabIndex = 0;
            this.txtChatWith.Text = "Đang Chat Với: ";
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(909, 681);
            this.Controls.Add(this.panelRights);
            this.Controls.Add(this.panelLeft);
            this.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ChatForm";
            this.Text = "ChatForm";
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.panelRights.ResumeLayout(false);
            this.panelRights.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelRights;
        private System.Windows.Forms.Button btnAddUID;
        private System.Windows.Forms.TextBox txtUID;
        private System.Windows.Forms.TextBox txtChatWith;
        private System.Windows.Forms.ListBox listBoxFriend;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.RichTextBox richTextBoxMessages;
    }
}