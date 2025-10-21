using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrgHelpers.Logging;
using TrgHelpers.PasswordHelper;

namespace mails
{
    public static class MaildropService
    {
        public static async Task CreateMaildropAccountAndTabAsync(TabControl tabMaildropInner)
        {
            var account = await CreateMaildropAccountAsync();
            if (account != null)
            {
                AddMaildropTab(account, tabMaildropInner);
            }
        }

        public static async Task<MailAccount> CreateMaildropAccountAsync()
        {
            string randomUser = Guid.NewGuid().ToString("N")[..8];
            string address = $"{randomUser}@maildrop.cc";
            string password = PasswordHelper.RandomAlphaNumeric(12);

            var account = new MailAccount
            {
                Address = address,
                Password = password,
                Token = null,
                CreatedAt = DateTime.UtcNow
            };

            await MailAccountStorage.AddAccountToListAsync("accounts.json", account);
            Logging.LogInfo($"Compte Maildrop créé: {address}");
            Clipboard.SetText(address);

            return account;
        }

        public static void AddMaildropTab(MailAccount account, TabControl tabMaildropInner)
        {
            var tab = new TabPage(account.Address);

            var lstEmails = new ListBox
            {
                Location = new System.Drawing.Point(10, 40),
                Size = new System.Drawing.Size(400, 610),
                Tag = account
            };
            tab.Controls.Add(lstEmails);

            var rtbBody = new RichTextBox
            {
                Location = new System.Drawing.Point(420, 40),
                Size = new System.Drawing.Size(980, 610),
                ReadOnly = true
            };
            tab.Controls.Add(rtbBody);

            var btnRefresh = new Button
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(100, 25),
                Text = "Rafraîchir",
                Tag = lstEmails
            };
            btnRefresh.Click += async (s, e) => await LoadMaildropEmailsAsync(account, lstEmails, rtbBody);
            tab.Controls.Add(btnRefresh);

            lstEmails.SelectedIndexChanged += async (s, e) =>
            {
                if (lstEmails.SelectedItem is not MailListItem item) return;

                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                var response = await client.GetAsync($"https://maildrop.cc/api/message/{item.Id}");
                if (!response.IsSuccessStatusCode) return;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                string body = doc.RootElement.GetProperty("text").GetString() ?? "";
                rtbBody.Text = body;
            };

            tabMaildropInner.TabPages.Add(tab);
            _ = LoadMaildropEmailsAsync(account, lstEmails, rtbBody);
        }

        public static async Task LoadMaildropEmailsAsync(MailAccount account, ListBox lstEmails, RichTextBox? rtbBody = null)
        {
            try
            {
                string user = account.Address.Split('@')[0];
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                var response = await client.GetAsync($"https://maildrop.cc/api/inbox/{user}");
                if (!response.IsSuccessStatusCode) return;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var messages = doc.RootElement.GetProperty("msgs");

                lstEmails.Items.Clear();
                foreach (var msg in messages.EnumerateArray())
                {
                    string from = msg.GetProperty("from").GetString() ?? "(Inconnu)";
                    string subject = msg.GetProperty("subject").GetString() ?? "(Sans sujet)";
                    string id = msg.GetProperty("id").GetString() ?? "";
                    lstEmails.Items.Add(new MailListItem { Id = id, Display = $"{from} - {subject}" });
                }

                if (lstEmails.Items.Count > 0 && rtbBody != null)
                    lstEmails.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Logging.LogError($"Erreur LoadMaildropEmailsAsync: {ex}");
            }
        }
    }
}
