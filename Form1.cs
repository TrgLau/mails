using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrgHelpers.Logging;

namespace mails
{
    public partial class Form1 : Form
    {
        private bool mailTmEnabled = false;
        private bool maildropEnabled = false;

        public Form1()
        {
            InitializeComponent();

            aideToolStripMenuItem_APropos.Click += (s, e) =>
            {
                MessageBox.Show("Debug v0.1\n(c) 2025 TrogLau", "À propos");
            };

            btnMailTm.Click += async (s, e) =>
            {
                await MailTmService.CreateMailTmAccountAndTabAsync(tabAccountsRoot, tabAccountsInner);
                mailTmEnabled = true;
            };

            btnMaildrop.Click += async (s, e) =>
            {
                await MaildropService.CreateMaildropAccountAndTabAsync(tabMaildropInner);
                maildropEnabled = true;
            };

            tabAccounts.Selecting += TabAccounts_Selecting;
            tabAccounts.SelectedIndexChanged += TabAccounts_SelectedIndexChanged;
            tabAccountsInner.SelectedIndexChanged += TabAccountsInner_SelectedIndexChanged;
            tabMaildropInner.SelectedIndexChanged += TabMaildropInner_SelectedIndexChanged;

            btnCopyEmail.Click += BtnCopyEmail_Click;
            btnCopyPassword.Click += BtnCopyPassword_Click;

            _ = LoadStoredAccountsAsync();
        }
        private async Task LoadStoredAccountsAsync()
        {
            var accounts = await MailAccountStorage.LoadAccountListAsync("accounts.json");
            if (accounts == null || accounts.Count == 0)
            {
                mailTmEnabled = false;
                tabAccountsRoot.Text = "(inactif)";
                tabMaildropRoot.Text = "(inactif)";
                return;
            }

            foreach (var account in accounts)
            {
                if (account.Address.EndsWith("@maildrop.cc"))
                {
                    MaildropService.AddMaildropTab(account, tabMaildropInner);
                    maildropEnabled = true;
                }
                else
                {
                    MailTmService.AddAccountTab(account, tabAccountsInner);
                    mailTmEnabled = true;
                }
            }
            tabAccountsRoot.Text = mailTmEnabled ? "mail.tm" : "(inactif)";
            tabMaildropRoot.Text = maildropEnabled ? "maildrop" : "(inactif)";
        }
        private void TabAccounts_Selecting(object? sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tabAccountsRoot && !mailTmEnabled)
            {
                e.Cancel = true;
                MessageBox.Show("Aucun compte mail.tm enregistré. Créez un compte pour activer cet onglet.", "mail.tm inactif");
            }
        }

        private void TabAccounts_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabAccounts.SelectedTab == tabAccountsRoot)
            {
                if (tabAccountsInner.TabPages.Count > 0)
                {
                    var firstTab = tabAccountsInner.TabPages[0];
                    var account = GetAccountFromTab(firstTab);
                    if (account != null)
                    {
                        UpdateAccountInfo(account.Address, account.Password, account.CreatedAt, "Actif");
                    }
                }
                else
                {
                    ClearAccountInfo();
                }
            }
            else if (tabAccounts.SelectedTab == tabMaildropRoot)
            {
                if (tabMaildropInner.TabPages.Count > 0)
                {
                    var firstTab = tabMaildropInner.TabPages[0];
                    var account = GetAccountFromTab(firstTab);
                    if (account != null)
                    {
                        UpdateAccountInfo(account.Address, account.Password, account.CreatedAt, "Actif");
                    }
                }
                else
                {
                    ClearAccountInfo();
                }
            }
        }

        public void UpdateAccountInfo(string email, string password, DateTime creationDate, string status)
        {
            txtEmail.Text = email;
            txtPassword.Text = password;
            txtCreationDate.Text = creationDate.ToString("dd/MM/yyyy HH:mm:ss");
            txtStatusInfo.Text = status;
        }

        public void ClearAccountInfo()
        {
            txtEmail.Text = "";
            txtPassword.Text = "";
            txtCreationDate.Text = "";
            txtStatusInfo.Text = "";
        }

        private MailAccount? GetAccountFromTab(TabPage tab)
        {
            // Chercher le ListBox dans l'onglet et récupérer le Tag qui contient l'account
            var listBox = tab.Controls.OfType<ListBox>().FirstOrDefault();
            if (listBox?.Tag is MailAccount account)
            {
                return account;
            }
            return null;
        }

        private void TabAccountsInner_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabAccountsInner.SelectedTab != null)
            {
                var account = GetAccountFromTab(tabAccountsInner.SelectedTab);
                if (account != null)
                {
                    UpdateAccountInfo(account.Address, account.Password, account.CreatedAt, "Actif");
                }
            }
        }

        private void TabMaildropInner_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabMaildropInner.SelectedTab != null)
            {
                var account = GetAccountFromTab(tabMaildropInner.SelectedTab);
                if (account != null)
                {
                    UpdateAccountInfo(account.Address, account.Password, account.CreatedAt, "Actif");
                }
            }
        }

        private void BtnCopyEmail_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmail.Text))
            {
                Clipboard.SetText(txtEmail.Text);
                lblStatus.Text = "Email copié dans le presse-papiers";
            }
        }

        private void BtnCopyPassword_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPassword.Text))
            {
                Clipboard.SetText(txtPassword.Text);
                lblStatus.Text = "Mot de passe copié dans le presse-papiers";
            }
        }
    }
}
