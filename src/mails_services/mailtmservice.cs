using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrgHelpers.Logging;
using TrgHelpers.PasswordHelper;

namespace mails
{
    public static class MailTmService
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task CreateMailTmAccountAndTabAsync(TabPage tabAccountsRoot, TabControl tabAccountsInner)
        {
            var account = await CreateMailTmAccountAsync();
            if (account != null)
            {
                AddAccountTab(account, tabAccountsInner);
                tabAccountsRoot.Text = "mail.tm";
                Clipboard.SetText($"Adresse: {account.Address}\nMot de passe: {account.Password}");
                Logging.LogInfo($"Adresse et mot de passe copiés dans le presse-papiers pour {account.Address}");
            }
        }

        public static async Task<MailAccount?> CreateMailTmAccountAsync()
        {
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                var domainResp = await client.GetAsync("https://api.mail.tm/domains");
                domainResp.EnsureSuccessStatusCode();
                var domainJson = await domainResp.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(domainJson);
                string domain = doc.RootElement.GetProperty("hydra:member")[0].GetProperty("domain").GetString()!;

                string randomUser = Guid.NewGuid().ToString("N")[..8];
                string address = $"{randomUser}@{domain}";
                string password = PasswordHelper.RandomAlphaNumeric(12);

                var payload = new { address, password };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.mail.tm/accounts", content);
                string respText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Logging.LogError($"Erreur création compte mail.tm: {respText}");
                    MessageBox.Show($"Erreur lors de la création du compte :\n{respText}", "mail.tm - erreur");
                    return null;
                }

                Logging.LogInfo($"Compte mail.tm créé: {address}");

                var account = new MailAccount
                {
                    Address = address,
                    Password = password,
                    Token = null,
                    CreatedAt = DateTime.UtcNow
                };

                await MailAccountStorage.AddAccountToListAsync("accounts.json", account);
                await MailAccountStorage.UpdateTokenAsync("accounts.json", account.Address, null);

                return account;
            }
            catch (Exception ex)
            {
                Logging.LogError($"Exception dans CreateMailTmAccount: {ex}");
                MessageBox.Show($"Exception : {ex.Message}", "mail.tm - erreur");
                return null;
            }
        }

        public static void AddAccountTab(MailAccount account, TabControl tabAccountsInner)
        {
            var tab = new TabPage(account.Address);

            var lstEmails = new ListBox
            {
                Location = new System.Drawing.Point(10, 40),
                Size = new System.Drawing.Size(400, 610),
                Tag = account
            };
            lstEmails.SelectedIndexChanged += async (s, e) =>
            {
                if (lstEmails.SelectedItem is not MailListItem item) return;
                var rtbBody = tab.Controls.OfType<RichTextBox>().FirstOrDefault();
                if (rtbBody == null) return;

                string? token = await GetMailTmTokenAsync(account.Address, account.Password);
                if (token == null) return;

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.Timeout = TimeSpan.FromSeconds(10);

                var response = await client.GetAsync($"https://api.mail.tm/messages/{item.Id}");
                if (!response.IsSuccessStatusCode) return;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                string body = doc.RootElement.GetProperty("text").GetString() ?? "";
                rtbBody.Text = body;
            };
            tab.Controls.Add(lstEmails);

            var rtbBodyBox = new RichTextBox
            {
                Location = new System.Drawing.Point(420, 40),
                Size = new System.Drawing.Size(980, 610),
                ReadOnly = true
            };
            tab.Controls.Add(rtbBodyBox);

            var btnRefresh = new Button
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(100, 25),
                Text = "Rafraîchir",
                Tag = lstEmails
            };
            btnRefresh.Click += async (s, e) => await LoadEmailsAsync(account, lstEmails);
            tab.Controls.Add(btnRefresh);

            tabAccountsInner.TabPages.Add(tab);
            _ = LoadEmailsAsync(account, lstEmails);
        }

        public static async Task<string?> GetMailTmTokenAsync(string address, string password)
        {
            try
            {
                using var client = new HttpClient();
                var payload = new { address, password };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.mail.tm/token", content);
                if (!response.IsSuccessStatusCode) return null;

                using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                return doc.RootElement.GetProperty("token").GetString();
            }
            catch
            {
                return null;
            }
        }

        private static async Task LoadEmailsAsync(MailAccount account, ListBox lstEmails)
        {
            string? token = await GetMailTmTokenAsync(account.Address, account.Password);
            if (token == null) return;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            client.Timeout = TimeSpan.FromSeconds(10);

            var response = await client.GetAsync("https://api.mail.tm/messages");
            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var messages = doc.RootElement.GetProperty("hydra:member");

            lstEmails.Items.Clear();
            foreach (var msg in messages.EnumerateArray())
            {
                string subject = msg.GetProperty("subject").GetString() ?? "(Sans sujet)";
                string from = msg.GetProperty("from").GetProperty("address").GetString() ?? "(Inconnu)";
                string id = msg.GetProperty("id").GetString() ?? "";
                lstEmails.Items.Add(new MailListItem { Id = id, Display = $"{from} - {subject}" });
            }
        }
    }
}
