namespace mails
{
    partial class Form1
    {
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fichierToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aideToolStripMenuItem_Aide;
        private System.Windows.Forms.ToolStripMenuItem aideToolStripMenuItem_APropos;
        private System.Windows.Forms.ToolStripSeparator aideToolStripMenuItem_Separator;

        private System.Windows.Forms.TabControl tabAccounts;
        private System.Windows.Forms.TabPage tabAccountsRoot;
        private System.Windows.Forms.TabControl tabAccountsInner;

        private System.Windows.Forms.TabPage tabMaildropRoot;
        private System.Windows.Forms.TabControl tabMaildropInner;

        private System.Windows.Forms.Button btnMailTm;
        private System.Windows.Forms.Button btnMaildrop;
        private System.Windows.Forms.Label lblStatus;

        private System.Windows.Forms.Panel panelInfo;
        private System.Windows.Forms.GroupBox groupBoxInfo;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnCopyPassword;
        private System.Windows.Forms.Label lblCreationDate;
        private System.Windows.Forms.TextBox txtCreationDate;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Button btnCopyEmail;
        private System.Windows.Forms.Label lblStatusInfo;
        private System.Windows.Forms.TextBox txtStatusInfo;

        private void InitializeComponent()
        {
           
            this.menuStrip1 = new MenuStrip();
            this.fichierToolStripMenuItem = new ToolStripMenuItem();
            this.aideToolStripMenuItem = new ToolStripMenuItem();
            this.aideToolStripMenuItem_Aide = new ToolStripMenuItem();
            this.aideToolStripMenuItem_Separator = new ToolStripSeparator();
            this.aideToolStripMenuItem_APropos = new ToolStripMenuItem();

            this.tabAccounts = new TabControl();
            this.tabAccountsRoot = new TabPage();
            this.tabAccountsInner = new TabControl();

            this.tabMaildropRoot = new TabPage();
            this.tabMaildropInner = new TabControl();

         
            this.btnMailTm = new Button();
            this.btnMaildrop = new Button();

            this.lblStatus = new Label();

       
            this.panelInfo = new Panel();
            this.groupBoxInfo = new GroupBox();
            this.lblPassword = new Label();
            this.txtPassword = new TextBox();
            this.btnCopyPassword = new Button();
            this.lblCreationDate = new Label();
            this.txtCreationDate = new TextBox();
            this.lblEmail = new Label();
            this.txtEmail = new TextBox();
            this.btnCopyEmail = new Button();
            this.lblStatusInfo = new Label();
            this.txtStatusInfo = new TextBox();

            this.SuspendLayout();

            this.menuStrip1.Items.AddRange(new ToolStripItem[] {
                this.fichierToolStripMenuItem,
                this.aideToolStripMenuItem
            });
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Size = new System.Drawing.Size(1450, 24);
            this.fichierToolStripMenuItem.Text = "Fichier";

            this.aideToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.aideToolStripMenuItem_Aide,
                this.aideToolStripMenuItem_Separator,
                this.aideToolStripMenuItem_APropos
            });
            this.aideToolStripMenuItem.Text = "Aide";
            this.aideToolStripMenuItem_Aide.Text = "Aide";
            this.aideToolStripMenuItem_APropos.Text = "À propos...";

            this.btnMailTm.Location = new System.Drawing.Point(12, 40);
            this.btnMailTm.Size = new System.Drawing.Size(200, 30);
            this.btnMailTm.Text = "Créer mail jetable (mail.tm)";

            this.btnMaildrop.Location = new System.Drawing.Point(222, 40);
            this.btnMaildrop.Size = new System.Drawing.Size(200, 30);
            this.btnMaildrop.Text = "Créer mail jetable (Maildrop)";

            this.tabAccounts.Location = new System.Drawing.Point(12, 80);
            this.tabAccounts.Size = new System.Drawing.Size(1000, 708);

            this.tabAccountsRoot.Text = "mail.tm";
            this.tabAccountsInner.Dock = DockStyle.Fill;
            this.tabAccountsRoot.Controls.Add(this.tabAccountsInner);

            this.tabMaildropRoot.Text = "maildrop";
            this.tabMaildropInner.Dock = DockStyle.Fill;
            this.tabMaildropRoot.Controls.Add(this.tabMaildropInner);

            this.tabAccounts.Controls.Add(this.tabAccountsRoot);
            this.tabAccounts.Controls.Add(this.tabMaildropRoot);

            this.lblStatus.Location = new System.Drawing.Point(12, 800);
            this.lblStatus.Size = new System.Drawing.Size(1000, 20);
            this.lblStatus.Text = "Prêt.";

            this.panelInfo.Location = new System.Drawing.Point(1020, 80);
            this.panelInfo.Size = new System.Drawing.Size(400, 720);
            this.panelInfo.BorderStyle = BorderStyle.FixedSingle;

            this.groupBoxInfo.Location = new System.Drawing.Point(10, 10);
            this.groupBoxInfo.Size = new System.Drawing.Size(380, 700);
            this.groupBoxInfo.Text = "Informations du compte";
            this.groupBoxInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);

            this.lblEmail.Location = new System.Drawing.Point(15, 30);
            this.lblEmail.Size = new System.Drawing.Size(100, 20);
            this.lblEmail.Text = "Email:";
            this.lblEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);

            this.txtEmail.Location = new System.Drawing.Point(15, 50);
            this.txtEmail.Size = new System.Drawing.Size(280, 20);
            this.txtEmail.ReadOnly = true;
            this.txtEmail.BackColor = System.Drawing.Color.LightGray;

            this.btnCopyEmail.Location = new System.Drawing.Point(300, 50);
            this.btnCopyEmail.Size = new System.Drawing.Size(65, 20);
            this.btnCopyEmail.Text = "Copier";
            this.btnCopyEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);

            this.lblPassword.Location = new System.Drawing.Point(15, 90);
            this.lblPassword.Size = new System.Drawing.Size(100, 20);
            this.lblPassword.Text = "Mot de passe:";
            this.lblPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);

            this.txtPassword.Location = new System.Drawing.Point(15, 110);
            this.txtPassword.Size = new System.Drawing.Size(280, 20);
            this.txtPassword.ReadOnly = true;
            this.txtPassword.BackColor = System.Drawing.Color.LightGray;
            this.txtPassword.UseSystemPasswordChar = true;

            this.btnCopyPassword.Location = new System.Drawing.Point(300, 110);
            this.btnCopyPassword.Size = new System.Drawing.Size(65, 20);
            this.btnCopyPassword.Text = "Copier";
            this.btnCopyPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);

            this.lblCreationDate.Location = new System.Drawing.Point(15, 150);
            this.lblCreationDate.Size = new System.Drawing.Size(100, 20);
            this.lblCreationDate.Text = "Date de création:";
            this.lblCreationDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);

            this.txtCreationDate.Location = new System.Drawing.Point(15, 170);
            this.txtCreationDate.Size = new System.Drawing.Size(350, 20);
            this.txtCreationDate.ReadOnly = true;
            this.txtCreationDate.BackColor = System.Drawing.Color.LightGray;

            this.lblStatusInfo.Location = new System.Drawing.Point(15, 210);
            this.lblStatusInfo.Size = new System.Drawing.Size(100, 20);
            this.lblStatusInfo.Text = "Statut:";
            this.lblStatusInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);

            this.txtStatusInfo.Location = new System.Drawing.Point(15, 230);
            this.txtStatusInfo.Size = new System.Drawing.Size(350, 20);
            this.txtStatusInfo.ReadOnly = true;
            this.txtStatusInfo.BackColor = System.Drawing.Color.LightGray;


            this.groupBoxInfo.Controls.Add(this.lblEmail);
            this.groupBoxInfo.Controls.Add(this.txtEmail);
            this.groupBoxInfo.Controls.Add(this.btnCopyEmail);
            this.groupBoxInfo.Controls.Add(this.lblPassword);
            this.groupBoxInfo.Controls.Add(this.txtPassword);
            this.groupBoxInfo.Controls.Add(this.btnCopyPassword);
            this.groupBoxInfo.Controls.Add(this.lblCreationDate);
            this.groupBoxInfo.Controls.Add(this.txtCreationDate);
            this.groupBoxInfo.Controls.Add(this.lblStatusInfo);
            this.groupBoxInfo.Controls.Add(this.txtStatusInfo);

            this.panelInfo.Controls.Add(this.groupBoxInfo);

            this.ClientSize = new System.Drawing.Size(1450, 850);
            this.Controls.Add(this.tabAccounts);
            this.Controls.Add(this.btnMailTm);
            this.Controls.Add(this.btnMaildrop);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.panelInfo);
            this.Controls.Add(this.menuStrip1);

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }
    }
}
